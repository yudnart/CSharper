using CSharper.Errors;
using CSharper.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CSharper.Results.Abstractions;

/// <summary>
/// Provides an abstract base class for result types, representing either a success or failure with associated errors
/// in a functional programming context.
/// </summary>
public abstract class ResultBase
{
    private readonly Lazy<string> _cachedToString;

    /// <summary>
    /// The error associated with a failure result, or null for a success result.
    /// </summary>
    private readonly Error? _error = null;

    /// <summary>
    /// Gets a value indicating whether the result is successful.
    /// </summary>
    /// <value><c>true</c> if the result represents a success; otherwise, <c>false</c>.</value>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a failure.
    /// </summary>
    /// <value><c>true</c> if the result represents a failure; otherwise, <c>false</c>.</value>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error associated with a failure result.
    /// </summary>
    /// <value>The <see cref="Error"/> object describing the failure, or <c>null</c> if the result is successful.</value>
    /// <exception cref="InvalidOperationException">Thrown if accessed on a success result.</exception>
    public Error? Error => IsFailure
        ? _error
        : throw new InvalidOperationException("Cannot access error property of a success result.");

    /// <summary>
    /// Initializes a new instance of <see cref="ResultBase"/> as a success result.
    /// </summary>
    /// <remarks>
    /// This constructor creates a success result with no associated errors, suitable for operations that complete successfully.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = new Result(); // Success result
    /// </code>
    /// </example>
    protected ResultBase()
    {
        IsSuccess = true;
        EnsureInvariants();
        _cachedToString = new(() => SuccessFormatBuilder().ToString());
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ResultBase"/> as a failure result with an error.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// This constructor creates a failure result with the specified error, used to represent operations that fail.
    /// </remarks>
    /// <example>
    /// <code>
    /// var error = new Error("Operation failed");
    /// var result = new Result(error); // Failure result
    /// </code>
    /// </example>
    protected ResultBase(Error error)
    {
        _error = error ?? throw new ArgumentNullException(nameof(error));
        IsSuccess = false;
        EnsureInvariants();
        _cachedToString = new(() => ErrorFormatBuilder(error).ToString());
    }

    /// <summary>
    /// Builds a <see cref="StringBuilder"/> containing the string representation for a success result.
    /// </summary>
    /// <returns>A <see cref="StringBuilder"/> initialized with the result type name and success details.</returns>
    /// <remarks>
    /// The base implementation initializes the <see cref="StringBuilder"/> with <c>{TypeName}: Success</c>.
    /// Derived classes can override this method to append additional details, such as a value in <c>Result{T}</c>.
    /// Ensure the implementation is efficient to maintain performance in logging scenarios.
    /// </remarks>
    protected virtual StringBuilder SuccessFormatBuilder() =>
        new($"{GetType().Name}: Success");

    /// <summary>
    /// Builds a <see cref="StringBuilder"/> containing the string representation for a failure result.
    /// </summary>
    /// <param name="error">The non-null error associated with the failure result.</param>
    /// <returns>A <see cref="StringBuilder"/> initialized with the result type name and error details.</returns>
    /// <remarks>
    /// The base implementation initializes the <see cref="StringBuilder"/> with <c>{TypeName}: Error: {error}</c>.
    /// Derived classes can override this method to customize the error representation.
    /// Ensure the implementation is efficient to maintain performance in logging scenarios.
    /// </remarks>
    protected virtual StringBuilder ErrorFormatBuilder(Error error) =>
        new($"{GetType().GetFriendlyTypeName()}: {Error}");

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>
    /// A string in the format <c>{TypeName}: Success</c> for success results or <c>{TypeName}: Error: {Error}</c> for failure results,
    /// as defined by <see cref="SuccessFormatBuilder"/> and <see cref="ErrorFormatBuilder"/>. The result is cached for performance.
    /// </returns>
    /// <remarks>
    /// The output includes the result type name for context and is optimized for logging and debugging, such as in
    /// <c>CSharper.Mediator.LoggingBehavior</c>. The string is computed once and cached using <see cref="Lazy{T}"/>.
    /// Derived classes can customize the output by overriding <see cref="SuccessFormatBuilder"/> and <see cref="ErrorFormatBuilder"/>.
    /// </remarks>
    public override string ToString() => _cachedToString.Value;

    /// <summary>
    /// Validates the consistency of the result's state, ensuring success results have no errors and failure results have at least one.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a success result has errors or a failure result has no errors.
    /// </exception>
    /// <remarks>
    /// This method is called internally during construction to enforce the invariant that success results have no errors
    /// and failure results have a valid error. It is excluded from code coverage as it represents defensive validation.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    private void EnsureInvariants()
    {
        if (IsSuccess && _error != null)
        {
            throw new InvalidOperationException("Success result cannot have error.");
        }
        else if (IsFailure && _error == null)
        {
            throw new InvalidOperationException("Failure result must have error.");
        }
    }
}