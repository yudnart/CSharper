﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents an error with a message, optional code, and optional detailed errors.
/// </summary>
public class Error : ErrorBase
{
    /// <summary>
    /// The marker used to indent error detail messages.
    /// </summary>
    public const string IndentMarker = ">";

    /// <summary>
    /// Gets the read-only list of detailed errors associated with this error.
    /// </summary>
    /// <value>A read-only list of <see cref="ErrorDetail"/> objects.</value>
    public IReadOnlyList<ErrorDetail> ErrorDetails { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class with a message, optional code, and optional error details.
    /// </summary>
    /// <param name="message">The descriptive message of the error.</param>
    /// <param name="code">The optional error code for identification. Defaults to null.</param>
    /// <param name="errorDetails">An array of detailed errors associated with this error. Defaults to an empty array.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="message"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="errorDetails"/> is null.</exception>
    public Error(
        string message, string? code = null, params ErrorDetail[] errorDetails)
        : base(message, code)
    {
        ErrorDetails = (errorDetails ?? []).ToList().AsReadOnly();
    }

    /// <summary>
    /// Returns a string representation of the error, including the message, optional code, and error details.
    /// </summary>
    /// <returns>
    /// A formatted string starting with the base error message and code (if present),
    /// followed by each <see cref="ErrorDetail"/> on a new line, indented with <see cref="IndentMarker"/>.
    /// </returns>
    public override string ToString()
    {
        StringBuilder sb = new(base.ToString());
        foreach (ErrorDetail detail in ErrorDetails)
        {
            sb.AppendLine();

            string detailMessage = detail.ToString();
            sb.Append(IndentMarker[0]);
            if (!detailMessage.StartsWith(IndentMarker))
            {
                sb.Append(' ');
            }
            sb.Append(detailMessage);
        }
        return sb.ToString();
    }
}