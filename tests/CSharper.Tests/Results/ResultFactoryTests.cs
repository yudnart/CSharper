using CSharper.Errors;
using CSharper.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Results;

public sealed class ResultFactoryTests
{
    [Fact]
    public void Ok_Default_CreatesSuccessResult()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
    }

    [Theory]
    [InlineData("String value")]
    [InlineData(42)]
    [InlineData(42.00)]
    [InlineData(42L)]
    [InlineData(true)]
    [InlineData(false)]
    public void OkT_WithValue_CreatesSuccessResultWithValue(object value)
    {
        // Act
        Result<object> result = Result.Ok(value);

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
        result.Value.Should().Be(value);
    }

    [Fact]
    public void OkT_NullValue_CreatesSuccessResultWithNullValue()
    {
        // Arrange
        string nullString = null!;

        // Act
        Result<string> result = Result.Ok(nullString);

        // Assert
        ResultTestHelpers.AssertSuccessResult(result);
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Fail_SingleError_CreatesFailureWithOneError()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001");

        // Act
        Result result = Result.Fail(error);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void FailT_SingleError_CreatesFailureWithOneError()
    {
        // Arrange
        Error error = new("Something went wrong", "ERR001");

        // Act
        Result<string> result = Result.Fail<string>(error);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void Fail_MultipleErrors_CreatesFailureWithAllErrors()
    {
        // Arrange
        Error mainError = new("Primary error", "ERR002");
        Error detailError1 = new("Detail 1", null);
        Error detailError2 = new("Detail 2", "ERR003");

        // Act
        Result result = Result.Fail(mainError, detailError1, detailError2);

        // Assert
        ResultTestHelpers.AssertFailureResult(result, mainError, detailError1, detailError2);
    }

    [Fact]
    public void FailT_MultipleErrors_CreatesFailureWithAllErrors()
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

    [Fact]
    public void Fail_NullCausedByError_ThrowsArgumentNullException()
    {
        // Arrange
        Error nullError = null!;

        // Act & Assert
        Action act = () => Result.Fail(nullError);
        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void FailT_NullCausedByError_ThrowsArgumentNullException()
    {
        // Arrange
        Error nullError = null!;

        // Act & Assert
        Action act = () => Result.Fail<int>(nullError);
        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Fail_MessageCodePath_CreatesFailureWithError()
    {
        // Arrange
        string message = "Invalid data";
        string code = "ERR004";
        string path = "Data.Field";

        // Act
        Result result = Result.Fail(message, code);

        // Assert
        ResultTestHelpers.AssertFailureResult(result);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(message);
        result.Errors[0].Code.Should().Be(code);
    }

    [Fact]
    public void FailT_MessageCodePath_CreatesFailureWithError()
    {
        // Arrange
        string message = "Invalid data";
        string code = "ERR004";
        string path = "Data.Field";

        // Act
        Result<double> result = Result.Fail<double>(message, code);

        // Assert
        ResultTestHelpers.AssertFailureResult(result);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Message.Should().Be(message);
        result.Errors[0].Code.Should().Be(code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Fail_InvalidMessage_ThrowsArgumentException(string? invalidMessage)
    {
        // Act & Assert
        Action act = () => Result.Fail(invalidMessage!);
        AssertHelper.AssertArgumentException<ArgumentException>(act);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FailT_InvalidMessage_ThrowsArgumentException(string? invalidMessage)
    {
        // Act & Assert
        Action act = () => Result.Fail<string>(invalidMessage!);
        AssertHelper.AssertArgumentException<ArgumentException>(act);
    }

    #region Collect

    [Fact]
    public void Collect_NullResults_ThrowsArgumentNullException()
    {
        // Arrange
        IEnumerable<ResultLike> results = null!;

        // Act
        Action act = () => Result.Sequence(results);

        // Assert
        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Collect_EmptyResults_ThrowsArgumentException()
    {
        // Arrange
        IEnumerable<ResultLike> results = new List<ResultLike>();

        // Act
        Action act = () => Result.Sequence(results);

        // Assert
        AssertHelper.AssertArgumentException<ArgumentException>(act);
    }

    [Fact]
    public void Collect_AllSuccessfulResults_ReturnsSuccess()
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
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertSuccessResult(actual);
    }

    [Fact]
    public void Collect_SingleSuccessfulResult_ReturnsSuccess()
    {
        // Arrange
        Result result = Result.Ok();
        List<ResultLike> results = [result];

        // Act
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertSuccessResult(actual);
    }

    [Fact]
    public void Collect_AllFailedResults_ReturnsFailureWithAllErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001");
        Error error2 = new("Error 2", "ERR002");
        Error error3 = new("Error 3", "ERR003");
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
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertFailureResult(actual,
            error1, error2, error3);
    }

    [Fact]
    public void Collect_SingleFailedResult_ReturnsFailureWithErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001");
        Error error2 = new("Error 2", "ERR002");
        Result result = Result.Fail(error1, error2);
        List<ResultLike> results = [result];

        // Act
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertFailureResult(result);
        actual.Errors.Should().HaveCount(2);
        actual.Errors.Should().ContainInOrder(error1, error2);
    }

    [Fact]
    public void Collect_MixedResults_ReturnsFailureWithAllErrors()
    {
        // Arrange
        Error error1 = new("Error 1", "ERR001");
        Error error2 = new("Error 2", "ERR002");
        Result result1 = Result.Ok();
        Result<string> result2 = Result.Fail<string>(error1);
        Result<int> result3 = Result.Fail<int>(error2);
        List<ResultLike> results =
        [
            result1,
            result2,
            result3
        ];

        // Act
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertFailureResult(actual, error1, error2);
    }

    [Fact]
    public void Collect_FactoryResults_ResolvesFactoriesAndReturnsCorrectResult()
    {
        // Arrange
        Error error = new("Error 1", "ERR001");
        Func<ResultBase> successFactory = () => Result.Ok();
        Func<ResultBase> failureFactory = () => Result.Fail(error);
        List<ResultLike> results =
        [
            successFactory,
            failureFactory
        ];

        // Act
        Result actual = Result.Sequence(results);

        // Assert
        ResultTestHelpers.AssertFailureResult(actual, error);
    }

    #endregion
}