using CSharper.Extensions;
using System;
using System.Threading.Tasks;

namespace CSharper.Functional.Validation.Internal;

/// <summary>
/// Represents a validation rule with a predicate and error details for use in <see cref="ResultValidator"/> workflows.
/// </summary>
internal sealed class ValidationRule
{
    /// <summary>
    /// Gets the asynchronous predicate that determines if the validation passes.
    /// </summary>
    /// <value>A function that asynchronously evaluates to <c>true</c> if the validation passes, 
    /// otherwise <c>false</c>.</value>
    public Func<ValueTask<bool>> Predicate { get; }

    /// <summary>
    /// Gets the error message to return if the predicate fails.
    /// </summary>
    /// <value>The descriptive message describing the validation failure.</value>
    public string ErrorMessage { get; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code identifying the validation failure, or <c>null</c> if not specified.</value>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the optional path to the property being validated.
    /// </summary>
    /// <value>The property path indicating the validation context, or <c>null</c> if not specified.</value>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationRule"/> with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">The synchronous predicate to evaluate the validation condition.</param>
    /// <param name="errorMessage">The descriptive message of the error if the predicate fails.</param>
    /// <param name="errorCode">The optional error code for programmatic identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This constructor wraps the synchronous predicate in a <see cref="Task{TResult}"/> for compatibility with
    /// <see cref="ResultValidator"/>, which evaluates predicates asynchronously.
    /// </remarks>
    /// <example>
    /// <code>
    /// var rule = new ValidationRule(() => true, "Value must be valid", "INVALID", "value");
    /// </code>
    /// </example>
    public ValidationRule(
        Func<bool> predicate, 
        string errorMessage, 
        string? errorCode = default, 
        string? path = default)
        : this(errorMessage, errorCode, path)
    {
        predicate.ThrowIfNull(nameof(predicate));
        Predicate = () => new ValueTask<bool>(predicate());
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationRule"/> with an asynchronous predicate.
    /// </summary>
    /// <param name="predicate">The asynchronous predicate to evaluate the validation condition.</param>
    /// <param name="errorMessage">The descriptive message of the error if the predicate fails.</param>
    /// <param name="errorCode">The optional error code for programmatic identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This constructor is used for asynchronous validation scenarios in <see cref="ResultValidator"/>,
    /// allowing predicates that perform I/O or complex computations.
    /// </remarks>
    /// <example>
    /// <code>
    /// var rule = new ValidationRule(async () => await Task.FromResult(true), "Value must be valid", "INVALID", "value");
    /// </code>
    /// </example>
    public ValidationRule(
        Func<Task<bool>> predicate, 
        string errorMessage, 
        string? errorCode = default, 
        string? path = default)
        : this(errorMessage, errorCode, path)
    {
        predicate.ThrowIfNull(nameof(predicate));
        Predicate = () => new ValueTask<bool>(predicate());
    }

    private ValidationRule(
        string errorMessage, string? errorCode, string? path)
    {
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));
        Predicate = default!;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Path = path;
    }
}