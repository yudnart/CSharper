using CSharper.Extensions;
using CSharper.Functional.Validation.Internal;
using CSharper.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharper.Functional.Validation;

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
    /// <param name="initial">The initial <see cref="Result{T}"/> containing the value to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="initial"/> is null.</exception>
    /// <remarks>
    /// This constructor is intended for internal use within the CSharper library.
    /// </remarks>
    internal ResultValidator(Result<T> initial)
    {
        _initialResult = initial
            ?? throw new ArgumentNullException(nameof(initial));
        _rules = [];
    }

    /// <summary>
    /// Adds a synchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    public ResultValidator<T> And(
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        _rules.Add(new(predicate, errorMessage, errorCode, path));
        return this;
    }

    /// <summary>
    /// Adds an asynchronous predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The asynchronous condition to evaluate on the result's value, returning true if valid.</param>
    /// <param name="errorMessage">The error message for the <see cref="ValidationFailure"/> if <paramref name="predicate"/> returns false.</param>
    /// <param name="errorCode">Optional error code for the <see cref="ValidationFailure"/>.</param>
    /// <param name="path">Optional path for the <see cref="ValidationFailure"/>, indicating the error context (e.g., a field name).</param>
    /// <returns>The current <see cref="ResultValidator{T}"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    public ResultValidator<T> And(
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        _rules.Add(new(predicate, errorMessage, errorCode, path));
        return this;
    }

    /// <summary>
    /// Evaluates all predicates and returns the validation result.
    /// </summary>
    /// <param name="errorMessage">The primary error message for a failed result (defaults to <see cref="ValidationError.DefaultErrorMessage"/>).</param>
    /// <param name="errorCode">Optional error code for the failed result (defaults to <see cref="ValidationError.DefaultErrorCode"/>).</param>
    /// <returns>
    /// The initial <see cref="Result{T}"/> if successful and all predicates pass;
    /// otherwise, a failed <see cref="Result{T}"/> with a <see cref="ValidationError"/> containing error details.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    public Result<T> Validate(
        string errorMessage = ValidationError.DefaultErrorMessage,
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        T value = _initialResult.Value;
        List<ValidationFailure> failures = [];
        foreach (ValidationRule<T> rule in _rules)
        {
            ValueTask<bool> task = rule.Predicate(value);
            bool result = ResultValidator.UnwrapValueTask(task);

            if (result)
            {
                continue;
            }

            failures.Add(new(rule.ErrorMessage, rule.ErrorCode, rule.Path));
        }

        return failures.Count == 0
            ? _initialResult
            : Result.Fail<T>(new(errorMessage, errorCode, [.. failures]));
    }

    /// <summary>
    /// Evaluates all predicates asynchronously and returns the validation result.
    /// </summary>
    /// <param name="errorMessage">The primary error message for a failed result (defaults to <see cref="ValidationError.DefaultErrorMessage"/>).</param>
    /// <param name="errorCode">Optional error code for the failed result (defaults to <see cref="ValidationError.DefaultErrorCode"/>).</param>
    /// <returns>
    /// A <see cref="Task{Result}"/> that completes with the initial <see cref="Result{T}"/> if successful and all predicates pass;
    /// otherwise, a failed <see cref="Result{T}"/> with a <see cref="ValidationError"/> containing error details.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    public async Task<Result<T>> ValidateAsync(
        string errorMessage = ValidationError.DefaultErrorMessage,
        string errorCode = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        T value = _initialResult.Value;
        IEnumerable<Task<ValidationFailure?>> tasks = _rules
            .Select(EvaluatePredicate(value));

        ValidationFailure?[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

        ValidationFailure[] failures = [.. results.Where(r => r != null)!];

        return failures.Length == 0
            ? _initialResult
            : Result.Fail<T>(new(errorMessage, errorCode, failures));
    }

    private static Func<ValidationRule<T>, Task<ValidationFailure?>> EvaluatePredicate(T value)
    {
        return async rule =>
        {
            return await rule.Predicate(value).ConfigureAwait(false)
                ? null : new ValidationFailure(rule.ErrorMessage, rule.ErrorCode, rule.Path);
        };
    }
}
