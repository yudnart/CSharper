using CSharper.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSharper.Errors;

/// <summary>
/// Represents a basic error with a required code and optional message.
/// </summary>
public class Error : IEquatable<Error>
{
    private readonly Lazy<string> _cachedToString;

    /// <summary>
    /// Gets the error code for programmatic identification.
    /// </summary>/// <value>The required error code.</value>
    public string Code { get; }

    /// <summary>
    /// Gets the optional error message describing the error.
    /// </summary>
    /// <value>The descriptive message of the error, or null if not specified.</value>
    public string? Message { get; }

    /// <summary>
    /// Gets the optional context data associated with the error.
    /// </summary>
    /// <value>A read-only dictionary containing key-value pairs of error context, or an empty dictionary if not specified.</value>
    public Dictionary<string, object?> Data { get; } = [];

    /// <summary>
    /// Initializes a new instance of <see cref="Error"/> with a required code and optional message.
    /// </summary>
    /// <param name="code">The required error code for identification.</param>
    /// <param name="message">The optional descriptive message of the error. Defaults to null.</param>
    /// <param name="data">Provide optional context data for the error.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="code"/> is null, empty, or whitespace.</exception>
    public Error(string code, string? message = null, object? data = null)
    {
        Guard.ThrowIfNullOrWhitespace(code, nameof(code));
        Code = code;
        Message = message?.Trim();
        Data = ToDictionary(data) ?? [];
        _cachedToString = new Lazy<string>(() => StringFormatBuilder().ToString());
    }

    /// <summary>
    /// Returns a string representation of the error in the format "Code={Code}, Message={Message}".
    /// Message is omitted if null or empty.
    /// </summary>
    public override string ToString() => _cachedToString.Value;

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    /// <param name="other">The object to compare with the current error.</param>
    /// <returns>true if the specified object is an <see cref="Error"/> with the same <see cref="Code"/> and <see cref="Message"/>; otherwise, false.</returns>
    public bool Equals(Error? other)
    {
        if (other == null)
        {
            return false;
        }

        return Code == other.Code
            && Message == other.Message
            && Data.Count == other.Data.Count
            && Data.All(kvp => other.Data.TryGetValue(kvp.Key, out object? otherValue) && Equals(kvp.Value, otherValue));
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current error.
    /// </summary>
    /// <param name="obj">The object to compare with the current error.</param>
    /// <returns>true if the specified object is an <see cref="Error"/> with the same <see cref="Message"/> and <see cref="Code"/>; otherwise, false.</returns>
    public override bool Equals(object? obj) => Equals(obj as Error);

    /// <summary>
    /// Returns a hash code for the current error.
    /// </summary>
    public override int GetHashCode() => (Code, Message).GetHashCode();

    /// <summary>
    /// Creates and returns a StringBuilder with the string representation of the error.
    /// </summary>
    /// <returns>A StringBuilder containing the error's string representation.</returns>
    protected virtual StringBuilder StringFormatBuilder()
    {
        StringBuilder sb = new($"Type={GetType().Name}, Code={Code}");
        if (!string.IsNullOrWhiteSpace(Message))
        {
            sb.Append($", Message={Message}");
        }
        return sb;
    }

    /// <summary>
    /// Converts an object to a dictionary, handling dictionaries and anonymous objects.
    /// </summary>
    /// <param name="data">The object to convert.</param>
    /// <returns>A dictionary containing the object's key-value pairs, or null if the input is null.</returns>
    private static Dictionary<string, object?>? ToDictionary(object? data)
    {
        if (data == null)
            return null;

        // Handle any dictionary-like structure that implements IEnumerable<KeyValuePair<string, object?>>
        if (data is IEnumerable<KeyValuePair<string, object?>> kvps1)
        {
            return kvps1.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Handle dictionaries with non-nullable object values
        if (data is IEnumerable<KeyValuePair<string, object>> kvps2)
        {
            return kvps2.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value);
        }

        // Handle non-generic IDictionary (legacy types)
        if (data is IDictionary dict)
        {
            return dict.Cast<DictionaryEntry>()
                .Where(e => e.Key is string)
                .ToDictionary(e => (string)e.Key, e => e.Value)!;
        }

        // Handle POCOs and anonymous objects via reflection
        return data.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(prop => prop.Name, prop => prop.GetValue(data))!;
    }

    /// <inheritdoc/>
    public static bool operator ==(Error? left, Error? right) 
        => Equals(left, right);

    /// <inheritdoc/>
    public static bool operator !=(Error? left, Error? right) 
        => !Equals(left, right);
}