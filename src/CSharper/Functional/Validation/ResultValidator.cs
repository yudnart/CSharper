using CSharper.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharper.Functional.Validation.Internal;

namespace CSharper.Functional.Validation;

/// <summary>
/// Validates a <see cref="Result"/> by chaining synchronous or asynchronous predicates,
/// collecting errors if any predicates fail.
/// </summary>
public sealed class ResultValidator
{
    private readonly Result _initialResult;
    private readonly List<ValidationRule> _rules = [];

    /// <summary>
    /// Initializes a new instance of <see cref="ResultValidator"/> with the specified result context.
    /// </summary>
    /// <param name="initial">The initial <see cref="Result"/> to validate, serving as the context for chaining.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="initial"/> is null.</exception>
    internal ResultValidator(Result initial)
    {
        _initialResult = initial ?? throw new ArgumentNullException(nameof(initial));
    }

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The condition to evaluate, returning true if valid.</param>
    /// <param name="errorMessage">The error message to include if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the validation error.</param>
    /// <param name="path">Optional path indicating the context of the error (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    public ResultValidator And(
        Func<bool> predicate, 
        string errorMessage, 
        string? errorCode = null, 
        string? path = null)
    {
        _rules.Add(new ValidationRule(predicate, errorMessage, errorCode, path));
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
    public ResultValidator And(
        Func<Task<bool>> predicate, 
        string errorMessage, 
        string? errorCode = null, 
        string? path = null)
    {
        _rules.Add(new ValidationRule(predicate, errorMessage, errorCode, path));
        return this;
    }

    /// <summary>
    /// Evaluates all predicates and returns the validation result synchronously.
    /// </summary>
    /// <param name="errorMessage">The primary error message for a failed result (defaults to <see cref="ValidationError.DefaultErrorMessage"/>).</param>
    /// <param name="errorCode">Optional error code for the failed result (defaults to <see cref="ValidationError.DefaultErrorCode"/>).</param>
    /// <returns>The initial <see cref="Result"/> if successful and all predicates pass; otherwise, a failed <see cref="Result"/> with a <see cref="ValidationError"/> containing error details.</returns>
    /// <remarks>
    /// This method processes all rules sequentially on the calling thread.
    /// Any asynchronous predicates will be blocked synchronously.
    /// </remarks>
    public Result Validate(
        string errorMessage = ValidationError.DefaultErrorMessage, 
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        List<ValidationFailure> failures = [];
        foreach (ValidationRule rule in _rules)
        {
            ValueTask<bool> task = rule.Predicate();
            bool result = UnwrapValueTask(task);

            if (result)
            {
                continue;
            }

            failures.Add(new(rule.ErrorMessage, rule.ErrorCode, rule.Path));
        }

        return failures.Count == 0
            ? _initialResult
            : Result.Fail(new(errorMessage, errorCode, [.. failures]));
    }

    /// <summary>
    /// Evaluates all predicates asynchronously and returns the validation result.
    /// </summary>
    /// <param name="errorMessage">The primary error message for a failed result (defaults to <see cref="ValidationError.DefaultErrorMessage"/>).</param>
    /// <param name="errorCode">Optional error code for the failed result (defaults to <see cref="ValidationError.DefaultErrorCode"/>).</param>
    /// <returns>The initial <see cref="Result"/> if successful and all predicates pass; otherwise, a failed <see cref="Result"/> with a <see cref="ValidationError"/> containing error details.</returns>
    public async Task<Result> ValidateAsync(
        string errorMessage = ValidationError.DefaultErrorMessage, 
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        IEnumerable<Task<ValidationFailure?>> tasks = _rules
            .Select(EvaluatePredicate());

        ValidationFailure?[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

        ValidationFailure[] failures = [.. results.Where(r => r != null)!];

        return failures.Length == 0
            ? _initialResult
            : Result.Fail(new ValidationError(errorMessage, errorCode, failures));
    }

    private static Func<ValidationRule, Task<ValidationFailure?>> EvaluatePredicate()
    {
        return async rule =>
        {
            return await rule.Predicate().ConfigureAwait(false)
                ? null : new ValidationFailure(rule.ErrorMessage, rule.ErrorCode, rule.Path);
        };
    }

    internal static bool UnwrapValueTask(ValueTask<bool> task)
    {
        return task.IsCompleted && task.IsCompletedSuccessfully
            ? task.Result : task.GetAwaiter().GetResult();
    }
}