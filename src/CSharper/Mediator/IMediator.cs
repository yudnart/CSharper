using CSharper.Results;
using System.Threading;
using System.Threading.Tasks;

namespace CSharper.Mediator;

/// <summary>
/// Defines a mediator for sending requests and receiving results.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a non-generic request and returns a <see cref="Result"/> indicating success or failure.
    /// </summary>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that resolves to a <see cref="Result"/> representing the outcome of the request.</returns>
    Task<Result> Send(IRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a generic request and returns a <see cref="Result{TValue}"/> containing the result value or errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned by the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task that resolves to a <see cref="Result{TValue}"/> representing the outcome of the request.</returns>
    Task<Result<TValue>> Send<TValue>(IRequest<TValue> request, CancellationToken cancellationToken);
}
