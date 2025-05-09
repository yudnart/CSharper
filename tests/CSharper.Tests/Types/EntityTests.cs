using CSharper.Events;
using CSharper.Types;
using FluentAssertions;
using Moq;

namespace CSharper.Tests.Types;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(Entity))]
public sealed class EntityTests
{
    [Fact]
    public void IsTransient_IdIsNull_ReturnsTrue()
    {
        // Arrange
        TestEntity entity = new(null!);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTransient_IdIsEmpty_ReturnsTrue()
    {
        // Arrange
        TestEntity entity = new(string.Empty);

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsTransient_IdIsSet_ReturnsFalse()
    {
        // Arrange
        TestEntity entity = new("test-id");

        // Act
        bool result = entity.IsTransient();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void QueueEvent_EventProvided_AddsToQueue()
    {
        // Arrange
        TestEntity entity = new("test-id");
        DomainEvent domainEvent = new Mock<DomainEvent>().Object;

        // Act
        entity.QueueTestEvent(domainEvent);
        IEnumerable<DomainEvent> events = entity.FlushEvents();

        // Assert
        events.Should().ContainSingle()
            .Which.Should().Be(domainEvent);
    }

    [Fact]
    public void FlushEvents_EventsExist_ReturnsAllAndClearQueue()
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
        Assert.Multiple(() =>
        {
            events.Should().HaveCount(2)
                  .And.Contain(event1)
                  .And.Contain(event2);
            eventsAfterFlush.Should().BeEmpty();
        });
    }

    [Fact]
    public void Equals_SameIdAndType_ReturnsTrue()
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
    public void Equals_DifferentIds_ReturnsFalse()
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
    public void Equals_DifferenTypes_ReturnsFalse()
    {
        // Arrange
        TestEntity entity1 = new("test-id-1");
        object entity2 = new();

        // Act
        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_TransientEntities_ReturnsFalse()
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
    public void OperatorEqual_BothNull_ReturnsTrue()
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
    public void OperatorNotEqual_OneNull_ReturnsTrue()
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