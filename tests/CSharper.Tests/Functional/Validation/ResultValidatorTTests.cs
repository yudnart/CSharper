using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Errors;
using CSharper.Tests.Results;
using FluentAssertions;

namespace BECU.Libraries.Results.Tests.Validation;

public sealed class ResultValidatorTTests
{
    [Fact]
    public void Ctor_ValidParams_Succeeds()
    {
        // Act
        ResultValidator<int> result = new(Result.Ok(42));

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Ctor_NullContext_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => _ = new ResultValidator<int>(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void And_WithValidParams_ReturnsCurrentInstance()
    {
        // Arrange
        Result<int> initial = Result.Ok(42);
        ResultValidator<int> sut = new(initial);

        // Act
        ResultValidator<int> result = sut
            .And(x => x < 100, "Value must be less than 100");

        // Assert
        result.Should().BeSameAs(sut);
    }

    [Fact]
    public void And_WithNullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        Result<int> initial = Result.Ok(42);
        ResultValidator<int> sut = new(initial);

        Func<int, bool> predicate = null!;
        Action act = () => sut.And(predicate, "Error");

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void And_WithInvalidMessage_ThrowsArgumentNullException(string? message)
    {
        // Arrange
        Result<int> initial = Result.Ok(42);
        ResultValidator<int> sut = new(initial);

        Action act = () => sut.And(x => x < 100, message!);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Validate_WithAllPredicatesPassing_ReturnsOriginalResult()
    {
        // Arrange
        Error error1 = ErrorTestData.Error;
        Error error2 = ErrorTestData.ErrorWithCode;
        int value = 42;
        Result<int> initial = Result.Ok(42);
        ResultValidator<int> sut = initial
            .Ensure(x => x > 0, error1.Message)
            .And(x => x < 100, error2.Message);

        // Act
        Result<int> result = sut.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial, value);
            result.Should().BeSameAs(initial);
        });
    }

    [Fact]
    public void Validate_WithSinglePredicateFail_ReturnsFailedResult()
    {
        // Arrange
        int value = -42;
        Result<int> initial = Result.Ok(value);
        Error error1 = ErrorTestData.Error;
        Error error2 = ErrorTestData.ErrorWithCode;
        string failedMessage = error1.Message;
        ResultValidator<int> sut = initial
            .Ensure(x => x > 0, failedMessage)
            .And(x => x < 100, error2.Message);

        // Act
        Result<int> result = sut.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial, value);
            ResultTestUtility.AssertFailureResult(result, failedMessage);
        });
    }

    [Fact]
    public void Validate_WithMultiplePredicatesFailing_ReturnsFailedResultWithAllErrors()
    {
        // Arrange
        int value = 150;
        Result<int> initial = Result.Ok(value);
        Error error1 = ErrorTestData.Error;
        Error error2 = ErrorTestData.ErrorNoCode;
        Error error3 = ErrorTestData.ErrorWithCode;
        string failedMessage1 = error2.Message;
        string failedMessage2 = error3.Message;
        ResultValidator<int> ResultValidator = initial
            .Ensure(x => x > 0, error1.Message) // pass
            .And(x => x < 100, failedMessage1) // fail
            .And(x => x % 17 == 0, failedMessage2); // fail

        // Act
        Result<int> result = ResultValidator.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial, value);
            ResultTestUtility.AssertFailureResult(result, [failedMessage1, failedMessage2]);
        });
    }

    [Fact]
    public void Validate_WithFailedInitialResult_BypassEvaluationAndReturnsFailureResult()
    {
        // Arrange
        Error error = new("Initial error");
        Result<int> initial = Result.Fail<int>(error);
        ResultValidator<int> ResultValidator = initial
            .Ensure(x => x > 0, "Value must be positive");

        // Act
        Result<int> result = ResultValidator.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertFailureResult(initial);
            result.Should().BeSameAs(initial);
        });
    }

    [Fact]
    public void Validate_WithNoPredicates_ReturnsOriginalResult()
    {
        // Arrange
        decimal value = 42.0M;
        Result<decimal> initial = Result.Ok(value);
        ResultValidator<decimal> ResultValidator = new(initial);

        // Act
        Result<decimal> result = ResultValidator.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial, value);
            result.Should().BeSameAs(initial);
        });
    }

    [Fact]
    public void Validate_WithReferenceType_WorksCorrectly()
    {
        // Arrange
        string value = "String";
        Error error1 = ErrorTestData.Error;
        Error error2 = ErrorTestData.ErrorNoCode;
        Error error3 = ErrorTestData.ErrorWithCode;
        ResultValidator<string> ResultValidator = Result.Ok(value)
            .Ensure(s => ReferenceEquals(s, value), error1.Message)
            .And(s => !string.IsNullOrEmpty(s), error2.Message)
            .And(s => s.Length <= 10, error3.Message);

        // Act
        Result<string> result = ResultValidator.Validate();

        // Assert
        ResultTestUtility.AssertSuccessResult(result, value);
    }
}
