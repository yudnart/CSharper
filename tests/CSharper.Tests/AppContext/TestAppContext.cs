using CSharper.AppContext;
using Moq;

namespace CSharper.Tests.AppContext;

/// <summary>
/// A test implementation of <see cref="IAppContext"/> for use in unit tests.
/// </summary>
internal sealed class TestAppContext : IAppContext
{

    public const string DefaultRequestId = "req123";
    public const string DefaultCorrelationId = "correlationId456";
    public const string DefaultUserId = "user1";
    public const string DefaultTenantId = "tenant1";
    public const string DefaultClientIpAddress = "192.168.1.1";
    public const string DefaultUserAgent = "Chrome";
    public const string DefaultRequestPath = "/api/test";
    public static readonly Dictionary<string, object> DefaultExtensions = new()
    {
        { "FeatureFlag", "enabled" },
        { "SessionId", "sess123" }
    };

    /// <summary>
    /// Gets the context of the current user making the request.
    /// </summary>
    public IUserContext CurrentUser { get; private set; } = default!;

    /// <summary>
    /// Gets the timestamp when the request was initiated.
    /// </summary>
    public DateTimeOffset Timestamp { get; private set; }

    /// <summary>
    /// Gets a unique identifier for the current request.
    /// </summary>
    public string RequestId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a correlation identifier linking related requests or operations.
    /// </summary>
    public string CorrelationId { get; private set; } = string.Empty;

    /// <summary>
    /// Gets an optional identifier indicating the cause of this request, if applicable.
    /// </summary>
    public string? CausationId { get; private set; }

    /// <summary>
    /// Gets the optional client IP address from which the request originated.
    /// </summary>
    public string? ClientIpAddress { get; private set; }

    /// <summary>
    /// Gets the optional user agent string of the client making the request.
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// Gets the optional path or endpoint of the request.
    /// </summary>
    public string? RequestPath { get; private set; }

    /// <summary>
    /// Gets a dictionary for storing additional request-specific data or metadata.
    /// </summary>
    public IReadOnlyDictionary<string, object> Extensions { get; private set; } 
        = new Dictionary<string, object>();

    /// <summary>
    /// Retrieves a typed value from the Extensions dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the extension data.</param>
    /// <returns>The value, or default(T) if not found or not convertible.</returns>
    public T? GetExtension<T>(string key)
    {
        if (Extensions.TryGetValue(key, out object? extension) && extension is T actual)
        {
            return actual;
        }
        return default!;
    }

    public static IAppContext GetDefaultAppContext()
    {
        Mock<IUserContext> userMock = new();
        userMock.Setup(user => user.UserId).Returns(DefaultUserId);
        userMock.Setup(user => user.TenantId).Returns(DefaultTenantId);

        return new TestAppContext()
        {
            RequestId = DefaultRequestId,
            CorrelationId = DefaultCorrelationId,
            CurrentUser = userMock.Object,
            ClientIpAddress = DefaultClientIpAddress,
            UserAgent = DefaultUserAgent,
            RequestPath = DefaultRequestPath,
            Extensions = DefaultExtensions
        };
    }

    public static IAppContext GetAppContext(Dictionary<string, object> extensions)
    {
        Mock<IUserContext> userMock = new();
        userMock.Setup(user => user.UserId).Returns(DefaultUserId);
        userMock.Setup(user => user.TenantId).Returns(DefaultTenantId);

        return new TestAppContext()
        {
            RequestId = DefaultRequestId,
            CorrelationId = DefaultCorrelationId,
            CurrentUser = userMock.Object,
            ClientIpAddress = DefaultClientIpAddress,
            UserAgent = DefaultUserAgent,
            RequestPath = DefaultRequestPath,
            Extensions = extensions
        };
    }
}
