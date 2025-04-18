﻿using CSharper.Events;
using CSharper.Utilities;
using System;
using System.Collections.Generic;

namespace CSharper.Types;

/// <summary>
/// Represents an entity with a string identifier.
/// </summary>
public abstract class Entity : Entity<string>
{
    // Intentionally blank
}

/// <summary>
/// Represents a base class for entities, which are objects
/// that are defined by its identity and has a lifecycle.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TId>
{
    /// <summary>
    /// Gets or sets the identifier of the entity.
    /// </summary>
    public virtual TId Id { get; protected set; } = default!;

    /// <summary>
    /// Determines whether the entity has been persisted.
    /// </summary>
    /// <returns>
    /// <c>false</c> if the entity has been persisted; otherwise, <c>true</c>.
    /// </returns>
    public virtual bool IsTransient()
    {
        return Id is null || Id.Equals(default(TId))
            || (Id is string idStr && string.IsNullOrWhiteSpace(idStr));
    }

    #region Metadata

    private readonly Dictionary<string, object> _metadata = [];

    /// <summary>
    /// Attempts to get the metadata associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The expected type of the metadata value.</typeparam>
    /// <param name="key">The key of the metadata.</param>
    /// <param name="value">The value of the metadata if found.</param>
    /// <returns><c>true</c> if the metadata is found and is of the expected type; otherwise, <c>false</c>.</returns>
    public bool TryGetMetadata<T>(string key, out T value)
    {
        if (_metadata.TryGetValue(key, out var rawValue) && rawValue is T typedValue)
        {
            value = typedValue;
            return true;
        }
        value = default!;
        return false;
    }

    /// <summary>
    /// Sets the metadata for the entity.
    /// </summary>
    /// <param name="key">The key of the metadata.</param>
    /// <param name="value">The value of the metadata.</param>
    public void SetMetadata(string key, object value)
    {
        _metadata[key] = value;
    }

    /// <summary>
    /// Unsets the metadata for the entity.
    /// </summary>
    /// <param name="key">The key of the metadata to unset.</param>
    /// <returns><c>true</c> if the metadata was successfully removed; otherwise, <c>false</c>.</returns>
    public bool UnsetMetadata(string key)
    {
        return _metadata.Remove(key);
    }

    #endregion

    #region Events

    private readonly Queue<DomainEvent> _domainEvents = [];

    /// <summary>
    /// Queues a domain event to be dispatched.
    /// </summary>
    /// <param name="domainEvent">The domain event to be queued.</param>
    protected void QueueDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Enqueue(domainEvent);
    }

    /// <summary>
    /// Flushes the domain events and returns them as an enumerable.
    /// </summary>
    /// <returns>An enumerable of domain events.</returns>
    public IEnumerable<DomainEvent> FlushDomainEvents()
    {
        while (_domainEvents.TryDequeueCommon(out DomainEvent? result))
        {
            yield return result!;
        }
    }

    #endregion

    #region Equality

    /// <summary>
    /// Determines whether two specified entities have the same value.
    /// </summary>
    /// <param name="a">The first entity to compare.</param>
    /// <param name="b">The second entity to compare.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="a"/> is the same as the value of <paramref name="b"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator ==(Entity<TId> a, Entity<TId> b)
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
    /// Determines whether two specified entities have different values.
    /// </summary>
    /// <param name="a">The first entity to compare.</param>
    /// <param name="b">The second entity to compare.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="a"/> is different from the value of <paramref name="b"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool operator !=(Entity<TId> a, Entity<TId> b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// <c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetUnproxiedType(this) != GetUnproxiedType(other))
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return false;
        }

        return Id!.Equals(other.Id);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return (GetUnproxiedType(this).ToString() + Id).GetHashCode();
    }

    #endregion

    #region Helpers

    private static Type GetUnproxiedType(object obj)
        => TypeHelper.GetUnproxiedType(obj);

    #endregion
}