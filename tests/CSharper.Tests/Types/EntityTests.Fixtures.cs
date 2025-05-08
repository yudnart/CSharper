using CSharper.Events;
using CSharper.Types;

namespace CSharper.Tests.Types;

internal sealed class TestEntity : Entity<string>
{
    public TestEntity(string id)
    {
        Id = id;
    }

    public void QueueTestEvent(DomainEvent domainEvent)
    {
        QueueEvent(domainEvent);
    }
}

internal sealed class TestAuditableEntity : AuditableEntity<string>
{
    public TestAuditableEntity(
        string id, 
        DateTimeOffset createdAt, 
        string createdBy, 
        DateTimeOffset? lastModifiedAt = null, 
        string? lastModifiedBy = null)
    {
        Id = id;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        LastModifiedAt = lastModifiedAt;
        LastModifiedBy = lastModifiedBy;
    }

    /// <summary>
    /// Helper to test protected setters
    /// </summary>
    public void SetProperties(
        string id,
        DateTimeOffset createdAt,
        string createdBy,
        DateTimeOffset? lastModifiedAt = null,
        string? lastModifiedBy = null)
    {
        Id = id;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        LastModifiedAt = lastModifiedAt;
        LastModifiedBy = lastModifiedBy;
    }
}

internal class TestSoftDeleteEntity : SoftDeleteEntity<string>
{
    public TestSoftDeleteEntity(string id)
    {
        Id = id;
    }

    public void SetCreatedAt(DateTimeOffset createdAt)
    {
        CreatedAt = createdAt;
    }

    public void SetCreatedBy(string createdBy)
    {
        CreatedBy = createdBy;
    }

    public void SetIsDeleted(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }

    public void SetDeletedAt(DateTimeOffset? deletedAt)
    {
        DeletedAt = deletedAt;
    }

    public void SetDeletedBy(string? deletedBy)
    {
        DeletedBy = deletedBy;
    }
}