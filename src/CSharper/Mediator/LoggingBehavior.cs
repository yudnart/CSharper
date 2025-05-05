using CSharper.AppContext;
using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSharper.Mediator;

/// <summary>
/// Implements logging behavior for mediator requests, capturing request details, context, and outcomes.
/// </summary>
/// <remarks>
/// Logs request start, success, failure, and exceptions with structured properties, including a serialized request and
/// application context (if available). Generates a correlation ID when no context is present for traceability.
/// </remarks>
internal sealed class LoggingBehavior : IBehavior
{
    private const string _anonymousUserId = "Anonymous";
    private const string _notApplicable = "N/A";
    private const string _requestCompleted = "Request {RequestType} completed successfully.";
    private const string _requestFailedWithException = "Request {RequestType} failed. (CorrelationId: {CorrelationId})";
    private const string _requestFailedWithErrors = "Request {RequestType} failed with errors:\n{Errors}";
    private const string _requestProcessing = "Processing request {RequestType}";
    private const string _requestSerializationFailed = "Failed to serialize request {RequestType}. Using ToString instead.";
    private const string _requestTruncated = "Request JSON truncated to {MaxLength} characters.";
    private const string _extensionsLimited = "Extensions limited to {MaxExtensions} entries.";

    private const string _request = "Request";
    private const string _requestType = "RequestType";

    // Expose internal for unit testing
    internal const string _extensionPrefix = "_";

    private const int _maxRequestJsonLength = 10000;
    private const int _maxExtensions = 50;

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = false
    };

    /// <summary>
    /// The logger instance used for recording request events and errors.
    /// </summary>
    private readonly ILogger<LoggingBehavior> _logger;

    /// <summary>
    /// The optional application context providing metadata like user details and request IDs.
    /// </summary>
    private readonly IAppContext? _appContext;

    /// <summary>
    /// A generated correlation ID used when no application context is available.
    /// </summary>
    private string? _correlationId;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for recording events.</param>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="logger"/> is null.</exception>
    public LoggingBehavior(ILogger<LoggingBehavior> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _appContext = serviceProvider.GetService<IAppContext>();
    }

    /// <summary>
    /// Handles the request by logging its start, processing it, and logging its outcome.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="next">The delegate to invoke the next behavior or handler.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task representing the result of the request processing.</returns>
    /// <exception cref="Exception">Rethrows any exception from processing, after logging it.</exception>
    public async Task<Result> Handle(IRequest request, BehaviorDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            // Log request start
            LogRequest(request);

            // Proceed with the request and log success or failure
            return await next(request, cancellationToken)
                .Tap(() => LogSuccess(request))
                .TapError(errors => LogFailure(request, errors));
        }
        catch (Exception ex)
        {
            // Log exception
            _logger.LogError(ex,
                _requestFailedWithException,
                request.GetType().Name,
                _correlationId ?? _appContext?.CorrelationId);

            throw;
        }
    }

    /// <summary>
    /// Logs a failed request with associated errors.
    /// </summary>
    /// <param name="request">The request that failed.</param>
    /// <param name="errors">The errors causing the failure.</param>
    private void LogFailure(IRequest request, Error[] errors)
    {
        errors ??= [];
        _logger.LogWarning(_requestFailedWithErrors,
            request.GetType().Name,
            string.Join<Error>(Environment.NewLine, errors));
    }

    /// <summary>
    /// Logs the start of a request, including its serialized form and context (if available).
    /// </summary>
    /// <param name="request">The request to log.</param>
    /// <remarks>
    /// Serializes the request to JSON, with a fallback to <see cref="object.ToString"/> if serialization fails.
    /// Includes application context properties and extensions (if available) or a generated correlation ID in the log scope.
    /// Truncates large request JSON and limits extensions to prevent excessive log size.
    /// </remarks>
    private void LogRequest(IRequest request)
    {
        // Serialize the request with fallback
        string requestJson;
        try
        {
            requestJson = JsonSerializer.Serialize(request, request.GetType(), _serializerOptions);
            if (requestJson.Length > _maxRequestJsonLength)
            {
#if NET6_0_OR_GREATER
                requestJson = requestJson[.._maxRequestJsonLength];
#else
                requestJson = requestJson.Substring(0, _maxRequestJsonLength);
#endif
                _logger.LogWarning(_requestTruncated, _maxRequestJsonLength);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, _requestSerializationFailed, request.GetType().Name);
            requestJson = request.ToString() ?? _notApplicable;
        }

        // Prepare log properties
        Dictionary<string, object?> logProperties = new()
        {
            { _requestType, request.GetType().Name },
            { _request, requestJson }
        };

        // Add app context properties or generate correlation ID
        if (_appContext is not null)
        {
            logProperties.Add(nameof(IAppContext.RequestId), _appContext.RequestId);
            logProperties.Add(nameof(IAppContext.CorrelationId), _appContext.CorrelationId);
            logProperties.Add(nameof(IAppContext.CurrentUser.UserId), _appContext.CurrentUser?.UserId ?? _anonymousUserId);
            logProperties.Add(nameof(IAppContext.ClientIpAddress), _appContext.ClientIpAddress ?? _notApplicable);
            logProperties.Add(nameof(IAppContext.UserAgent), _appContext.UserAgent ?? _notApplicable);
            logProperties.Add(nameof(IAppContext.RequestPath), _appContext.RequestPath ?? _notApplicable);

            if (!string.IsNullOrWhiteSpace(_appContext.CurrentUser?.TenantId))
            {
                logProperties.Add(nameof(IAppContext.CurrentUser.TenantId), _appContext.CurrentUser!.TenantId);
            }

            // Add extensions with prefix, limiting count
            int extensionCount = 0;
            foreach (KeyValuePair<string, object> extension in _appContext.Extensions)
            {
                if (extensionCount >= _maxExtensions)
                {
                    _logger.LogWarning(_extensionsLimited, _maxExtensions);
                    break;
                }
                logProperties[$"{_extensionPrefix}{extension.Key}"] = extension.Value;
                extensionCount++;
            }
        }
        else
        {
            _correlationId = Guid.NewGuid().ToString();
            logProperties.Add(nameof(IAppContext.CorrelationId), _correlationId);
        }

        // Log with structured properties
        using (_logger.BeginScope(logProperties))
        {
            _logger.LogInformation(_requestProcessing, request.GetType().Name);
        }
    }

    /// <summary>
    /// Logs a successful request completion.
    /// </summary>
    /// <param name="request">The request that completed successfully.</param>
    private void LogSuccess(IRequest request)
    {
        _logger.LogInformation(_requestCompleted, request.GetType().Name);
    }
}
