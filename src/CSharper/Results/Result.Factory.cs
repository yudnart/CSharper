using CSharper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Results;

public sealed partial class Result
{
    #region Result

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Result"/> representing a successful operation.</returns>
    public static Result Ok() => _success;

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with the specified errors.
    /// </summary>
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    public static Result Fail(Error causedBy, params Error[] details)
        => new(causedBy, details);

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with an error constructed from the specified message, code, and path.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path indicating the error's context. Defaults to <c>null</c>.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public static Result Fail(string message, string? code = null, string? path = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        return Fail(new Error(message, code, path));
    }

    #endregion

    #region Result<TValue>

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> instance with the specified value.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a successful operation.</returns>
    public static Result<TValue> Ok<TValue>(TValue value)
        => Result<TValue>.Ok(value);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> instance with the specified errors.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    public static Result<TValue> Fail<TValue>(Error causedBy, params Error[] details)
        => Result<TValue>.Fail(causedBy, details);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> instance with an error constructed from the specified message, code, and path.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path indicating the error's context. Defaults to <c>null</c>.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public static Result<TValue> Fail<TValue>(
        string message, string? code = null, string? path = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        return Fail<TValue>(new Error(message, code, path));
    }

    #endregion

    #region Collect

    /// <summary>
    /// Aggregates multiple results into a single <see cref="Result"/>, succeeding only if all results succeed.
    /// </summary>
    /// <param name="results">The collection of results to aggregate.</param>
    /// <returns>
    /// A successful <see cref="Result"/> if all <paramref name="results"/> are successful;
    /// otherwise, a failed <see cref="Result"/> containing all errors from failed results.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="results"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="results"/> is empty.</exception>
    /// <remarks>
    /// The <see cref="ResultLike"/> type allows aggregation of both <see cref="Result"/> and <see cref="Result{T}"/>
    /// instances, extracting their underlying <see cref="ResultBase"/> for evaluation.
    /// </remarks>
    public static Result Collect(IEnumerable<ResultLike> results)
    {
        results.ThrowIfNull(nameof(results));

        if (!results.Any())
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(results));
        }

        List<Error> errors = [];

        foreach (ResultLike result in results)
        {
            ResultBase actual = result.Value;
            if (actual.IsFailure)
            {
                errors.AddRange(actual.Errors);
            }
        }

        return errors.Count == 0 ? Ok() : Fail(errors[0], [.. errors.Skip(1)]);
    }

    #endregion
}
