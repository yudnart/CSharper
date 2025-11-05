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
    [MemberData(nameof(FailWithDataTestCases))]
    public void Fail_WithCodeAndData_ReturnsFailureResult(string code, object data)
    {
        // Act
        Result result = Result.Fail(code, data);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be(code);
            error.Data.Should().NotBeNull().And.NotBeEmpty();
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void Fail_WithCodeAndData_InvalidCode_ThrowsArgumentException(string? code)
    {
        // Arrange
        var data = new { Value = 42 };
        Action act = () => Result.Fail(code!, data);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Fact]
    public void Fail_WithCodeAndData_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        Action act = () => Result.Fail("ERR001", data: null!);

        // Act & Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Fact]
    public void Try_SuccessfulAction_ReturnsSuccessResult()
    {
        // Arrange
        bool wasExecuted = false;
        Action action = () => wasExecuted = true;

        // Act
        Result result = Result.Try(action);

        // Assert
        Assert.Multiple(() =>
        {
            wasExecuted.Should().BeTrue();
            TestUtility.AssertSuccess(result);
        });
    }

    [Fact]
    public void Try_ThrowingAction_ReturnsFailureResult()
    {
        // Arrange
        const string exceptionMessage = "Test exception";
        Action action = () => throw new InvalidOperationException(exceptionMessage);

        // Act
        Result result = Result.Try(action);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be("EXCEPTION");
            error.Message.Should().Be(exceptionMessage);
            error.Data.Should().NotBeNull();
        });
    }

    [Fact]
    public void Try_ThrowingAction_WithCustomErrorCode_ReturnsFailureWithCustomCode()
    {
        // Arrange
        const string customErrorCode = "CUSTOM_ERROR";
        Action action = () => throw new ArgumentException("Test error");

        // Act
        Result result = Result.Try(action, customErrorCode);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);
            result.Error!.Code.Should().Be(customErrorCode);
        });
    }

    [Fact]
    public void Try_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Action nullAction = null!;

        // Act
        Action act = () => Result.Try(nullAction);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void Try_InvalidErrorCode_ThrowsArgumentException(string? errorCode)
    {
        // Arrange
        Action action = () => { };

        // Act
        Action act = () => Result.Try(action, errorCode!);

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Fact]
    public void TryFunc_SuccessfulFunc_ReturnsSuccessResultWithValue()
    {
        // Arrange
        const int expectedValue = 42;
        Func<int> func = () => expectedValue;

        // Act
        Result<int> result = Result.Try(func);

        // Assert
        TestUtility.AssertSuccess(result, expectedValue);
    }

    [Fact]
    public void TryFunc_ThrowingFunc_ReturnsFailureResult()
    {
        // Arrange
        const string exceptionMessage = "Test exception";
        Func<int> func = () => throw new InvalidOperationException(exceptionMessage);

        // Act
        Result<int> result = Result.Try(func);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be("EXCEPTION");
            error.Message.Should().Be(exceptionMessage);
            error.Data.Should().NotBeNull();
        });
    }

    [Fact]
    public void TryFunc_ThrowingFunc_WithCustomErrorCode_ReturnsFailureWithCustomCode()
    {
        // Arrange
        const string customErrorCode = "CUSTOM_FUNC_ERROR";
        Func<string> func = () => throw new ArgumentException("Test error");

        // Act
        Result<string> result = Result.Try(func, customErrorCode);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);
            result.Error!.Code.Should().Be(customErrorCode);
        });
    }

    [Fact]
    public void TryFunc_NullFunc_ThrowsArgumentNullException()
    {
        // Arrange
        Func<int> nullFunc = null!;

        // Act
        Action act = () => Result.Try(nullFunc);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void TryFunc_InvalidErrorCode_ThrowsArgumentException(string? errorCode)
    {
        // Arrange
        Func<int> func = () => 42;

        // Act
        Action act = () => Result.Try(func, errorCode!);

        // Assert
        act.Should().ThrowExactly<ArgumentException>()
            .And.ParamName.Should().NotBeNull();
    }

    [Fact]
    public async Task TryAsync_SuccessfulAction_ReturnsSuccessResult()
    {
        // Arrange
        bool wasExecuted = false;
        Func<Task> action = async () =>
        {
            await Task.Delay(1);
            wasExecuted = true;
        };

        // Act
        Result result = await Result.Try(action);

        // Assert
        Assert.Multiple(() =>
        {
            wasExecuted.Should().BeTrue();
            TestUtility.AssertSuccess(result);
        });
    }

    [Fact]
    public async Task TryAsync_ThrowingAction_ReturnsFailureResult()
    {
        // Arrange
        const string exceptionMessage = "Async test exception";
        Func<Task> action = async () =>
        {
            await Task.Delay(1);
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act
        Result result = await Result.Try(action);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be("EXCEPTION");
            error.Message.Should().Be(exceptionMessage);
            error.Data.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task TryAsync_ThrowingAction_WithCustomErrorCode_ReturnsFailureWithCustomCode()
    {
        // Arrange
        const string customErrorCode = "ASYNC_ERROR";
        Func<Task> action = () => Task.FromException(new ArgumentException("Async error"));

        // Act
        Result result = await Result.Try(action, customErrorCode);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);
            result.Error!.Code.Should().Be(customErrorCode);
        });
    }

    [Fact]
    public void TryAsync_NullAction_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Task> nullAction = null!;

        // Act
        Func<Task> act = async () => await Result.Try(nullAction);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void TryAsync_InvalidErrorCode_ThrowsArgumentException(string? errorCode)
    {
        // Arrange
        Func<Task> action = () => Task.CompletedTask;

        // Act
        Func<Task> act = async () => await Result.Try(action, errorCode!);

        // Assert
        act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task TryAsyncFunc_SuccessfulFunc_ReturnsSuccessResultWithValue()
    {
        // Arrange
        const int expectedValue = 42;
        Func<Task<int>> func = async () =>
        {
            await Task.Delay(1);
            return expectedValue;
        };

        // Act
        Result<int> result = await Result.Try(func);

        // Assert
        TestUtility.AssertSuccess(result, expectedValue);
    }

    [Fact]
    public async Task TryAsyncFunc_ThrowingFunc_ReturnsFailureResult()
    {
        // Arrange
        const string exceptionMessage = "Async func exception";
        Func<Task<int>> func = async () =>
        {
            await Task.Delay(1);
            throw new InvalidOperationException(exceptionMessage);
        };

        // Act
        Result<int> result = await Result.Try(func);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);

            Error error = result.Error!;
            error.Code.Should().Be("EXCEPTION");
            error.Message.Should().Be(exceptionMessage);
            error.Data.Should().NotBeNull();
        });
    }

    [Fact]
    public async Task TryAsyncFunc_ThrowingFunc_WithCustomErrorCode_ReturnsFailureWithCustomCode()
    {
        // Arrange
        const string customErrorCode = "ASYNC_FUNC_ERROR";
        Func<Task<string>> func = () => Task.FromException<string>(new ArgumentException("Async func error"));

        // Act
        Result<string> result = await Result.Try(func, customErrorCode);

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertFailure(result);
            result.Error!.Code.Should().Be(customErrorCode);
        });
    }

    [Fact]
    public void TryAsyncFunc_NullFunc_ThrowsArgumentNullException()
    {
        // Arrange
        Func<Task<int>> nullFunc = null!;

        // Act
        Func<Task> act = async () => await Result.Try(nullFunc);

        // Assert
        act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Theory]
    [MemberData(
        nameof(TestData.NullOrEmptyStrings),
        MemberType = typeof(TestData)
    )]
    public void TryAsyncFunc_InvalidErrorCode_ThrowsArgumentException(string? errorCode)
    {
        // Arrange
        Func<Task<int>> func = () => Task.FromResult(42);

        // Act
        Func<Task> act = async () => await Result.Try(func, errorCode!);

        // Assert
        act.Should().ThrowAsync<ArgumentException>();
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

    public static TheoryData<string, object> FailWithDataTestCases()
    {
        return new TheoryData<string, object>
        {
            { "ERR001", new { Value = 42 } },
            { "ERR002", new { Name = "Test", Id = 1 } },
            { "ERR003", new { Key = "value", Count = 5 } },
            { "ERR004", new { Numbers = new[] { 1, 2, 3 }, Total = 6 } }
        };
    }
}
