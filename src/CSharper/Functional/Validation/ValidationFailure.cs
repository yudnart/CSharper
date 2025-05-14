using CSharper.Errors;
using System.Text;

namespace CSharper.Results.Validation;

/// <summary>
/// Represents a detailed validation error with a message, optional code, and optional path indicating the error's context.
/// </summary>
public sealed class ValidationFailure : ErrorDetail
{
    /// <summary>
    /// Gets the optional path indicating the context of the validation error (e.g., a field or property name).
    /// </summary>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationFailure"/> with the specified message, code, and path.
    /// </summary>
    /// <inheritdoc cref="ErrorDetail.ErrorDetail(string, string?)"/>
    /// <param name="message">The error message describing the validation failure.</param>
    /// <param name="code">An optional code identifying the type of validation error.</param>
    /// <param name="path">An optional path indicating the context of the error (e.g., a field or property name).</param>
    /// <remarks>
    /// This class is used to provide detailed information about specific validation failures within a <see cref="ValidationError"/>,
    /// typically in conjunction with <see cref="ResultValidator"/> or <see cref="ResultValidator{T}"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// var detail = new ValidationErrorDetail("Value must be positive", "POSITIVE", "value");
    /// </code>
    /// </example>
    public ValidationFailure(
        string message,
        string? code = null,
        string? path = null) : base(message, code)
    {
        Path = path;
    }

    /// <summary>
    /// Returns a string representation of the validation error detail, including the message, code, and path.
    /// </summary>
    /// <returns>A formatted string describing the error detail.</returns>
    /// <example>
    /// <code>
    /// var detail = new ValidationErrorDetail("Value must be positive", "POSITIVE", "value");
    /// string result = detail.ToString();
    /// // Output: Value must be positive, Code=POSITIVE, Path=value
    /// </code>
    /// </example>
    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());

        if (!string.IsNullOrWhiteSpace(Path))
        {
            sb.Append($", Path={Path}");
        }

        return sb.ToString();
    }
}