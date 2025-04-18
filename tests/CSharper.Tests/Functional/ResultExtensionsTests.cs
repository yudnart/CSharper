using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public sealed class ResultExtensionsTests
{
    #region Bind Tests

    [Fact]
    public void Bind_SuccessResult_CallsNextAndReturnsResult()
    {
        Result initial = Result.Ok();
        Func<Result> next = Result.Ok;

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertSuccessResult(result);
    }

    [Fact]
    public void BindT_SuccessResult_CallsNextAndReturnsResultT()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Result<int>> next = () => Result.Ok(value);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertSuccessResult(result, value);
    }

    [Fact]
    public void Bind_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Result> next = Result.Ok;

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void BindT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Result<int>> next = () => Result.Ok(42);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertFailureResult(result, error);
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

    #endregion

    #region MapError Tests

    [Fact]
    public void MapError_FailureResult_ReturnsResultFailureT()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);

        Result<int> result = initial.MapError<int>();

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void MapError_SuccessResult_ThrowsInvalidOperationException()
    {
        Result initial = Result.Ok();

        Action act = () => initial.MapError<int>();

        AssertHelper.AssertException<InvalidOperationException>(act);
    }

    #endregion

    #region Match Tests

    [Fact]
    public void Match_SuccessOnly_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<int> onSuccess = () => value;

        int? result = initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public void Match_SuccessOnlyFailure_ReturnsDefault()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<int> onSuccess = () => 42;

        int? result = initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_SuccessOnlyNullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = null!;

        Action act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<int> onSuccess = () => value;
        Func<Error[], int> onFailure = _ => -1;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public void Match_SuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_SuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = null!;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Recover Tests

    [Fact]
    public void Recover_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = initial.Recover(onFailure);

        ResultTestHelpers.AssertSuccessResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Recover_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = initial.Recover(onFailure);

        ResultTestHelpers.AssertSuccessResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Recover_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Action<Error[]> onFailure = null!;

        Action act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public void Tap_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = initial.Tap(action);

        ResultTestHelpers.AssertSuccessResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Tap_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = initial.Tap(action);

        ResultTestHelpers.AssertFailureResult(result, error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Action action = null!;

        Action act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region TapError Tests

    [Fact]
    public void TapError_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = initial.TapError(action);

        ResultTestHelpers.AssertFailureResult(result, error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void TapError_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = initial.TapError(action);

        ResultTestHelpers.AssertSuccessResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Action<Error[]> action = null!;

        Action act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion
}