using CSharpStarter.Results;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpStarter.Mediator;

/// <summary>
/// Defines a marker interface for request.
/// </summary>
public interface IRequest
{
    // Intentionally blank
}

/// <summary>
/// Defines a handler for processing requests that return a <see cref="Result"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request, implementing <see cref="IRequest"/>.</typeparam>
public interface IRequestHandler<TRequest> where TRequest : IRequest
{
    /// <summary>
    /// Handles the specified request and returns a <see cref="Result"/> indicating success or failure.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that resolves to a <see cref="Result"/> representing the outcome of the request.</returns>
    Task<Result> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a marker interface for requests that return a value.
/// </summary>
/// <typeparam name="TValue">The type of the value returned by the request.</typeparam>
public interface IRequest<TValue> : IRequest
{
    // Intentionally blank
}

/// <summary>
/// Defines a handler for processing requests that return a <see cref="Result{TValue}"/>.
/// </summary>
/// <typeparam name="TRequest">The type of the request, implementing <see cref="IRequest{TValue}"/>.</typeparam>
/// <typeparam name="TValue">The type of the value returned by the request.</typeparam>
public interface IRequestHandler<TRequest, TValue> where TRequest : IRequest<TValue>
{
    /// <summary>
    /// Handles the specified request and returns a <see cref="Result{TValue}"/> containing the result value or errors.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that resolves to a <see cref="Result{TValue}"/> representing the outcome of the request.</returns>
    Task<Result<TValue>> Handle(TRequest request, CancellationToken cancellationToken);
}