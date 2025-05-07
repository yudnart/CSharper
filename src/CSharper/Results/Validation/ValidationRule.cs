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
    public string Message { get; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or <c>null</c> if not specified.</value>
    public string? Code { get; }

    /// <summary>
    /// Gets the optional path to the property being validated.
    /// </summary>
    /// <value>The property path, or <c>null</c> if not specified.</value>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationRule"/> class.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate the validation condition.</param>
    /// <param name="message">The descriptive message of the error if the predicate fails.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public ValidationRule(Func<bool> predicate, string message, string? code, string? path)
    {
        predicate.ThrowIfNull(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));

        Predicate = () => Task.FromResult(predicate());
        Message = message;
        Code = code;
        Path = path;
    }

    public ValidationRule(Func<Task<bool>> predicate, string message, string? code, string? path)
    {
        Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        message.ThrowIfNullOrWhitespace(nameof(message));

        Message = message;
        Code = code;
        Path = path;
    }
}
