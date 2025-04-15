using CSharper.Results;
using CSharper.Utilities;
using System;
using System.Threading.Tasks;

namespace CSharper.Functional;

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
    public static async Task<Result> Bind(this Result result, Func<Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? await next() : result;
    }

    /// <summary>
    /// Chains a <see cref="Result"/> to an asynchronous operation producing a typed result if successful; otherwise, maps the error to a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result<T>> Bind<T>(this Result result, Func<Task<Result<T>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? await next() : result.MapError<T>();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to a synchronous operation if the result is successful.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind(this Task<Result> asyncResult, Func<Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return (await asyncResult).Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to a synchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result<T>> Bind<T>(this Task<Result> asyncResult, Func<Result<T>> next)
    {
        next.ThrowIfNull(nameof(next));
        return (await asyncResult).Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to an asynchronous operation if the result is successful.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind(this Task<Result> asyncResult, Func<Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        Result result = await asyncResult;
        return await result.Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result"/> to an asynchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result<T>> Bind<T>(this Task<Result> asyncResult, Func<Task<Result<T>>> next)
    {
        next.ThrowIfNull(nameof(next));
        Result result = await asyncResult;
        return result.IsSuccess ? await next() : result.MapError<T>();
    }

    #endregion

    #region MapError

    /// <summary>
    /// Maps an asynchronous <see cref="Result"/> to a typed result, preserving errors if the result is a failure.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result with preserved errors if failed, or an empty successful result.</returns>
    public static async Task<Result<T>> MapError<T>(this Task<Result> asyncResult)
    {
        return (await asyncResult).MapError<T>();
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
    public static async Task<T?> Match<T>(this Result result, Func<Task<T>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return result.IsSuccess ? await onSuccess() : default;
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
        Func<Task<T>> onSuccess, Func<Error[], Task<T>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess ? onSuccess() : onFailure([.. result.Errors]);
    }

    /// <summary>
    /// Executes a synchronous success handler if the asynchronous <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static async Task<T?> Match<T>(this Task<Result> asyncResult, Func<T> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return (await asyncResult).Match(onSuccess);
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
    public static async Task<T> Match<T>(this Task<Result> asyncResult,
        Func<T> onSuccess, Func<Error[], T> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return (await asyncResult).Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Executes an asynchronous success handler if the asynchronous <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static async Task<T?> Match<T>(this Task<Result> asyncResult, Func<Task<T>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        Result result = await asyncResult;
        return await result.Match(onSuccess);
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
    public static async Task<T> Match<T>(this Task<Result> asyncResult,
        Func<Task<T>> onSuccess, Func<Error[], Task<T>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        Result result = await asyncResult;
        return await result.Match(onSuccess, onFailure);
    }

    #endregion

    #region Recover

    /// <summary>
    /// Recovers from a failed <see cref="Result"/> by executing an asynchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is null.</exception>
    public static async Task<Result> Recover(this Result result, Func<Error[], Task> onFailure)
    {
        onFailure.ThrowIfNull(nameof(onFailure));
        if (result.IsSuccess)
        {
            return result;
        }
        await onFailure([.. result.Errors]);
        return Result.Ok();
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result"/> by executing a synchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onFailure">The synchronous action to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is null.</exception>
    public static async Task<Result> Recover(this Task<Result> asyncResult, Action<Error[]> onFailure)
    {
        onFailure.ThrowIfNull(nameof(onFailure));
        return (await asyncResult).Recover(onFailure);
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result"/> by executing an asynchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is null.</exception>
    public static async Task<Result> Recover(this Task<Result> asyncResult, Func<Error[], Task> onFailure)
    {
        onFailure.ThrowIfNull(nameof(onFailure));
        Result result = await asyncResult;
        return await result.Recover(onFailure);
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
    public static async Task<Result> Tap(this Result result, Func<Task> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsSuccess)
        {
            await action();
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect if the asynchronous <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result> Tap(this Task<Result> asyncResult, Action action)
    {
        action.ThrowIfNull(nameof(action));
        return (await asyncResult).Tap(action);
    }

    /// <summary>
    /// Performs an asynchronous side-effect if the asynchronous <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result> Tap(this Task<Result> asyncResult, Func<Task> action)
    {
        action.ThrowIfNull(nameof(action));
        Result result = await asyncResult;
        return await result.Tap(action);
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
    public static async Task<Result> TapError(this Result result, Func<Error[], Task> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsFailure)
        {
            await action([.. result.Errors]);
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect if the asynchronous <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result> TapError(this Task<Result> asyncResult, Action<Error[]> action)
    {
        action.ThrowIfNull(nameof(action));
        return (await asyncResult).TapError(action);
    }

    /// <summary>
    /// Performs an asynchronous side-effect if the asynchronous <see cref="Result"/> is a failure, then returns the original result.
    /// </summary>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result> TapError(this Task<Result> asyncResult, Func<Error[], Task> action)
    {
        action.ThrowIfNull(nameof(action));
        Result result = await asyncResult;
        return await result.TapError(action);
    }

    #endregion
}