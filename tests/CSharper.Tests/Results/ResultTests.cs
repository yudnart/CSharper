using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Abstractions;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Results.ResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Results;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(Result))]
public sealed class ResultTests
{
    [Fact]
    public void Ok_ReturnsSuccessResult()
    {
        // Act
        Result result = Result.Ok();

        // Assert
        TestUtility.AssertSuccess(result);
    }

    [Fact]
    public void Fail_WithError_ReturnsFailureResult()
    {
        // Act
        Error error = ErrorTestData.Error;
        Result result = Result.Fail(ErrorTestData.Error);

        // Assert
        TestUtility.AssertFailure(result, error);
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
        nameof(TestData.FailValidParams),
        MemberType = typeof(TestData)
    )]
    public void Fail_ValidParams_ReturnsFailureResult(
        string code, string? message = null)
    {
        // Act
        Result result = Result.Fail(code, message);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be(code);
            error.Message.Should().Be(message);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
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
        ResultBase[] failures = [
            .. results.Where(r => r.Value.IsFailure).Select(r => r.Value)];
        bool expectedIsSuccess = failures.Length == 0;

        // Act
        Result result = Result.Sequence(results);

        // Assert
        Assert.Multiple(() =>
        {
            if (expectedIsSuccess)
            {
                TestUtility.AssertSuccess(result);
            }
            else
            {
                TestUtility.AssertFailure(result);

                AggregateError error = result.Error
                    .Should().BeOfType<AggregateError>().Subject;

                IEnumerable<Error> details = failures
                    .Select(r => r.Error!);
                
                error.Details.Should().ContainInOrder(details);
            }
        });
    }

    [Theory]
    [MemberData(nameof(SequenceInvalidTestCases))]
    public void Sequence_InvalidParams_ThrowArgumentException(
        ResultLike[] results, string code)
    {
        // Arrange
        Action act = () => Result.Sequence(results, code);

        // Act & Assert
        Assert.Multiple(() =>
        {
            if (results == null || results.Length == 0)
            {
                act.Should().ThrowExactly<ArgumentException>()
                    .And.ParamName.Should().NotBeNull();
            }
            else if (string.IsNullOrWhiteSpace(code))
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
        Func<ResultBase> failureResultDelegate = () => Result.Fail(ErrorTestData.ErrorNoMessage);

        TheoryData<ResultLike[]> testCases = [];

        // All Success
        testCases.Add([Result.Ok(), Result.Ok(42), successResultDelegate]);

        // Some Failures
        testCases.Add([
            Result.Ok(),
            Result.Fail(ErrorTestData.Error),
            Result<int>.Fail(ErrorTestData.DetailedError),
            successResultDelegate
        ]);

        // All Failures
        testCases.Add(
        [
            failureResultDelegate,
            Result.Fail(ErrorTestData.Error),
            Result<int>.Fail(ErrorTestData.ErrorWithMessage)
        ]);

        return testCases;
    }

    public static TheoryData<ResultLike[], string> SequenceInvalidTestCases()
    {
        return new TheoryData<ResultLike[], string>
        {
            { null!, ErrorTestData.Error.Code },
            { [], ErrorTestData.Error.Code },
            { [Result.Ok()], null! },
            { [Result.Ok()], "" },
            { [Result.Ok()], " " }
        };
    }
}
