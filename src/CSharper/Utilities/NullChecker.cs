using System;

namespace CSharper.Utilities;

/// <summary>
/// Provides utility methods for null and whitespace validation.
/// </summary>
public static class NullChecker
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified object is null.
    /// </summary>
    /// <param name="obj">The object to check for null.</param>
    /// <param name="propertyName">The name of the property being checked.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
    public static void ThrowIfNull(this object obj, string propertyName)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(propertyName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the specified string is null, empty, or whitespace.
    /// </summary>
    /// <param name="str">The string to check for null or whitespace.</param>
    /// <param name="propertyName">The name of the property being checked.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="str"/> is null, empty, or whitespace.</exception>
    public static void ThrowIfNullOrWhitespace(this string str, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", propertyName);
        }
    }
}