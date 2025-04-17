using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

public class ResultTTests
{
    [Theory]
    [InlineData("String value")]
    [InlineData(42)]
    [InlineData(42.00)]
    [InlineData(42L)]
    [InlineData(true)]
    [InlineData(false)]
    public void SuccessResult_Should_HaveCorrectValue_AndState(object value)
    {
        // Act
        Result<object> result = Result.Ok(value);

        // Assert
        ResultTestHelpers.AssertResult(result);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void SuccessResult_WithNullValue_Should_HaveNullValue_AndCorrectState()
    {
        // Arrange
        string nullString = null!;

        // Act
        Result<string> result = Result.Ok(nullString);

        // Assert
        ResultTestHelpers.AssertResult(result);
        result.Value.Should().Be(nullString);
    }

    [Fact]
    public void FailureResult_WithSingleError_Should_HaveCorrectErrors_AndThrowOnValueAccess()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001", "User.Name");

        // Act
        Result<string> result = Result.Fail<string>(error);
        Action act = () => _ = result.Value;

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Be(error);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot access the value of a failed result.");
    }

    [Fact]
    public void FailureResult_WithMultipleErrors_Should_HaveCorrectErrors_AndThrowOnValueAccess()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002", "System");
        Error detailError1 = new("Detail 1", null, "System.Subsystem");
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        Result<int> result = Result.Fail<int>(mainError, detailError1, detailError2);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().ContainInOrder(mainError, detailError1, detailError2);
        Action act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot access the value of a failed result.");
    }
}