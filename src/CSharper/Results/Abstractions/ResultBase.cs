using CSharper.Errors;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CSharper.Results.Abstractions;

/// <summary>
/// Provides an abstract base class for result types, representing either a success or failure with associated errors
/// in a functional programming context.
/// </summary>
public abstract class ResultBase
{
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
        ValidateErrors();
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
        ValidateErrors();
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>
    /// <c>"Success"</c> for a success result, or a formatted string with the error details for a failure result.
    /// </returns>
    /// <remarks>
    /// For a success result, returns <c>"Success"</c>. For a failure result, returns a string in the format
    /// <c>"Error: {error}"</c>, where <c>{error}</c> is the result of <see cref="Error.ToString"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var success = new Result();
    /// Console.WriteLine(success.ToString()); // Output: Success
    /// 
    /// var error = new Error("Operation failed");
    /// var failure = new Result(error);
    /// Console.WriteLine(failure.ToString()); // Output: Error: Operation failed
    /// </code>
    /// </example>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return "Success";
        }

        return $"Error: {Error}";
    }

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
    [ExcludeFromCodeCoverage
#if NET8_0_OR_GREATER
        (Justification = "Unreachable defensive code.")
#endif
    ]
    private void ValidateErrors()
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