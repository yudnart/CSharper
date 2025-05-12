using CSharper.Results;
using CSharper.Types;
using CSharper.Types.Proxy;
using FluentAssertions;
using System.Diagnostics;

namespace CSharper.Tests.Types;

[Collection(nameof(SequentialTests))]
[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ValueObject))]
public sealed class ValueObjectTests
{
    private readonly string _name = "Test";
    private readonly int _value = 42;
    private readonly string _differentName = "Different";
    private readonly int _differentValue = 99;

    public ValueObjectTests()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    [Fact]
    public void Equals_SameComponents_ReturnsTrue()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_name, _value);

        // Act
        bool result = obj1.Equals(obj2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentComponents_ReturnsFalse()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_differentName, _differentValue);

        // Act
        bool result = obj1.Equals(obj2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_NullObject_ReturnsFalse()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);

        // Act
        bool result = obj.Equals(null);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().BeFalse();
            (obj! == null!).Should().BeFalse();
            (null! == obj!).Should().BeFalse();
        });
    }

    [Fact]
    public void Equals_DifferentType_ReturnsFalse()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        object other = new();

        // Act
        bool result = obj.Equals(other);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_DifferenProxyTypes_ReturnsFalse()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        object other = new();

        // Act
        // Set the proxy type delegate to toggle between string and int.
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
        {
            // Use Stopwatch for high-resolution timing
            long ticks = Stopwatch.GetTimestamp();
            // Toggle between two types based on ticks
            return (ticks % 2 == 0) ? typeof(string) : typeof(int);
        });

        bool result = obj.Equals(other);

        // Assert
        result.Should().BeFalse();

        // Clean up
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    [Fact]
    public void Equals_SameObject_ReturnsTrue()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);

        // Act
        bool result = obj.Equals(obj);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_SameComponents_ReturnsSameHashCode()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_name, _value);

        // Act
        int hash1 = obj1.GetHashCode();
        int hash2 = obj2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_DifferentComponents_ReturnsDifferentHashCode()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_differentName, _differentValue);

        // Act
        int hash1 = obj1.GetHashCode();
        int hash2 = obj2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void GetHashCode_Cached_ReturnsSameHashCode()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);

        // Act
        int hash1 = obj.GetHashCode();
        int hash2 = obj.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void CompareTo_NullObject_ReturnsOne()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        object nullObject = null!;

        // Act
        int result = obj.CompareTo(nullObject);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CompareTo_NotValueObject_ReturnsOne()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        object obj2 = new();

        // Act
        int result = obj1.CompareTo(obj2);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CompareTo_DifferentProxyTypes_CompareTypeNames()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_differentName, _differentValue);

        Type mockType1 = typeof(TestValueObject);
        Type mockType2 = typeof(Result);

        string obj1TypeName = mockType1.ToString();
        string obj2TypeName = mockType2.ToString();

        int expected = obj1TypeName.CompareTo(obj2TypeName);

        // mock GetUnproxiedType to return mock type.
        bool calledOnce = false;
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
        {
            if (!calledOnce)
            {
                calledOnce = true;
                return mockType1;
            }
            return mockType2;
        });

        // Act
        int result = obj1.CompareTo(obj2);

        // Assert
        result.Should().Be(expected);

        // Clean up
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    [Fact]
    public void CompareTo_SameComponents_ReturnsZero()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_name, _value);

        // Act
        int result = obj1.CompareTo(obj2);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CompareTo_DifferentComponents_ReturnsNonZero()
    {
        // Arrange
        TestValueObject obj1 = new(_name, _value);
        TestValueObject obj2 = new(_differentName, _differentValue);

        // Act
        int result = obj1.CompareTo(obj2);

        // Assert
        result.Should().NotBe(0);
    }

    [Fact]
    public void CompareTo_NullComponents_HandlesCorrectly()
    {
        // Arrange
        TestValueObject obj1 = new(null!, _value);
        TestValueObject obj2 = new(_name, null!);

        // Act
        int result1 = obj1.CompareTo(obj2);
        int result2 = obj2.CompareTo(obj1);

        // Assert
        Assert.Multiple(() =>
        {
            result1.Should().Be(-1);
            result2.Should().Be(1);
        });
    }

    [Fact]
    public void OperatorEquals_BothNull_ReturnsTrue()
    {
        // Arrange
        TestValueObject? obj1 = null;
        TestValueObject? obj2 = null;

        // Act
        bool result = obj1! == obj2!;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEquals_OneNull_ReturnsTrue()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        TestValueObject? nullObj = null;

        // Act
        bool result = obj != nullObj!;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetUnproxiedType_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => ProxyTypeHelper.GetUnproxiedType(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    private class TestValueObject(string name, int? value) : ValueObject
    {
        public string Name { get; } = name;
        public int? Value { get; } = value;

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
        }
    }
}