using CSharper.Errors;
using System;

namespace CSharper.Validation;

/// <summary>
/// Represents a validation error with a collection of property-specific failures, each including a path.
/// </summary>
public sealed class ValidationError : Error
{
    /// <summary>
    /// The default error message used when no specific message is provided.
    /// </summary>
    public const string DefaultErrorMessage = "One or more validation errors occurred.";

    /// <summary>
    /// The default error code used when no specific code is provided.
    /// </summary>
    public const string DefaultErrorCode = "VALIDATION_ERROR";

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class with a general message, optional code, and a collection of validation-specific error details.
    /// </summary>
    /// <param name="message">The general descriptive message of the validation error. Defaults to <see cref="DefaultErrorMessage"/>.</param>
    /// <param name="code">The optional error code for identification. Defaults to <see cref="DefaultErrorCode"/>.</param>
    /// <param name="errorDetails">The collection of validation-specific error details. Defaults to an empty array.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorDetails"/> is null.</exception>
    public ValidationError(
        string message = DefaultErrorMessage,
        string code = DefaultErrorCode,
        params ValidationErrorDetail[] errorDetails)
        : base(message, code, errorDetails)
    {
        if (string.IsNullOrWhiteSpace(Code))
        {
            Code = DefaultErrorCode;
        }
    }
}