using System;

namespace CSharper.Errors;

/// <summary>
/// Represents a detailed error with a message and optional code.
/// </summary>
public class ErrorDetail : ErrorBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorDetail"/> class with a message and optional code.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public ErrorDetail(string message, string? code = null) : base(message, code)
    {
        // Intentionally blank
    }
}
