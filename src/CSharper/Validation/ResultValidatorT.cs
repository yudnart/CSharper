using CSharper.Results;
using CSharper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Validation;

/// <summary>
/// Represents a wrapper that combines a <see cref="Result{T}"/> with validation rules for chaining validation.
/// </summary>
/// <typeparam name="T">The type of the value in the result.</typeparam>
public sealed class ResultValidator<T>
{
    private readonly Result<T> _result;
    private readonly List<ValidationRule<T>> _rules = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultValidator{T}"/> class with an initial result and a single validation rule.
    /// </summary>
    /// <param name="result">The initial result to check before validation.</param>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    internal ResultValidator(Result<T> result, Func<T, bool> predicate, string message, string? code, string? path)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        _rules.Add(new ValidationRule<T>(predicate, message, code, path));
    }

    /// <summary>
    /// Adds a validation rule to the validator.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <returns>The current <see cref="ResultValidator{T}"/> instance for method chaining.</returns>
    public ResultValidator<T> And(Func<T, bool> predicate, string message, string? code = null, string? path = null)
    {
        _rules.Add(new ValidationRule<T>(predicate, message, code, path));
        return this;
    }

    /// <summary>
    /// Evaluates the validation rules if the initial result is successful.
    /// </summary>
    /// <param name="message">The general descriptive message for the validation error, if any. Defaults to <c>null</c>.</param>
    /// <param name="code">The optional error code for the validation error. Defaults to <c>null</c>.</param>
    /// <returns>The initial <see cref="Result{T}"/> if it is a failure; otherwise, the result of the validation.</returns>
    public Result<T> Validate(string? message = null, string? code = null)
    {
        if (_result.IsFailure)
        {
            return _result;
        }

        IEnumerable<ValidationRule<T>> errors = _rules.Where(r => !r.Predicate(_result.Value));

        if (!errors.Any())
        {
            return _result;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            message = ValidationError.DefaultErrorMessage;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            code = ValidationError.DefaultErrorCode;
        }

        ValidationErrorDetail[] errorDetails = [.. errors
            .Select(r => new ValidationErrorDetail(r.Message, r.Code, r.Path))];

        return Result.Fail<T>(new ValidationError(message!, code!, errorDetails));
    }
}