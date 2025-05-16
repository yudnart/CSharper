using CSharper.Extensions;
using CSharper.Results;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Functional.Validation;

/// <summary>
/// Provides extension methods for handling <see cref="ResultValidator"/> operations
/// in a functional programming style.
/// </summary>
[DebuggerStepThrough]
public static class ResultValidatorExtensions
{
    #region And

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain of an asynchronous <see cref="ResultValidator"/>.
    /// </summary>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> containing the validation chain.</param>
    /// <param name="predicate">The condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing the <see cref="ResultValidator"/> for further chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; validator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// Task&lt;ResultValidator&gt; chained = validator.And(() => true, "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncValidator
            .Then(v => v.And(predicate, errorMessage, errorCode, path));
    }

    /// <summary>
    /// Adds an asynchronous predicate and associated error to the validation chain of an asynchronous <see cref="ResultValidator"/>.
    /// </summary>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> containing the validation chain.</param>
    /// <param name="predicate">The asynchronous condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing the <see cref="ResultValidator"/> for further chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; validator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// Task&lt;ResultValidator&gt; chained = validator.And(async () => await Task.FromResult(true), "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncValidator
            .Then(v => v.And(predicate, errorMessage, errorCode, path));
    }

    #endregion

    #region Ensure

    /// <summary>
    /// Starts a validation chain for a synchronous <see cref="Result"/> with a synchronous predicate.
    /// </summary>
    /// <param name="result">The initial <see cref="Result"/> to validate.</param>
    /// <param name="predicate">The condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A new <see cref="ResultValidator"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result result = Result.Ok();
    /// ResultValidator validator = result.Ensure(() => true, "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static ResultValidator Ensure(this Result result,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        result.ThrowIfNull(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    /// <summary>
    /// Starts a validation chain for a synchronous <see cref="Result"/> with an asynchronous predicate.
    /// </summary>
    /// <param name="result">The initial <see cref="Result"/> to validate.</param>
    /// <param name="predicate">The asynchronous condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A new <see cref="ResultValidator"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result result = Result.Ok();
    /// ResultValidator validator = result.Ensure(async () => await Task.FromResult(true), "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static ResultValidator Ensure(this Result result,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        result.ThrowIfNull(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    /// <summary>
    /// Starts a validation chain for an asynchronous <see cref="Result"/> with a synchronous predicate.
    /// </summary>
    /// <param name="asyncResult">The asynchronous <see cref="Result"/> to validate.</param>
    /// <param name="predicate">The condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing a new <see cref="ResultValidator"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncResult"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;Result&gt; asyncResult = Task.FromResult(Result.Ok());
    /// Task&lt;ResultValidator&gt; validator = asyncResult.Ensure(() => true, "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncResult.ThrowIfNull(nameof(asyncResult));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(v => v.Ensure(predicate, errorMessage, errorCode, path));
    }

    /// <summary>
    /// Starts a validation chain for an asynchronous <see cref="Result"/> with an asynchronous predicate.
    /// </summary>
    /// <param name="asyncResult">The asynchronous <see cref="Result"/> to validate.</param>
    /// <param name="predicate">The asynchronous condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing a new <see cref="ResultValidator"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncResult"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;Result&gt; asyncResult = Task.FromResult(Result.Ok());
    /// Task&lt;ResultValidator&gt; validator = asyncResult.Ensure(async () => await Task.FromResult(true), "Invalid input", "INVALID", "input");
    /// </code>
    /// </example>
    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncResult.ThrowIfNull(nameof(asyncResult));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(v => v.Ensure(predicate, errorMessage, errorCode, path));
    }

    #endregion

    #region Bind

    /// <summary>
    /// Chains a <see cref="ResultValidator"/> to a synchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <param name="validator">The <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if validation succeeds.</param>
    /// <returns>The result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator validator = new ResultValidator(Result.Ok());
    /// Result Next() => Result.Ok();
    /// Result final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Result Bind(this ResultValidator validator, Func<Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator"/> to a synchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if validation succeeds.</param>
    /// <returns>The typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator validator = new ResultValidator(Result.Ok());
    /// Result&lt;int&gt; Next() => Result.Ok(42);
    /// Result&lt;int&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Result<T> Bind<T>(this ResultValidator validator, Func<Result<T>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator"/> to an asynchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <param name="validator">The <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator validator = new ResultValidator(Result.Ok());
    /// async Task&lt;Result&gt; Next() => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind(this ResultValidator validator, Func<Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.ValidateAsync().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator"/> to an asynchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator validator = new ResultValidator(Result.Ok());
    /// async Task&lt;Result&lt;int&gt;&gt; Next() => await Task.FromResult(Result.Ok(42));
    /// Task&lt;Result&lt;int&gt;&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<T>> Bind<T>(this ResultValidator validator, Func<Task<Result<T>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.ValidateAsync().Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator"/> to a synchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; asyncValidator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// Result Next() => Result.Ok();
    /// Task&lt;Result&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator"/> to a synchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; asyncValidator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// Result&lt;int&gt; Next() => Result.Ok(42);
    /// Task&lt;Result&lt;int&gt;&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<T>> Bind<T>(this Task<ResultValidator> asyncValidator, Func<Result<T>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator"/> to an asynchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; asyncValidator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// async Task&lt;Result&gt; Next() => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator
            .Then(v => v.Bind(next))
            .Unwrap();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator"/> to an asynchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&gt; asyncValidator = Task.FromResult(new ResultValidator(Result.Ok()));
    /// async Task&lt;Result&lt;int&gt;&gt; Next() => await Task.FromResult(Result.Ok(42));
    /// Task&lt;Result&lt;int&gt;&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<T>> Bind<T>(this Task<ResultValidator> asyncValidator, Func<Task<Result<T>>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator
            .Then(v => v.Bind(next))
            .Unwrap();
    }

    #endregion
}