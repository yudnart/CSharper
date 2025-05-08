using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results;
using CSharper.Results.Validation;
using System;
using System.Diagnostics;

namespace CSharper.Functional;

[DebuggerStepThrough]
/// <summary>
/// Provides extension methods for handling synchronous <see cref="Result{T}"/> operations
/// in a functional programming style.
/// </summary>
public static class ResultTExtensions
{
    /// <summary>
    /// Chains a <see cref="Result{T}"/> to a synchronous operation producing a non-generic result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the chained operation, or a mapped error result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Result Bind<T>(this Result<T> result, Func<T, Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next(result.Value) : result.MapError();
    }

    /// <summary>
    /// Chains a <see cref="Result{T}"/> to a synchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The typed result of the chained operation, or a mapped error result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next(result.Value) : result.MapError<T, U>();
    }

    /// <summary>
    /// Transforms a <see cref="Result{T}"/> value using a synchronous mapping function if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="transform">The synchronous function to transform the value if <paramref name="result"/> is successful.</param>
    /// <returns>A new <see cref="Result{U}"/> with the transformed value, or a mapped error result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transform"/> is null.</exception>
    public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> transform)
    {
        transform.ThrowIfNull(nameof(transform));
        return result.IsSuccess
            ? Result.Ok(transform(result.Value))
            : result.MapError<T, U>();
    }

    /// <summary>
    /// Maps a failed <see cref="Result{T}"/> to a non-generic <see cref="Result"/>, preserving errors.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>A non-generic <see cref="Result"/> with preserved errors if failed, or a successful result if not failed.</returns>
    public static Result MapError<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Success result cannot map to an failed result.");
        }
        return Result.Fail(result.Error!);
    }

    /// <summary>
    /// Maps a failed <see cref="Result{T}"/> to a typed <see cref="Result{U}"/>, preserving errors.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>A typed <see cref="Result{U}"/> with preserved errors if failed, or a successful result if not failed.</returns>
    public static Result<U> MapError<T, U>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Success result cannot map to an failed result.");
        }
        return Result.Fail<U>(result.Error!);
    }

    /// <summary>
    /// Executes a synchronous success handler for a <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the success handler, or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static U? Match<T, U>(this Result<T> result, Func<T, U> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return result.IsSuccess ? onSuccess(result.Value) : default;
    }

    /// <summary>
    /// Executes a synchronous handler based on the <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <param name="onFailure">The synchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static U Match<T, U>(this Result<T> result,
        Func<T, U> onSuccess, Func<Error, U> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error!);
    }

    /// <summary>
    /// Recovers from a failed <see cref="Result{T}"/> by executing a synchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="fallback">The synchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result if successful, or the result of the fallback function if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Result<T> Recover<T>(this Result<T> result, Func<Error, T> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return result.IsSuccess ? result : Result.Ok(fallback(result.Error!));
    }

    /// <summary>
    /// Performs a synchronous side-effect for a <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect for a <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Result<T> TapError<T>(this Result<T> result, Action<Error> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsFailure)
        {
            action(result.Error!);
        }
        return result;
    }
}
