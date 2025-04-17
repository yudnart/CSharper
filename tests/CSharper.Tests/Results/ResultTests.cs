using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

public class ResultTests
{
    [Fact]
    public void SuccessResult_Should_HaveCorrectState()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public void SuccessResult_ReferencesStaticValue()
    {
        // Act
        Result result1 = Result.Ok();
        Result result2 = Result.Ok();

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public void FailureResult_WithSingleError_Should_HaveCorrectState()
    {
        // Arrange
        Error error = new("Something went wrong.", "ERR001", "User.Name");

        // Act
        Result result = Result.Fail(error);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Be(error);
    }

    [Fact]
    public void FailureResult_WithMultipleErrors_Should_HaveCorrectState()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002", "System");
        Error detailError1 = new("Detail 1", null, "System.Subsystem");
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        Result result = Result.Fail(mainError, detailError1, detailError2);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().ContainInOrder(mainError, detailError1, detailError2);
    }
}