using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Functional;
using CSharper.Results;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Functional;

[DebuggerStepThrough]
/// <summary>
/// Provides extension methods for handling asynchronous <see cref="Result"/> and <see cref="Task{T}"/> operations
/// in a functional programming style.
/// </summary>
public static class AsyncResultExtensions
{
    #region Bind

    /// <summary>
    /// Chains a <see cref="Result"/> to an asynchronous operation if the result is successful; otherwise, returns the original result.
    /// </summary>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind(this Result result, Func<Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next() : Task.FromResult(result);
    }

    /// <summary>
    /// Chains a <see cref="Result"/> to an asynchronous operation producing a typed result if successful; otherwise, maps the error to a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<T>> Bind<T>(this Result result, Func<Task<Result<T>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next() : Task.FromResult(result.MapError<T>());
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to a synchronous operation if the result is successful.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind(this Task<Result> asyncResult, Func<Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.Then(r => r.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to a synchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<T>> Bind<T>(this Task<Result> asyncResult, Func<Result<T>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.Then(r => r.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to an asynchronous operation if the result is successful.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind(this Task<Result> asyncResult, Func<Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult
            .Then(r => r.Bind(next))
            .Unwrap();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to an asynchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<T>> Bind<T>(this Task<Result> asyncResult, Func<Task<Result<T>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult
            .Then(r => r.Bind(next))
            .Unwrap();
    }

    #endregion

    #region MapError

    /// <summary>
    /// Maps an asynchronous <see cref="Result"/> to a typed result, preserving errors if the result is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result with preserved errors if failed, or an empty successful result.</returns>
    public static Task<Result<T>> MapError<T>(this Task<Result> asyncResult)
    {
        return asyncResult.Then(r => r.MapError<T>());
    }

    #endregion

    #region Match

    /// <summary>
    /// Executes an asynchronous success handler if the <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<T?> Match<T>(this Result result, Func<Task<T>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        if (result.IsSuccess)
        {
            return onSuccess()!;
        }
        return Task.FromResult(default(T));
    }

    /// <summary>
    /// Executes an asynchronous handler based on the <see cref="Result"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the handlers.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<T> Match<T>(this Result result,
        Func<Task<T>> onSuccess, Func<Error, Task<T>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }

    /// <summary>
    /// Executes a synchronous success handler if the asynchronous <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<T?> Match<T>(this Task<Result> asyncResult, Func<T> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult.Then(r => r.Match(onSuccess));
    }

    /// <summary>
    /// Executes a synchronous handler based on the asynchronous <see cref="Result"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if the result is successful.</param>
    /// <param name="onFailure">The synchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<T> Match<T>(this Task<Result> asyncResult,
        Func<T> onSuccess, Func<Error, T> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult.Then(r => r.Match(onSuccess, onFailure));
    }

    /// <summary>
    /// Executes an asynchronous success handler if the asynchronous <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<T?> Match<T>(this Task<Result> asyncResult, Func<Task<T>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult
            .Then(r => r.Match(onSuccess))
            .Unwrap();
    }

    /// <summary>
    /// Executes an asynchronous handler based on the asynchronous <see cref="Result"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke if the result is successful.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<T> Match<T>(this Task<Result> asyncResult,
        Func<Task<T>> onSuccess, Func<Error, Task<T>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult
            .Then(r => r.Match(onSuccess, onFailure))
            .Unwrap();
    }

    #endregion

    #region Recover

    /// <summary>
    /// Recovers from a failed <see cref="Result"/> by executing an asynchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result> Recover(this Result result, Func<Error, Task> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return result.IsSuccess
            ? Task.FromResult(result)
            : fallback(result.Error!).Then(Result.Ok);
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result"/> by executing a synchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The synchronous action to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result> Recover(this Task<Result> asyncResult, Action<Error> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult.Then(r => r.Recover(fallback));
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result"/> by executing an asynchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result> Recover(this Task<Result> asyncResult, Func<Error, Task> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult
            .Then(r => r.Recover(fallback))
            .Unwrap();
    }

    #endregion

    #region Tap

    /// <summary>
    /// Performs an asynchronous side-effect if the <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> Tap(this Result result, Func<Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return result.IsSuccess
            ? action().Then(() => result)
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect if the asynchronous <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> Tap(this Task<Result> asyncResult, Action action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.Then(r => r.Tap(action));
    }

    /// <summary>
    /// Performs an asynchronous side-effect if the asynchronous <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> Tap(this Task<Result> asyncResult, Func<Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult
            .Then(r => r.Tap(action))
            .Unwrap();
    }

    #endregion

    #region TapError

    /// <summary>
    /// Performs an asynchronous side-effect if the <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> TapError(this Result result, Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return result.IsFailure
            ? action(result.Error!).Then(() => result)
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect if the asynchronous <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> TapError(this Task<Result> asyncResult, Action<Error> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.Then(r => r.TapError(action));
    }

    /// <summary>
    /// Performs an asynchronous side-effect if the asynchronous <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result> TapError(this Task<Result> asyncResult, Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult
            .Then(r => r.TapError(action))
            .Unwrap();
    }

    #endregion
}
