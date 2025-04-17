using CSharper.Functional;
using CSharper.Results;
using CSharper.Tests.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;

namespace CSharper.Tests.Functional;

public class ResultValidationChainTests
{
    [Fact]
    public void Constructor_WithNullResult_ThrowsArgumentNullException()
    {
        Result<string> initialResult = null!;

        Action act = () => new ResultValidationChain<string>(initialResult);

        AssertUtility.AssertArgumentException(act);
    }

    [Fact]
    public void And_WithNullPredicate_ThrowsArgumentNullException()
    {
        Result<string> initialResult = Result.Ok("test");
        ResultValidationChain<string> chain = new(initialResult);
        Func<string, bool> predicate = null!;
        Error error = new("Invalid", code: "INVALID");

        Action act = () => chain.And(predicate, error);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void And_WithNullError_ThrowsArgumentNullException()
    {
        var initialResult = Result.Ok("test");
        var chain = new ResultValidationChain<string>(initialResult);
        Func<string, bool> predicate = s => true;
        Error error = null!;

        Action act = () => chain.And(predicate, error);

        AssertUtility.AssertArgumentException<ArgumentNullException>(act);
    }

    [Fact]
    public void Collect_WithSuccessfulResultAndPassingPredicates_ReturnsSuccess()
    {
        Result<string> initialResult = Result.Ok("test");
        ResultValidationChain<string> chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, new("Too short", code: "SHORT"))
            .And(s => s.All(char.IsLetter), new("Not letters", code: "INVALID"));

        Result<string> result = chain.Collect();

        ResultTestHelpers.AssertResult(result);
        result.Value.Should().Be("test");
    }

    [Fact]
    public void Collect_WithSuccessfulResultAndFailingPredicates_ReturnsFailureWithErrors()
    {
        Result<string> initialResult = Result.Ok("12");
        Error error1 = new("Too short", code: "SHORT", path: "input");
        Error error2 = new("Not letters", code: "INVALID", path: "input");
        ResultValidationChain<string> chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 2, error1)
            .And(s => s.All(char.IsLetter), error2);

        Result<string> result = chain.Collect();

