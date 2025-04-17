using CSharper.Results;
using CSharper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Functional;

/// <summary>
/// Provides extension methods for handling synchronous <see cref="Result"/> operations
/// in a functional programming style.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Chains a <see cref="Result"/> to a synchronous operation if the result is successful; otherwise, returns the original result.
    /// </summary>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Result Bind(this Result result, Func<Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next() : result;
    }

    /// <summary>
    /// Chains a <see cref="Result"/> to a synchronous operation producing a typed result if successful; otherwise, maps the error to a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>The typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Result<T> Bind<T>(this Result result, Func<Result<T>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next() : result.MapError<T>();
    }

    /// <summary>
    /// Maps a failed <see cref="Result"/> to a typed failed result, preserving errors.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>A typed <see cref="Result{T}"/> with preserved errors if failed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> is successful.</exception>
    public static Result<T> MapError<T>(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Cannot map errors from a successful Result.");
        }
        IReadOnlyList<Error> errors = result.Errors;
        return Result.Fail<T>(errors[0], [.. errors.Skip(1)]);
    }

    /// <summary>
    /// Executes a synchronous success handler if the <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static T? Match<T>(this Result result, Func<T> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return result.IsSuccess ? onSuccess() : default;
    }

    /// <summary>
    /// Executes a synchronous handler based on the <see cref="Result"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the handlers.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <param name="onFailure">The synchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static T Match<T>(this Result result,
        Func<T> onSuccess, Func<Error[], T> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess ? onSuccess() : onFailure([.. result.Errors]);
    }

    /// <summary>
    /// Recovers from a failed <see cref="Result"/> by executing a synchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onFailure">The synchronous action to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is null.</exception>
    public static Result Recover(this Result result, Action<Error[]> onFailure)
    {
        onFailure.ThrowIfNull(nameof(onFailure));
        if (result.IsSuccess)
        {
            return result;
        }
        onFailure([.. result.Errors]);
        return Result.Ok();
    }

    /// <summary>
    /// Performs a synchronous side-effect if the <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The synchronous action to perform if <paramref name="result"/> is successful.</param>
    /// <returns>The original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Result Tap(this Result result, Action action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsSuccess)
        {
            action();
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect if the <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Result TapError(this Result result, Action<Error[]> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsFailure)
        {
            action([.. result.Errors]);
        }
        return result;
    }
}
