using CSharper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    /// Evaluate all predicates and return the appropriate <see cref="Result"/>.
    /// </summary>
    /// <param name="errorMessage">
    /// The primary <see cref="ValidationError"/> message for a failure result.</param>
    /// <param name="errorCode">Optional <see cref="ValidationError"/> code.</param>
    /// <returns>A successful result if all predicates are true. Otherwise,
    /// return a failure result.</returns>
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

        ValidationErrorDetail[] errorDetails = [.. errors
            .Select(e => new ValidationErrorDetail(e.ErrorMessage, e.ErrorCode, e.Path))];

        return Result.Fail(new ValidationError(errorMessage, errorCode, errorDetails));
    }
}
