using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public sealed class ResultTExtensionsTests
{
    #region Bind Tests

    [Fact]
    public void Bind_SuccessResult_CallsNextAndReturnsResult()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result> next = _ => Result.Ok();

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertSuccessResult(result);
    }

    [Fact]
    public void Bind_FailureResult_ReturnsMappedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Result> next = _ => Result.Ok();

        Result result = initial.Bind(next);

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void Bind_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result> next = null!;

        Action act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void BindT_SuccessResult_CallsNextAndReturnsResultT()
    {
        string initialValue = "42";
        int expected = int.Parse(initialValue);
        Result<string> initial = Result.Ok(initialValue);
        Func<string, Result<int>> next = s => Result.Ok(expected);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertSuccessResult(result, expected);
    }

    [Fact]
    public void BindT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Result<int>> next = _ => Result.Ok(42);

        Result<int> result = initial.Bind(next);

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void BindT_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Result<int>> next = null!;

        Action act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Ensure Tests

    [Fact]
    public void Ensure_SuccessResult_ReturnsValidationChain()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = s => s.Length > 0;
        Error error = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = initial.Ensure(predicate, error);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertSuccessResult(result, "test");
    }

    [Fact]
    public void Ensure_FailureResult_ReturnsValidationChainWithFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, bool> predicate = s => s.Length > 0;
        Error validationError = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = initial.Ensure(predicate, validationError);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void Ensure_NullPredicate_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = null!;
        Error error = new("Too short", code: "SHORT");

        Action act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Ensure_NullError_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, bool> predicate = s => s.Length > 0;
        Error error = null!;

        Action act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Map Tests

    [Fact]
    public void Map_SuccessResult_TransformsValueAndReturnsResult()
    {
        Result<string> initial = Result.Ok("42");
        Func<string, int> map = s => int.Parse(s);
        int expected = 42;

        Result<int> result = initial.Map(map);

        ResultTestHelpers.AssertSuccessResult(result, expected);
    }

    [Fact]
    public void Map_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, int> map = s => int.Parse(s);

        Result<int> result = initial.Map(map);

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void Map_NullMap_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> map = null!;

        Action act = () => initial.Map(map);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region MapError Tests

    [Fact]
    public void MapError_FailureResult_ReturnsNonTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);

        Result result = initial.MapError();

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void MapError_SuccessResult_ThrowsInvalidOperationException()
    {
        Result<string> initial = Result.Ok("test");

        Action act = () => initial.MapError();

        AssertHelper.AssertException<InvalidOperationException>(act);
    }

    [Fact]
    public void MapErrorT_FailureResult_ReturnsTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);

        Result<int> result = initial.MapError<string, int>();

        ResultTestHelpers.AssertFailureResult(result, error);
    }

    [Fact]
    public void MapErrorT_SuccessResult_ThrowsInvalidOperationException()
    {
        Result<string> initial = Result.Ok("test");

        Action act = () => initial.MapError<string, int>();

        AssertHelper.AssertException<InvalidOperationException>(act);
    }

    #endregion

    #region Match Tests

    [Fact]
    public void Match_SuccessOnly_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        int expected = 4;

        int? result = initial.Match(onSuccess);

        result.Should().Be(expected);
    }

    [Fact]
    public void Match_SuccessOnlyFailure_ReturnsDefault()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<string, int> onSuccess = s => s.Length;

        int? result = initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_SuccessOnlyNullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = null!;

        Action act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = _ => -1;
        int expected = 4;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(expected);
    }

    [Fact]
    public void Match_SuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_SuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_SuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = null!;

        Action act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Recover Tests

    [Fact]
    public void Recover_FailureResult_CallsFallbackAndReturnsResult()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        string recoveredValue = "recovered";
        Func<Error[], Result<string>> fallback = _ => Result.Ok(recoveredValue);

        Result<string> result = initial.Recover(fallback);

        ResultTestHelpers.AssertSuccessResult(result, recoveredValue);
    }

    [Fact]
    public void Recover_SuccessResult_DoesNotCallFallbackAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        Func<Error[], Result<string>> fallback = _ => Result.Ok("recovered");

        Result<string> result = initial.Recover(fallback);

        ResultTestHelpers.AssertSuccessResult(result, originalValue);
    }

    [Fact]
    public void Recover_NullFallback_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<Error[], Result<string>> fallback = null!;

        Action act = () => initial.Recover(fallback);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public void Tap_SuccessResult_CallsActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = initial.Tap(action);

        ResultTestHelpers.AssertSuccessResult(result, originalValue);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void Tap_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = initial.Tap(action);

        ResultTestHelpers.AssertFailureResult(result, error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Action<string> action = null!;

        Action act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region TapError Tests

    [Fact]
    public void TapError_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = initial.TapError(action);

        ResultTestHelpers.AssertFailureResult(result, error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public void TapError_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = initial.TapError(action);

        ResultTestHelpers.AssertSuccessResult(result, originalValue);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Action<Error[]> action = null!;

        Action act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion
}