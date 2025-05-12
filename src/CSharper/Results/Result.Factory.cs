using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Results;

public sealed partial class Result
{
    public static object IndentMarker { get; private set; }
    #region Result

    /// <summary>
    /// Creates a successful <see cref="Result"/> instance.
    /// </summary>
    /// <returns>A new <see cref="Result"/> representing a successful operation.</returns>
    public static Result Ok() => _success;

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    public static Result Fail(Error error) => new(error);

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with an error constructed from the specified message and code.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to null.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public static Result Fail(string message, string? code = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        return Fail(new Error(message, code));
    }

    #endregion

    #region Result<T>

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> instance with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A new <see cref="Result{T}"/> representing a successful operation.</returns>
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance with the specified error.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="error">The primary error causing the failure.</param>
    /// <returns>A new <see cref="Result{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    public static Result<TValue> Fail<TValue>(Error error) => Result<TValue>.Fail(error);

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> instance with an error constructed from the specified message and code.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to null.</param>
    /// <returns>A new <see cref="Result{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public static Result<TValue> Fail<TValue>(string message, string? code = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        return Fail<TValue>(new Error(message, code));
    }

    #endregion

    #region Sequence

    /// <summary>
    /// Aggregates a sequence of results, returning a success result if all are successful,
    /// or a failure result with combined error details if any fail.
    /// </summary>
    /// <param name="results">The sequence of results to aggregate, each wrapped as a <see cref="ResultLike"/>.</param>
    /// <param name="message">The message for the combined error if any failures occur.</param>
    /// <param name="code">The optional code for the combined error. Defaults to null.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if all <paramref name="results"/> are successful;
    /// otherwise, a failure result with an <see cref="Error"/> containing all error details.
    /// </returns>
    /// <remarks>
    /// For each failed result, an <see cref="ErrorDetail"/> is added with the error's message and code (unindented),
    /// followed by its error details with messages prefixed with "&gt; ".
    /// The order of <see cref="Error.ErrorDetails"/> matches the order of <paramref name="results"/>,
    /// with each failed result contributing its error's details sequentially.
    /// <para>
    /// The <paramref name="results"/> parameter accepts <see cref="ResultLike"/> objects, which can wrap
    /// <see cref="Result"/>, <see cref="Result{T}"/>, or factory methods producing such results.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="results"/> is null or empty, or when <paramref name="message"/> is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Thrown when a <see cref="ResultLike"/> contains an invalid result type or factory.
    /// </exception>
    public static Result Sequence(
        IEnumerable<ResultLike> results, string message, string? code = null)
    {
        if (results == null || !results.Any())
        {
            throw new ArgumentException(
                "Collection cannot be null/empty.", nameof(results));
        }

        message.ThrowIfNullOrWhitespace(nameof(message));

        List<ErrorDetail> errorDetails = [];

        foreach (ResultLike result in results)
        {
            ResultBase actual = result.Value;
            if (actual.IsSuccess)
            {
                continue;
            }

            Error? error = actual.Error;
            if (error != null)
            {
                // Add Error as an ErrorDetail (no indentation)
                errorDetails.Add(new(error.Message, error.Code));
                // Add Error's ErrorDetails with indented Message
                foreach (ErrorDetail detail in error.ErrorDetails)
                {
                    errorDetails.Add(new($"{Error.IndentMarker} {detail.Message}", detail.Code));
                }
            }
        }

        return errorDetails.Count == 0
            ? Ok() : Fail(new(message, code, [.. errorDetails]));
    }

    #endregion
}