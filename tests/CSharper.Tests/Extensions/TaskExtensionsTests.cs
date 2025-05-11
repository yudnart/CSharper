using CSharper.Extensions;
using FluentAssertions;

namespace CSharper.Tests.Extensions;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(CSharper.Extensions.TaskExtensions))]
public sealed class TaskExtensionsTests
{
    private readonly int _value = 42;
    private readonly string _errorMessage = "Test error";
    private readonly string _secondaryErrorMessage = "Secondary error";

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
    public async Task Then_TaskCanceled_ThrowsTaskCanceledException()
    {
        // Arrange
        CancellationTokenSource cts = new();
        cts.Cancel();
        Task task = Task.FromCanceled(cts.Token);
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<TaskCanceledException>()
            .WithMessage($"Task canceled. TaskID={task.Id}, Status={task.Status}.");
    }

    [Fact]
    public async Task Then_TaskFaultedSingleException_ThrowsInnerException()
    {
        // Arrange
        InvalidOperationException innerException = new(_errorMessage);
        Task task = Task.FromException(innerException);
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<InvalidOperationException>()
            .WithMessage(_errorMessage);
    }

    [Fact]
    public async Task Then_TaskFaultedMultipleExceptions_ThrowsAggregateException()
    {
        // Arrange
        InvalidOperationException innerException1 = new(_errorMessage);
        ArgumentException innerException2 = new(_secondaryErrorMessage);
        AggregateException aggregateException = new(innerException1, innerException2);
        Task task = Task.FromException(aggregateException);
        int next() => _value;

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should().ThrowExactlyAsync<AggregateException>()
            .Where(ex => ex.InnerExceptions.Count == 2 &&
                         ex.InnerExceptions[0].Message == _errorMessage &&
                         ex.InnerExceptions[1].Message == _secondaryErrorMessage);
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
    public async Task ThenT_TaskCanceled_ThrowsTaskCanceledException()
    {
        // Arrange
        CancellationTokenSource cts = new();
        cts.Cancel();
        Task<int> task = Task.FromCanceled<int>(cts.Token);
        string next(int x) => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should()
            .ThrowExactlyAsync<TaskCanceledException>()
            .WithMessage($"Task canceled. TaskID={task.Id}, Status={task.Status}.");
    }

    [Fact]
    public async Task ThenT_TaskFaultedSingleException_ThrowsInnerException()
    {
        // Arrange
        InvalidOperationException innerException = new(_errorMessage);
        Task<int> task = Task.FromException<int>(innerException);
        string next(int x) => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(_errorMessage);
    }

    [Fact]
    public async Task ThenT_TaskFaultedMultipleExceptions_ThrowsAggregateException()
    {
        // Arrange
        InvalidOperationException innerException1 = new(_errorMessage);
        ArgumentException innerException2 = new(_secondaryErrorMessage);
        AggregateException aggregateException = new(innerException1, innerException2);
        Task<int> task = Task.FromException<int>(aggregateException);
        string next(int x) => x.ToString();

        // Act
        Func<Task> act = async () => await task.Then(next);

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .Where(ex => ex.InnerExceptions.Count == 2 &&
                         ex.InnerExceptions[0].Message == _errorMessage &&
                         ex.InnerExceptions[1].Message == _secondaryErrorMessage);
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
}