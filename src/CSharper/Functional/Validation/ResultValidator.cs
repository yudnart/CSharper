using CSharper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// Validates a <see cref="Result"/> by chaining synchronous or asynchronous predicates,
/// collecting errors if any predicates fail.
/// </summary>
public sealed class ResultValidator
{
    private readonly Result _initialResult;
    private readonly List<ValidationRule> _rules;

    /// <summary>
    /// Initializes a new instance of <see cref="ResultValidator"/> with the specified result context.
    /// </summary>
    /// <param name="context">The initial <see cref="Result"/> to validate, serving as the context for chaining.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
    /// <remarks>
    /// This constructor is intended for internal use within the CSharper library.
    /// </remarks>
    internal ResultValidator(Result context)
    {
        _initialResult = context
            ?? throw new ArgumentNullException(nameof(context));
        _rules = [];
    }

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message to include if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the validation error.</param>
    /// <param name="path">Optional path indicating the context of the error (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator(Result.Ok())
    ///     .And(() => true, "Invalid input", "INVALID", "input");
    /// Result result = validator.Validate();
    /// </code>
    /// </example>
    public ResultValidator And(
        Func<bool> predicate,
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
    /// <param name="predicate">The asynchronous condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message to include if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the validation error.</param>
    /// <param name="path">Optional path indicating the context of the error (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// The predicate is evaluated only if the initial <see cref="Result"/> is successful.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator(Result.Ok())
    ///     .And(async () => await Task.FromResult(true), "Invalid input", "INVALID", "input");
    /// Result result = validator.Validate();
    /// </code>
    /// </example>
    public ResultValidator And(
        Func<Task<bool>> predicate,
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
    /// <returns>The initial <see cref="Result"/> if successful and all predicates pass; otherwise, a failed <see cref="Result"/> with a <see cref="ValidationError"/> containing error details.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    /// <remarks>
    /// Asynchronous predicates are executed concurrently. If the initial <see cref="Result"/> is a failure, it is returned immediately without evaluating predicates.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new ResultValidator(Result.Ok())
    ///     .And(() => false, "Invalid input", "INVALID", "input")
    ///     .And(async () => await Task.FromResult(false), "Too short", "SHORT", "input");
    /// Result result = validator.Validate("Validation failed", "VAL_FAIL");
    /// </code>
    /// </example>
    public Result Validate(
        string errorMessage = ValidationError.DefaultErrorMessage,
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        // Run all predicates concurrently
        Task<bool>[] tasks = [.. _rules.Select(r => r.Predicate())];
        bool[] results = Task.WhenAll(tasks).GetAwaiter().GetResult();

        // Collect errors based on results
        ValidationRule[] errors = [.. _rules
            .Select((rule, index) => new { Rule = rule, Result = results[index] })
            .Where(x => !x.Result)
            .Select(x => x.Rule)];

        if (errors.Length == 0)
        {
            return _initialResult;
        }

        ValidationFailure[] errorDetails = [.. errors
            .Select(e => new ValidationFailure(e.ErrorMessage, e.ErrorCode, e.Path))];

        return Result.Fail(new ValidationError(errorMessage, errorCode, errorDetails));
    }
}