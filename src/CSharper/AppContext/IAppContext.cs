using System;
using System.Collections.Generic;

namespace CSharper.AppContext;

/// <summary>
/// Defines the contract for providing contextual information about a request being processed.
/// </summary>
/// <remarks>
/// This interface encapsulates metadata related to a request, such as user information, timing, 
/// identifiers, and client details. It is designed to provide a standardized way to access 
/// request-specific data across the application, supporting features like logging, tracing, 
/// and security enforcement.
/// </remarks>
public interface IAppContext
{
    /// <summary>
    /// Gets the context of the current user making the request.
    /// </summary>
    /// <remarks>
    /// Provides access to user-related information (e.g., user ID, roles, tenant ID) for 
    /// authorization or auditing purposes.
    /// </remarks>
    IUserContext CurrentUser { get; }

    /// <summary>
    /// Gets the timestamp when the request was initiated.
    /// </summary>
    /// <remarks>
    /// Represents the exact time (in UTC) the request was received, useful for logging or 
    /// tracking request timing.
    /// </remarks>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets a unique identifier for the current request.
    /// </summary>
    /// <remarks>
    /// Used to distinguish this request instance, aiding in tracing and debugging.
    /// </remarks>
    string RequestId { get; }

    /// <summary>
    /// Gets a correlation identifier linking related requests or operations.
    /// </summary>
    /// <remarks>
    /// Facilitates tracking a sequence of related actions across services or requests, often 
    /// used in distributed systems.
    /// </remarks>
    string CorrelationId { get; }

    /// <summary>
    /// Gets an optional identifier indicating the cause of this request, if applicable.
    /// </summary>
    /// <remarks>
    /// May reference a prior request or event that triggered this one, enhancing traceability; 
    /// null if not applicable.
    /// </remarks>
    string? CausationId { get; }

    /// <summary>
    /// Gets the optional client IP address from which the request originated.
    /// </summary>
    /// <remarks>
    /// Useful for logging or security purposes; null if not available or not applicable.
    /// </remarks>
    string? ClientIpAddress { get; }

    /// <summary>
    /// Gets the optional user agent string of the client making the request.
    /// </summary>
    /// <remarks>
    /// Identifies the client’s browser or application type for logging or analytics; null if 
    /// not provided.
    /// </remarks>
    string? UserAgent { get; }

    /// <summary>
    /// Gets the optional path or endpoint of the request.
    /// </summary>
    /// <remarks>
    /// Indicates the target resource or route of the request, useful for logging or routing 
    /// analysis; null if not specified.
    /// </remarks>
    string? RequestPath { get; }

    /// <summary>
    /// Gets a dictionary for storing additional request-specific data or metadata.
    /// </summary>
    /// <remarks>
    /// Allows attachment of custom key-value pairs to the request context, providing 
    /// extensibility for various use cases. Avoid storing sensitive data without sanitization.
    /// </remarks>
    IReadOnlyDictionary<string, object> Extensions { get; }

    /// <summary>
    /// Retrieves a typed value from the Extensions dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key of the extension data.</param>
    /// <returns>The value, or default(T) if not found or not convertible.</returns>
    /// <remarks>
    /// Provides type-safe access to extension data, reducing runtime errors.
    /// </remarks>
    T? GetExtension<T>(string key);
}
