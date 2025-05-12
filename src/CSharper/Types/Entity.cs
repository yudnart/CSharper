using CSharper.Extensions;
using CSharper.Types.Proxy;
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
    private int? _cachedHashCode = null;

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
            || Id is string idStr && string.IsNullOrWhiteSpace(idStr);
    }

    #region Events

    private readonly Queue<DomainEvent> _evenStore = [];

    /// <summary>
    /// Queues a domain event to be dispatched.
    /// </summary>
    /// <param name="event">The domain event to be queued.</param>
    protected void QueueEvent(DomainEvent @event)
    {
        @event.ThrowIfNull(nameof(@event));
        _evenStore.Enqueue(@event);
    }

    /// <summary>
    /// Flushes the domain events and returns them as an enumerable.
    /// </summary>
    /// <returns>An enumerable of domain events.</returns>
    public IEnumerable<DomainEvent> FlushEvents()
    {
        while (_evenStore.TryDequeueCommon(out DomainEvent? result))
        {
            yield return result!;
        }
    }

    #endregion

    #region Equality

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
        if (!_cachedHashCode.HasValue)
        {
            _cachedHashCode = (GetUnproxiedType(this)
                .ToString() + Id)
                .GetHashCode();
        }
        return _cachedHashCode.Value;
    }

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

    #endregion

    #region Helpers

    private static Type GetUnproxiedType(object obj)
        => ProxyTypeHelper.GetUnproxiedType(obj);

    #endregion
}