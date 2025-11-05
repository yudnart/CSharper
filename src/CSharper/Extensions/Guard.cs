using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Extensions;

/// <summary>
/// Provides utility methods for null and whitespace validation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the specified object is null.
    /// </summary>
    /// <param name="obj">The object to check for null.</param>
    /// <param name="propertyName">The name of the property being checked.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
    public static void ThrowIfNull(object obj, string propertyName)
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
    public static void ThrowIfNullOrWhitespace(string str, string propertyName)
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
    public static void ThrowIfNullOrEmpty<T>(IEnumerable<T> collection, string propertyName)
    {
        if (collection == null || !collection.Any())
        {
            throw new ArgumentException("Collection cannot be null or empty.", propertyName);
        }
    }

    /// <summary>
    /// Throw exception from <paramref name="factory"/>
    /// if <paramref name="predicate"/> is true.
    /// </summary>
    /// <typeparam name="TException"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="factory"></param>
    public static void ThrowIf<TException>(
        Func<bool> predicate,
        Func<TException> factory) where TException : Exception
    {
        ThrowIfNull(predicate, nameof(predicate));
        ThrowIfNull(factory, nameof(factory));
        if (predicate())
        {
            throw factory();
        }
    }
}
