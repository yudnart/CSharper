namespace CSharper.RequestContext;

/// <summary>
/// Defines the contract for providing contextual information about a request being processed.
/// </summary>
/// <remarks>
/// This interface encapsulates metadata related to a request, such as user information, timing, 
/// identifiers, and client details. It is designed to provide a standardized way to access 
/// request-specific data across the application, supporting features like logging, tracing, 
/// and security enforcement.
/// </remarks>
public interface IRequestContext : IRequestTrace, IUserContext
{
    // Intentionally blank
}
