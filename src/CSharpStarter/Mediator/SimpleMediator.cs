using CSharpStarter.Functional;
using CSharpStarter.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpStarter.Mediator;

/// <summary>
/// Implements a mediator that routes requests through a pipeline of behaviors and handlers.
/// </summary>
/// <remarks>
/// This class uses the mediator pattern to decouple request initiation from processing, supporting
/// both global and request-specific behaviors (e.g., logging, validation) before invoking a handler.
/// It leverages dependency injection and caching for performance.
/// </remarks>
internal sealed class SimpleMediator : IMediator
{
    private const string HANDLE_METHOD_NAME = "Handle";
    private const string HANDLE_NOT_FOUND_ERROR_FORMAT = "Handle method not found on handler type {0}";

    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, (object Handler, MethodInfo HandleMethod)> _handlerCache = new();
    private readonly IReadOnlyCollection<IBehavior<IRequest>> _globalBehaviors;
    private readonly ConcurrentDictionary<Type, IReadOnlyList<IBehavior<IRequest>>> _specificBehaviorCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    /// <param name="globalBehaviors">The global behaviors to apply to all requests.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> or <paramref name="globalBehaviors"/> is null.</exception>
    public SimpleMediator(IServiceProvider serviceProvider, IEnumerable<IBehavior> globalBehaviors)
    {
        _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));
        _globalBehaviors = globalBehaviors?.ToList().AsReadOnly()
            ?? throw new ArgumentNullException(nameof(globalBehaviors));
    }

    /// <inheritdoc />
    public async Task<Result> Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await ExecutePipeline(request, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<TValue>> Send<TValue>(
        IRequest<TValue> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await ExecutePipeline(request, cancellationToken);
    }

    #region Internal

    /// <summary>
    /// Executes the pipeline for a request without a return value.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the pipeline execution.</returns>
    private Task<Result> ExecutePipeline(IRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<IBehavior<IRequest>> behaviors = [.. _globalBehaviors, .. ResolveBehaviors(request)];
        BehaviorDelegate next = Handle;
        foreach (IBehavior<IRequest> behavior in behaviors.Reverse())
        {
            BehaviorDelegate previous = next;
            next = (req, ct) => behavior.Handle(req, previous, ct);
        }

        return next(request, cancellationToken);
    }

    /// <summary>
    /// Executes the pipeline for a request with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of value expected in the response.</typeparam>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the pipeline execution, including the value.</returns>
    private Task<Result<TValue>> ExecutePipeline<TValue>(
        IRequest<TValue> request, CancellationToken cancellationToken)
    {
        IEnumerable<IBehavior<IRequest>> behaviors = [.. _globalBehaviors, .. ResolveBehaviors(request)];

        Result<TValue> result = null!;
        BehaviorDelegate next = (req, ct) =>
        {
            return Handle((IRequest<TValue>)req, ct)
                .Tap(value => result = Result.Ok(value))
                .Bind(_ => Result.Ok());
        };

        foreach (IBehavior<IRequest> behavior in behaviors.Reverse())
        {
            BehaviorDelegate previous = next;
            next = (req, ct) => behavior.Handle(req, previous, ct);
        }

        return next(request, cancellationToken).Bind(() => result);
    }

    /// <summary>
    /// Retrieves or caches a handler and its Handle method for a given request type.
    /// </summary>
    /// <param name="requestType">The type of the request.</param>
    /// <param name="handlerBaseType">The base handler type (e.g., <see cref="IRequestHandler{TRequest}"/>).</param>
    /// <param name="valueType">The return value type, if applicable. Null for void requests.</param>
    /// <returns>A tuple containing the handler instance and its Handle method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the handler or Handle method is not found.</exception>
    private (object Handler, MethodInfo HandleMethod) GetCachedHandler(
        Type requestType, Type handlerBaseType, Type? valueType = null)
    {
        return _handlerCache.GetOrAdd(requestType, _ =>
        {
            Type handlerType = valueType == null
                ? handlerBaseType.MakeGenericType(requestType)
                : handlerBaseType.MakeGenericType(requestType, valueType);
            object handler = _serviceProvider.GetService(handlerType)
                ?? throw new InvalidOperationException(
                    $"No handler registered for request type {requestType.FullName}. " +
                    $"Expected handler type: {handlerType.FullName}");
            MethodInfo handleMethod = handlerType.GetMethod(HANDLE_METHOD_NAME)
                ?? throw new InvalidOperationException(string.Format(HANDLE_NOT_FOUND_ERROR_FORMAT, handlerType.FullName));
            return (handler, handleMethod);
        });
    }

    /// <summary>
    /// Invokes the handler for a request without a return value.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the handler execution.</returns>
    private Task<Result> Handle(IRequest request, CancellationToken cancellationToken)
    {
        (object handler, MethodInfo handleMethod) = GetCachedHandler(
            request.GetType(), typeof(IRequestHandler<>));
        return (Task<Result>)handleMethod.Invoke(handler, [request, cancellationToken])!;
    }

    /// <summary>
    /// Invokes the handler for a request with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of value returned by the handler.</typeparam>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the handler execution, including the value.</returns>
    private Task<Result<TValue>> Handle<TValue>(IRequest<TValue> request, CancellationToken cancellationToken)
    {
        (object handler, MethodInfo handleMethod) = GetCachedHandler(
            request.GetType(), typeof(IRequestHandler<,>), typeof(TValue));
        return (Task<Result<TValue>>)handleMethod.Invoke(handler, [request, cancellationToken])!;
    }

    /// <summary>
    /// Resolves behaviors specific to a request type, caching the result.
    /// </summary>
    /// <param name="request">The request for which to resolve behaviors.</param>
    /// <returns>A collection of behaviors adapted to the <see cref="IBehavior{IRequest}"/> interface.</returns>
    private IReadOnlyList<IBehavior<IRequest>> ResolveBehaviors(IRequest request)
    {
        Type requestType = request.GetType();

        return _specificBehaviorCache.GetOrAdd(requestType, _ =>
        {
            Type behaviorType = typeof(IBehavior<>).MakeGenericType(requestType);
            var specificBehaviors = _serviceProvider.GetServices(behaviorType);

            Type behaviorAdapter = typeof(BehaviorAdapter<>).MakeGenericType(requestType);
            return specificBehaviors
                .Where(b => b != null)
                .Select(behavior => (IBehavior<IRequest>)Activator.CreateInstance(behaviorAdapter, behavior)!)
                .ToList()
                .AsReadOnly();
        });
    }

    /// <summary>
    /// Adapts a type-specific behavior to the generic <see cref="IBehavior{IRequest}"/> interface.
    /// </summary>
    /// <typeparam name="TRequest">The specific request type the behavior handles.</typeparam>
    /// <remarks>
    /// This adapter allows type-specific behaviors to be invoked in the pipeline for requests of type <typeparamref name="TRequest"/>.
    /// </remarks>
    private sealed class BehaviorAdapter<TRequest> : IBehavior<IRequest>
        where TRequest : IRequest
    {
        private readonly IBehavior<TRequest> _requestBehavior;

        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorAdapter{TRequest}"/> class.
        /// </summary>
        /// <param name="requestBehavior">The type-specific behavior to adapt.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="requestBehavior"/> is null.</exception>
        public BehaviorAdapter(IBehavior<TRequest> requestBehavior)
        {
            _requestBehavior = requestBehavior ?? throw new ArgumentNullException(nameof(requestBehavior));
        }

        /// <summary>
        /// Handles the request by delegating to the type-specific behavior if the request matches the expected type.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="next">The delegate to invoke the next behavior or handler in the pipeline.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task yielding the result of the behavior or the next delegate.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request type does not match <typeparamref name="TRequest"/>.</exception>
        public Task<Result> Handle(IRequest request, BehaviorDelegate next, CancellationToken cancellationToken)
        {
            if (request is TRequest specificRequest)
            {
                return _requestBehavior.Handle(specificRequest, (req, ct) => next(req, ct), cancellationToken);
            }
            throw new InvalidOperationException(
                $"Behavior expects request of type {typeof(TRequest).FullName}, but received {request.GetType().FullName}");
        }
    }

    #endregion
}