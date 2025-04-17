using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public sealed class AsyncResultExtensionsTests
{
    #region Bind Tests

    [Fact]
    public async Task BindAsync_SuccessResult_CallsNextAndReturnsResult()
    {
        Result initial = Result.Ok();
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_FailureResult_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_SuccessResult_CallsNextAndReturnsResultT()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(value));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task BindAsyncT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result initial = Result.Fail(error);
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_NullNext_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsync_TaskResult_CallsNextAndReturnsResult()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result> next = () => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_TaskResultFailure_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Result> next = () => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_TaskResultNullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_TaskResult_CallsNextAndReturnsResultT()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Result<int>> next = () => Result.Ok(value);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Result<int>> next = () => Result.Ok(42);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_TaskResultNullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Result<int>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsync_CallsNextAndReturnsResult()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncFailure_ReturnsOriginalFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Task<Result>> next = () => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_TaskResultAsyncNullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultAsync_CallsNextAndReturnsResultT()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(value));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, value);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultAsyncFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));
        Func<Task<Result<int>>> next = () => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_TaskResultAsyncNullNext_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region MapError Tests

    [Fact]
    public async Task MapErrorAsync_FailureResult_ReturnsResultFailureT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result> initial = Task.FromResult(Result.Fail(error));

        Result<int> result = await initial.MapError<int>();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public async Task MapErrorAsync_SuccessResult_ThrowsInvalidOperationException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());

        Func<Task> act = () => initial.MapError<int>();

        await AssertHelper.AssertException<InvalidOperationException>(act);
    }

    #endregion

    #region Match Tests

    [Fact]
    public async Task MatchAsync_SuccessOnly_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_SuccessOnlyFailure_ReturnsDefault()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Task<int>> onSuccess = () => Task.FromResult(42);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_SuccessOnlyNullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_SuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Result initial = Result.Ok();
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_SuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_SuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_SuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSync_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<int> onSuccess = () => value;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncFailure_ReturnsDefault()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<int> onSuccess = () => 42;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncSuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<int> onSuccess = () => value;
        Func<Error[], int> onFailure = _ => -1;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncSuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncSuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncSuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<int> onSuccess = () => 42;
        Func<Error[], int> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_TaskResultFailure_ReturnsDefault()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Task<int>> onSuccess = () => Task.FromResult(42);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_TaskResultNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        int value = 42;
        Func<Task<int>> onSuccess = () => Task.FromResult(value);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(value);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Task<int>> onSuccess = () => Task.FromResult(42);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_TaskResultSuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_TaskResultSuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
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
    public async Task RecoverAsync_FailureResult_CallsOnFailureAndReturnsSuccess()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task RecoverAsync_SuccessResult_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void RecoverAsync_NullOnFailure_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Error[], Task> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSync_CallsOnFailureAndReturnsSuccess()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSyncSuccess_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action<Error[]> onFailure = _ => wasCalled = true;

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void RecoverAsync_TaskResultSyncNullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Action<Error[]> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RecoverAsync_TaskResult_CallsOnFailureAndReturnsSuccess()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSuccess_DoesNotCallOnFailureAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Error[], Task> onFailure = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Recover(onFailure);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void RecoverAsync_TaskResultNullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Error[], Task> onFailure = null!;

        Func<Task> act = () => initial.Recover(onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public async Task TapAsync_SuccessResult_CallsActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_FailureResult_DoesNotCallActionAndReturnsOriginal()
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
    public void TapAsync_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Ok();
        Func<Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapAsync_TaskResultSync_CallsActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action action = () => wasCalled = true;

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_TaskResultSyncFailure_DoesNotCallActionAndReturnsOriginal()
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
    public void TapAsync_TaskResultSyncNullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Action action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapAsync_TaskResult_CallsActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Task> action = () => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_TaskResultFailure_DoesNotCallActionAndReturnsOriginal()
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
    public void TapAsync_TaskResultNullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        Func<Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region TapError Tests

    [Fact]
    public async Task TapErrorAsync_FailureResult_CallsActionAndReturnsOriginal()
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
    public async Task TapErrorAsync_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        Result initial = Result.Ok();
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_NullAction_ThrowsArgumentNullException()
    {
        Result initial = Result.Fail("Error", code: "FAIL");
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapErrorAsync_TaskResultSync_CallsActionAndReturnsOriginal()
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
    public async Task TapErrorAsync_TaskResultSyncSuccess_DoesNotCallActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_TaskResultSyncNullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Action<Error[]> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapErrorAsync_TaskResult_CallsActionAndReturnsOriginal()
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
    public async Task TapErrorAsync_TaskResultSuccess_DoesNotCallActionAndReturnsOriginal()
    {
        Task<Result> initial = Task.FromResult(Result.Ok());
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_TaskResultNullAction_ThrowsArgumentNullException()
    {
        Task<Result> initial = Task.FromResult(Result.Fail("Error", code: "FAIL"));
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion
}