using CSharper.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Validation;

/// <summary>
/// Represents a validator that aggregates validation rules and evaluates them to produce a result.
/// </summary>
public sealed class Validator
{
    private readonly List<ValidationRule> _rules = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="Validator"/> class with a single validation rule.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    public Validator(Func<bool> predicate, string message, string? code, string? path)
    {
        And(predicate, message, code, path);
    }

    /// <summary>
    /// Adds a validation rule to the validator.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <returns>The current <see cref="Validator"/> instance for method chaining.</returns>
    public Validator And(Func<bool> predicate, string message, string? code, string? path)
    {
        _rules.Add(new(predicate, message, code, path));
        return this;
    }

    /// <summary>
    /// Evaluates all validation rules and returns the result.
    /// </summary>
    /// <param name="message">The general descriptive message for the validation error, if any. Defaults to <c>null</c>.</param>
    /// <param name="code">The optional error code for the validation error. Defaults to <c>null</c>.</param>
    /// <returns>A <see cref="Result"/> indicating success if all rules pass, or failure with a <see cref="ValidationError"/> if any rules fail.</returns>
    public Result Validate(string? message = null, string? code = null)
    {
        IEnumerable<ValidationRule> brokenRules = _rules.Where(r => !r.Predicate());

        if (!brokenRules.Any())
        {
            return Result.Ok();
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            message = ValidationError.DefaultErrorMessage;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            code = ValidationError.DefaultErrorCode;
        }

        ValidationErrorDetail[] errorDetails = [.. brokenRules
            .Select(r => new ValidationErrorDetail(r.Message, r.Code, r.Path))];

        return Result.Fail(new ValidationError(message!, code!, errorDetails));
    }
}
