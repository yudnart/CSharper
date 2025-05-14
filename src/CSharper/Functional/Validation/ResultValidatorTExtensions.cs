using CSharper.Extensions;
using CSharper.Functional;
using CSharper.Results.Validation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// Provides extension methods for handling synchronous and asynchronous <see cref="ResultValidator{T}"/> operations
/// in a functional programming style.
/// </summary>
[DebuggerStepThrough]
public static class ResultValidatorTExtensions
{
    #region And

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain of an asynchronous <see cref="ResultValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="predicate">The synchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing the <see cref="ResultValidator{T}"/> for further chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result{T}"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; validator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// Task&lt;ResultValidator&lt;int&gt;&gt; chained = validator.And(x => x > 0, "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static Task<ResultValidator<T>> And<T>(this Task<ResultValidator<T>> asyncValidator,
        Func<T, bool> predicate,
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
    /// Adds an asynchronous predicate and associated error to the validation chain of an asynchronous <see cref="ResultValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="predicate">The asynchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing the <see cref="ResultValidator{T}"/> for further chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result{T}"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; validator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// Task&lt;ResultValidator&lt;int&gt;&gt; chained = validator.And(async x => await Task.FromResult(x > 0), "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static Task<ResultValidator<T>> And<T>(this Task<ResultValidator<T>> asyncValidator,
        Func<T, Task<bool>> predicate,
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
    /// Starts a validation chain for a synchronous <see cref="Result{T}"/> with a synchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial <see cref="Result{T}"/> to validate.</param>
    /// <param name="predicate">The synchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A new <see cref="ResultValidator{T}"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// ResultValidator&lt;int&gt; validator = result.Ensure(x => x > 0, "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static ResultValidator<T> Ensure<T>(this Result<T> result,
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        result.ThrowIfNull(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator<T>(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    /// <summary>
    /// Starts a validation chain for a synchronous <see cref="Result{T}"/> with an asynchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The initial <see cref="Result{T}"/> to validate.</param>
    /// <param name="predicate">The asynchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A new <see cref="ResultValidator{T}"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result.Ok(42);
    /// ResultValidator&lt;int&gt; validator = result.Ensure(async x => await Task.FromResult(x > 0), "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static ResultValidator<T> Ensure<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        result.ThrowIfNull(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator<T>(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    /// <summary>
    /// Starts a validation chain for an asynchronous <see cref="Result{T}"/> with a synchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous <see cref="Result{T}"/> to validate.</param>
    /// <param name="predicate">The synchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing a new <see cref="ResultValidator{T}"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncResult"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// Task&lt;ResultValidator&lt;int&gt;&gt; validator = asyncResult.Ensure(x => x > 0, "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static Task<ResultValidator<T>> Ensure<T>(
        this Task<Result<T>> asyncResult,
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncResult.ThrowIfNull(nameof(asyncResult));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(r => r.Ensure(predicate, errorMessage, errorCode, path));
    }

    /// <summary>
    /// Starts a validation chain for an asynchronous <see cref="Result{T}"/> with an asynchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous <see cref="Result{T}"/> to validate.</param>
    /// <param name="predicate">The asynchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>A task containing a new <see cref="ResultValidator{T}"/> for chaining validation rules.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncResult"/> or <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// This method initializes a new validation chain with the provided predicate.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;Result&lt;int&gt;&gt; asyncResult = Task.FromResult(Result.Ok(42));
    /// Task&lt;ResultValidator&lt;int&gt;&gt; validator = asyncResult.Ensure(async x => await Task.FromResult(x > 0), "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public static Task<ResultValidator<T>> Ensure<T>(
        this Task<Result<T>> asyncResult,
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        asyncResult.ThrowIfNull(nameof(asyncResult));
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(r => r.Ensure(predicate, errorMessage, errorCode, path));
    }

    #endregion

    #region Bind

    /// <summary>
    /// Chains a <see cref="ResultValidator{T}"/> to a synchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>The result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator&lt;int&gt; validator = new ResultValidator&lt;int&gt;(Result.Ok(42));
    /// Result Next(int value) => Result.Ok();
    /// Result final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Result Bind<T>(this ResultValidator<T> validator, Func<T, Result> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator{T}"/> to a synchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>The typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator&lt;int&gt; validator = new ResultValidator&lt;int&gt;(Result.Ok(42));
    /// Result&lt;string&gt; Next(int value) => Result.Ok(value.ToString());
    /// Result&lt;string&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Result<U> Bind<T, U>(
        this ResultValidator<T> validator, Func<T, Result<U>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator{T}"/> to an asynchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator&lt;int&gt; validator = new ResultValidator&lt;int&gt;(Result.Ok(42));
    /// async Task&lt;Result&gt; Next(int value) => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(
        this ResultValidator<T> validator, Func<T, Task<Result>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains a <see cref="ResultValidator{T}"/> to an asynchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// ResultValidator&lt;int&gt; validator = new ResultValidator&lt;int&gt;(Result.Ok(42));
    /// async Task&lt;Result&lt;string&gt;&gt; Next(int value) => await Task.FromResult(Result.Ok(value.ToString()));
    /// Task&lt;Result&lt;string&gt;&gt; final = validator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(
        this ResultValidator<T> validator, Func<T, Task<Result<U>>> next)
    {
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator{T}"/> to a synchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; asyncValidator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// Result Next(int value) => Result.Ok();
    /// Task&lt;Result&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Result> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator{T}"/> to a synchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The synchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; asyncValidator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// Result&lt;string&gt; Next(int value) => Result.Ok(value.ToString());
    /// Task&lt;Result&lt;string&gt;&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Result<U>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous operation producing a non-generic result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; asyncValidator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// async Task&lt;Result&gt; Next(int value) => await Task.FromResult(Result.Ok());
    /// Task&lt;Result&gt; final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result> Bind<T>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Task<Result>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator
            .Then(v => v.Bind(next))
            .Unwrap();
    }

    /// <summary>
    /// Chains an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous operation producing a typed result if validation succeeds.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncValidator">The asynchronous <see cref="ResultValidator{T}"/> to evaluate.</param>
    /// <param name="next">The asynchronous function to invoke with the validated value if validation succeeds.</param>
    /// <returns>A task containing the typed result of the chained operation, or a mapped error result if validation fails.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncValidator"/> or <paramref name="next"/> is null.</exception>
    /// <example>
    /// <code>
    /// Task&lt;ResultValidator&lt;int&gt;&gt; asyncValidator = Task.FromResult(new ResultValidator&lt;int&gt;(Result.Ok(42)));
    /// async Task&lt;Result&lt;string&gt; Next(int value) => await Task.FromResult(Result.Ok(value.ToString()));
    /// Task&lt;Result&lt;string>> final = asyncValidator.Bind(Next);
    /// </code>
    /// </example>
    public static Task<Result<U>> Bind<T, U>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Task<Result<U>>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator
            .Then(v => v.Bind(next))
            .Unwrap();
    }

    #endregion
}