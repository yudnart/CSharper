using CSharper.Types;
using FluentAssertions;

namespace CSharper.Tests.Types;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(AuditableEntity))]
public class AuditableEntityTests
{
    private readonly string _id = "TestId";
    private readonly DateTimeOffset _createdAt = new(2023, 1, 1, 12, 0, 0, TimeSpan.Zero);
    private readonly string _createdBy = "User1";
    private readonly DateTimeOffset _lastModifiedAt = new(2023, 1, 2, 12, 0, 0, TimeSpan.Zero);
    private readonly string _lastModifiedBy = "User2";

    [Fact]
    public void Ctor_ValidValues_SetsProperties()
    {
        // Arrange
        TestAuditableEntity entity = new(_id, _createdAt, _createdBy, _lastModifiedAt, _lastModifiedBy);

        // Assert
        Assert.Multiple(() =>
        {
            entity.Id.Should().Be(_id);
            entity.CreatedAt.Should().Be(_createdAt);
            entity.CreatedBy.Should().Be(_createdBy);
            entity.LastModifiedAt.Should().Be(_lastModifiedAt);
            entity.LastModifiedBy.Should().Be(_lastModifiedBy);
        });
    }

    [Fact]
    public void Ctor_NullLastModifiedValues_SetsNullProperties()
    {
        // Arrange
        TestAuditableEntity entity = new(_id, _createdAt, _createdBy);

        // Assert
        Assert.Multiple(() =>
        {
            entity.Id.Should().Be(_id);
            entity.CreatedAt.Should().Be(_createdAt);
            entity.CreatedBy.Should().Be(_createdBy);
            entity.LastModifiedAt.Should().BeNull();
            entity.LastModifiedBy.Should().BeNull();
        });
    }

    [Fact]
    public void Ctor_NullCreatedBy_SetsDefault()
    {
        // Arrange
        TestAuditableEntity entity = new(_id, _createdAt, null!);

        // Assert
        Assert.Multiple(() =>
        {
            entity.Id.Should().Be(_id);
            entity.CreatedAt.Should().Be(_createdAt);
            entity.CreatedBy.Should().BeNull();
            entity.LastModifiedAt.Should().BeNull();
            entity.LastModifiedBy.Should().BeNull();
        });
    }

    [Fact]
    public void SetProperties_ValidValues_UpdatesProperties()
    {
        // Arrange
        TestAuditableEntity entity = new(_id, _createdAt, _createdBy);
        string newId = "NewId";
        DateTimeOffset newCreatedAt = new(2023, 2, 1, 12, 0, 0, TimeSpan.Zero);
        string newCreatedBy = "NewUser";
        DateTimeOffset newLastModifiedAt = new(2023, 2, 2, 12, 0, 0, TimeSpan.Zero);
        string newLastModifiedBy = "NewModifier";

        // Act
        entity.SetProperties(
            newId, newCreatedAt, newCreatedBy, newLastModifiedAt, newLastModifiedBy);

        // Assert
        Assert.Multiple(() =>
        {
            entity.Id.Should().Be(newId);
            entity.CreatedAt.Should().Be(newCreatedAt);
            entity.CreatedBy.Should().Be(newCreatedBy);
            entity.LastModifiedAt.Should().Be(newLastModifiedAt);
            entity.LastModifiedBy.Should().Be(newLastModifiedBy);
        });
    }

    [Fact]
    public void SetProperties_NullValues_SetsNullProperties()
    {
        // Arrange
        TestAuditableEntity entity = new(_id, _createdAt, _createdBy);

        // Act
        entity.SetProperties(null!, _createdAt, null!, null, null);

        // Assert
        Assert.Multiple(() =>
        {
            entity.CreatedAt.Should().Be(_createdAt);
            entity.CreatedBy.Should().BeNull();
            entity.LastModifiedAt.Should().BeNull();
            entity.LastModifiedBy.Should().BeNull();
        });
    }

    [Fact]
    public void Inheritance_AuditableEntity_InheritsFromAuditableEntityString()
    {
        // Arrange
        Type auditableEntityType = typeof(AuditableEntity);
        Type auditableEntityStringType = typeof(AuditableEntity<string>);

        // Assert
        auditableEntityType.BaseType.Should().Be(auditableEntityStringType);
    }
}
