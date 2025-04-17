using CSharper.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Results;

public class ResultFactoryTests
{
    [Fact]
    public void Ok_Should_Create_SuccessResult()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        ResultTestHelpers.AssertResult(result);
    }

    [Theory]
    [InlineData("String value")]
    [InlineData(42)]
    [InlineData(42.00)]
    [InlineData(42L)]
    [InlineData(true)]
    [InlineData(false)]
    public void OkT_Should_Create_SuccessResult_WithValue(object value)
    {
        // Act
        Result<object> result = Result.Ok(value);

        // Assert
        ResultTestHelpers.AssertResult(result);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void OkT_WithNullValue_Should_Create_SuccessResult_WithNullValue()
    {
        // Arrange
        string nullString = null!;

        // Act
        Result<string> result = Result.Ok(nullString);

        // Assert
        ResultTestHelpers.AssertResult(result);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Fail_WithSingleError_Should_Create_FailureResult_WithOneError()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001", "User.Name");

        // Act
        Result result = Result.Fail(error);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void FailT_WithSingleError_Should_Create_FailureResult_WithOneError()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001", "User.Name");

        // Act
        Result<string> result = Result.Fail<string>(error);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Fail_WithMultipleErrors_Should_Create_FailureResult_WithAllErrors()
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

    [Fact]
    public void FailT_WithMultipleErrors_Should_Create_FailureResult_WithAllErrors()
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
    }

    [Fact]
    public void Fail_WithNullCausedByError_Should_ThrowArgumentNullException()
    {
        Error nullError = null!;
        // Act & Assert
        Action act = () => Result.Fail(nullError);
        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void FailT_WithNullCausedByError_Should_ThrowArgumentNullException()
    {
        Error nullError = null!;
        // Act & Assert
        Action act = () => Result.Fail<int>(nullError);
        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Fail_WithMessageCodePath_Should_Create_FailureResult_WithError()
    {
        // Arrange
        string message = "Invalid data";
        string code = "ERR004";
        string path = "Data.Field";

        // Act
        Result result = Result.Fail(message, code, path);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(message);
        result.Errors[0].Code.Should().Be(code);
        result.Errors[0].Path.Should().Be(path);
    }

    [Fact]
    public void FailT_WithMessageCodePath_Should_Create_FailureResult_WithError()
    {
        // Arrange
        string message = "Invalid data";
        string code = "ERR004";
        string path = "Data.Field";

        // Act
        Result<double> result = Result.Fail<double>(message, code, path);

        // Assert
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(message);
        result.Errors[0].Code.Should().Be(code);
        result.Errors[0].Path.Should().Be(path);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Fail_WithInvalidMessage_Should_ThrowArgumentException(string? invalidMessage)
    {
        // Act & Assert
        Action act = () => Result.Fail(invalidMessage!);
        AssertUtility.AssertArgumentException<ArgumentException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FailT_WithInvalidMessage_Should_ThrowArgumentException(string? invalidMessage)
    {
        // Act & Assert
        Action act = () => Result.Fail<string>(invalidMessage!);
        act.Should().Throw<ArgumentException>()
            .Which.Should().Match<ArgumentException>(e =>
                !string.IsNullOrWhiteSpace(e.ParamName)
                && !string.IsNullOrWhiteSpace(e.Message));
    }

    #region Collect

    [Fact]
    public void Collect_WithNullResults_Should_ThrowArgumentNullException()
    {
        // Arrange
        IEnumerable<ResultLike> results = null!;

        // Act
        Action act = () => Result.Collect(results);

        // Assert
        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Collect_WithEmptyResults_Should_ThrowArgumentException()
    {
        // Arrange
        IEnumerable<ResultLike> results = new List<ResultLike>();

        // Act
        Action act = () => Result.Collect(results);

        // Assert
        AssertUtility.AssertArgumentException<ArgumentException>(act);
    }

    [Fact]
    public void Collect_WithAllSuccessfulResults_Should_ReturnSuccess()
    {
        // Arrange
        Result result1 = Result.Ok();
        Result<string> result2 = Result.Ok("Success");
        Result<int> result3 = Result.Ok(42);
        List<ResultLike> results =
        [
            result1,
            result2,
            result3
        ];

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual);
    }

    [Fact]
    public void Collect_WithSingleSuccessfulResult_Should_ReturnSuccess()
    {
        // Arrange
        Result result = Result.Ok();
        List<ResultLike> results = [result];

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual);
    }

    [Fact]
    public void Collect_WithAllFailedResults_Should_ReturnFailureWithAllErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001", "Path1");
        Error error2 = new("Error 2", "ERR002", "Path2");
        Error error3 = new("Error 3", "ERR003", "Path3");
        Result result1 = Result.Fail(error1);
        Result<string> result2 = Result.Fail<string>(error2);
        Result<int> result3 = Result.Fail<int>(error3);
        List<ResultLike> results =
        [
            result1,
            result2,
            result3
        ];

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual, false);
        actual.Errors.Should().HaveCount(3);
        actual.Errors.Should().ContainInOrder(error1, error2, error3);
    }

    [Fact]
    public void Collect_WithSingleFailedResult_Should_ReturnFailureWithErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001", "Path1");
        Error error2 = new("Error 2", "ERR002", "Path2");
        Result result = Result.Fail(error1, error2);
        List<ResultLike> results = [result];

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual, false);
        actual.Errors.Should().HaveCount(2);
        actual.Errors.Should().ContainInOrder(error1, error2);
    }

    [Fact]
    public void Collect_WithMixedResults_Should_ReturnFailureWithAllErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001", "Path1");
        Error error2 = new("Error 2", "ERR002", "Path2");
        Result result1 = Result.Ok();
        Result<string> result2 = Result.Fail<string>(error1);
        Result<int> result3 = Result.Fail<int>(error2);
        List<ResultLike> results = new List<ResultLike>
            {
                result1,
                result2,
                result3
            };

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual, false);
        actual.Errors.Should().HaveCount(2);
        actual.Errors.Should().ContainInOrder(error1, error2);
    }

    [Fact]
    public void Collect_WithFactoryResults_Should_ResolveFactoriesAndReturnCorrectResult()
    {
        // Arrange
        Error error = new("Error 1", "ERR001", "Path1");
        Func<ResultBase> successFactory = () => Result.Ok();
        Func<ResultBase> failureFactory = () => Result.Fail(error);
        List<ResultLike> results =
        [
            successFactory,
            failureFactory
        ];

        // Act
        Result actual = Result.Collect(results);

        // Assert
        ResultTestHelpers.AssertResult(actual, false);
        actual.Errors.Should().HaveCount(1);
        actual.Errors.Should().ContainInOrder(error);
    }

    #endregion
}
