using CSharper.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Errors.ErrorTestData;
using TestUtility = CSharper.Tests.Errors.ErrorTestUtility;

namespace CSharper.Tests.Errors;

public sealed class ErrorTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ErrorCtorValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Ctor_ValidParams_Succeeds(string message, string? code, ErrorDetail[]? errorDetails)
    {
        // Act
        Error result = new(message, code, errorDetails!);

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
        Action act = () => _ = new Error(message!);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly(
        string description, Error sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }
}
