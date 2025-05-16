using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Functional;

/// <summary>
/// Provides extension methods for handling asynchronous <see cref="Result{T}"/> 
/// and <see cref="Task{T}"/> operations where T is <see cref="Result{T}"/> 
/// in a functional programming style.
/// </summary>
[DebuggerStepThrough]
public static class AsyncResultTExtensions
{
    #region Bind

    /// <summary>
    /// Chains a <see cref="Result{T}"/> to an asynchronous operation if successful, using the result's value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// async Task&lt;Result&gt; NextAsync(int value) => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = result.Bind(NextAsync);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(this Result<T> result, Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess
            ? next(result.Value)
            : Task.FromResult(result.MapError());
    }

    /// <summary>
    /// Chains a <see cref="Result{T}"/> to an asynchronous operation producing a typed result if successful.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if <paramref name="result"/> is successful.</param>
    /// <returns>The typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// async Task&lt;Result&lt;string&gt;&gt; NextAsync(int value) => await Task.FromResult(Result.Ok(value.ToString()));
    /// Task&lt;Result&lt;string&gt;&gt; final = result.Bind(NextAsync);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(this Result<T> result, Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess
            ? next(result.Value)
            : Task.FromResult(result.MapError<T, U>());
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to a synchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// Result Next(int value) => Result.Ok();
    /// Task&lt;Result&gt; final = asyncResult.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(this Task<Result<T>> asyncResult, Func<T, Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.Then(r => r.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to a synchronous operation producing a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// Result&lt;string&gt; Next(int value) => Result.Ok(value.ToString());
    /// Task&lt;Result&lt;string&gt;&gt; final = asyncResult.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Result<U>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult.Then(r => r.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to an asynchronous operation if successful.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The result of the chained operation or the mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// async Task&lt;Result&gt; NextAsync(int value) => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = asyncResult.Bind(NextAsync);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult
            .Then(r => r.Bind(next))
            .Unwrap();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="Result{T}"/> to an asynchronous operation producing a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The typed result of the chained operation or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// async Task&lt;Result&lt;string&gt;&gt; NextAsync(int value) => await Task.FromResult(Result.Ok(value.ToString()));
    /// Task&lt;Result&lt;string&gt;&gt; final = asyncResult.Bind(NextAsync);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return asyncResult
            .Then(r => r.Bind(next))
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
    /// <returns>The transformed result or a mapped error result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transform"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// string Transform(int value) => value.ToString();
    /// Task&lt;Result&lt;string&gt;&gt; final = asyncResult.Map(Transform);
    /// </code>
    /// </example>
    public static Task<Result<U>> Map<T, U>(this Task<Result<T>> asyncResult, Func<T, U> transform)
    {
        transform.ThrowIfNull(nameof(transform));
        return asyncResult.Then(r => r.Map(transform));
    }

    #endregion

    #region MapError

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to an untyped result, preserving the error if failed.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>An untyped result with the preserved error if failed.</returns>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// Task&lt;Result&gt; final = failed.MapError();
    /// </code>
    /// </example>
    public static Task<Result> MapError<T>(this Task<Result<T>> asyncResult)
    {
        return asyncResult.Then(r => r.MapError());
    }

    /// <summary>
    /// Maps an asynchronous <see cref="Result{T}"/> to a typed result, preserving the error if failed.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to map.</param>
    /// <returns>A typed result with the preserved error if failed.</returns>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// Task&lt;Result&lt;string&gt;&gt; final = failed.MapError&lt;int, string&gt;();
    /// </code>
    /// </example>
    public static Task<Result<U>> MapError<T, U>(this Task<Result<T>> asyncResult)
    {
        return asyncResult.Then(r => r.MapError<T, U>());
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
    /// <returns>The result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// async Task&lt;string&gt; OnSuccessAsync(int value) => await Task.FromResult(value.ToString());
    /// Task&lt;string?&gt; final = result.Match(OnSuccessAsync);
    /// </code>
    /// </example>
    public static Task<U?> Match<T, U>(this Result<T> result, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        if (result.IsSuccess)
        {
            return onSuccess(result.Value)!;
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
    /// <param name="onFailure">The asynchronous function to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Fail&lt;int&gt;("Error");
    /// async Task&lt;string&gt; OnSuccessAsync(int value) => await Task.FromResult(value.ToString());
    /// async Task&lt;string&gt; OnFailureAsync(Error e) => await Task.FromResult(e.ToString());
    /// Task&lt;string&gt; final = result.Match(OnSuccessAsync, OnFailureAsync);
    /// </code>
    /// </example>
    public static Task<U> Match<T, U>(this Result<T> result,
        Func<T, Task<U>> onSuccess, Func<Error, Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error!);
    }

    /// <summary>
    /// Executes an asynchronous success handler for an asynchronous <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// async Task&lt;string&gt; OnSuccessAsync(int value) => await Task.FromResult(value.ToString());
    /// Task&lt;string?&gt; final = asyncResult.Match(OnSuccessAsync);
    /// </code>
    /// </example>
    public static Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<U>> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult
            .Then(r => r.Match(onSuccess))
            .Unwrap();
    }

    /// <summary>
    /// Executes an asynchronous handler based on the asynchronous <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The asynchronous function to invoke with the value if the result is successful.</param>
    /// <param name="onFailure">The asynchronous function to invoke with the error if the result is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// async Task&lt;string&gt; OnSuccessAsync(int value) => await Task.FromResult(value.ToString());
    /// async Task&lt;string&gt; OnFailureAsync(Error e) => await Task.FromResult(e.ToString());
    /// Task&lt;string&gt; final = asyncResult.Match(OnSuccessAsync, OnFailureAsync);
    /// </code>
    /// </example>
    public static Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, Task<U>> onSuccess, Func<Error, Task<U>> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult
            .Then(r => r.Match(onSuccess, onFailure))
            .Unwrap();
    }

    /// <summary>
    /// Executes a synchronous success handler for an asynchronous <see cref="Result{T}"/> if successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the success handler.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if the result is successful.</param>
    /// <returns>The result of the success handler or <c>default(U)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// string OnSuccess(int value) => value.ToString();
    /// Task&lt;string?&gt; final = asyncResult.Match(OnSuccess);
    /// </code>
    /// </example>
    public static Task<U?> Match<T, U>(this Task<Result<T>> asyncResult, Func<T, U> onSuccess)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        return asyncResult.Then(r => r.Match(onSuccess));
    }

    /// <summary>
    /// Executes a synchronous handler based on the asynchronous <see cref="Result{T}"/> state (success or failure).
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the value returned by the handlers.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke with the value if the result is successful.</param>
    /// <param name="onFailure">The synchronous function to invoke with the error if the result is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// string OnSuccess(int value) => value.ToString();
    /// string OnFailure(Error e) => e.ToString();
    /// Task&lt;string&gt; final = asyncResult.Match(OnSuccess, OnFailure);
    /// </code>
    /// </example>
    public static Task<U> Match<T, U>(this Task<Result<T>> asyncResult,
        Func<T, U> onSuccess, Func<Error, U> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return asyncResult.Then(r => r.Match(onSuccess, onFailure));
    }

    #endregion

    #region Recover

    /// <summary>
    /// Recovers from a failed <see cref="Result{T}"/> by executing an asynchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// async Task&lt;int&gt; FallbackAsync(Error e) => await Task.FromResult(0);
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.Recover(FallbackAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> Recover<T>(this Result<T> result,
        Func<Error, Task<T>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return result.IsSuccess
            ? Task.FromResult(result)
            : fallback(result.Error!).Then(Result.Ok);
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using a synchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The synchronous function to invoke with the error if the result is a failure.</param>
    /// <returns>The original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// int Fallback(Error e) => 0;
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.Recover(Fallback);
    /// </code>
    /// </example>
    public static Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error, T> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult.Then(r => r.Recover(fallback));
    }

    /// <summary>
    /// Recovers from a failed asynchronous <see cref="Result{T}"/> using an asynchronous fallback function.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="fallback">The asynchronous function to invoke with the error if the result is a failure.</param>
    /// <returns>The original result if successful, or the result of the fallback function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// async Task&lt;int&gt; FallbackAsync(Error e) => await Task.FromResult(0);
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.Recover(FallbackAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> Recover<T>(this Task<Result<T>> asyncResult,
        Func<Error, Task<T>> fallback)
    {
        fallback.ThrowIfNull(nameof(fallback));
        return asyncResult
            .Then(r => r.Recover(fallback))
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
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// async Task OnSuccessAsync(int value) => Console.WriteLine(value);
    /// Task&lt;Result&lt;int&gt;&gt; final = result.Tap(OnSuccessAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> Tap<T>(this Result<T> result, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return result.IsSuccess
            ? action(result.Value).Then(() => result)
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with the value if the result is successful.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// void OnSuccess(int value) => Console.WriteLine(value);
    /// Task&lt;Result&lt;int&gt;&gt; final = asyncResult.Tap(OnSuccess);
    /// </code>
    /// </example>
    public static Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Action<T> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.Then(r => r.Tap(action));
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if successful, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the value if the result is successful.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// async Task OnSuccessAsync(int value) => Console.WriteLine(value);
    /// Task&lt;Result&lt;int&gt;&gt; final = asyncResult.Tap(OnSuccessAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> Tap<T>(this Task<Result<T>> asyncResult, Func<T, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult
            .Then(r => r.Tap(action))
            .Unwrap();
    }

    #endregion

    #region TapError

    /// <summary>
    /// Performs an asynchronous side-effect for a <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// async Task OnFailureAsync(Error e) => Console.WriteLine(e.ToString());
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.TapError(OnFailureAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> TapError<T>(this Result<T> result, Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return result.IsFailure
            ? action(result.Error!).Then(() => result)
            : Task.FromResult(result);
    }

    /// <summary>
    /// Performs a synchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The synchronous action to perform with the error if the result is a failure.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// void OnFailure(Error e) => Console.WriteLine(e.ToString());
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.TapError(OnFailure);
    /// </code>
    /// </example>
    public static Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Action<Error> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult.Then(r => r.TapError(action));
    }

    /// <summary>
    /// Performs an asynchronous side-effect for an asynchronous <see cref="Result{T}"/> if failed, then returns the original result.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to evaluate.</param>
    /// <param name="action">The asynchronous action to perform with the error if the result is a failure.</param>
    /// <returns>The original <see cref="Result{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; failed = Task.FromResult(Result.Fail&lt;int&gt;("Error"));
    /// async Task OnFailureAsync(Error e) => Console.WriteLine(e.ToString());
    /// Task&lt;Result&lt;int&gt;&gt; final = failed.TapError(OnFailureAsync);
    /// </code>
    /// </example>
    public static Task<Result<T>> TapError<T>(this Task<Result<T>> asyncResult,
        Func<Error, Task> action)
    {
        action.ThrowIfNull(nameof(action));
        return asyncResult
            .Then(r => r.TapError(action))
            .Unwrap();
    }

    #endregion
}