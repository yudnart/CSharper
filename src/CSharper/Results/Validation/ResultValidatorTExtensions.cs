using CSharper.Extensions;
using CSharper.Functional;
using CSharper.Results.Validation;
using System;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

public static class ResultValidatorTExtensions
{
    private static bool Noop<T>(T _) => true;

    #region And

    /// <summary>
    /// Adds a predicate and associated error to the validation chain of a
    /// <see cref="ResultValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="asyncValidator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the result's value.</param>
    /// <param name="error">The <see cref="Error"/> to include if the predicate fails.</param>
    /// <returns>The updated <see cref="ResultValidator{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or predicate is null</exception>
    /// <exception cref="ArgumentException">Thrown if message is null or whitespace.</exception>
    public static Task<ResultValidator<T>> And<T>(this Task<ResultValidator<T>> asyncValidator,
        Func<T, bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        return asyncValidator
            .Then(v => v.And(predicate, message, code, path));
    }

    /// <summary>
    /// Adds a predicate and associated error to the validation chain of an
    /// asynchronous <see cref="ResultValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="asyncValidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the result's value.</param>
    /// <param name="error">The <see cref="Error"/> to include if the predicate fails.</param>
    /// <returns>A Task containing the updated <see cref="ResultValidator{T}"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or predicate is null</exception>
    /// <exception cref="ArgumentException">Thrown if message is null or whitespace.</exception>
    public static Task<ResultValidator<T>> And<T>(
        this Task<ResultValidator<T>> asyncValidator,
        Func<T, Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        predicate.ThrowIfNull(nameof(predicate));
        return asyncValidator
            .Then(v => v.And(predicate, message, code, path));
    }

    #endregion

    #region Ensure

    /// <summary>
    /// Validates a <see cref="Result{T}"/> using a synchronous predicate, returning a builder for further validation.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the value if <paramref name="result"/> is successful.</param>
    /// <param name="error">The error to include if the predicate fails.</param>
    /// <returns>A <see cref="ResultValidator{T}"/> for chaining additional validations.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static ResultValidator<T> Ensure<T>(this Result<T> result,
        Func<T, bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return new ResultValidator<T>(result)
            .And(predicate, message, code, path);
    }

    /// <summary>
    /// Validates a <see cref="Result{T}"/> using an asynchronous predicate, returning a builder to handle the outcome.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="predicate">The asynchronous predicate to evaluate the value if <paramref name="result"/> is successful.</param>
    /// <param name="message">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidator{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="message"/> is null.</exception>
    public static ResultValidator<T> Ensure<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return new ResultValidator<T>(result)
            .And(predicate, message, code, path);
    }

    /// <summary>
    /// Validates an asynchronous <see cref="Result{T}"/> using a synchronous predicate, returning a builder.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to validate.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the value if the result is successful.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidator{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static Task<ResultValidator<T>> Ensure<T>(
        this Task<Result<T>> asyncResult,
        Func<T, bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return asyncResult
            .Then(r => r.Ensure(predicate, message, code, path));
    }

    /// <summary>
    /// Validates an asynchronous <see cref="Result{T}"/> using an asynchronous predicate, returning a builder.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncResult">The asynchronous result to validate.</param>
    /// <param name="predicate">The asynchronous predicate to evaluate the value if the result is successful.</param>
    /// <param name="error">The error to use if the predicate fails.</param>
    /// <returns>A <see cref="Task{T}"/> containing a <see cref="ResultValidator{T}"/> for further processing.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public static Task<ResultValidator<T>> Ensure<T>(
        this Task<Result<T>> asyncResult,
        Func<T, Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNull(nameof(message));
        return asyncResult
            .Then(r => r.Ensure(predicate, message, code, path));
    }

    #endregion

    #region Bind

    /// <summary>
    /// Binds the validator result to a function that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or next is null.</exception>
    public static Result Bind<T>(this ResultValidator<T> validator, Func<T, Result> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validator result to a function that returns a <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result{U}"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or next is null.</exception>
    public static Result<U> Bind<T, U>(
        this ResultValidator<T> validator, Func<T, Result<U>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validator result to an asynchronous function that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or next is null.</exception>
    public static Task<Result> Bind<T>(
        this ResultValidator<T> validator, Func<T, Task<Result>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validator result to an asynchronous function that returns a <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if builder or next is null.</exception>
    public static Task<Result<U>> Bind<T, U>(
        this ResultValidator<T> validator, Func<T, Task<Result<U>>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validator result of an asynchronous <see cref="ResultValidator{T}"/> to a function
    /// that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="asyncValidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncBuilder or next is null.</exception>
    public static Task<Result> Bind<T>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Result> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Binds the validator result of an asynchronous <see cref="ResultValidator{T}"/> to a function
    /// that returns a <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncValidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncBuilder or next is null.</exception>
    public static Task<Result<U>> Bind<T, U>(
        this Task<ResultValidator<T>> asyncValidator, Func<T, Result<U>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    /// <summary>
    /// Binds the validator result of an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous
    /// function that returns a <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <param name="asyncValidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncBuilder or next is null.</exception>
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
    /// Binds the validator result of an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous
    /// function that returns a <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the validator result context.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncValidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncBuilder or next is null.</exception>
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
