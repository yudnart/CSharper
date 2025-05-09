using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Abstractions;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Results.ResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Results;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(Result))]
public sealed class ResultTests
{
    [Fact]
    public void Ok_ReturnsSuccessResult()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        TestUtility.AssertSuccessResult(result);
    }

    [Fact]
    public void Fail_WithError_ReturnsFailureResult()
    {
        // Act
        Error error = ErrorTestData.Error;
        Result result = Result.Fail(ErrorTestData.Error);

        // Assert
        TestUtility.AssertFailureResult(result, error);
    }

    [Fact]
    public void Fail_NullError_ThrowsArgumentNullException()
    {
        // Arrange
        Error nullError = null!;

        // Act
        Action act = () => _ = Result.Fail(nullError);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.FailValidTestCases),
        MemberType = typeof(TestData)
    )]
    public void Fail_ValidParams_ReturnsFailureResult(
        string message, string? code = null)
    {
        // Act
        Result result = Result.Fail(message, code);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailureResult(result);

            Error error = result.Error!;
            error.Message.Should().Be(message);
            error.Code.Should().Be(code);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.FailInvalidMessageTestCases),
        MemberType = typeof(TestData)
    )]
    public void Fail_InvalidMessage_ThrowArgumentNullException(
        string? message)
    {
        // Arrange
        Action act = () => Result.Fail(message!);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(nameof(SequenceValidTestCases))]
    public void Sequence_ValidParams_ReturnsExpectedResult(ResultLike[] results)
    {
        // Arrange
        string message = "Sequence error.";
        ResultBase[] failures = [
            .. results.Where(r => r.Value.IsFailure).Select(r => r.Value)];
        bool expectedIsSuccess = failures.Length == 0;

        // Act
        Result result = Result.Sequence(results, message);

        // Assert
        Assert.Multiple(() =>
        {
            if (expectedIsSuccess)
            {
                TestUtility.AssertSuccessResult(result);
            }
            else
            {
                TestUtility.AssertFailureResult(result);
                List<ErrorDetail> errorDetails = [];
                foreach (ResultBase failure in failures)
                {
                    Error failureError = failure.Error!;
                    errorDetails.Add(new(failureError.Message, failureError.Code));
                    foreach (ErrorDetail errorDetail in failureError.ErrorDetails)
                    {
                        string _message = $"> {errorDetail.Message}";
                        errorDetails.Add(new(_message, errorDetail.Code));
                    }
                }
                Error error = result.Error!;
                error.Message.Should().Be(message);
                error.ErrorDetails.Should().ContainInOrder(errorDetails);
            }
        });
    }

    [Theory]
    [MemberData(nameof(SequenceInvalidTestCases))]
    public void Sequence_InvalidParams_ThrowArgumentException(
        ResultLike[] results, string message)
    {
        // Arrange
        Action act = () => Result.Sequence(results, message);

        // Act & Assert
        Assert.Multiple(() =>
        {
            if (results == null || results.Length == 0)
            {
                act.Should().ThrowExactly<ArgumentException>()
                    .And.ParamName.Should().NotBeNull();
            }
            else if (string.IsNullOrWhiteSpace(message))
            {
                act.Should().ThrowExactly<ArgumentException>()
                    .And.ParamName.Should().NotBeNull();
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ToStringTestCases),
        MemberType = typeof(TestData)
    )]
    public void ToString_FormatsCorrectly(string description, Result sut, string expected)
    {
        // Act
        string result = sut.ToString();

        // Assert
        result.Should().Be(expected, description);
    }

    public static TheoryData<ResultLike[]> SequenceValidTestCases()
    {
        Func<ResultBase> successResultDelegate = Result.Ok;
        Func<ResultBase> failureResultDelegate = () => Result.Fail(ErrorTestData.ErrorNoCode);

        TheoryData<ResultLike[]> testCases = [];

        // All Success
        testCases.Add([Result.Ok(), Result.Ok(42), successResultDelegate]);

        // Some Failures
        testCases.Add([
            Result.Ok(),
            Result.Fail(ErrorTestData.Error),
            Result<int>.Fail(ErrorTestData.ErrorWithDetails),
            successResultDelegate
        ]);

        // All Failures
        testCases.Add(
        [
            failureResultDelegate,
            Result.Fail(ErrorTestData.Error),
            Result<int>.Fail(ErrorTestData.ErrorWithCode)
        ]);

        return testCases;
    }

    public static TheoryData<ResultLike[], string> SequenceInvalidTestCases()
    {
        return new TheoryData<ResultLike[], string>
        {
            { null!, ErrorTestData.Error.Message },
            { [], ErrorTestData.Error.Message },
            { [Result.Ok()], null! },
            { [Result.Ok()], "" },
            { [Result.Ok()], " " }
        };
    }
}
