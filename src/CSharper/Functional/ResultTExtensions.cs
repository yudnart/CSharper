using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results;
using System;
using System.Diagnostics;

namespace CSharper.Functional;

/// <summary>
/// Provides extension methods for handling synchronous <see cref="Result{T}"/> operations
/// in a functional programming style.
/// </summary>
[DebuggerStepThrough]
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
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// Result Next(int value) => Result.Ok();
    /// Result final = result.Bind(Next);
    /// </code>
    /// </example>
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
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// Result&lt;string&gt; Next(int value) => Result.Ok(value.ToString());
    /// Result&lt;string&gt; final = result.Bind(Next);
    /// </code>
    /// </example>
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
    /// <returns>The transformed value in a new result, or a mapped error result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="transform"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// string Transform(int value) => value.ToString();
    /// Result&lt;string&gt; final = result.Map(Transform);
    /// </code>
    /// </example>
    public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> transform)
    {
        transform.ThrowIfNull(nameof(transform));
        return result.IsSuccess
            ? Result.Ok(transform(result.Value))
            : result.MapError<T, U>();
    }

    /// <summary>
    /// Maps a failed <see cref="Result{T}"/> to a non-generic <see cref="Result"/>, preserving the error.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>A non-generic result with the preserved error.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> is successful.</exception>
    /// <remarks>
    /// This method throws an <see cref="InvalidOperationException"/> if the result is successful, as mapping a success to a failed result is invalid.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// Result final = failed.MapError();
    /// </code>
    /// </example>
    public static Result MapError<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Success result cannot map to an failed result.");
        }
        return Result.Fail(result.Error!);
    }

    /// <summary>
    /// Maps a failed <see cref="Result{T}"/> to a typed <see cref="Result{U}"/>, preserving the error.
    /// </summary>
    /// <typeparam name="T">The type of the input result value.</typeparam>
    /// <typeparam name="U">The type of the output result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>A typed result with the preserved error.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> is successful.</exception>
    /// <remarks>
    /// This method throws an <see cref="InvalidOperationException"/> if the result is successful, as mapping a success to a failed result is invalid.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// Result&lt;string&gt; final = failed.MapError&lt;int, string&gt;();
    /// </code>
    /// </example>
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
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// string OnSuccess(int value) => value.ToString();
    /// string? final = result.Match(OnSuccess);
    /// </code>
    /// </example>
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
    /// <param name="onFailure">The synchronous function to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Fail&lt;int&gt;("Error");
    /// string OnSuccess(int value) => value.ToString();
    /// string OnFailure(Error e) => e.ToString();
    /// string final = result.Match(OnSuccess, OnFailure);
    /// </code>
    /// </example>
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
    /// <param name="fallback">The synchronous function to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result if successful, or the result of the fallback function if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="fallback"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// int Fallback(Error e) => 0;
    /// Result&lt;int&gt; final = failed.Recover(Fallback);
    /// </code>
    /// </example>
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
    /// <returns>The original result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// void OnSuccess(int value) => Console.WriteLine(value);
    /// Result&lt;int&gt; final = result.Tap(OnSuccess);
    /// </code>
    /// </example>
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
    /// <param name="action">The synchronous action to perform with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; failed = Result.Fail&lt;int&gt;("Error");
    /// void OnFailure(Error e) => Console.WriteLine(e.ToString());
    /// Result&lt;int&gt; final = failed.TapError(OnFailure);
    /// </code>
    /// </example>
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