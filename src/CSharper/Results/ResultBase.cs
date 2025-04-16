using System;
using System.Collections.Generic;
using System.Text;

namespace CSharper.Results;

/// <summary>
/// Provides an abstract base class for result types, representing either a success or failure with associated errors.
/// </summary>
public abstract class ResultBase
{
    private readonly List<Error> _errors = [];

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
    /// Gets the collection of errors associated with a failure result.
    /// </summary>
    /// <value>A read-only list of <see cref="Error"/> objects, empty for success results.</value>
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

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
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    protected ResultBase(Error causedBy, params Error[] details)
    {
        if (causedBy == null)
        {
            throw new ArgumentNullException(nameof(causedBy));
        }
        details ??= [];

        IsSuccess = false;
        _errors.AddRange([causedBy, .. details]);

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

        StringBuilder sb = new();
        sb.AppendLine("Failure:");

        foreach (Error error in Errors)
        {
            sb.AppendLine($"- {error}");
        }

        return sb.ToString();
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
        if (IsSuccess && _errors.Count > 0)
        {
            throw new InvalidOperationException("Success results cannot have errors.");
        }
        else if (IsFailure && _errors.Count == 0)
        {
            throw new InvalidOperationException("Failure results must have at least one error.");
        }
    }
}
