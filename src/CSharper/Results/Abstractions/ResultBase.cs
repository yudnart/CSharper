using CSharper.Errors;
using System;

namespace CSharper.Results.Abstractions;

/// <summary>
/// Provides an abstract base class for result types, representing either a success or failure with associated errors.
/// </summary>
public abstract class ResultBase
{
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

    public Error? Error => IsFailure
        ? _error
        : throw new InvalidOperationException("Cannot access error property of a success result.");

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class as a success result.
    /// </summary>
    protected ResultBase()
    {
        IsSuccess = true;
        ValidateErrors();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultBase"/> class as a failure result with errors.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
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
    /// <c>"Success"</c> for a success result, or a formatted string listing errors for a failure result.
    /// </returns>
    /// <remarks>
    /// For failure results, the format is: <c>Failure:\n- {error1}\n- {error2}\n...</c>,
    /// where each error is represented by its <see cref="Error.ToString"/> output.
    /// </remarks>
    public override string ToString()
    {
        if (IsSuccess)
        {
            return "Success";
        }

        return $"Error: {Error}";
    }

    /// <summary>
    /// Validates the consistency of the result's state, ensuring success results does not
    /// have any errors and failure results have at least one.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if a success result has errors or a failure result has no errors.
    /// </exception>
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
