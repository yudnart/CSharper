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
    public TestAuditableEntity(string id)
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

    public void SetLastModifiedAt(DateTimeOffset? lastModifiedAt)
    {
        LastModifiedAt = lastModifiedAt;
    }

    public void SetLastModifiedBy(string? lastModifiedBy)
    {
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