using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Functional;

[DebuggerStepThrough]
/// <summary>
/// Provides extension methods for handling asynchronous <see cref="Result{T}"/> 
/// and <see cref="T:Task{T}"/> where T is <see cref="Result{T}"/>
/// operations in a functional programming style.
/// </summary>
public static class AsyncResultTExtensions
{
    #region Bind

    /// <summary>
    /// Chains a <see cref="Result{T}"/> to an asynchronous operation if successful, using the result's value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind<T>(this Result<T> result, Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        //return result.IsSuccess ? await next(result.Value) : result.MapError();
        return result.IsSuccess
            ? next(result.Value).ContinueWith(task => task
                .HandleFault().Or(task => task.Result))
            : Task.FromResult(result.MapError());
    }

    /// <summary>
    /// Chains a <see cref="Result{T}"/> to an asynchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<U>> Bind<T, U>(this Result<T> result, Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess
            ? next(result.Value).ContinueWith(task => task
                .HandleFault().Or(task => task.Result))
            : Task.FromResult(result.MapError<T, U>());
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to a synchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind<T>(this Task<Result<T>> asyncResult, Func<T, Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Bind(next)));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to a synchronous operation producing a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Result<U>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Bind(next)));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to an asynchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind<T>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Bind(next)))
            .Unwrap();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to an asynchronous operation producing a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Bind(next)))
            .Unwrap();
    }

    #endregion

    #region Map

    /// <summary>
    /// Transforms an asynchronous <see cref="Result{T}"/> value using a synchronous mapping function if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to transform.</param>
    /// <param name="transform">The synchronous function to transform the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the transformed result or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transform"/> is null.</exception>
    public static Task<Result<U>> Map<T, U>(this Task<Result<T>> asyncResult, Func<T, U> transform)
    {
        transform.ThrowIfNull(nameof(transform));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Map(transform)));
    }

    #endregion

    #region MapError

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to an untyped result, preserving errors if failed.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing an untyped result with preserved errors if failed.</returns>
    public static Task<Result> MapError<T>(this Task<Result<T>> asyncResult)
    {
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.MapError()));
    }

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to a typed result, preserving errors if failed.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing a typed result with preserved errors if failed.</returns>
    public static Task<Result<U>> MapError<T, U>(this Task<Result<T>> asyncResult)
    {
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.MapError<T, U>()));
    }

    #endregion

    #region Match

    /// <summary>
    /// Executes an asynchronous success handler for a <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<U?> Match<T, U>(this Result<T> result, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        if (result.IsSuccess)
        {
            return onSuccess(result.Value).ContinueWith(task => task
                .HandleFault().Or(task => task.Result))!;
        }
        return Task.FromResult(default(U));
    }

    /// <summary>
    /// Executes an asynchronous handler based on the <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<U> Match<T, U>(this Result<T> result,
        Func<T, Task<U>> onSuccess, Func<Error, Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error!);
    }

    /// <summary>
    /// Executes a synchronous success handler for an asynchronous <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, U> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Match(onSuccess)));
    }

    /// <summary>
    /// Executes a synchronous handler based on the asynchronous <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if the result is successful.</param>
    /// <param name="onFailure">The synchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, U> onSuccess, Func<Error, U> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Match(onSuccess, onFailure)));
    }

    /// <summary>
    /// Executes an asynchronous success handler for an asynchronous <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    public static Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Match(onSuccess)))
            .Unwrap();
    }

    /// <summary>
    /// Executes an asynchronous handler based on the asynchronous <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <param name="onFailure">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    public static Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<U>> onSuccess, Func<Error, Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Match(onSuccess, onFailure)))
            .Unwrap();
    }

    #endregion

    #region Recover

    /// <summary>
    /// Recovers from a failed <see cref="Result{T}"/> by executing an asynchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result<T>> Recover<T>(this Result<T> result,
        Func<Error, Task<T>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return result.IsSuccess
            ? Task.FromResult(result)
            : fallback(result.Error!).ContinueWith(task => task
                .HandleFault().Or(task => Result.Ok(task.Result)));
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using a synchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The synchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error, T> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Recover(fallback)));
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using an asynchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error, Task<T>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Recover(fallback)))
            .Unwrap();
    }

    #endregion

    #region Tap

    /// <summary>
    /// Performs an asynchronous side-effect for a <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the value if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return result.IsSuccess
            ? action(result.Value).ContinueWith(task => task
                .HandleFault().Or(task => result))
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Action<T> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Tap(action)));
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.Tap(action)))
            .Unwrap();
    }

    #endregion

    #region TapError

    /// <summary>
    /// Performs an asynchronous side-effect for a <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if <paramref name="result"/> is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> TapError<T>(this Result<T> result, Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        action.ThrowIfNull(nameof(action));
        return result.IsFailure
            ? action(result.Error).ContinueWith(task => task
                .HandleFault().Or(task => result))
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Action<Error> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.TapError(action)));
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.ContinueWith(task => task
            .HandleFault().Or(task => task.Result.TapError(action)))
            .Unwrap();
    }

    #endregion
}