        ResultTestHelpers.AssertResult(result, false);
        result.Errors.Should().HaveCount(2);
        result.Errors[0].Should().Be(error1);
        result.Errors[1].Should().Be(error2);
    }

    [Fact]
    public void Collect_WithFailedResult_ReturnsOriginalFailure()
    {
        Error error1 = new("Initial failure", "FAIL");
        Error error2 = new("Too short", code: "SHORT");
        Result<string> initialResult = Result.Fail<string>(error1);
        ResultValidationChain<string> chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, error2);

        Result<string> result = chain.Collect();

        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().Be(error1);
    }

    [Fact]
    public void Bind_SyncToSyncNonTyped_WithValidChain_ReturnsSuccess()
    {
        var initialResult = Result.Ok("test");
        var chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, new("Too short", code: "SHORT"));
        Func<string, Result> next = _ => Result.Ok();

        var result = chain.Bind(next);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Bind_SyncToSyncTyped_WithValidChain_ReturnsTransformedSuccess()
    {
        var initialResult = Result.Ok("42");
        var chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.All(char.IsDigit), new("Not digits", code: "INVALID"));
        Func<string, Result<int>> next = s => Result.Ok(int.Parse(s));

        var result = chain.Bind(next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task Bind_SyncToAsyncNonTyped_WithValidChain_ReturnsSuccess()
    {
        var initialResult = Result.Ok("test");
        var chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, new("Too short", code: "SHORT"));
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        var result = await chain.Bind(next);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Bind_SyncToAsyncTyped_WithValidChain_ReturnsTransformedSuccess()
    {
        var initialResult = Result.Ok("42");
        var chain = new ResultValidationChain<string>(initialResult)
            .And(s => s.All(char.IsDigit), new("Not digits", code: "INVALID"));
        Func<string, Task<Result<int>>> next = s => Task.FromResult(Result.Ok(int.Parse(s)));

        var result = await chain.Bind(next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void Bind_SyncToSyncNonTyped_WithNullBuilder_ThrowsArgumentNullException()
    {
        ResultValidationChain<string> chain = null;
        Func<string, Result> next = _ => Result.Ok();

        Action act = () => chain.Bind(next);

        act.Should().Throw<ArgumentNullException>().WithParameterName("builder");
    }

    [Fact]
    public void Bind_SyncToSyncNonTyped_WithNullNext_ThrowsArgumentNullException()
    {
        var initialResult = Result.Ok("test");
        var chain = new ResultValidationChain<string>(initialResult);
        Func<string, Result> next = null;

        Action act = () => chain.Bind(next);

        act.Should().Throw<ArgumentNullException>().WithParameterName("next");
    }

    [Fact]
    public async Task And_Async_WithValidChain_ReturnsUpdatedChain()
    {
        var initialResult = Result.Ok("test");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult));
        var predicate = new Func<string, bool>(s => s.Length > 0);
        var error = new Error("Too short", code: "SHORT");

        var updatedChain = await chainTask.And(predicate, error);
        var result = updatedChain.Collect();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("test");
    }

    [Fact]
    public async Task And_Async_WithNullAsyncBuilder_ThrowsArgumentNullException()
    {
        Task<ResultValidationChain<string>> chainTask = null;
        var predicate = new Func<string, bool>(s => true);
        var error = new Error("Invalid", code: "INVALID");

        Func<Task> act = () => chainTask.And(predicate, error);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("asyncBuilder");
    }

    [Fact]
    public async Task Bind_AsyncToSyncNonTyped_WithValidChain_ReturnsSuccess()
    {
        var initialResult = Result.Ok("test");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, new("Too short", code: "SHORT")));
        Func<string, Result> next = _ => Result.Ok();

        var result = await chainTask.Bind(next);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Bind_AsyncToSyncTyped_WithValidChain_ReturnsTransformedSuccess()
    {
        var initialResult = Result.Ok("42");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult)
            .And(s => s.All(char.IsDigit), new("Not digits", code: "INVALID")));
        Func<string, Result<int>> next = s => Result.Ok(int.Parse(s));

        var result = await chainTask.Bind(next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task Bind_AsyncToAsyncNonTyped_WithValidChain_ReturnsSuccess()
    {
        var initialResult = Result.Ok("test");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult)
            .And(s => s.Length > 0, new("Too short", code: "SHORT")));
        Func<string, Task<Result>> next = _ => Task.FromResult(Result.Ok());

        var result = await chainTask.Bind(next);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Bind_AsyncToAsyncTyped_WithValidChain_ReturnsTransformedSuccess()
    {
        var initialResult = Result.Ok("42");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult)
            .And(s => s.All(char.IsDigit), new("Not digits", code: "INVALID")));
        Func<string, Task<Result<int>>> next = s => Task.FromResult(Result.Ok(int.Parse(s)));

        var result = await chainTask.Bind(next);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public async Task Bind_AsyncToSyncNonTyped_WithNullAsyncBuilder_ThrowsArgumentNullException()
    {
        Task<ResultValidationChain<string>> chainTask = null;
        Func<string, Result> next = _ => Result.Ok();

        Func<Task> act = () => chainTask.Bind(next);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("asyncBuilder");
    }

    [Fact]
    public async Task Bind_AsyncToSyncNonTyped_WithNullNext_ThrowsArgumentNullException()
    {
        var initialResult = Result.Ok("test");
        var chainTask = Task.FromResult(new ResultValidationChain<string>(initialResult));
        Func<string, Result> next = null;

        Func<Task> act = () => chainTask.Bind(next);

        await act.Should().ThrowAsync<ArgumentNullException>().WithParameterName("next");
    }
}