using CSharper.Extensions;
using System;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents an error with a message and optional code.
/// </summary>
public abstract class ErrorBase
{
    /// <summary>
    /// Gets the error message describing the issue.
    /// </summary>
    /// <value>The descriptive message of the error.</value>
    public string Message { get; }

    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or null if not specified.</value>
    public string? Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorBase"/> class with a message and optional code.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to null.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is null, empty, or whitespace.</exception>
    protected ErrorBase(string message, string? code = null)
    {
        message.ThrowIfNullOrWhitespace(nameof(message));
        Message = message;
        Code = code;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    /// <param name="obj">The object to compare with the current error.</param>
    /// <returns>true if the specified object is an <see cref="ErrorBase"/> with the same <see cref="Message"/> and <see cref="Code"/>; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is ErrorBase other)
        {
            return Message == other.Message && Code == other.Code;
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for the current error.
    /// </summary>
    /// <returns>A hash code based on the <see cref="Message"/> and <see cref="Code"/>.</returns>
    public override int GetHashCode() => (Message, Code).GetHashCode();

    /// <summary>
    /// Returns a string representation of the error, including the message and optional code.
    /// </summary>
    /// <returns>A formatted string in the format: "Error: {message}[, Code={code}]".</returns>
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