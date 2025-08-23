using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents an error with a required code, optional message, and a collection of detailed errors.
/// </summary>
public class DetailedError : Error
{
    /// <summary>
    /// The marker used to indent detailed error messages.
    /// </summary>
    public const char IndentMarker = '>';

    /// <summary>
    /// Gets the read-only list of detailed errors associated with this error.
    /// </summary>
    public IReadOnlyList<Error> Details { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="DetailedError"/> with a required code, optional message, and detailed errors.
    /// </summary>
    /// <param name="code">The required error code for identification.</param>
    /// <param name="message">The optional descriptive message of the error. Defaults to null.</param>
    /// <param name="details">An array of detailed errors. Defaults to an empty array.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="code"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="details"/> is null.</exception>
    public DetailedError(string code, string? message = null, params Error[] details)
        : base(code, message)
    {
        Details = (details ?? []).ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates and returns a StringBuilder with the string representation of the error, including details.
    /// </summary>
    protected override StringBuilder GetStringBuilder()
    {
        StringBuilder sb = base.GetStringBuilder();
        foreach (Error detail in Details)
        {
            sb.AppendLine();
            sb.Append(IndentMarker)
                .Append(' ')
                .Append(detail.ToString());
        }
        return sb;
    }
}