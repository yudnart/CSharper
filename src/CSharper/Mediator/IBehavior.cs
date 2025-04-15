using CSharper.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CSharper.Mediator;

/// <summary>
/// Defines a behavior for processing requests in the mediator pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request, which must implement <see cref="IRequest"/>.</typeparam>
/// <remarks>
/// This interface allows behaviors to intercept and process specific request types in the mediator pipeline.
/// Behaviors can perform tasks such as logging, validation, or authorization before or after the request handler is invoked.
/// Use <see cref="IBehavior{IRequest}"/> for global behaviors that apply to all requests.
/// </remarks>
public interface IBehavior<TRequest> where TRequest : IRequest
{
    /// <summary>
    /// Processes the request as part of the mediator pipeline.
    /// </summary>
    /// <param name="request">The request to process, of type <typeparamref name="TRequest"/>.</param>
    /// <param name="next">A delegate to invoke the next behavior or handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that resolves to a <see cref="Result"/> indicating the outcome of the behavior.</returns>
    /// <remarks>
    /// Implementations should call <paramref name="next"/> to continue the pipeline unless they intend to short-circuit it.
    /// A failed <see cref="Result"/> returned by this method will halt the pipeline and propagate errors to the caller.
    /// </remarks>
    Task<Result> Handle(TRequest request, BehaviorDelegate next, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a global behavior that processes all requests in the mediator pipeline.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IBehavior{TRequest}"/> for <see cref="IRequest"/>, allowing the behavior to
/// intercept any request type processed by the mediator. Use this for cross-cutting concerns like logging or
/// authentication that apply to all requests.
/// </remarks>
public interface IBehavior : IBehavior<IRequest>
{
}

/// <summary>
/// Represents a delegate for the next step in the mediator pipeline.
/// </summary>
/// <param name="request">The request to process.</param>
/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
/// <returns>A task that resolves to a <see cref="Result"/> indicating the outcome of the next step.</returns>
public delegate Task<Result> BehaviorDelegate(IRequest request, CancellationToken cancellationToken);
