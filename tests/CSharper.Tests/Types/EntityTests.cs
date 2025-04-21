using CSharper.Events;
using CSharper.Types;
using FluentAssertions;
using Moq;

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
    public void QueueDomainEvent_WhenEventProvided_ShouldAddToQueue()
    {
        // Arrange
        TestEntity entity = new("test-id");
        DomainEvent domainEvent = new Mock<DomainEvent>().Object;

        // Act
        entity.QueueTestEvent(domainEvent);
        IEnumerable<DomainEvent> events = entity.FlushEvents();

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
        List<DomainEvent> events = [.. entity.FlushEvents()];
        List<DomainEvent> eventsAfterFlush = [.. entity.FlushEvents()];

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
}