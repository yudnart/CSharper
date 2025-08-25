using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results.Abstractions;
using System;
using System.Collections.Generic;

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
    /// Creates a failed <see cref="Result"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    public static Result Fail(Error error) => new(error);

    /// <summary>
    /// Creates a failed <see cref="Result"/> instance with an error constructed from the specified message and code.
    /// </summary>
    /// <param name="code">The descriptive message of the error.</param>
    /// <param name="message">The optional error code for identification. Defaults to null.</param>
    /// <returns>A new <see cref="Result"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="code"/> is null, empty, or whitespace.</exception>
    public static Result Fail(string code, string? message = null)
    {
        code.ThrowIfNullOrWhitespace(nameof(code));
        return Fail(new Error(code, message));
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
    /// <param name="code">The optional error code for identification. Defaults to null.</param>
    /// <param name="message">The descriptive message of the error.</param>
    /// <returns>A new <see cref="Result{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public static Result<TValue> Fail<TValue>(string code, string? message = null)
    {
        code.ThrowIfNullOrWhitespace(nameof(code));
        return Fail<TValue>(new Error(code, message));
    }

    #endregion

    #region Sequence

    /// <summary>
    /// Evaluates a sequence of results and aggregates their errors.
    /// Returns Ok if all results are successful; otherwise, returns Fail with a single error
    /// if only one error exists, or a DetailedError with all errors if multiple errors exist.
    /// </summary>
    /// <param name="results">The collection of results to evaluate.</param>
    /// <param name="code">The failure error code. Defaults to "AggregateError".</param>
    /// <param name="message">[Optional] The failure error message.</param>
    /// <returns>A Result representing the aggregated outcome.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="results"/> is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown if any <see cref="ResultLike.Value"/> is null.</exception>
    public static Result Sequence(
        IEnumerable<ResultLike> results,
        string code = "AggregateError",
        string? message = null)
    {
        results.ThrowIfNullOrEmpty(nameof(results));
        code.ThrowIfNullOrWhitespace(nameof(code));

        List<Error> errors = [];

        foreach (ResultLike result in results)
        {
            ResultBase actual = result.Value;
            if (actual.IsFailure)
            {
                errors.Add(actual.Error!);
            }
        }


        return errors.Count switch
        {
            0 => Ok(),
            _ => Fail(AggregateError(errors, code, message))
        };
    }

    /// <summary>
    /// Creates a DetailedError aggregating multiple errors.
    /// </summary>
    /// <param name="errors">The errors to aggregate.</param>
    /// <param name="code">The error code for the aggregated error. Defaults to "AggregateError".</param>
    /// <param name="message">The optional message for the aggregated error. Defaults to "One or more operations failed."</param>
    /// <returns>A DetailedError containing the aggregated errors.</returns>
    public static AggregateError AggregateError(
        IEnumerable<Error> errors, 
        string code = "AggregateError", 
        string? message = "One or more operations failed.")
    {
        code.ThrowIfNullOrWhitespace(nameof(code));
        return new AggregateError(code, message, [.. errors]);
    }

    #endregion
}