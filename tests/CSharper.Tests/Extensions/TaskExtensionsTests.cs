using CSharper.Extensions;
using FluentAssertions;

namespace CSharper.Tests.Extensions;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(CSharper.Extensions.TaskExtensions))]
public sealed class TaskExtensionsTests
{
    [Fact]
    public async Task HandleFault_TaskIsFaulted_ThrowsException()
    {
        // Arrange
        string errorMessage = "Not implemented.";
        Task<int> sut()
        {
            throw new NotImplementedException(errorMessage);
        }

        // Act
        Func<Task<int>> act = () => sut().ContinueWith(task => task
            .HandleFault().Or(t => default(int)));

        // Assert
        await act.Should().ThrowExactlyAsync<NotImplementedException>()
            .WithMessage(errorMessage);
    }

    [Fact]
    public async Task HandleFault_TaskIsFaultedWithAggregateException_ThrowsFirstInnerException()
    {
        // Arrange
        string innerMessage = "Inner exception";
        InvalidOperationException innerException = new(innerMessage);
        AggregateException aggregateException = new(innerException);
        Task task = Task.FromException(aggregateException);

        // Act
        Func<Task> act = () => task.ContinueWith(task => task
            .HandleFault().Or(t => default(int)));

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(innerMessage);
    }

    [Fact]
    public async Task HandleFault_TaskIsNotFaulted_ReturnsOrElseResult()
    {
        // Arrange
        Task task = Task.CompletedTask;
        int value = 42;

        // Act
        int result = await task.ContinueWith(task => task
            .HandleFault().Or(_ => value));

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public async Task HandleFault_TaskIsFaultedWithMultipleInnerExceptions_ThrowsAggregateException()
    {
        // Arrange
        string innerErrorMessage1 = "First inner exception";
        string innerErrorMessage2 = "Second inner exception";
        InvalidOperationException innerException1 = new(innerErrorMessage1);
        ArgumentException innerException2 = new(innerErrorMessage2);
        AggregateException aggregateException = new(innerException1, innerException2);
        Task task = Task.FromException(aggregateException);

        // Act
        Func<Task> act = () => task.ContinueWith(task => task
            .HandleFault().Or(t => default(int)));

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .Where(ex => ex.InnerExceptions.Count == 2
                && ex.InnerExceptions[0].Message == innerErrorMessage1
                && ex.InnerExceptions[1].Message == innerErrorMessage2);
    }

    [Fact]
    public async Task HandleFault_GenericTaskIsNotFaulted_ReturnsOrElseResult()
    {
        // Arrange
        int expectedResult = 100;
        Task<int> task = Task.FromResult(expectedResult);

        // Act
        int result = await task.ContinueWith(task => task
            .HandleFault().Or(t => t.Result));

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task HandleFault_GenericTaskIsFaultedWithMultipleInnerExceptions_ThrowsAggregateException()
    {
        // Arrange
        string innerErrorMessage1 = "First inner exception";
        string innerErrorMessage2 = "Second inner exception";
        InvalidOperationException innerException1 = new(innerErrorMessage1);
        ArgumentException innerException2 = new(innerErrorMessage2);
        var aggregateException = new AggregateException(innerException1, innerException2);
        Task<int> task = Task.FromException<int>(aggregateException);

        // Act
        Func<Task> act = () => task.ContinueWith(task => task
            .HandleFault().Or(t => default(int)));

        // Assert
        await act.Should().ThrowAsync<AggregateException>()
            .Where(ex => ex.InnerExceptions.Count == 2
                && ex.InnerExceptions[0].Message == innerErrorMessage1
                && ex.InnerExceptions[1].Message == innerErrorMessage2);
    }
}
