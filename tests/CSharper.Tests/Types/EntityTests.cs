using CSharper.Types;
using CSharper.Types.Proxy;
using FluentAssertions;
using Moq;
using System.Diagnostics;

namespace CSharper.Tests.Types;

[Collection(nameof(SequentialTests))]
[Trait("Category", "Unit")]
[Trait("TestFor", nameof(Entity))]
public sealed class EntityTests
{
    public EntityTests()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

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
    public void Equals_ReferenceEquals_ReturnsTrue()
    {
        // Arrange
        TestEntity entity1 = new("test-id-1");
        TestEntity entity2 = entity1;

        // Act
        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferenProxyTypes_ReturnsFalse()
    {
        // Arrange
        TestEntity entity1 = new("test-id-1");
        TestEntity entity2 = new("test-id-2");

        // Act
        // Set the proxy type delegate to toggle between string and int.
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
        {
            // Use Stopwatch for high-resolution timing
            long ticks = Stopwatch.GetTimestamp();
            // Toggle between two types based on ticks
            return ticks % 2 == 0 ? typeof(string) : typeof(int);
        });

        bool result = entity1.Equals(entity2);

        // Assert
        result.Should().BeFalse();
        
        // Clean up
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
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

    [Theory]
    [MemberData(nameof(EqualTestData))]
    public void OperatorEqual(TestEntity obj1, TestEntity obj2, bool expected)
    {
        Assert.Multiple(() =>
        {
            (obj1 == obj2).Should().Be(expected);
            (obj2 == obj1).Should().Be(expected);
        });
    }

    [Theory]
    [MemberData(nameof(EqualTestData))]
    public void OperatorNotEqual_IsInverseOfEqualOperator(
        TestEntity obj1, TestEntity obj2, bool expected)
    {
        Assert.Multiple(() =>
        {
            (obj1 != obj2).Should().NotBe(expected);
            (obj2 != obj1).Should().NotBe(expected);
        });
    }

    [Fact]
    public void GetHashCode_ReturnsExpected()
    {
        // Arrange
        TestEntity testEntity = new("1");
        Type unproxiedTestEntity = ProxyTypeHelper.GetUnproxiedType(testEntity);
        int expected = (unproxiedTestEntity.ToString() + testEntity.Id)
            .GetHashCode();

        // Act
        int result = testEntity.GetHashCode();

        // Assert
        result.Should().Be(expected);
    }

    public static TheoryData<TestEntity, TestEntity, bool> EqualTestData()
    {
        TestEntity testEntity = new("1");
        TestEntity equalTestEntity = new("1");
        TestEntity notEqualTestEntity = new("2");
        return new TheoryData<TestEntity, TestEntity, bool>
        {
            // both null => true
            { null!, null!, true },
            // one null => false
            { null!, new TestEntity("2"), false },
            { new TestEntity("1"), null!, false },
            // both not null => return .Equals
            { testEntity, equalTestEntity, testEntity.Equals(equalTestEntity) },
            { testEntity, notEqualTestEntity, testEntity.Equals(notEqualTestEntity) },
        };
    }
}