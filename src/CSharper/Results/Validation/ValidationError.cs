using BECU.Libraries.Results.Validation;
using CSharper.Errors;
using System.Linq;
using System.Text;

namespace CSharper.Results.Validation;

/// <summary>
/// Represent a validation <see cref="Error"/> with an optional path.
/// </summary>
public sealed class ValidationError : Error
{
    public const string DefaultErrorMessage = "One or more validation(s) failed.";
    public const string DefaultErrorCode = "VALIDATION_ERROR";

    /// <inheritdoc cref="Error(string, string?, ErrorDetail[])"/>
    public ValidationError(
        string message = DefaultErrorMessage,
        string? code = DefaultErrorCode,
        params ValidationErrorDetail[] errorDetails)
        : base(message, code, errorDetails)
    {
        // Intentionally blank
    }

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

    private static void AppendDetail(StringBuilder sb, ValidationErrorDetail detail)
    {
        string formattedMessage = detail.Message
            .StartsWith(IndentMarker) ? $">{detail}" : $"> {detail}";
        sb.AppendLine();
        sb.Append(formattedMessage);
    }
}
