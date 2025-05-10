using CSharper.Events;
using CSharper.Types;

namespace CSharper.Tests.Types;

public sealed class TestEntity : Entity<string>
{
    public Action? EqualsIntercepter { get; set; }
    public TestEntity(string id)
    {
        Id = id;
    }

    public void QueueTestEvent(DomainEvent domainEvent)
    {
        QueueEvent(domainEvent);
    }

    public override bool Equals(object? obj)
    {
        EqualsIntercepter?.Invoke();
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
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
    public TestSoftDeleteEntity(
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
    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }
}