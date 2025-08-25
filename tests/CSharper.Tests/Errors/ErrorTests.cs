using CSharper.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Errors.ErrorTestData;
using TestUtility = CSharper.Tests.Errors.ErrorTestUtility;

namespace CSharper.Tests.Errors;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(Error))]
public sealed class ErrorTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ErrorCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Ctor_ValidParams_Succeeds(
        string code, string? message)
    {
        // Act
        Error result = new(code, message);

        // Assert
        TestUtility.AssertError(result, code, message);
    }

    [Theory]
    [MemberData(
        nameof(TestData.CtorNullOrEmptyCode),
        MemberType = typeof(TestData)
    )]
    public void Ctor_InvalidMessage_ThrowsArgumentException(string code)
    {
        // Arrange
        Action act = () => new Error(code);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Equals_MessageAndCodeAreEqual_ReturnsTrue(
        string message, string? code)
    {
        // Arrange
        Error sut = new(message, code);
        Error error = new(message, code);

        // Act
        bool result = sut.Equals(error);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().BeTrue();
            sut.GetHashCode().Should().Be(error.GetHashCode());
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Equals_MessageAndCodeAreNotEqual_ReturnsFalse(
        string message, string? code)
    {
        // Arrange
        Error sut = new(message, code);
        Error[] errors = [
            new($"{message}Test", code),
            new(message, $"{code}Test"),
            new($"{message}Test", $"{code}Test"),
        ];

        // Act
        bool[] results = [.. errors.Select(e => sut.Equals(e))];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (bool result in results)
            {
                result.Should().BeFalse();
                sut.GetHashCode().Should().NotBe(result.GetHashCode());
            }
        });
    }

    [Fact]
    public void Equals_TypeNotErrorBase_ReturnsFalse()
    {
        // Arrange
        Error sut = new("Test error");
        object other = new();

        // Act
        bool result = sut.Equals(other);

        // Assert
        result.Should().BeFalse();
    }
}
