using CSharper.Types;
using FluentAssertions;

namespace CSharper.Tests.Types;

public sealed class AuditableEntityTests
{
    [Fact]
    public void AuditableEntity_CreatedAt_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");
        DateTimeOffset createdAt = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        entity.SetCreatedAt(createdAt);

        // Assert
        entity.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void AuditableEntity_CreatedBy_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");
        string createdBy = "user1";

        // Act
        entity.SetCreatedBy(createdBy);

        // Assert
        entity.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    public void AuditableEntity_LastModifiedAt_WhenNotSet_ShouldBeNull()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");

        // Act & Assert
        entity.LastModifiedAt.Should().BeNull();
    }

    [Fact]
    public void AuditableEntity_LastModifiedAt_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");
        DateTimeOffset lastModifiedAt = new(2023, 2, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        entity.SetLastModifiedAt(lastModifiedAt);

        // Assert
        entity.LastModifiedAt.Should().Be(lastModifiedAt);
    }

    [Fact]
    public void AuditableEntity_LastModifiedBy_WhenNotSet_ShouldBeNull()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");

        // Act & Assert
        entity.LastModifiedBy.Should().BeNull();
    }

    [Fact]
    public void AuditableEntity_LastModifiedBy_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestAuditableEntity entity = new("test-id");
        string lastModifiedBy = "user2";

        // Act
        entity.SetLastModifiedBy(lastModifiedBy);

        // Assert
        entity.LastModifiedBy.Should().Be(lastModifiedBy);
    }

    [Fact]
    public void AuditableEntity_IsTransient_WhenIdIsEmpty_ShouldReturnTrue()
    {
        // Arrange
        TestAuditableEntity entity = new(string.Empty);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void SoftDeleteEntity_IsDeleted_WhenNotSet_ShouldBeFalse()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");

        // Act & Assert
        entity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void SoftDeleteEntity_IsDeleted_WhenSetToTrue_ShouldStoreValue()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");

        // Act
        entity.SetIsDeleted(true);

        // Assert
        entity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void SoftDeleteEntity_DeletedAt_WhenNotSet_ShouldBeNull()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");

        // Act & Assert
        entity.DeletedAt.Should().BeNull();
    }

    [Fact]
    public void SoftDeleteEntity_DeletedAt_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");
        DateTimeOffset deletedAt = new(2023, 3, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        entity.SetDeletedAt(deletedAt);

        // Assert
        entity.DeletedAt.Should().Be(deletedAt);
    }

    [Fact]
    public void SoftDeleteEntity_DeletedBy_WhenNotSet_ShouldBeNull()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");

        // Act & Assert
        entity.DeletedBy.Should().BeNull();
    }

    [Fact]
    public void SoftDeleteEntity_DeletedBy_WhenSet_ShouldStoreValue()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");
        string deletedBy = "user3";

        // Act
        entity.SetDeletedBy(deletedBy);

        // Assert
        entity.DeletedBy.Should().Be(deletedBy);
    }

    [Fact]
    public void SoftDeleteEntity_InheritsAuditableProperties_CreatedAt_ShouldBeSettable()
    {
        // Arrange
        TestSoftDeleteEntity entity = new("test-id");
        DateTimeOffset createdAt = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        entity.SetCreatedAt(createdAt);

        // Assert
        entity.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void SoftDeleteEntity_IsTransient_WhenIdIsEmpty_ShouldReturnTrue()
    {
        // Arrange
        TestSoftDeleteEntity entity = new(string.Empty);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    #region Test Helpers

    private class TestAuditableEntity : AuditableEntity<string>
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

    private class TestSoftDeleteEntity : SoftDeleteEntity<string>
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

    #endregion
}