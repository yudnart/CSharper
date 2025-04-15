using System;

namespace CSharper.Events;

/// <summary>
/// Defines a contract for an event that includes a timestamp indicating when the event occurred.
/// </summary>
public interface IEvent
{
    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the date and time of the event, including timezone information.
    /// </value>
    DateTimeOffset Timestamp { get; }
}