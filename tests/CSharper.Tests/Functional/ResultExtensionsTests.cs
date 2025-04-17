using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void Bind_SuccessResult_CallsNextAndReturnsResult()
    {
        Result initial = Result.Ok();
        Func<Result> next = Result.Ok;

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public void BindT_SuccessResult_CallsNextAndReturnsResultT()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Result<int>> next = () => Result.Ok(value);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public void Bind_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", "FAIL");
        Result initial = Result.Fail(error);
        Func<Result> next = Result.Ok;

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", "FAIL");
        Result initial = Result.Fail(error);

        int value = 42;
        Func<Result<int>> next = () => Result.Ok(value);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Result> next = null!;

        Action act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void BindT_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Result<int>> next = null!;

        Action act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MapError_FailureResult_ReturnsResultFailureT()
    {
        Result initial = Result.Fail("Error", code: "FAIL");

        Result<int> result = initial.MapError<int>();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ToString().Should().Be("Error (FAIL)");
    }

    [Fact]
    public void MapError_SuccessResult_ThrowsInvalidOperationException()
    {
        Result initial = Result.Ok();

        Action act = () => initial.MapError<int>();

        AssertHelper.AssertException<InvalidOperationException>(act);
    }

    [Fact]
    public void Match_SuccessOnly_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<int> onSuccess = () => value;

        int? result = initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public void Match_SuccessOnly_FailureResult_ReturnsDefault()
    {
        Result initial = Result.Fail("Error");
        int value = 42;
        Func<int> onSuccess = () => value;

        int? result = initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_SuccessOnly_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = null!;

        Action act = () => initial.Match(onSuccess!);

        AssertHelper.AssertException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<int> onSuccess = () => value;
        Func<Error[], int> onFailure = _ => -1;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public void Match_SuccessAndFailure_FailureResult_CallsOnFailureAndReturnsValue()
    {
        Result initial = Result.Fail("Error");
        int value = 42;
        Func<int> onSuccess = () => value;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_SuccessAndFailure_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = null!;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Recover_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Recover_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Recover_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Action<Error[]> onFailure = null!;

        Action act = () => initial.Recover(onFailure);

        AssertHelper.AssertException<ArgumentNullException>(act);
    }

    [Fact]
    public void Tap_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Tap_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors[0].Should().Be(error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Action action = null!;

        Action act = () => initial.Tap(action);

        AssertHelper.AssertException<ArgumentNullException>(act);
    }

    [Fact]
    public void TapError_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors[0].Should().Be(error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void TapError_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error");
        Action<Error[]> action = null!;

        Action act = () => initial.TapError(action);

        AssertHelper.AssertException<ArgumentNullException>(act);
    }
}
