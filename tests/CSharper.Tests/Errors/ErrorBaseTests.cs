using CSharper.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Errors.ErrorTestData;
using TestUtility = CSharper.Tests.Errors.ErrorTestUtility;

namespace CSharper.Tests.Errors;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ErrorBase))]
public sealed class ErrorBaseTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ErrorBaseCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Ctor_ValidParams_Succeeds(
        string message, string? code)
    {
        // Act
        TestError result = new(message, code);

        // Assert
        TestUtility.AssertError(result, message, code);
    }

    [Theory]
    [MemberData(
        nameof(TestData.CtorInvalidMessageTestCases),
        MemberType = typeof(TestData)
    )]
    public void Ctor_InvalidMessage_ThrowsArgumentException(string? message)
    {
        // Arrange
        Action act = () => new TestError(message!);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorBaseCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Equals_MessageAndCodeAreEqual_ReturnsTrue(
        string message, string? code)
    {
        // Act
        TestError sut = new(message, code);
        TestError error = new(message, code);

        // Assert
        Assert.Multiple(() =>
        {
            sut.Should().BeEquivalentTo(error);
            sut.GetHashCode().Should().Be(error.GetHashCode());
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorBaseCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Equals_MessageAndCodeAreNotEqual_ReturnsFalse(
        string message, string? code)
    {
        // Act
        TestError sut = new(message, code);
        TestError[] errors = [
            new($"{message}Test", code),
            new(message, $"{code}Test"),
            new($"{message}Test", $"{code}Test"),
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (TestError error in errors)
            {
                sut.Should().NotBeEquivalentTo(error);
                sut.GetHashCode().Should().NotBe(error.GetHashCode());
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorBaseToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly(
        string description, ErrorBase sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }

    private sealed class TestError(string message, string? code = null)
        : ErrorBase(message, code)
    {
        // Intentionally blank
    }
}
