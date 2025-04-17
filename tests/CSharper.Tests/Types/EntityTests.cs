using CSharper.Events;
using CSharper.Types;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharper.Tests.Types;

public sealed class EntityTests
{
    [Fact]
    public void IsTransient_WhenIdIsNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity entity = new(null!);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTransient_WhenIdIsEmpty_ShouldReturnTrue()
    {
        // Arrange
        TestEntity entity = new(string.Empty);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTransient_WhenIdIsSet_ShouldReturnFalse()
    {
        // Arrange
        TestEntity entity = new("test-id");

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void SetMetadata_WhenKeyAndValueProvided_ShouldStoreMetadata()
    {
        // Arrange
        TestEntity entity = new("test-id");
        string key = "test-key";
        string value = "test-value";

        // Act
        entity.SetMetadata(key, value);
        bool result = entity.TryGetMetadata(key, out string retrievedValue);

        // Assert
        result.Should().BeTrue();
        retrievedValue.Should().Be(value);
    }

    [Fact]
    public void TryGetMetadata_WhenKeyDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        TestEntity entity = new("test-id");

        // Act
        bool result = entity.TryGetMetadata<string>("nonexistent-key", out string value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void UnsetMetadata_WhenKeyExists_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        TestEntity entity = new("test-id");
        string key = "test-key";
        entity.SetMetadata(key, "test-value");

        // Act
        bool result = entity.UnsetMetadata(key);

        // Assert
        result.Should().BeTrue();
        entity.TryGetMetadata<string>(key, out _).Should().BeFalse();
    }

    [Fact]
    public void UnsetMetadata_WhenKeyDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        TestEntity entity = new("test-id");

        // Act
        bool result = entity.UnsetMetadata("nonexistent-key");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void QueueDomainEvent_WhenEventProvided_ShouldAddToQueue()
    {
        // Arrange
        TestEntity entity = new("test-id");
        DomainEvent domainEvent = new Mock<DomainEvent>().Object;

        // Act
        entity.QueueTestEvent(domainEvent);
        IEnumerable<DomainEvent> events = entity.FlushDomainEvents();

        // Assert
        events.Should().ContainSingle().Which.Should().Be(domainEvent);
    }

    [Fact]
    public void FlushDomainEvents_WhenEventsExist_ShouldReturnAllAndClearQueue()
    {
        // Arrange
        TestEntity entity = new("test-id");
        DomainEvent event1 = new Mock<DomainEvent>().Object;
        DomainEvent event2 = new Mock<DomainEvent>().Object;
        entity.QueueTestEvent(event1);
        entity.QueueTestEvent(event2);

        // Act
        List<DomainEvent> events = entity.FlushDomainEvents().ToList();
        List<DomainEvent> eventsAfterFlush = entity.FlushDomainEvents().ToList();

        // Assert
        events.Should().HaveCount(2)
              .And.Contain(event1)
              .And.Contain(event2);
        eventsAfterFlush.Should().BeEmpty();
    }

    [Fact]
    public void Equals_WhenSameIdAndType_ShouldReturnTrue()
    {
        // Arrange
        TestEntity entity1 = new("test-id");
        TestEntity entity2 = new("test-id");

        // Act
        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenDifferentIds_ShouldReturnFalse()
    {
        // Arrange
        TestEntity entity1 = new("test-id-1");
        TestEntity entity2 = new("test-id-2");

        // Act
        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenTransientEntities_ShouldReturnFalse()
    {
        // Arrange
        TestEntity entity1 = new(null!);
        TestEntity entity2 = new(null!);

        // Act
        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void OperatorEquality_WhenBothNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity entity1 = null!;
        TestEntity entity2 = null!;

        // Act
        bool result = entity1 == entity2;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void OperatorInequality_WhenOneNull_ShouldReturnTrue()
    {
        // Arrange
        TestEntity entity1 = new("test-id");
        TestEntity entity2 = null!;

        // Act
        bool result = entity1 != entity2;

        // Assert
        result.Should().BeTrue();
    }

    #region Test Helpers

    private sealed class TestEntity : Entity<string>
    {
        public TestEntity(string id)
        {
            Id = id;
        }

        public void QueueTestEvent(DomainEvent domainEvent)
        {
            QueueDomainEvent(domainEvent);
        }
    }

    #endregion
}