using CSharper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// A builder for chaining validation predicates on a <see cref="Result{T}"/> to evaluate its value and collect errors.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
public sealed class ResultValidator<T>
{
    private readonly Result<T> _initialResult;
    private readonly List<ValidationRule<T>> _rules;

    /// <summary>
    /// Initializes a new instance of <see cref="ResultValidator{T}"/> with the specified result context.
    /// </summary>
    /// <param name="context">The initial <see cref="Result{T}"/> containing the value to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
    /// <remarks>
    /// This constructor is intended for internal use within the CSharper library.
    /// </remarks>
    internal ResultValidator(Result<T> context)
    {
        _initialResult = context
            ?? throw new ArgumentNullException(nameof(context));
        _rules = [];
    }

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationErrorDetail"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationErrorDetail"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationErrorDetail"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result{T}"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator&lt;int&gt;(Result.Ok(42))
    ///     .And(x => x > 0, "Value must be positive", "POSITIVE", "value");
    /// Result&lt;int&gt; result = validator.Validate();
    /// </code>
    /// </example>
    public ResultValidator<T> And(
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        _rules.Add((new(predicate, errorMessage, errorCode, path)));
        return this;
    }

    /// <summary>
    /// Adds an asynchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The asynchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationErrorDetail"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationErrorDetail"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationErrorDetail"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result{T}"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator&lt;int&gt;(Result.Ok(42))
    ///     .And(async x => await Task.FromResult(x > 0), "Value must be positive", "POSITIVE", "value");
    /// Result&lt;int&gt; result = validator.Validate();
    /// </code>
    /// </example>
    public ResultValidator<T> And(
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        _rules.Add((new(predicate, errorMessage, errorCode, path)));
        return this;
    }

    /// <summary>
    /// Evaluates all predicates and returns the validation result.
    /// </summary>
    /// <param name="errorMessage">The primary error message for a failed result (defaults to <see cref="ValidationError.DefaultErrorMessage"/>).</param>
    /// <param name="errorCode">Optional error code for the failed result (defaults to <see cref="ValidationError.DefaultErrorCode"/>).</param>
    /// <returns>The initial <see cref="Result{T}"/> if successful and all predicates pass; otherwise, a failed <see cref="Result{T}"/> with a <see cref="ValidationError"/> containing error details.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// Asynchronous predicates are executed concurrently. If the initial <see cref="Result{T}"/> is a failure, it is returned immediately without evaluating predicates.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator&lt;int&gt;(Result.Ok(42))
    ///     .And(x => x > 0, "Value must be positive", "POSITIVE", "value")
    ///     .And(async x => await Task.FromResult(x &lt; 100), "Value too large", "LARGE", "value");
    /// Result&lt;int&gt; result = validator.Validate("Validation failed", "VAL_FAIL");
    /// </code>
    /// </example>
    public Result<T> Validate(
        string errorMessage = ValidationError.DefaultErrorMessage,
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        T value = _initialResult.Value;
        // Run all predicates concurrently
        Task<bool>[] tasks = [.. _rules.Select(r => r.Predicate(value))];
        bool[] results = Task.WhenAll(tasks).GetAwaiter().GetResult();

        // Collect errors based on results
        ValidationRule<T>[] errors = [.. _rules
            .Select((rule, index) => new { Rule = rule, Result = results[index] })
            .Where(x => !x.Result)
            .Select(x => x.Rule)];

        if (errors.Length == 0)
        {
            return _initialResult;
        }

        ValidationErrorDetail[] errorDetails = errors
            .Select(e => new ValidationErrorDetail(e.ErrorMessage, e.ErrorCode, e.Path))
            .ToArray();

        return Result.Fail<T>(new ValidationError(errorMessage, errorCode, errorDetails));
    }
}