using CSharper.Extensions;
using FluentAssertions;

namespace CSharper.Tests.Extensions;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(CSharper.Extensions.TaskExtensions))]
public sealed class TaskExtensionsTests
{
    [Fact]
    public async Task HandleFault_TaskIsFaulted_ThrowsActualException()
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
}
