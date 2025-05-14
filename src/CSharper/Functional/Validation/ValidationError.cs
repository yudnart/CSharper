using CSharper.Errors;
using System.Linq;
using System.Text;

namespace CSharper.Results.Validation;

/// <summary>
/// Represents a validation-specific <see cref="Error"/> with detailed error information, including an optional path.
/// </summary>
public sealed class ValidationError : Error
{
    /// <summary>
    /// The default error message used when no specific message is provided.
    /// </summary>
    public const string DefaultErrorMessage = "One or more validation(s) failed.";

    /// <summary>
    /// The default error code used when no specific code is provided.
    /// </summary>
    public const string DefaultErrorCode = "VALIDATION_ERROR";

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationError"/> with the specified message, code, and error details.
    /// </summary>
    /// <param name="message">The primary error message (defaults to <see cref="DefaultErrorMessage"/>).</param>
    /// <param name="code">The optional error code (defaults to <see cref="DefaultErrorCode"/>).</param>
    /// <param name="validationFailures">An array of <see cref="ValidationFailure"/> describing specific validation failure.</param>
    /// <remarks>
    /// This class is used to aggregate validation errors, typically in conjunction with <see cref="ResultValidator{T}"/> or <see cref="ResultValidator"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var errorDetails = new[] { new ValidationErrorDetail("Invalid input", "INVALID", "input") };
    /// var validationError = new ValidationError("Validation failed", "VAL_FAIL", errorDetails);
    /// </code>
    /// </example>
    public ValidationError(
        string message = DefaultErrorMessage,
        string? code = DefaultErrorCode,
        params ValidationFailure[] validationFailures)
        : base(message, code, validationFailures)
    {
        // Intentionally blank
    }

    /// <summary>
    /// Returns a string representation of the validation error, including the message, code, and detailed error information.
    /// </summary>
    /// <returns>A formatted string describing the error and its details.</returns>
    /// <remarks>
    /// The output includes the primary message, optional code, and each <see cref="ValidationFailure"/> on a new line, with indentation for details.
    /// Details starting with an indent marker (> or >>) are formatted accordingly, while others are prefixed with "> ".
    /// </remarks>
    /// <example>
    /// <code>
    /// var errorDetails = new[] { new ValidationErrorDetail("Invalid input", "INVALID", "input") };
    /// var validationError = new ValidationError("Validation failed", "VAL_FAIL", errorDetails);
    /// string result = validationError.ToString();
    /// // Output: Validation failed, Code=VAL_FAIL
    /// // > Path: input - Invalid input (INVALID)
    /// </code>
    /// </example>
    public override string ToString()
    {
        StringBuilder sb = new(Message);

        if (!string.IsNullOrWhiteSpace(Code))
        {
            sb.Append($", Code={Code}");
        }

        foreach (ValidationFailure detail in ErrorDetails.Cast<ValidationFailure>())
        {
            AppendDetail(sb, detail);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Appends a single <see cref="ValidationFailure"/> to the provided <see cref="StringBuilder"/> with appropriate formatting.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the detail to.</param>
    /// <param name="detail">The <see cref="ValidationFailure"/> to format and append.</param>
    /// <remarks>
    /// Details starting with an indent marker (> or >>) are formatted as is, while others are prefixed with "> ".
    /// Each detail is appended on a new line.
    /// </remarks>
    private static void AppendDetail(StringBuilder sb, ValidationFailure detail)
    {
        string formattedMessage = detail.Message
            .StartsWith(IndentMarker) ? $"{IndentMarker}{detail}" : $"{IndentMarker} {detail}";
        sb.AppendLine();
        sb.Append(formattedMessage);
    }
}