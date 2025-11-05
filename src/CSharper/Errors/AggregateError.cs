using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents an error that aggregates a collection of nested errors, with a required code and optional message.
/// </summary>
/// <remarks>
/// This is a data model, not an exception, used for structured error reporting.
/// </remarks>
public class AggregateError : Error
{
    /// <summary>
    /// The marker used to indent nested error messages.
    /// </summary>
    public const char IndentMarker = '>';

    /// <summary>
    /// Gets a read-only list of nested errors providing additional details.
    /// </summary>
    /// <value>A read-only list of <see cref="Error"/> instances, never null.</value>
    public IReadOnlyList<Error> Details { get; }

    /// <inheritdoc/>
    /// <summary>
    /// Initializes a new instance of <see cref="AggregateError"/> with a required code, optional message, and detailed errors.
    /// </summary>
    /// <param name="code">The required error code for identification.</param>
    /// <param name="message">The optional descriptive message of the error. Defaults to null.</param>
    /// <param name="data">Provide optional context data for the error.</param>
    /// <param name="details">An array of detailed errors. Defaults to an empty array.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="code"/> is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="details"/> is null.</exception>
    public AggregateError(
        string code, 
        string? message = null, 
        object? data = null, params Error[] details)
        : base(code, message, data)
    {
        details = details ?? throw new ArgumentNullException(nameof(details));
        Details = details.Length == 0 ? Array.Empty<Error>() : details.ToList().AsReadOnly();
    }

    /// <summary>
    /// Determines whether the specified error is equal to the current error.
    /// </summary>
    public bool Equals(AggregateError? other)
    {
        if (other == null)
        {
            return false;
        }

        return base.Equals(other) 
            && Details.SequenceEqual(other.Details);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as AggregateError);

    /// <summary>
    /// Returns a hash code for the current error.
    /// </summary>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = base.GetHashCode();
            foreach (Error detail in Details)
            {
                hash = (hash * 23) + (detail?.GetHashCode() ?? 0);
            }
            return hash;
        }
    }

    /// <summary>
    /// Creates and returns a StringBuilder with the string representation of the error, including nested errors.
    /// </summary>
    protected override StringBuilder StringFormatBuilder()
    {
        StringBuilder sb = base.StringFormatBuilder();
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