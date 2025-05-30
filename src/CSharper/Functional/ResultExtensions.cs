﻿using CSharper.Errors;
using CSharper.Extensions;
using CSharper.Results;
using System;
using System.Diagnostics;

namespace CSharper.Functional;

/// <summary>
/// Provides extension methods for handling <see cref="Result"/> operations
/// in a functional programming style.
/// </summary>
[DebuggerStepThrough]
public static class ResultExtensions
{
    /// <summary>
    /// Chains a <see cref="Result"/> to a synchronous operation if successful; otherwise, returns the original result.
    /// </summary>
    /// <param name="result">The initial result to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the chained operation or the original result if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result initial = Result.Ok();
    /// Result Next() => Result.Ok();
    /// Result final = initial.Bind(Next);
    /// </code>
    /// </example>
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
    /// <example>
    /// <code>
    /// Result initial = Result.Ok();
    /// Result&lt;int&gt; Next() => Result.Ok(42);
    /// Result&lt;int&gt; final = initial.Bind(Next);
    /// </code>
    /// </example>
    public static Result<T> Bind<T>(this Result result, Func<Result<T>> next)
    {
        next.ThrowIfNull(nameof(next));
        return result.IsSuccess ? next() : result.MapError<T>();
    }

    /// <summary>
    /// Maps a failed <see cref="Result"/> to a typed failed result, preserving the error.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <returns>The typed result with the preserved error if failed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="result"/> is successful.</exception>
    /// <remarks>
    /// This method throws an <see cref="InvalidOperationException"/> if the result is successful, as mapping a success to a failed result is invalid.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result failed = Result.Fail("Error");
    /// Result&lt;int&gt; final = failed.MapError&lt;int&gt;();
    /// </code>
    /// </example>
    public static Result<T> MapError<T>(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Success result cannot map to an failed result.");
        }
        return Result.Fail<T>(result.Error!);
    }

    /// <summary>
    /// Executes a synchronous success handler if the <see cref="Result"/> is successful; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the success handler.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onSuccess">The synchronous function to invoke if <paramref name="result"/> is successful.</param>
    /// <returns>The result of the success handler or <c>default(T)</c> if failed.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result result = Result.Ok();
    /// string OnSuccess() => "Success";
    /// string? final = result.Match(OnSuccess);
    /// </code>
    /// </example>
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
    /// <param name="onFailure">The synchronous function to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The result of the appropriate handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result result = Result.Fail("Error");
    /// string OnSuccess() => "Success";
    /// string OnFailure(Error e) => e.ToString();
    /// string final = result.Match(OnSuccess, OnFailure);
    /// </code>
    /// </example>
    public static T Match<T>(this Result result, Func<T> onSuccess, Func<Error, T> onFailure)
    {
        onSuccess.ThrowIfNull(nameof(onSuccess));
        onFailure.ThrowIfNull(nameof(onFailure));
        return result.IsSuccess ? onSuccess() : onFailure(result.Error!);
    }

    /// <summary>
    /// Recovers from a failed <see cref="Result"/> by executing a synchronous failure handler and returning a successful result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="onFailure">The synchronous action to invoke with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original result if successful, or a successful <see cref="Result"/> after handling failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="onFailure"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result failed = Result.Fail("Error");
    /// void OnFailure(Error e) => Console.WriteLine(e.ToString());
    /// Result final = failed.Recover(OnFailure);
    /// </code>
    /// </example>
    public static Result Recover(this Result result, Action<Error> onFailure)
    {
        onFailure.ThrowIfNull(nameof(onFailure));
        if (result.IsSuccess)
        {
            return result;
        }
        onFailure(result.Error!);
        return Result.Ok();
    }

    /// <summary>
    /// Performs a synchronous side-effect if the <see cref="Result"/> is successful, then returns the original result.
    /// </summary>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The synchronous action to perform if <paramref name="result"/> is successful.</param>
    /// <returns>The original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result result = Result.Ok();
    /// void OnSuccess() => Console.WriteLine("Success");
    /// Result final = result.Tap(OnSuccess);
    /// </code>
    /// </example>
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
    /// <param name="action">The synchronous action to perform with the error if <paramref name="result"/> is a failure.</param>
    /// <returns>The original <see cref="Result"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is null.</exception>
    /// <example>
    /// <code>
    /// Result failed = Result.Fail("Error");
    /// void OnFailure(Error e) => Console.WriteLine(e.ToString());
    /// Result final = failed.TapError(OnFailure);
    /// </code>
    /// </example>
    public static Result TapError(this Result result, Action<Error> action)
    {
        action.ThrowIfNull(nameof(action));
        if (result.IsFailure)
        {
            action(result.Error!);
        }
        return result;
    }
}