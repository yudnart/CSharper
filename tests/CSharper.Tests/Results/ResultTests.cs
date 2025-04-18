using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

public sealed class ResultTests
{
    [Fact]
    public void Result_Success_HasCorrectState()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
    }

    [Fact]
    public void Result_Success_ReferencesStaticValue()
    {
        // Act
        Result result1 = Result.Ok();
        Result result2 = Result.Ok();

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public void Result_FailureWithSingleError_HasCorrectState()
    {
        // Arrange
        Error error = new("Something went wrong.", "ERR001", "User.Name");

        // Act
        Result result = Result.Fail(error);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void Result_FailureWithMultipleErrors_HasCorrectState()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002", "System");
        Error detailError1 = new("Detail 1", null, "System.Subsystem");
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        Result result = Result.Fail(mainError, detailError1, detailError2);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, mainError, detailError1, detailError2);
    }
}