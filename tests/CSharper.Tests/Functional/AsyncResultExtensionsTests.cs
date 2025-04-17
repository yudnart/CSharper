using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public class AsyncResultExtensionsTests
{
    #region Bind Tests

    [Fact]
    public async Task Bind_ResultToAsyncNonTyped_SuccessResult_CallsNextAndReturnsResult()
    {
        Result initial = Result.Ok();
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task Bind_ResultToAsyncNonTyped_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_ResultToAsyncNonTyped_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Bind_ResultToAsyncTyped_SuccessResult_CallsNextAndReturnsResultT()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(value));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task Bind_ResultToAsyncTyped_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_ResultToAsyncTyped_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Bind_TaskResultToSyncNonTyped_SuccessResult_CallsNextAndReturnsResult()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result> next = () => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task Bind_TaskResultToSyncNonTyped_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Result> next = () => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_TaskResultToSyncNonTyped_NullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Bind_TaskResultToSyncTyped_SuccessResult_CallsNextAndReturnsResultT()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Result<int>> next = () => Result.Ok(value);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task Bind_TaskResultToSyncTyped_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Result<int>> next = () => Result.Ok(42);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_TaskResultToSyncTyped_NullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result<int>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Bind_TaskResultToAsyncNonTyped_SuccessResult_CallsNextAndReturnsResult()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task Bind_TaskResultToAsyncNonTyped_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_TaskResultToAsyncNonTyped_NullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Bind_TaskResultToAsyncTyped_SuccessResult_CallsNextAndReturnsResultT()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(value));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task Bind_TaskResultToAsyncTyped_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void Bind_TaskResultToAsyncTyped_NullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region MapError Tests

    [Fact]
    public async Task MapError_TaskResult_FailureResult_ReturnsResultFailureT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));

        Result<int> result = await initial.MapError<int>();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public async Task MapError_TaskResult_SuccessResult_ThrowsInvalidOperationException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());

        Func<Task> act = initial.MapError<int>;

        await AssertHelper.AssertException<InvalidOperationException>(act);
    }

    #endregion

    #region Match Tests

    [Fact]
    public async Task Match_ResultAsyncSuccessOnly_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_ResultAsyncSuccessOnly_FailureResult_ReturnsDefault()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Task<int>> onSuccess = () => Task.FromResult(42);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_ResultAsyncSuccessOnly_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Match_ResultAsyncSuccessAndFailure_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_ResultAsyncSuccessAndFailure_FailureResult_CallsOnFailureAndReturnsValue()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_ResultAsyncSuccessAndFailure_NullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_ResultAsyncSuccessAndFailure_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Match_TaskResultSyncSuccessOnly_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<int> onSuccess = () => value;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_TaskResultSyncSuccessOnly_FailureResult_ReturnsDefault()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<int> onSuccess = () => 42;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_TaskResultSyncSuccessOnly_NullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Match_TaskResultSyncSuccessAndFailure_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<int> onSuccess = () => value;
        Func<Error[], int> onFailure = _ => -1;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_TaskResultSyncSuccessAndFailure_FailureResult_CallsOnFailureAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_TaskResultSyncSuccessAndFailure_NullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_TaskResultSyncSuccessAndFailure_NullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Match_TaskResultAsyncSuccessOnly_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_TaskResultAsyncSuccessOnly_FailureResult_ReturnsDefault()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Task<int>> onSuccess = () => Task.FromResult(42);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void Match_TaskResultAsyncSuccessOnly_NullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Match_TaskResultAsyncSuccessAndFailure_SuccessResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task Match_TaskResultAsyncSuccessAndFailure_FailureResult_CallsOnFailureAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void Match_TaskResultAsyncSuccessAndFailure_NullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Match_TaskResultAsyncSuccessAndFailure_NullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Recover Tests

    [Fact]
    public async Task Recover_ResultAsync_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Recover_ResultAsync_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Recover_ResultAsync_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Error[], Task> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Recover_TaskResultSync_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Recover_TaskResultSync_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Recover_TaskResultSync_NullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Action<Error[]> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Recover_TaskResultAsync_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Recover_TaskResultAsync_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Recover_TaskResultAsync_NullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Error[], Task> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public async Task Tap_ResultAsync_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Tap_ResultAsync_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_ResultAsync_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Tap_TaskResultSync_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Tap_TaskResultSync_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_TaskResultSync_NullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Action action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task Tap_TaskResultAsync_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Tap_TaskResultAsync_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void Tap_TaskResultAsync_NullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region TapError Tests

    [Fact]
    public async Task TapError_ResultAsync_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapError_ResultAsync_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_ResultAsync_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapError_TaskResultSync_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapError_TaskResultSync_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_TaskResultSync_NullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Action<Error[]> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapError_TaskResultAsync_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapError_TaskResultAsync_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapError_TaskResultAsync_NullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion
}