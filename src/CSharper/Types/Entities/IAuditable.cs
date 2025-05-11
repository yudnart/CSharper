using System;

namespace CSharper.Types.Entities;

/// <summary>
/// Defines a contract for auditable entities with creation and modification audit information.
/// </summary>
public interface IAuditable : IAuditCreate, IAuditUpdate
{
    // Intentionally blank
}

/// <summary>
/// Defines a contract for entities that support soft deletion with audit information.
/// </summary>
public interface ISoftDelete : IAuditable
{
    /// <summary>
    /// Gets a value indicating whether the entity is marked as deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    /// Gets the date and time when the entity was marked as deleted, if applicable.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    /// Gets the identifier of the user or system that marked the entity as deleted.
    /// </summary>
    string? DeletedBy { get; }
}

/// <summary>
/// Defines a contract for entities with creation audit information.
/// </summary>
public interface IAuditCreate
{
    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Gets the identifier of the user or system that created the entity.
    /// </summary>
    string CreatedBy { get; }
}

/// <summary>
/// Defines a contract for entities with modification audit information.
/// </summary>
public interface IAuditUpdate
{
    /// <summary>
    /// Gets the date and time when the entity was last modified, if applicable.
    /// </summary>
    DateTimeOffset? LastModifiedAt { get; }

    /// <summary>
    /// Gets the identifier of the user or system that last modified the entity.
    /// </summary>
    string? LastModifiedBy { get; }
}