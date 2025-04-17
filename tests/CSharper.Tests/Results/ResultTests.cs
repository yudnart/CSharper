using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

public class ResultTests
{
    [Fact]
    public void Success_HasValidProperties()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Success_ReferencesStaticValue()
    {
        // Act
        Result result1 = Result.Ok();
        Result result2 = Result.Ok();

        // Assert
        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public void Fail_WithSingleError_Should_Create_FailureResult_WithOneError()
    {
        // Arrange
        string message = "Something went wrong.";
        string code = "ERR001";
        string path = "User.Name";
        Error error = new Error(message, code, path);

        // Act
        Result result = Result.Fail(error);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Be(error);
        result.Errors[0].Message.Should().Be(message);
        result.Errors[0].Code.Should().Be(code);
        result.Errors[0].Path.Should().Be(path);
    }

    [Fact]
    public void Fail_WithMultipleErrors_Should_Create_FailureResult_WithAllErrors()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002", "System");
        Error detailError1 = new("Detail 1", null, "System.Subsystem");
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        var result = Result.Fail(mainError, detailError1, detailError2);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().ContainInOrder(mainError, detailError1, detailError2);
    }

    [Fact]
    public void Fail_WithNullCausedByError_Should_ThrowArgumentNullException()
    {
        Error nullCasuedBy = null!;
        // Act & Assert
        Action act = () => Result.Fail(nullCasuedBy);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToString_SuccessResult_Should_ReturnSuccess()
    {
        // Arrange
        Result result = Result.Ok();

        // Act
        string resultString = result.ToString();

        // Assert
        resultString.Should().Be("Success");
    }

    [Fact]
    public void ToString_FailureResult_Should_ReturnFormattedErrorList()
    {
        // Arrange
        Error error1 = new("Invalid input", "ERR004", "User.Input");
        Error error2 = new("Missing field", null, "User.Email");
        Error error3 = new("Server error", "ERR005");
        Result result = Result.Fail(error1, error2, error3);

        // Act
        string resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Failure:")
            .And.Contain($"{Environment.NewLine}- {error1}")
            .And.Contain($"{Environment.NewLine}- {error2}")
            .And.Contain($"{Environment.NewLine}- {error3}");
    }

    [Fact]
    public void Error_ToString_Should_BeUsedInFailureResult()
    {
        // Arrange
        string message = "Test error.";
        string code = "ERR007";
        string path = "Mock.Path";
        Error error = new(message, code, path);
        Result result = Result.Fail(error);

        // Act
        string resultString = result.ToString();

        // Assert
        resultString.Should().Contain("Failure:")
            .And.Contain($"{Environment.NewLine}- {error}");
    }
}