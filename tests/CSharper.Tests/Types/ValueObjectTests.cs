using CSharper.Types;
using CSharper.Types.Utilities;
using FluentAssertions;

namespace CSharper.Tests.Types;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ValueObject))]
public sealed class ValueObjectTests
{
    private readonly string _name = "Test";
    private readonly int _value = 42;
    private readonly string _differentName = "Different";
    private readonly int _differentValue = 99;

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
        TypeHelper.ConfigureGetUnproxiedTypeDelegate(_ => typeof(object));

        // Act
        bool result = obj.Equals(other);

        // Assert
        result.Should().BeFalse();
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
    public void CompareTo_NullObject_ReturnsPositive()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);

        // Act
        int result = obj.CompareTo((object?)null);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CompareTo_DifferentType_CompareTypeNames()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        object other = new();
        TypeHelper.ConfigureGetUnproxiedTypeDelegate(obj => 
            obj is TestValueObject ? typeof(TestValueObject) : typeof(object));

        string objTypeName = typeof(TestValueObject).ToString();
        string otherTypeName = typeof(object).ToString();

        int expected = string
            .Compare(objTypeName, otherTypeName, StringComparison.Ordinal);
        // Act
        int result = obj.CompareTo(other);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void CompareTo_NonValueObject_ReturnsPositive()
    {
        // Arrange
        TestValueObject obj = new(_name, _value);
        object other = new();
        TypeHelper.ConfigureGetUnproxiedTypeDelegate(_ => typeof(TestValueObject));

        // Act
        int result = obj.CompareTo(other);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public void CompareTo_NullComponents_HandlesCorrectly()
    {
        // Arrange
        TestValueObject obj1 = new(null!, _value);
        TestValueObject obj2 = new(null!, _value);

        // Act
        int result = obj1.CompareTo(obj2);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void CompareTo_NonComparableComponents_UsesEquals()
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
        Action act = () => TypeHelper.GetUnproxiedType(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    private class TestValueObject : ValueObject
    {
        public string Name { get; }
        public int Value { get; }

        public TestValueObject(string name, int value)
        {
            Name = name;
            Value = value;
        }

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Value;
        }
    }
}