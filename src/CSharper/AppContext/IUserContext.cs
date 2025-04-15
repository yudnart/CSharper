using System.Collections.Generic;
using System.Security.Claims;

namespace CSharper.AppContext;

/// <summary>
/// Defines the contract for accessing information about the current user in a request context.
/// </summary>
/// <remarks>
/// This interface provides a standardized way to retrieve user-related data, such as identity, 
/// authentication status, roles, and claims. It supports features like authorization, auditing, 
/// and user-specific processing throughout the application.
/// </remarks>
public interface IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the user, if available.
    /// </summary>
    /// <remarks>
    /// Represents the user’s ID (e.g., a GUID or username); null if the user is not authenticated 
    /// or no ID is provided.
    /// </remarks>
    string? UserId { get; }

    /// <summary>
    /// Gets the name of the user, if available.
    /// </summary>
    /// <remarks>
    /// Typically the display name or full name of the user; null if not provided or not applicable.
    /// </remarks>
    string? Name { get; }

    /// <summary>
    /// Gets the email address of the user, if available.
    /// </summary>
    /// <remarks>
    /// Useful for identification or communication purposes; null if not provided or not applicable.
    /// </remarks>
    string? Email { get; }

    /// <summary>
    /// Gets the tenant identifier for the user, if applicable.
    /// </summary>
    /// <remarks>
    /// Null if the application is not multi-tenant or no tenant is associated with the user.
    /// </remarks>
    string? TenantId { get; }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    /// <remarks>
    /// Returns true if the user has a valid identity; false for anonymous or unauthenticated users.
    /// </remarks>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the collection of claims associated with the user.
    /// </summary>
    /// <remarks>
    /// Provides access to the user’s security claims (e.g., roles, permissions, custom attributes), 
    /// which can be used for fine-grained authorization or additional context.
    /// </remarks>
    IReadOnlyCollection<Claim> Claims { get; }

    /// <summary>
    /// Gets the user’s roles.
    /// </summary>
    /// <remarks>
    /// Provides a convenient way to access role claims without manually filtering Claims.
    /// </remarks>
    IReadOnlyCollection<string> Roles { get; }

    /// <summary>
    /// Retrieves the value of a specific claim, converted to the requested type.
    /// </summary>
    /// <typeparam name="T">The type to convert the claim value to.</typeparam>
    /// <param name="claimType">The type of the claim to retrieve.</param>
    /// <returns>The claim value, or default(T) if the claim is not found or cannot be converted.</returns>
    /// <remarks>
    /// Simplifies access to individual claim values with type safety, reducing the need to manually 
    /// iterate through Claims.
    /// </remarks>
    T? GetClaimValue<T>(string claimType);

    /// <summary>
    /// Determines whether the user is in the specified role.
    /// </summary>
    /// <param name="role">The role to check for membership.</param>
    /// <returns>True if the user has the specified role; otherwise, false.</returns>
    /// <remarks>
    /// Used for role-based authorization to verify if the user has the required permissions.
    /// </remarks>
    bool IsInRole(string role);
}
