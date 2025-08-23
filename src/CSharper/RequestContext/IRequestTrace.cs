using System;

namespace CSharper.RequestContext;

/// <summary>
/// Provides request identification details for tracing and debugging.
/// </summary>
public interface IRequestTrace
{
    /// <summary>
    /// Gets a unique identifier for the current request.
    /// </summary>
    string RequestId { get; }

    /// <summary>
    /// Gets a correlation identifier linking related requests or operations.
    /// </summary>
    string CorrelationId { get; }

    /// <summary>
    /// Gets an optional identifier indicating the cause of this request, if applicable.
    /// </summary>
    string? CausationId { get; }

    /// <summary>
    /// Gets the timestamp when the request was initiated.
    /// </summary>
    /// <remarks>
    /// Represents the exact time (in UTC) the request was received, useful for logging or 
    /// tracking request timing.
    /// </remarks>
    DateTimeOffset Timestamp { get; }
}
