using CSharper.Extensions;
using FluentAssertions;

namespace CSharper.Tests.Extensions;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(CSharper.Extensions.TypeExtensions))]
public sealed class TypeExtensionsTests
{
    [Fact]
    public void GetFriendlyTypeName_NullType_ThrowsArgumentNullException()
    {
        // Arrange
        Type type = null!;

        // Act
        Action act = () => type.GetFriendlyTypeName();

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().Be(nameof(type));
    }

    [Theory]
    [InlineData(typeof(string), "string")]
    [InlineData(typeof(int), "int")]
    [InlineData(typeof(long), "long")]
    [InlineData(typeof(double), "double")]
    [InlineData(typeof(bool), "bool")]
    [InlineData(typeof(object), "object")]
    [InlineData(typeof(void), "void")]
    public void GetFriendlyTypeName_SimpleType_ReturnsKeywordEquivalent(Type type, string expected)
    {
        // Arrange
        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetFriendlyTypeName_NonGenericTypeWithoutKeyword_ReturnsTypeName()
    {
        // Arrange
        Type type = typeof(DateTime);

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("DateTime");
    }

    [Fact]
    public void GetFriendlyTypeName_NullableType_ReturnsUnderlyingTypeWithQuestionMark()
    {
        // Arrange
        Type type = typeof(int?);

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("int?");
    }

    [Fact]
    public void GetFriendlyTypeName_GenericTypeWithSingleParameter_ReturnsCorrectFormat()
    {
        // Arrange
        Type type = typeof(List<string>);

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("List<string>");
    }

    [Fact]
    public void GetFriendlyTypeName_GenericTypeWithMultipleParameters_ReturnsCorrectFormat()
    {
        // Arrange
        Type type = typeof(Dictionary<int, string>);

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("Dictionary<int, string>");
    }

    [Fact]
    public void GetFriendlyTypeName_NestedGenericType_ReturnsCorrectFormat()
    {
        // Arrange
        Type type = typeof(List<Dictionary<int, string>>);

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("List<Dictionary<int, string>>");
    }

    [Fact]
    public void GetFriendlyTypeName_GenericParameterType_ReturnsTypeName()
    {
        // Arrange
        Type type = typeof(List<>).GetGenericArguments()[0]; // Gets the 'T' in List<T>

        // Act
        string result = type.GetFriendlyTypeName();

        // Assert
        result.Should().Be("T");
    }
}