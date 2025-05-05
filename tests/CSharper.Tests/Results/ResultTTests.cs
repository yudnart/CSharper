using CSharper.Errors;
using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

public sealed class ResultTTests
{
    [Theory]
    [InlineData("String value")]
    [InlineData(42)]
    [InlineData(42.00)]
    [InlineData(42L)]
    [InlineData(true)]
    [InlineData(false)]
    public void ResultT_SuccessWithValue_HasCorrectStateAndValue(object value)
    {
        // Act
        Result<object> result = Result.Ok(value);

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ResultT_SuccessWithNullValue_HasCorrectStateAndNullValue()
    {
        // Arrange
        string nullString = null!;

        // Act
        Result<string> result = Result.Ok(nullString);

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
        result.Value.Should().Be(nullString);
    }

    [Fact]
    public void ResultT_FailureWithSingleError_HasCorrectErrorsAndThrowsOnValueAccess()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001");

        // Act
        Result<string> result = Result.Fail<string>(error);
        Action act = () => _ = result.Value;

        // Assert
        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void ResultT_FailureWithMultipleErrors_HasCorrectErrorsAndThrowsOnValueAccess()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002");
        Error detailError1 = new("Detail 1", null);
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        Result<int> result = Result.Fail<int>(mainError, detailError1, detailError2);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, mainError, detailError1, detailError2);
    }
}