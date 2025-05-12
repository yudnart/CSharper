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

    [Theory]
    [MemberData(nameof(EqualsTestData))]
    public void EqualsTest(string description, ValueObject left, object right, bool expected)
    {
        Assert.Multiple(() =>
        {
            left.Equals(right).Should().Be(expected, description);
            if (right != null!)
            {
                right.Equals(left).Should().Be(expected, description);
            }
        });
    }

    [Fact]
    public void Equals_DifferenProxyTypes_ReturnsFalse()
    {
        // Arrange
        TestValueObject<int> obj = new(_value);
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
    public void GetHashCode_SameComponents_ReturnsSameHashCode()
    {
        // Arrange
        TestValueObject<int> obj1 = new(_value);
        TestValueObject<int> obj2 = new(_value);

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
        TestValueObject<int> obj1 = new(_value);
        TestValueObject<int> obj2 = new(_differentValue);

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
        TestValueObject<int> obj = new(_value);

        // Act
        int hash1 = obj.GetHashCode();
        int hash2 = obj.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Theory]
    [MemberData(nameof(CompareToTestData))]
    public void CompareToTest(
        string description, ValueObject sut, object obj, int expected)
    {
        int result = sut.CompareTo(obj);
        result.Should().Be(expected, description);
    }

    [Fact]
    public void CompareTo_DifferentProxyTypes_CompareTypeNames()
    {
        // Arrange
        TestValueObject<int> obj1 = new(_value);
        TestValueObject<int> obj2 = new(_differentValue);

        Type mockType1 = typeof(TestValueObject<int>);
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
    public void GetUnproxiedType_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => ProxyTypeHelper.GetUnproxiedType(null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    public static IEnumerable<object[]> EqualsTestData()
    {
        TestValueObject<int> testData1 = new(42);
        TestValueObject<int> equalTestData1 = new(testData1.Value);
        TestValueObject<int> notEqualTestData1 = new(24);
        TestValueObject<string> nullTestValue = new(null!);

        // description, left, right, expected
        yield return ["Same components", testData1, equalTestData1, true];
        yield return ["Different components", testData1, notEqualTestData1, false];
        yield return ["Null object", testData1, null!, false];
        yield return ["Reference object", testData1, testData1, true];
        yield return ["Different types", testData1, new object(), false];
    }

    public static IEnumerable<object[]> CompareToTestData()
    {
        int value1 = 42;
        int value2 = 24;
        NonComparableObject nonComparableObject1 = new(value1);
        NonComparableObject nonComparableObject2 = new(value2);

        TestValueObject<int> nullTestData = null!;
        TestValueObject<int> testData1 = new(value1);
        TestValueObject<int> testData2 = new(value2);
        TestValueObject<string> testData3 = new("Hello world");
        TestValueObject<NonComparableObject> testData4 = new(nonComparableObject1);
        TestValueObject<NonComparableObject> testData5 = new(nonComparableObject2);
        TestValueObject<string> nullTestValue = new(null!);

        // description, sut, obj, expected
        yield return ["Null object", testData1, nullTestData, 1];
        yield return ["Different type", testData1, new object(), 1];
        yield return ["Both has null components", nullTestValue, nullTestValue, 0];
        yield return ["Right side has null component", testData3, nullTestValue, 1];
        yield return ["Left side has null component", nullTestValue, testData3, -1];
        yield return ["Both IComparable", testData1, testData2, value1.CompareTo(value2)];
        yield return ["Non IComparable", testData4, testData5, -1];
    }

    private class TestValueObject<T>(T value) : ValueObject
    {
        public T Value { get; } = value;

        public override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value!;
        }
    }

    private class NonComparableObject(int value)
    {
        public int Value => value;
    }
}