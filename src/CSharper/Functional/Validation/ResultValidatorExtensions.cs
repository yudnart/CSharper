using CSharper.Extensions;
using CSharper.Functional;
using CSharper.Results.Validation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

[DebuggerStepThrough]
/// <summary>
/// Provides extension methods for handling both synchronous and asynchronous <see cref="ResultValidator{T}"/> 
/// operations in a functional programming style.
/// </summary>
public static class ResultValidatorExtensions
{
    #region And

    /// <summary>
    /// Adds a predicate and associated error to the validation chain of a
    /// <see cref="ResultValidator"/>.</summary>
    /// <param name="asyncValidator">
    /// The async <see cref="ResultValidator"/> containing the validation chain.</param>
    /// <param name="predicate">The condition to evaluate.</param>
    /// <param name="errorMessage">
    /// The <see cref="Error"/> errorMessage to include if the predicate fails.
    /// </param>
    /// <param name="errorCode">
    /// [Optional] The <see cref="Error"/> errorCode to include if the predicate fails.
    /// </param>
    /// <param name="path">
    /// [Optional] The <see cref="Error"/> path to include if the predicate fails.
    /// </param>
    /// <returns>
    /// The same <see cref="ResultValidator"/> is returned to enable chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if validator or predicate is null</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if errorMessage is null or whitespace.</exception>
    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncValidator
            .Then(v => v.And(predicate, errorMessage, errorCode, path));
    }

    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncValidator
            .Then(v => v.And(predicate, errorMessage, errorCode, path));
    }

    #endregion

    #region Ensure

    public static ResultValidator Ensure(this Result result,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    public static ResultValidator Ensure(this Result result,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return new ResultValidator(result)
            .And(predicate, errorMessage, errorCode, path);
    }

    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(v => v.Ensure(predicate, errorMessage, errorCode, path));
    }

    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        return asyncResult
            .Then(v => v.Ensure(predicate, errorMessage, errorCode, path));
    }

    #endregion

    #region Bind

    public static Result Bind(this ResultValidator validator, Func<Result> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    public static Result<T> Bind<T>(this ResultValidator validator, Func<Result<T>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    public static Task<Result> Bind(this ResultValidator validator, Func<Task<Result>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    public static Task<Result<T>> Bind<T>(this ResultValidator validator, Func<Task<Result<T>>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    public static Task<Result> Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    public static Task<Result<T>> Bind<T>(this Task<ResultValidator> asyncValidator, Func<Result<T>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.Then(v => v.Bind(next));
    }

    public static Task<Result> Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator
            .Then(v => v.Bind(next))
            .Unwrap();
    }

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
