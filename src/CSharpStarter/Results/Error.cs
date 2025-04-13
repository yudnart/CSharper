using System;
using System.Text;

namespace CSharpStarter.Results;

/// <summary>
/// Represents an error with a message, optional code, and optional path for contextual information.
/// </summary>
public sealed class Error
{
    /// <summary>
    /// Gets the error message describing the issue.
    /// </summary>
    /// <value>The descriptive message of the error.</value>
    public string Message { get; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or <c>null</c> if not specified.</value>
    public string? Code { get; }

    /// <summary>
    /// Gets the optional path indicating the context or location of the error.
    /// </summary>
    /// <value>The error path, or <c>null</c> if not specified.</value>
    public string? Path { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with a message and optional code and path.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to <c>null</c>.</param>
    /// <param name="path">The optional path indicating the error's context. Defaults to <c>null</c>.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    public Error(string message, string? code = null, string? path = null)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message cannot be null, empty, or whitespace.", nameof(message));
        }

        Message = message;
        Code = code;
        Path = path;
    }

    /// <summary>
    /// Returns a string representation of the error, including the path, message, and code if present.
    /// </summary>
    /// <returns>A formatted string describing the error.</returns>
    /// <remarks>
    /// The format is: <c>[Path: {path} - ]{message} [({code})]</c>, where <c>Path</c> and <c>Code</c> are included only if non-empty.
    /// </remarks>
    public override string ToString()
    {
        StringBuilder sb = new();

        // Add Path if present
        if (!string.IsNullOrWhiteSpace(Path))
        {
            sb.Append("Path: ");
            sb.Append(Path);
            sb.Append(" - ");
        }

        // Add Message (always present)
        sb.Append(Message);

        // Add Code if present
        if (!string.IsNullOrWhiteSpace(Code))
        {
            sb.Append(" (");
            sb.Append(Code);
            sb.Append(')');
        }

        return sb.ToString();
    }
}