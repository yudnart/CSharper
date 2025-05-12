using CSharper.Extensions;
using System;
using System.Threading.Tasks;

namespace CSharper.Results.Validation;

/// <summary>
/// Represents a validation rule for a specific value type with a predicate and error details for use in <see cref="ResultValidator{T}"/> workflows.
/// </summary>
/// <typeparam name="T">The type of the value to validate.</typeparam>
public sealed class ValidationRule<T>
{
    /// <summary>
    /// Gets the asynchronous predicate that determines if the validation passes for a value of type <typeparamref name="T"/>.
    /// </summary>
    /// <value>A function that asynchronously evaluates a value of type <typeparamref name="T"/> to <c>true</c> if the validation passes, otherwise <c>false</c>.</value>
    public Func<T, Task<bool>> Predicate { get; }

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
    /// Initializes a new instance of <see cref="ValidationRule{T}"/> with a synchronous predicate.
    /// </summary>
    /// <param name="predicate">The synchronous predicate to evaluate the validation condition for a value of type <typeparamref name="T"/>.</param>
    /// <param name="errorMessage">The descriptive message of the error if the predicate fails.</param>
    /// <param name="errorCode">The optional error code for programmatic identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This constructor wraps the synchronous predicate in a <see cref="Task{TResult}"/> for compatibility with
    /// <see cref="ResultValidator{T}"/>, which evaluates predicates asynchronously.
    /// </remarks>
    /// <example>
    /// <code>
    /// var rule = new ValidationRule&lt;int&gt;(x => x > 0, "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public ValidationRule(
        Func<T, bool> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        predicate.ThrowIfNull(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));

        Predicate = value => Task.FromResult(predicate(value));
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Path = path;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationRule{T}"/> with an asynchronous predicate.
    /// </summary>
    /// <param name="predicate">The asynchronous predicate to evaluate the validation condition for a value of type <typeparamref name="T"/>.</param>
    /// <param name="errorMessage">The descriptive message of the error if the predicate fails.</param>
    /// <param name="errorCode">The optional error code for programmatic identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path to the property being validated. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="errorMessage"/> is null, empty, or whitespace.</exception>
    /// <remarks>
    /// This constructor is used for asynchronous validation scenarios in <see cref="ResultValidator{T}"/>,
    /// allowing predicates that perform I/O or complex computations.
    /// </remarks>
    /// <example>
    /// <code>
    /// var rule = new ValidationRule&lt;int&gt;(async x => await Task.FromResult(x > 0), "Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public ValidationRule(
        Func<T, Task<bool>> predicate,
        string errorMessage,
        string? errorCode = null,
        string? path = null)
    {
        Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        errorMessage.ThrowIfNullOrWhitespace(nameof(errorMessage));

        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Path = path;
    }
}