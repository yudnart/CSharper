using BECU.Libraries.Results.Validation;
using CSharper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Results.Validation;

public sealed class ResultValidator
{
    private readonly Result _initialResult;
    private readonly List<ValidationRule> _rules;

    /// <summary>
    /// Initializes a new instance of a <see cref="ResultValidator"/>.
    /// </summary>
    /// <param name="context">
    /// The <see cref="Result"/> context for functional chaining.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context"/> is null.</exception>
    internal ResultValidator(Result context)
    {
        _initialResult = context
            ?? throw new ArgumentNullException(nameof(context));
        _rules = [];
    }

    /// <summary>
    /// Adds a predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The condition to evaluate.</param>
    /// <param name="message">
    /// The <see cref="ValidationErrorDetail"/> message to include if the specified
    /// <paramref name="predicate"/> is <see langword="false"/>.
    /// </param>
    /// <param name="code">Optional <see cref="ValidationErrorDetail"/> code.</param>
    /// <param name="path">Optional <see cref="ValidationErrorDetail"/> path.</param>
    /// <returns>
    /// The current <see cref="ResultValidator"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="predicate"/> is null</exception>
    /// <exception cref="ArgumentException">
    /// Thrown if <paramref name="message"/> is null or whitespace.</exception>
    public ResultValidator And(
        Func<bool> predicate,
        string message,
        string? code = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));
        _rules.Add((new(predicate, message, code, path)));
        return this;
    }

    /// <summary>
    /// Evaluate all predicates and return the appropriate <see cref="Result"/>.
    /// </summary>
    /// <param name="message">
    /// The primary <see cref="ValidationError"/> message for a failure result.</param>
    /// <param name="code">Optional <see cref="ValidationError"/> code.</param>
    /// <returns>A successful result if all predicates are true. Otherwise,
    /// return a failure result.</returns>
    public Result Validate(
        string message = ValidationError.DefaultErrorMessage,
        string code = ValidationError.DefaultErrorCode)
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        IEnumerable<ValidationRule> errors = _rules.Where(r => !r.Predicate());

        if (!errors.Any())
        {
            return _initialResult;
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            message = ValidationError.DefaultErrorMessage;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            code = ValidationError.DefaultErrorCode;
        }

        ValidationErrorDetail[] errorDetails = errors
            .Select(e => new ValidationErrorDetail(e.Message, e.Code, e.Path))
            .ToArray();

        return Result.Fail(new ValidationError(message, code, errorDetails));
    }
}
