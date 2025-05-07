using CSharper.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Errors.ErrorTestData;
using TestUtility = CSharper.Tests.Errors.ErrorTestUtility;

namespace CSharper.Tests.Errors;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ErrorDetail))]
public sealed class ErrorDetailTests
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
        ErrorDetail result = new(message, code);

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
        Action act = () => new ErrorDetail(message!);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ErrorBaseToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly(
        string description, ErrorBase initial, string expected)
    {
        // Arrange
        ErrorDetail sut = new(initial.Message, initial.Code);

        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }
}
