using CSharper.Utilities;
using System;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Provides a base class for error types with a message and optional code.
/// </summary>
public abstract class ErrorBase
{
    /// <summary>
    /// Gets the error message describing the issue.
    /// </summary>
    /// <value>The descriptive message of the error.</value>
    public string Message { get; protected set; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or <c>null</c> if not specified.</value>
    public string? Code { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorBase"/> class with a message and optional code.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    protected ErrorBase(string message, string? code = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        Message = message;
        Code = code;
    }

    /// <summary>
    /// Returns a string representation of the error, including the message and code if present.
    /// </summary>
    /// <returns>A formatted string describing the error.</returns>
    /// <remarks>
    /// The format is: <c>{message}[, Code={code}]</c>, where <c>Code</c> is included only if non-empty.
    /// </remarks>
    public override string ToString()
    {
        StringBuilder sb = new(Message);
        if (!string.IsNullOrWhiteSpace(Code))
        {
            sb.Append($", Code={Code}");
        }
        return sb.ToString();
    }
}