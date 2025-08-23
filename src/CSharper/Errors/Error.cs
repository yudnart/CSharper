using CSharper.Extensions;
using System;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents a basic error with a required code and optional message.
/// </summary>
public class Error
{
    /// <summary>
    /// Gets the optional error code for programmatic identification.
    /// </summary>
    /// <value>The error code, or null if not specified.</value>
    public string Code { get; }

    /// <summary>
    /// Gets the error message describing the issue.
    /// </summary>
    /// <value>The descriptive message of the error.</value>
    public string? Message { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Error"/> with a required code and optional message.
    /// </summary>
    /// <param name="code">The required error code for identification.</param>
    /// <param name="message">The optional descriptive message of the error. Defaults to null.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="code"/> is null, empty, or whitespace.</exception>
    public Error(string code, string? message = null)
    {
        code.ThrowIfNullOrWhitespace(nameof(code));
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    public override string ToString() => GetStringBuilder().ToString();

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    /// <param name="obj">The object to compare with the current error.</param>
    /// <returns>true if the specified object is an <see cref="Error"/> with the same <see cref="Message"/> and <see cref="Code"/>; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is Error other)
        {
            return Code == other.Code && Message == other.Message;
        }
        return false;
    }

    /// <summary>
    /// Returns a hash code for the current error.
    /// </summary>
    public override int GetHashCode() => (Code, Message).GetHashCode();

    /// <summary>
    /// Creates and returns a StringBuilder with the string representation of the error.
    /// </summary>
    /// <returns>A StringBuilder containing the error's string representation.</returns>
    protected virtual StringBuilder GetStringBuilder()
    {
        StringBuilder sb = new();
        sb.Append($"Code={Code}");
        if (!string.IsNullOrWhiteSpace(Message))
        {
            sb.Append($", Message={Message}");
        }
        return sb;
    }
}