using CSharper.Functional;
using CSharper.Results;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace CSharper.Mediator;

/// <summary>
/// Implements a mediator that routes requests through a pipeline of behaviors and handlers.
/// </summary>
/// <remarks>
/// This class uses the mediator pattern to decouple request initiation from processing, supporting
/// both global and request-specific behaviors (e.g., logging, validation) before invoking a handler.
/// Global behaviors execute in registration order (first registered executes first), followed by
/// request-specific behaviors in registration order, then the handler. It optimizes performance by
/// caching handler types and method information, assuming the mediator is scoped to handle
/// dependencies like the application context correctly.
/// </remarks>
internal sealed class SimpleMediator : IMediator
{
    private const string HANDLE_METHOD_NAME = "Handle";
    private const string HANDLE_NOT_FOUND_ERROR_FORMAT = "Handle method not found on handler type {0}";

    private readonly IServiceProvider _serviceProvider;
    private readonly IBehavior<IRequest>[] _globalBehaviors;
    private readonly ConcurrentDictionary<Type, (Type HandlerType, MethodInfo HandleMethod)> _handlerCache = new();
    private readonly ConcurrentDictionary<Type, Func<IServiceProvider, IBehavior<IRequest>[]>> _behaviorFactoryCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleMediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The scoped service provider for resolving dependencies.</param>
    /// <param name="globalBehaviors">The global behaviors to apply to all requests, in registration order.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceProvider"/> is null.</exception>
    public SimpleMediator(IServiceProvider serviceProvider, IEnumerable<IBehavior> globalBehaviors)
    {
        _serviceProvider = serviceProvider
            ?? throw new ArgumentNullException(nameof(serviceProvider));
        _globalBehaviors = globalBehaviors?.ToArray() ?? [];
    }

    /// <inheritdoc />
    public Task<Result> Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return ExecutePipeline(request, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<TValue>> Send<TValue>(
        IRequest<TValue> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return ExecutePipeline(request, cancellationToken);
    }

    #region Internal

    /// <summary>
    /// Executes the pipeline for a request without a return value.
    /// </summary>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the pipeline execution.</returns>
    private async Task<Result> ExecutePipeline(IRequest request, CancellationToken cancellationToken)
    {
        IBehavior<IRequest>[] behaviors = [.. _globalBehaviors, .. ResolveBehaviors(request)];
        if (behaviors.Length == 0)
        {
            return await Handle(request, cancellationToken);
        }

        async Task<Result> Execute(int index, IRequest req, CancellationToken ct)
        {
            if (index >= behaviors.Length)
            {
                return await Handle(req, ct);
            }
            return await behaviors[index].Handle(req, (r, c) => Execute(index + 1, r, c), ct);
        }

        return await Execute(0, request, cancellationToken);
    }

    /// <summary>
    /// Executes the pipeline for a request with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of value expected in the response.</typeparam>
    /// <param name="request">The request to process.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the pipeline execution, including the value.</returns>
    private async Task<Result<TValue>> ExecutePipeline<TValue>(
        IRequest<TValue> request, CancellationToken cancellationToken)
    {
        IBehavior<IRequest>[] behaviors = [.. _globalBehaviors, .. ResolveBehaviors(request)];
        if (behaviors.Length == 0)
        {
            return await Handle(request, cancellationToken);
        }

        async Task<Result> Execute(int index, CancellationToken ct)
        {
            if (index >= behaviors.Length)
            {
                return Result.Ok();
            }
            return await behaviors[index].Handle(request, (r, c) => Execute(index + 1, c), ct);
        }

        return await Execute(0, cancellationToken)
            .Bind(() => Handle(request, cancellationToken));
    }

    /// <summary>
    /// Retrieves or caches the handler type and its Handle method for a given request type.
    /// </summary>
    /// <param name="requestType">The type of the request.</param>
    /// <param name="handlerBaseType">The base handler type (e.g., <see cref="IRequestHandler{TRequest}"/>).</param>
    /// <param name="valueType">The return value type, if applicable. Null for void requests.</param>
    /// <returns>A tuple containing the handler type and its Handle method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the handler type or Handle method is not found.</exception>
    private (Type HandlerType, MethodInfo HandleMethod) GetCachedHandler(Type requestType, Type handlerBaseType, Type? valueType = null)
    {
        return _handlerCache.GetOrAdd(requestType, _ =>
        {
            Type handlerType = valueType == null
                ? handlerBaseType.MakeGenericType(requestType)
                : handlerBaseType.MakeGenericType(requestType, valueType);
            MethodInfo handleMethod = handlerType.GetMethod(HANDLE_METHOD_NAME)
                ?? throw new InvalidOperationException(
                    string.Format(HANDLE_NOT_FOUND_ERROR_FORMAT, handlerType.FullName));
            return (handlerType, handleMethod);
        });
    }

    /// <summary>
    /// Invokes the handler for a request without a return value.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the handler execution.</returns>
    private async Task<Result> Handle(IRequest request, CancellationToken cancellationToken)
    {
        (Type handlerType, MethodInfo handleMethod) = GetCachedHandler(
            request.GetType(), typeof(IRequestHandler<>));
        object handler = _serviceProvider.GetRequiredService(handlerType);
        try
        {
            return await (Task<Result>)handleMethod.Invoke(handler, [request, cancellationToken])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    /// <summary>
    /// Invokes the handler for a request with a return value.
    /// </summary>
    /// <typeparam name="TValue">The type of value returned by the handler.</typeparam>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task yielding the result of the handler execution, including the value.</returns>
    private async Task<Result<TValue>> Handle<TValue>(IRequest<TValue> request, CancellationToken cancellationToken)
    {
        (Type handlerType, MethodInfo handleMethod) = GetCachedHandler(
            request.GetType(), typeof(IRequestHandler<,>), typeof(TValue));
        object handler = _serviceProvider.GetRequiredService(handlerType);
        try
        {
            return await (Task<Result<TValue>>)handleMethod.Invoke(handler, [request, cancellationToken])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }

    /// <summary>
    /// Resolves behaviors specific to a request type using a cached factory.
    /// </summary>
    /// <param name="request">The request for which to resolve behaviors.</param>
    /// <returns>A collection of behaviors adapted to the <see cref="IBehavior{IRequest}"/> interface.</returns>
    private IBehavior<IRequest>[] ResolveBehaviors(IRequest request)
    {
        Type requestType = request.GetType();
        Func<IServiceProvider, IBehavior<IRequest>[]> factory = _behaviorFactoryCache.GetOrAdd(requestType, _ =>
        {
            Type behaviorType = typeof(IBehavior<>).MakeGenericType(requestType);
            Type behaviorAdapter = typeof(BehaviorAdapter<>).MakeGenericType(requestType);
            return sp => sp.GetServices(behaviorType)
                .Where(b => b != null)
                .Select(behavior => (IBehavior<IRequest>)Activator.CreateInstance(behaviorAdapter, behavior)!)
                .ToArray();
        });
        return factory(_serviceProvider);
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
        /// Handles the request by delegating to the type-specific behavior.
        /// </summary>
        /// <param name="request">The request to handle.</param>
        /// <param name="next">The delegate to invoke the next behavior or handler in the pipeline.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A task yielding the result of the behavior.</returns>
        public Task<Result> Handle(IRequest request, BehaviorDelegate next, CancellationToken cancellationToken)
        {
            return _requestBehavior.Handle((TRequest)request, next, cancellationToken);
        }
    }

    #endregion
}