using CSharper.Functional;
using CSharper.Results.Validation;
using CSharper.Utilities;
using System;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// Provides extension methods for handling both synchronous and asynchronous <see cref="ResultValidator{T}"/> 
/// operations in a functional programming style.
/// </summary>
public static class ResultValidatorExtensions
{
    private static bool Noop() => true;

    #region And

    /// <summary>
    /// Adds a predicate and associated error to the validation chain of a
    /// <see cref="ResultValidator"/>.</summary>
    /// <param name="asyncValidator">
    /// The async <see cref="ResultValidator"/> containing the validation chain.</param>
    /// <param name="predicate">The condition to evaluate.</param>
    /// <param name="message">
    /// The <see cref="Error"/> message to include if the predicate fails.
    /// </param>
    /// <param name="code">
    /// [Optional] The <see cref="Error"/> code to include if the predicate fails.
    /// </param>
    /// <param name="path">
    /// [Optional] The <see cref="Error"/> path to include if the predicate fails.
    /// </param>
    /// <returns>
    /// The same <see cref="ResultValidator"/> is returned to enable chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if validator or predicate is null</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if message is null or whitespace.</exception>
    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        return asyncValidator
            .ContinueWith(task => task
                .HandleFault()
                .Or(task => task.Result.And(predicate, message, code, path)));
    }

    public static Task<ResultValidator> And(this Task<ResultValidator> asyncValidator,
        Func<Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        return asyncValidator
            .ContinueWith(task => task
                .HandleFault()
                .Or(task => task.Result.And(predicate, message, code, path)));
    }

    #endregion

    #region Ensure

    public static ResultValidator Ensure(this Result result,
        Func<bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return new ResultValidator(result)
            .And(predicate, message, code, path);
    }

    public static ResultValidator Ensure(this Result result,
        Func<Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return new ResultValidator(result)
            .And(predicate, message, code, path);
    }

    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return asyncResult.ContinueWith(task => task
            .HandleFault()
            .Or(task => task.Result.Ensure(predicate, message, code, path)));
    }

    public static Task<ResultValidator> Ensure(
        this Task<Result> asyncResult,
        Func<Task<bool>> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        return asyncResult.ContinueWith(task => task
            .HandleFault()
            .Or(task => task.Result.Ensure(predicate, message, code, path)));
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
        return asyncValidator.ContinueWith(task => task.Result.Bind(next));
    }

    public static Task<Result<T>> Bind<T>(this Task<ResultValidator> asyncValidator, Func<Result<T>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        return asyncValidator.ContinueWith(task => task.Result.Bind(next));
    }

    public static async Task<Result> Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        ResultValidator validator = await asyncValidator;
        return await validator.Bind(next);
    }

    public static async Task<Result<T>> Bind<T>(this Task<ResultValidator> asyncValidator, Func<Task<Result<T>>> next)
    {
        asyncValidator.ThrowIfNull(nameof(asyncValidator));
        next.ThrowIfNull(nameof(next));
        ResultValidator validator = await asyncValidator;
        return await validator.Bind(next);
    }

    #endregion
}
