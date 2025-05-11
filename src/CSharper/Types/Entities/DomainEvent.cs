using CSharper.Events;
using System;

namespace CSharper.Types.Entities;

/// <summary>
/// Represents an abstract base class for domain events that implement the <see cref="IEvent"/> interface.
/// </summary>
/// <remarks>
/// This class provides a default implementation for the <see cref="IEvent.Timestamp"/> property, 
/// initialized to the current UTC time when the event is created.
/// </remarks>
public abstract class DomainEvent : IEvent
{
    /// <inheritdoc />
    /// <summary>
    /// Gets the timestamp when the event occurred, initialized to the current UTC time.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the date and time of the event in UTC.
    /// </value>
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}