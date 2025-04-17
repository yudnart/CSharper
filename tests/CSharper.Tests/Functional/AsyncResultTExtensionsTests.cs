using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public sealed class AsyncResultTExtensionsTests
{
    #region Bind Tests

    [Fact]
    public async Task BindAsync_SuccessResult_CallsNextAndReturnsResult()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_FailureResult_ReturnsMappedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_SuccessResult_CallsNextAndReturnsResultT()
    {
        Result<string> initial = Result.Ok("42");
        int expected = 42;
        Func<string, Task<Result<int>>> next = _ => Task.FromResult(Result.Ok(expected));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task BindAsyncT_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Task<Result<int>>> next = _ => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_NullNext_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsync_TaskResult_CallsNextAndReturnsResult()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Result> next = _ => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_TaskResultFailure_ReturnsMappedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Result> next = _ => Result.Ok();

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_TaskResultNullNext_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Result> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_TaskResult_CallsNextAndReturnsResultT()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("42"));
        int expected = 42;
        Func<string, Result<int>> next = _ => Result.Ok(expected);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Result<int>> next = _ => Result.Ok(42);

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_TaskResultNullNext_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Result<int>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsync_CallsNextAndReturnsResult()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncFailure_ReturnsMappedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        Result result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsync_TaskResultAsyncNullNext_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<Result>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultAsync_CallsNextAndReturnsResultT()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("42"));
        int expected = 42;
        Func<string, Task<Result<int>>> next = _ => Task.FromResult(Result.Ok(expected));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task BindAsyncT_TaskResultAsyncFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Task<Result<int>>> next = _ => Task.FromResult(Result.Ok(42));

        Result<int> result = await initial.Bind(next);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void BindAsyncT_TaskResultAsyncNullNext_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<Result<int>>> next = null!;

        Func<Task> act = () => initial.Bind(next);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Ensure Tests

    [Fact]
    public async Task EnsureAsync_SuccessResult_ReturnsValidationChain()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error error = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, error);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, "test");
    }

    [Fact]
    public async Task EnsureAsync_FailureResult_ReturnsValidationChainWithFailure()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error validationError = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, validationError);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void EnsureAsync_NullPredicate_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<bool>> predicate = null!;
        Error error = new("Too short", code: "SHORT");

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void EnsureAsync_NullError_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error error = null!;

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task EnsureAsync_TaskResultSync_ReturnsValidationChain()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, bool> predicate = _ => true;
        Error error = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, error);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, "test");
    }

    [Fact]
    public async Task EnsureAsync_TaskResultSyncFailure_ReturnsValidationChainWithFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, bool> predicate = _ => true;
        Error validationError = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, validationError);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void EnsureAsync_TaskResultSyncNullPredicate_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, bool> predicate = null!;
        Error error = new("Too short", code: "SHORT");

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void EnsureAsync_TaskResultSyncNullError_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, bool> predicate = _ => true;
        Error error = null!;

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task EnsureAsync_TaskResult_ReturnsValidationChain()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error error = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, error);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, "test");
    }

    [Fact]
    public async Task EnsureAsync_TaskResultFailure_ReturnsValidationChainWithFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error validationError = new("Too short", code: "SHORT");

        ResultValidationChain<string> chain = await initial.Ensure(predicate, validationError);

        Result<string> result = chain.Collect();
        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void EnsureAsync_TaskResultNullPredicate_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<bool>> predicate = null!;
        Error error = new("Too short", code: "SHORT");

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void EnsureAsync_TaskResultNullError_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<bool>> predicate = _ => Task.FromResult(true);
        Error error = null!;

        Func<Task> act = () => initial.Ensure(predicate, error);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Map Tests

    [Fact]
    public async Task MapAsync_SuccessResult_TransformsValueAndReturnsResult()
    {
        Result<string> initial = Result.Ok("42");
        Func<string, Task<int>> map = s => Task.FromResult(int.Parse(s));
        int expected = 42;

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task MapAsync_FailureResult_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        Func<string, Task<int>> map = s => Task.FromResult(int.Parse(s));

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void MapAsync_NullMap_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> map = null!;

        Func<Task> act = () => initial.Map(map);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MapAsync_TaskResultSync_TransformsValueAndReturnsResult()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("42"));
        Func<string, int> map = s => int.Parse(s);
        int expected = 42;

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task MapAsync_TaskResultSyncFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, int> map = s => int.Parse(s);

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void MapAsync_TaskResultSyncNullMap_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> map = null!;

        Func<Task> act = () => initial.Map(map);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MapAsync_TaskResult_TransformsValueAndReturnsResult()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("42"));
        Func<string, Task<int>> map = s => Task.FromResult(int.Parse(s));
        int expected = 42;

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, expected);
    }

    [Fact]
    public async Task MapAsync_TaskResultFailure_MapsErrorsToResultT()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        Func<string, Task<int>> map = s => Task.FromResult(int.Parse(s));

        Result<int> result = await initial.Map(map);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public void MapAsync_TaskResultNullMap_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> map = null!;

        Func<Task> act = () => initial.Map(map);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region MapError Tests

    [Fact]
    public async Task MapErrorAsync_TaskResult_ReturnsNonTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));

        Result result = await initial.MapError();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public async Task MapErrorAsync_SuccessResult_ThrowsInvalidOperationException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));

        Func<Task> act = () => initial.MapError();

        await AssertHelper.AssertException<InvalidOperationException>(act);
    }

    [Fact]
    public async Task MapErrorAsyncT_TaskResult_ReturnsTypedFailure()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));

        Result<int> result = await initial.MapError<string, int>();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
    }

    [Fact]
    public async Task MapErrorAsyncT_TaskResultSuccess_ThrowsInvalidOperationException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));

        Func<Task<Result<int>>> act = () => initial.MapError<string, int>();

        await AssertHelper.AssertException<InvalidOperationException>(act);
    }

    #endregion

    #region Match Tests

    [Fact]
    public async Task MatchAsync_SuccessOnly_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        int expected = 4;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_SuccessOnlyFailure_ReturnsDefault()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_SuccessOnlyNullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_SuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);
        int expected = 4;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_SuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_SuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_SuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSync_CallsOnSuccessAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> onSuccess = s => s.Length;
        int expected = 4;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncFailure_ReturnsDefault()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<string, int> onSuccess = s => s.Length;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncSuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = _ => -1;
        int expected = 4;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncSuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = errors => errors.Length;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncSuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> onSuccess = null!;
        Func<Error[], int> onFailure = _ => -1;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_TaskResultSyncSuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, int> onSuccess = s => s.Length;
        Func<Error[], int> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResult_CallsOnSuccessAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        int expected = 4;

        int? result = await initial.Match(onSuccess);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_TaskResultFailure_ReturnsDefault()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);

        int? result = await initial.Match(onSuccess);

        result.Should().Be(default);
    }

    [Fact]
    public void MatchAsync_TaskResultNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> onSuccess = null!;

        Func<Task> act = () => initial.Match(onSuccess);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSuccessAndFailure_CallsOnSuccessAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);
        int expected = 4;

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(expected);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSuccessAndFailureFailure_CallsOnFailureAndReturnsValue()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = errors => Task.FromResult(errors.Length);

        int result = await initial.Match(onSuccess, onFailure);

        result.Should().Be(1);
    }

    [Fact]
    public void MatchAsync_TaskResultSuccessAndFailureNullOnSuccess_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> onSuccess = null!;
        Func<Error[], Task<int>> onFailure = _ => Task.FromResult(-1);

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void MatchAsync_TaskResultSuccessAndFailureNullOnFailure_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task<int>> onSuccess = s => Task.FromResult(s.Length);
        Func<Error[], Task<int>> onFailure = null!;

        Func<Task> act = () => initial.Match(onSuccess, onFailure);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Recover Tests

    [Fact]
    public async Task RecoverAsync_FailureResult_CallsFallbackAndReturnsResult()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        string recoveredValue = "recovered";
        Func<Error[], Task<Result<string>>> fallback = _ => Task.FromResult(Result.Ok(recoveredValue));

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, recoveredValue);
    }

    [Fact]
    public async Task RecoverAsync_SuccessResult_DoesNotCallFallbackAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        Func<Error[], Task<Result<string>>> fallback = _ => Task.FromResult(Result.Ok("recovered"));

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, originalValue);
    }

    [Fact]
    public void RecoverAsync_NullFallback_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<Error[], Task<Result<string>>> fallback = null!;

        Func<Task> act = () => initial.Recover(fallback);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSync_CallsFallbackAndReturnsResult()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        string recoveredValue = "recovered";
        Func<Error[], Result<string>> fallback = _ => Result.Ok(recoveredValue);

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, recoveredValue);
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSyncSuccess_DoesNotCallFallbackAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        Func<Error[], Result<string>> fallback = _ => Result.Ok("recovered");

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, originalValue);
    }

    [Fact]
    public void RecoverAsync_TaskResultSyncNullFallback_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<Error[], Result<string>> fallback = null!;

        Func<Task> act = () => initial.Recover(fallback);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task RecoverAsync_TaskResult_CallsFallbackAndReturnsResult()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        string recoveredValue = "recovered";
        Func<Error[], Task<Result<string>>> fallback = _ => Task.FromResult(Result.Ok(recoveredValue));

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, recoveredValue);
    }

    [Fact]
    public async Task RecoverAsync_TaskResultSuccess_DoesNotCallFallbackAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        Func<Error[], Task<Result<string>>> fallback = _ => Task.FromResult(Result.Ok("recovered"));

        Result<string> result = await initial.Recover(fallback);

        ResultTestHelpers.AssertResult(result, originalValue);
    }

    [Fact]
    public void RecoverAsync_TaskResultNullFallback_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<Error[], Task<Result<string>>> fallback = null!;

        Func<Task> act = () => initial.Recover(fallback);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region Tap Tests

    [Fact]
    public async Task TapAsync_SuccessResult_CallsActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Func<string, Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_FailureResult_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Func<string, Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapAsync_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Ok("test");
        Func<string, Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapAsync_TaskResultSync_CallsActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_TaskResultSyncFailure_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        bool wasCalled = false;
        Action<string> action = _ => wasCalled = true;

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapAsync_TaskResultSyncNullAction_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Action<string> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapAsync_TaskResult_CallsActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        bool wasCalled = false;
        Func<string, Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapAsync_TaskResultFailure_DoesNotCallActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        bool wasCalled = false;
        Func<string, Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.Tap(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapAsync_TaskResultNullAction_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Ok("test"));
        Func<string, Task> action = null!;

        Func<Task> act = () => initial.Tap(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion

    #region TapError Tests

    [Fact]
    public async Task TapErrorAsync_FailureResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Result<string> initial = Result.Fail<string>(error);
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapErrorAsync_SuccessResult_DoesNotCallActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Result<string> initial = Result.Ok(originalValue);
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_NullAction_ThrowsArgumentNullException()
    {
        Result<string> initial = Result.Fail<string>("Error", code: "FAIL");
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapErrorAsync_TaskResultSync_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapErrorAsync_TaskResultSyncSuccess_DoesNotCallActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        bool wasCalled = false;
        Action<Error[]> action = _ => wasCalled = true;

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_TaskResultSyncNullAction_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Action<Error[]> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public async Task TapErrorAsync_TaskResult_CallsActionAndReturnsOriginal()
    {
        Error error = new("Error", code: "FAIL");
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>(error));
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().ContainSingle(e => e == error);
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task TapErrorAsync_TaskResultSuccess_DoesNotCallActionAndReturnsOriginal()
    {
        string originalValue = "test";
        Task<Result<string>> initial = Task.FromResult(Result.Ok(originalValue));
        bool wasCalled = false;
        Func<Error[], Task> action = _ => { wasCalled = true; return Task.CompletedTask; };

        Result<string> result = await initial.TapError(action);

        ResultTestHelpers.AssertResult(result, originalValue);
        wasCalled.Should().BeFalse();
    }

    [Fact]
    public void TapErrorAsync_TaskResultNullAction_ThrowsArgumentNullException()
    {
        Task<Result<string>> initial = Task.FromResult(Result.Fail<string>("Error", code: "FAIL"));
        Func<Error[], Task> action = null!;

        Func<Task> act = () => initial.TapError(action);

        AssertHelper.AssertArgumentException<ArgumentNullException>(act);
    }

    #endregion
}