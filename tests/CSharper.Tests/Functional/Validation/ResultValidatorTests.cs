using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Results;
using FluentAssertions;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultValidator))]
public sealed class ResultValidatorTests
{
    [Fact]
    public void Ctor_ValidParams_Succeeds()
    {
        // Act
        ResultValidator result = new(Result.Ok());

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public void Ctor_NullContext_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => _ = new ResultValidator(null!);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void And_WithValidParams_ReturnsCurrentInstance()
    {
        // Arrange
        Result initial = Result.Ok();
        ResultValidator sut = new(initial);

        // Act
        ResultValidator result = sut
            .And(() => true, "Value must be less than 100");

        // Assert
        result.Should().BeSameAs(sut);
    }

    [Fact]
    public void And_WithNullPredicate_ThrowsArgumentNullException()
    {
        // Arrange
        Result initial = Result.Ok();
        ResultValidator sut = new(initial);

        Func<bool> predicate = null!;
        Action act = () => sut.And(predicate, "Error");

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void And_WithInvalidMessage_ThrowsArgumentException(string? message)
    {
        // Arrange
        Result initial = Result.Ok();
        ResultValidator sut = new(initial);

        Action act = () => sut.And(() => true, message!);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Validate_WithAllPredicatesPassing_ReturnsOriginalResult()
    {
        // Arrange
        Result initial = Result.Ok();
        ResultValidator sut = initial
            .Ensure(() => true, "Error 1")
            .And(() => true, "Error 2");

        // Act
        Result result = sut.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            result.Should().BeSameAs(initial);
        });
    }

    [Fact]
    public void Validate_WithSinglePredicateFail_ReturnsFailedResult()
    {
        // Arrange
        Result initial = Result.Ok();
        string[] messages = [
            "Error 1",
            "Error 2"
        ];
        string failedMessage = messages[0];
        ResultValidator sut = initial
            .Ensure(() => false, failedMessage)
            .And(() => true, messages[1]);

        // Act
        Result result = sut.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            ResultTestUtility.AssertFailureResult(result, failedMessage);
        });
    }

    [Fact]
    public void Validate_WithMultiplePredicatesFailing_ReturnsFailedResultWithAllErrors()
    {
        // Arrange
        Result initial = Result.Ok();
        string[] messages = [
            "Error 1",
            "Error 2",
            "Error 3"
        ];
        string failedMessage1 = messages[1];
        string failedMessage2 = messages[2];
        ResultValidator ResultValidator = initial
            .Ensure(() => true, messages[0]) // pass
            .And(() => false, failedMessage1) // fail
            .And(() => false, failedMessage2); // fail

        // Act
        Result result = ResultValidator.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            ResultTestUtility.AssertFailureResult(result, [failedMessage1, failedMessage2]);
        });
    }

    [Fact]
    public void Validate_WithInitialFailureResult_ReturnsInitialFailureResult()
    {
        // Arrange
        Error error = new("Initial error");
        Result initial = Result.Fail(error);
        ResultValidator ResultValidator = initial
            .Ensure(() => true, "No-op");

        // Act
        Result result = ResultValidator.Validate();

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
        Result initial = Result.Ok();
        ResultValidator ResultValidator = new(initial);

        // Act
        Result result = ResultValidator.Validate();

        // Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            result.Should().BeSameAs(initial);
        });
    }
}
