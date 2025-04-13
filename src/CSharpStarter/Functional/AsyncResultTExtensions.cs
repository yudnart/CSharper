using CSharpStarter.Results;
using CSharpStarter.Utilities;
using System;
using System.Threading.Tasks;

namespace CSharpStarter.Functional;

/// <summary>
/// Provides extension methods for handling asynchronous <see cref="Result{T}"/> and <see cref="Task{T}"/>
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
    public static async Task<Result> Bind<T>(this Result<T> result, Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? await next(result.Value) : result.MapError();
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
    public static async Task<Result<U>> Bind<T, U>(this Result<T> result, Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? await next(result.Value) : result.MapError<T, U>();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to a synchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind<T>(this Task<Result<T>> result, Func<T, Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return (await result).Bind(next);
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
    public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Result<U>> next)
    {
        next.ThrowIfNull(nameof(next));
        return (await asyncResult).Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to an asynchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind<T>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        Result<T> result = await asyncResult;
        return await result.Bind(next);
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
    public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        Result<T> result = await asyncResult;
        return result.IsSuccess ? await next(result.Value) : result.MapError<T, U>();
    }

    #endregion

    #region Ensure

    /// <summary>
    /// Validates a <see cref="Result{T}"/> using an asynchronous predicate, returning a builder to handle the outcome.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The asynchronous predicate to evaluate the value if <paramref name="result"/> is successful.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidationBuilder{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static async Task<ResultValidationBuilder<T>> Ensure<T>(this Result<T> result,
        Func<T, Task<bool>> predicate, Error error)
    {
        predicate.ThrowIfNull(nameof(predicate));
        error.ThrowIfNull(nameof(error));

        if (result.IsSuccess)
        {
            bool predicateResult = await predicate(result.Value);
            return result.Ensure(_ => predicateResult, error);
        }
        return new ResultValidationBuilder<T>(result);
    }

    /// <summary>
    /// Validates an asynchronous <see cref="Result{T}"/> using a synchronous predicate, returning a builder.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to validate.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the value if the result is successful.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidationBuilder{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static async Task<ResultValidationBuilder<T>> Ensure<T>(this Task<Result<T>> asyncResult,
        Func<T, bool> predicate, Error error)
    {
        predicate.ThrowIfNull(nameof(predicate));
        error.ThrowIfNull(nameof(error));
        return (await asyncResult).Ensure(predicate, error);
    }

    /// <summary>
    /// Validates an asynchronous <see cref="Result{T}"/> using an asynchronous predicate, returning a builder.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to validate.</param>
    /// <param name="predicate">The asynchronous predicate to evaluate the value if the result is successful.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidationBuilder{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static async Task<ResultValidationBuilder<T>> Ensure<T>(this Task<Result<T>> asyncResult,
        Func<T, Task<bool>> predicate, Error error)
    {
        predicate.ThrowIfNull(nameof(predicate));
        error.ThrowIfNull(nameof(error));
        Result<T> result = await asyncResult;
        return await result.Ensure(predicate, error);
    }

    #endregion

    #region Map

    /// <summary>
    /// Transforms a <see cref="Result{T}"/> value using an asynchronous mapping function if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="map">The asynchronous function to transform the value if <paramref name="result"/> is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the transformed result or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
    public static async Task<Result<U>> Map<T, U>(this Result<T> result, Func<T, Task<U>> map)
    {
        map.ThrowIfNull(nameof(map));
        if (result.IsSuccess)
        {
            U value = await map(result.Value);
            return Result.Ok(value);
        }
        return result.MapError<T, U>();
    }

    /// <summary>
    /// Transforms an asynchronous <see cref="Result{T}"/> value using a synchronous mapping function if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to transform.</param>
    /// <param name="map">The synchronous function to transform the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the transformed result or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
    public static async Task<Result<U>> Map<T, U>(this Task<Result<T>> asyncResult, Func<T, U> map)
    {
        map.ThrowIfNull(nameof(map));
        return (await asyncResult).Map(map);
    }

    /// <summary>
    /// Transforms an asynchronous <see cref="Result{T}"/> value using an asynchronous mapping function if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to transform.</param>
    /// <param name="map">The asynchronous function to transform the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the transformed result or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="map"/> is null.</exception>
    public static async Task<Result<U>> Map<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<U>> map)
    {
        map.ThrowIfNull(nameof(map));
        Result<T> result = await asyncResult;
        return await result.Map(map);
    }

    #endregion

    #region MapError

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to an untyped result, preserving errors if failed.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing an untyped result with preserved errors if failed.</returns>
    public static async Task<Result> MapError<T>(this Task<Result<T>> asyncResult)
    {
        return (await asyncResult).MapError();
    }

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to a typed result, preserving errors if failed.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A <see cref="Task{T}"/> containing a typed result with preserved errors if failed.</returns>
    public static async Task<Result<U>> MapError<T, U>(this Task<Result<T>> asyncResult)
    {
        return (await asyncResult).MapError<T, U>();
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
    public static async Task<U?> Match<T, U>(this Result<T> result, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return result.IsSuccess ? await onSuccess(result.Value) : default;
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
    public static async Task<U> Match<T, U>(this Result<T> result,
        Func<T, Task<U>> onSuccess, Func<Error[], Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure([.. result.Errors]);
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
    public static async Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, U> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return (await asyncResult).Match(onSuccess);
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
    public static async Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, U> onSuccess, Func<Error[], U> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return (await asyncResult).Match(onSuccess, onFailure);
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
    public static async Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        Result<T> result = await asyncResult;
        return result.IsSuccess ? await onSuccess(result.Value) : default;
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
    public static async Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<U>> onSuccess, Func<Error[], Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        Result<T> result = await asyncResult;
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure([.. result.Errors]);
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
    public static async Task<Result<T>> Recover<T>(this Result<T> result,
        Func<Error[], Task<Result<T>>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        if (result.IsSuccess)
        {
            return result;
        }
        return await fallback([.. result.Errors]);
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using a synchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The synchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static async Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error[], Result<T>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return (await asyncResult).Recover(fallback);
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using an asynchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    public static async Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error[], Task<Result<T>>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        Result<T> result = await asyncResult;
        return await result.Recover(fallback);
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
    public static async Task<Result<T>> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsSuccess)
        {
            await action(result.Value);
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Action<T> action)
    {
        action.ThrowIfNull(nameof(action));
        return (await asyncResult).Tap(action);
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the value if the result is successful.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        Result<T> result = await asyncResult;
        return await result.Tap(action);
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
    public static async Task<Result<T>> TapError<T>(this Result<T> result, Func<Error[], Task> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsFailure)
        {
            await action([.. result.Errors]);
        }
        return result;
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Action<Error[]> action)
    {
        action.ThrowIfNull(nameof(action));
        return (await asyncResult).TapError(action);
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with errors if the result is a failure.</param>
    /// <returns>A <see cref="Task{T}"/> containing the original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    public static async Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Func<Error[], Task> action)
    {
        action.ThrowIfNull(nameof(action));
        Result<T> result = await asyncResult;
        return await result.TapError(action);
    }

    #endregion
}
