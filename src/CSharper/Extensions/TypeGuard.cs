using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Extensions;

/// <summary>
/// Provides utility methods for null and whitespace validation.
/// </summary>
public static class TypeGuard
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

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the specified collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to check for null or empty.</param>
    /// <param name="propertyName">The name of the property being checked.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="collection"/> is null or empty.</exception>
    public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> collection, string propertyName)
    {
        if (collection == null || !collection.Any())
        {
            throw new ArgumentException("Collection cannot be null or empty.", propertyName);
        }
    }
}
