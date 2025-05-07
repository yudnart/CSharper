using CSharper.Errors;
using CSharper.Results;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Results.ResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace BECU.Libraries.Results.Tests;

[Trait("Category", "Unit")]
[Trait("TestOf", "ResultT")]
public sealed class ResultTTests
{
    [Theory]
    [MemberData(nameof(TValues))]
    public void OkT_ReturnsSuccessResult<T>(T value)
    {
        // Act
        Result<T> result = Result.Ok(value);

        // Assert
        TestUtility.AssertSuccessResult(result, value);
    }

    [Fact]
    public void FailT_WithError_ReturnsFailureResult()
    {
        // Act
        Error error = ErrorTestData.ErrorNoCode;
        Result<int> result = Result.Fail<int>(error);

        // Assert
        TestUtility.AssertFailureResult(result, error);
    }

    [Fact]
    public void FailT_NullError_ThrowsArgumentNullException()
    {
        // Arrange
        Error nullError = null!;

        // Act
        Action act = () => _ = Result.Fail<long>(nullError);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.FailValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void FailT_ValidParams_ReturnsFailureResult(
        string message, string? code = null)
    {
        // Act
        Result result = Result.Fail(message, code);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailureResult(result);
            result.Error!.Message.Should().Be(message);
            result.Error!.Code.Should().Be(code);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.FailInvalidMessageTestCases),
        MemberType = typeof(TestData)
    )]
    public void FailT_InvalidMessage_ThrowArgumentNullException(
        string? message)
    {
        // Arrange
        Action act = () => Result.Fail(message!);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly<T>(
        string description, Result<T> sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }

    public static IEnumerable<object[]> TValues()
    {
        yield return [42];
        yield return [42L];
        yield return [42.0];
        yield return ["Forty-two"];
        yield return [true];
        yield return [false];
    }
}
