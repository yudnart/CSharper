using CSharper.Utilities;
using System;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// Represents a validation rule with a predicate and an error message if the predicate fails.
/// </summary>
public sealed class ValidationRule
{
    /// <summary>
    /// Gets the predicate that determines if the validation passes.
    /// </summary>
    /// <value>The function that evaluates to <c>true</c> if the validation passes, otherwise <c>false</c>.</value>
    public Func<Task<bool>> Predicate { get; }

    /// <summary>
    /// Gets the error message to return if the predicate fails.
    /// </summary>
    /// <value>The descriptive message of the validation error.</value>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or <c>null</c> if not specified.</value>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the optional path to the property being validated.
    /// </summary>
    /// <value>The property path, or <c>null</c> if not specified.</value>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationRule"/> class.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="errorMessage">The descriptive message of the error if the predicate fails.</param>
    /// <param name="errorCode">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    public ValidationRule(
        Func<bool> predicate, string errorMessage, string? errorCode = default, string? path = default)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));

        Predicate = () => Task.FromResult(predicate());
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Path = path;
    }

    public ValidationRule(
        Func<Task<bool>> predicate, string errorMessage, string? errorCode = default, string? path = default)
    {
        Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));

        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Path = path;
    }
}
