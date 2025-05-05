using CSharper.Utilities;
using CSharper.Validation;
using System;

namespace CSharper.Results;

/// <summary>
/// Represents a wrapper that combines a <see cref="Result"/> with a <see cref="Validator"/> for chaining validation.
/// </summary>
public sealed class ResultValidator
{
    private readonly Result _result;
    private readonly Validator _validator;

    internal ResultValidator(Result result, Func<bool> predicate, string message, string? code, string? path)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        _validator = new(predicate, message, code, path);
    }

    /// <summary>
    /// Adds a validation rule to the internal validator.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <returns>The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    public ResultValidator And(Func<bool> predicate, string message, string? code = null, string? path = null)
    {
        _validator.And(predicate, message, code, path);
        return this;
    }

    /// <summary>
    /// Evaluates the validation rules if the initial result is successful.
    /// </summary>
    /// <param name="message">The general descriptive message for the validation error, if any. Defaults to <c>null</c>.</param>
    /// <param name="code">The optional error code for the validation error. Defaults to <c>null</c>.</param>
    /// <returns>The initial <see cref="Result"/> if it is a failure; otherwise, the result of the validator's evaluation.</returns>
    public Result Validate(string? message = null, string? code = null)
    {
        return _result.IsFailure ? _result : _validator.Validate(message, code);
    }
}