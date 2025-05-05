using CSharper.Errors;
using CSharper.Utilities;
using System;
using System.Text;

namespace CSharper.Validation;

/// <summary>
/// Represents a validation-specific error detail with a message, optional code, and property path.
/// </summary>
public sealed class ValidationErrorDetail : ErrorDetail
{
    /// <summary>
    /// Gets the path to the property that caused the error.
    /// </summary>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationErrorDetail"/> class with a message, optional code, and property path.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification.</param>
    /// <param name="path">The path to the property that failed validation.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> or <paramref name="path"/> is null, empty, or whitespace.</exception>
    public ValidationErrorDetail(string message, string? code, string? path)
        : base(message, code)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        Path = path;
    }

    /// <summary>
    /// Returns a string representation of the validation error detail, including the path, message, and code if present.
    /// </summary>
    /// <returns>A formatted string describing the error.</returns>
    /// <remarks>
    /// The format is: <c>Path: {path} - {message}[, Code={code}]</c>, where <c>Code</c> is included only if non-empty.
    /// </remarks>
    public override string ToString()
    {
        StringBuilder sb = new($"Path: {Path} - {Message}");
        if (!string.IsNullOrWhiteSpace(Code))
        {
            sb.Append($", Code={Code}");
        }
        return sb.ToString();
    }
}