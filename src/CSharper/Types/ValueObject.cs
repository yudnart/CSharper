using CSharper.Types.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharper.Types;

/// <summary>
/// Represents a base class for value objects in a Domain-Driven Design (DDD) context.
/// Value objects are immutable and compared based on their properties rather than identity.
/// </summary>
public abstract class ValueObject : IComparable, IComparable<ValueObject>
{
    private int? _cachedHashCode = null;

    /// <summary>
    /// When implemented in a derived class, returns the components that define the equality of the value object.
    /// </summary>
    /// <returns>An enumerable of objects representing the equality components.</returns>
    public abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetUnproxiedType(this) != GetUnproxiedType(obj))
        {
            return false;
        }

        try
        {
            ValueObject valueObject = (ValueObject)obj;
            return GetEqualityComponents()
                .SequenceEqual(valueObject.GetEqualityComponents());
        }
        catch
        {
            return false;
        }

    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        return _cachedHashCode.Value;
    }

    /// <summary>
    /// Compares the current object with another object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public virtual int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return 1;
        }

        if (obj is not ValueObject other)
        {
            return 1;
        }

        Type thisType = GetUnproxiedType(this);
        Type otherType = GetUnproxiedType(obj);

        if (thisType != otherType)
        {
            return thisType.ToString()
                .CompareTo(otherType.ToString());
        }

        object[] components = [.. GetEqualityComponents()];
        object[] otherComponents = [.. other.GetEqualityComponents()];

        for (int i = 0; i < components.Length; i++)
        {
            int comparison = CompareComponents(components[i], otherComponents[i]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        return 0;
    }

    /// <summary>
    /// Compares the current object with another value object.
    /// </summary>
    /// <param name="other">The value object to compare with the current object.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public virtual int CompareTo(ValueObject? other)
    {
        return CompareTo(other as object);
    }

    /// <summary>
    /// Compares two components.
    /// </summary>
    /// <param name="object1">The first component to compare.</param>
    /// <param name="object2">The second component to compare.</param>
    /// <returns>A value that indicates the relative order of the components being compared.</returns>
    private static int CompareComponents(object object1, object object2)
    {
        if (object1 is null && object2 is null)
        {
            return 0;
        }

        if (object1 is null)
        {
            return -1;
        }

        if (object2 is null)
        {
            return 1;
        }

        if (object1 is IComparable comparable1 && object2 is IComparable comparable2)
        {
            return comparable1.CompareTo(comparable2);
        }

        return object1.Equals(object2) ? 0 : -1;
    }

    /// <summary>
    /// Determines whether two specified value objects have the same value.
    /// </summary>
    /// <param name="a">The first value object to compare.</param>
    /// <param name="b">The second value object to compare.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(ValueObject? a, ValueObject? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    /// <summary>
    /// Determines whether two specified value objects have different values.
    /// </summary>
    /// <param name="a">The first value object to compare.</param>
    /// <param name="b">The second value object to compare.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(ValueObject? a, ValueObject? b)
    {
        return !(a == b);
    }

    private static Type GetUnproxiedType(object obj)
        => ProxyTypeHelper.GetUnproxiedType(obj);
}
