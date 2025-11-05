using CSharper.Extensions;
using FluentAssertions;

namespace CSharper.Tests.Extensions;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(CSharper.Extensions.TaskExtensions))]
public sealed class TaskExtensionsTests
{
    private readonly int _value = 42;
    private readonly string _errorMessage = "Test error";

    [Fact]
    public async Task Then_TaskSuccessful_ReturnsNext()
    {
        // Arrange
        Task sut = Task.CompletedTask;
        int next() => _value;

        // Act
        Task<int> resultTask = sut.Then(next);
        int result = await resultTask;

        // Assert
        Assert.Multiple(() =>
        {
            resultTask.Status.Should().Be(TaskStatus.RanToCompletion);
            result.Should().Be(_value);
        });
    }

    [Fact]
    public async Task ThenT_TaskSuccessful_ReturnsNext()
    {
        // Arrange
        Task<int> task = Task.FromResult(_value);
        string next(int x) => x.ToString();

        // Act
        Task<string> resultTask = task.Then(next);
        string result = await resultTask;

        // Assert
        Assert.Multiple(() =>
        {
            resultTask.Status.Should().Be(TaskStatus.RanToCompletion);
            result.Should().Be(_value.ToString());
        });
    }

    [Fact]
    public async Task Then_NullTask_ThrowsArgumentNullException()
    {
        // Arrange
        Task task = null!;
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<ArgumentNullException>())
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Then_NullNext_ThrowsArgumentNullException()
    {
        // Arrange
        Task task = Task.CompletedTask;
        Func<int> next = null!;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<ArgumentNullException>())
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ThenT_NullTask_ThrowsArgumentNullException()
    {
        // Arrange
        Task<int> task = null!;
        Func<int, string> next = x => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<ArgumentNullException>())
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ThenT_NullNext_ThrowsArgumentNullException()
    {
        // Arrange
        Task<int> task = Task.FromResult(_value);
        Func<int, string> next = null!;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<ArgumentNullException>())
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Then_TaskCanceled_ThrowsTaskCanceledException()
    {
        // Arrange
        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();
        Task task = Task.FromCanceled(cts.Token);
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task ThenT_TaskCanceled_ThrowsTaskCanceledException()
    {
        // Arrange
        var cts = new System.Threading.CancellationTokenSource();
        cts.Cancel();
        Task<int> task = Task.FromCanceled<int>(cts.Token);
        string next(int x) => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should().ThrowExactlyAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task Then_TaskFaulted_ThrowsAggregateException()
    {
        // Arrange
        var tcs = new TaskCompletionSource<bool>();
        tcs.SetException(new InvalidOperationException(_errorMessage));
        Task task = tcs.Task;
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<AggregateException>())
            .WithInnerException<InvalidOperationException>()
            .WithMessage($"*{_errorMessage}*");
    }

    [Fact]
    public async Task ThenT_TaskFaulted_ThrowsAggregateException()
    {
        // Arrange
        var tcs = new TaskCompletionSource<int>();
        tcs.SetException(new InvalidOperationException(_errorMessage));
        Task<int> task = tcs.Task;
        string next(int x) => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        (await act.Should().ThrowExactlyAsync<AggregateException>())
            .WithInnerException<InvalidOperationException>()
            .WithMessage($"*{_errorMessage}*");
    }

    [Fact]
    public async Task Then_AsyncTaskSuccessful_ReturnsNext()
    {
        // Arrange
        async Task task() => await Task.Delay(10);
        int next() => _value;

        // Act
        Task<int> resultTask = task().Then(next);
        int result = await resultTask;

        // Assert
        Assert.Multiple(() =>
        {
            resultTask.Status.Should().Be(TaskStatus.RanToCompletion);
            result.Should().Be(_value);
        });
    }

    [Fact]
    public async Task ThenT_AsyncTaskSuccessful_ReturnsNext()
    {
        // Arrange
        async Task<int> task()
        {
            await Task.Delay(10);
            return _value;
        }
        string next(int x) => x.ToString();

        // Act
        Task<string> resultTask = task().Then(next);
        string result = await resultTask;

        // Assert
        Assert.Multiple(() =>
        {
            resultTask.Status.Should().Be(TaskStatus.RanToCompletion);
            result.Should().Be(_value.ToString());
        });
    }
}