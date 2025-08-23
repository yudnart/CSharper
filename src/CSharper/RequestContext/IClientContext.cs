namespace CSharper.RequestContext;

/// <summary>
/// Provides client metadata for a request, typically for web applications.
/// </summary>
public interface IClientContext
{
    /// <summary>
    /// Gets the optional client IP address from which the request originated.
    /// </summary>
    string? ClientIpAddress { get; }

    /// <summary>
    /// Gets the optional user agent string of the client making the request.
    /// </summary>
    string? UserAgent { get; }

    /// <summary>
    /// Gets the optional path or endpoint of the request.
    /// </summary>
    string? RequestPath { get; }
}