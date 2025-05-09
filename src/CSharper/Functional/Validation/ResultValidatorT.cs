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
    /// Initializes a new instance of a <see cref="ResultValidator{T}"/>.
    /// </summary>
    /// <typeparam name="T">The value type of the <see cref="Result"/> context.</typeparam>
    /// <param name="context">
    /// The <see cref="Result{T}"/> that contains the value <see cref="T"/> to be
    /// evaluated.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context"/> is null.</exception>
    internal ResultValidator(Result<T> context)
    {
        _initialResult = context
            ?? throw new ArgumentNullException(nameof(context));
        _rules = [];
    }

    /// <summary>
    /// Adds a predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">A function that evaluates <typeparamref name="T"/>
    /// and returns <see langword="true"/> if the element meets the condition,
    /// otherwise <see langword="false"/>.</param>
    /// <param name="errorMessage">
    /// The <see cref="ValidationErrorDetail"/> message to include if the specified
    /// <paramref name="predicate"/> is <see langword="false"/>.
    /// </param>
    /// <param name="errorCode">Optional <see cref="ValidationErrorDetail"/> code.</param>
    /// <param name="path">Optional <see cref="ValidationErrorDetail"/> path.</param>
    /// <returns>
    /// The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="predicate"/> is null</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="errorMessage"/> is null or whitespace.</exception>
    public ResultValidator<T> And(
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNull(nameof(errorMessage));
        _rules.Add((new(predicate, errorMessage, errorCode, path)));
        return this;
    }

    public ResultValidator<T> And(
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNull(nameof(errorMessage));
        _rules.Add((new(predicate, errorMessage, errorCode, path)));
        return this;
    }

    /// <summary>
    /// Evaluate all predicates and return the appropriate <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="errorMessage">
    /// The primary <see cref="ValidationError"/> message for a failure result.</param>
    /// <param name="errorCode">Optional <see cref="ValidationError"/> code.</param>
    /// <returns>A successful result if all predicates are true. Otherwise,
    /// return a failure result.</returns>
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
