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
    /// <inheritdoc cref="Error(string, string?, ErrorDetail[])"/>
    /// <param name="message">The primary error message (defaults to <see cref="DefaultErrorMessage"/>).</param>
    /// <param name="code">The optional error code (defaults to <see cref="DefaultErrorCode"/>).</param>
    /// <param name="errorDetails">An array of <see cref="ValidationErrorDetail"/> objects describing specific validation failures.</param>
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
        params ValidationErrorDetail[] errorDetails)
        : base(message, code, errorDetails)
    {
        // Intentionally blank
    }

    /// <summary>
    /// Returns a string representation of the validation error, including the message, code, and detailed error information.
    /// </summary>
    /// <returns>A formatted string describing the error and its details.</returns>
    /// <remarks>
    /// The output includes the primary message, optional code, and each <see cref="ValidationErrorDetail"/> on a new line, with indentation for details.
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

        foreach (ValidationErrorDetail detail in ErrorDetails.Cast<ValidationErrorDetail>())
        {
            AppendDetail(sb, detail);
        }
        return sb.ToString();
    }

    /// <summary>
    /// Appends a single <see cref="ValidationErrorDetail"/> to the provided <see cref="StringBuilder"/> with appropriate formatting.
    /// </summary>
    /// <param name="sb">The <see cref="StringBuilder"/> to append the detail to.</param>
    /// <param name="detail">The <see cref="ValidationErrorDetail"/> to format and append.</param>
    /// <remarks>
    /// Details starting with an indent marker (> or >>) are formatted as is, while others are prefixed with "> ".
    /// Each detail is appended on a new line.
    /// </remarks>
    private static void AppendDetail(StringBuilder sb, ValidationErrorDetail detail)
    {
        string formattedMessage = detail.Message
            .StartsWith(IndentMarker) ? $">{detail}" : $"> {detail}";
        sb.AppendLine();
        sb.Append(formattedMessage);
    }
}