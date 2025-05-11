using System;

namespace CSharper.Types.Entities;

/// <summary>
/// Represents an auditable entity with a string identifier.
/// </summary>
public abstract class AuditableEntity : AuditableEntity<string>, IAuditable
{
    // Intentionally blank
}

/// <summary>
/// Represents an auditable entity with audit metadata such as creation and modification details.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class AuditableEntity<TId> : Entity<TId>, IAuditable
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    /// <value>The creation timestamp of the entity.</value>
    public DateTimeOffset CreatedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system that created the entity.
    /// </summary>
    /// <value>The creator of the entity.</value>
    public string CreatedBy { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets the date and time when the entity was last modified, if applicable.
    /// </summary>
    /// <value>The last modification timestamp, or <c>null</c> if not modified.</value>
    public DateTimeOffset? LastModifiedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system that last modified the entity, if applicable.
    /// </summary>
    /// <value>The last modifier of the entity, or the default value if not modified.</value>
    public string? LastModifiedBy { get; protected set; } = default!;
}

/// <summary>
/// Represents a soft-deletable entity with a string identifier.
/// </summary>
public abstract class SoftDeleteEntity : SoftDeleteEntity<string>
{
    // Intentionally blank
}

/// <summary>
/// Represents a soft-deletable entity with audit and deletion metadata.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
public abstract class SoftDeleteEntity<TId> : AuditableEntity<TId>, ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is marked as deleted.
    /// </summary>
    /// <value><c>true</c> if the entity is soft-deleted; otherwise, <c>false</c>.</value>
    public bool IsDeleted { get; protected set; } = false;

    /// <summary>
    /// Gets or sets the date and time when the entity was marked as deleted, if applicable.
    /// </summary>
    /// <value>The deletion timestamp, or <c>null</c> if not deleted.</value>
    public DateTimeOffset? DeletedAt { get; protected set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system that marked the entity as deleted, if applicable.
    /// </summary>
    /// <value>The identifier of the deleter, or <c>null</c> if not deleted.</value>
    public string? DeletedBy { get; protected set; }
}