using CSharper.Functional;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Errors;
using CSharper.Tests.Results;
using FluentAssertions;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultValidatorTExtensions))]
public sealed class ResultValidatorTExtensionsTests
{
    private static readonly string _errorMessage = ErrorTestData.Error.Message;

    private static bool _predicate<T>(T _) => true;
    private static Task<bool> _asyncPredicate<T>(T _) => Task.FromResult(_predicate(_));


    #region And

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task And<T>(Result<T> initial)
    {
        // Arrange
        ResultValidator<T> validator = new(initial);
        Task<ResultValidator<T>> sut = Task.FromResult(validator);

        // Act
        object[] results = [
            await sut.And(_predicate, _errorMessage),
            await sut.And(_asyncPredicate, _errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (object result in results)
            {
                ResultValidator<T> validator = result
                    .Should().BeOfType<ResultValidator<T>>().Subject;
                validator.Validate().Should().BeSameAs(initial);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_NullPredicate_ThrowsArgumentException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, bool> nullPredicate = null!;
        Func<T, Task<bool>> nullAsyncPredicate = null!;

        ResultValidator<T> validator = new(initial);
        Task<ResultValidator<T>> sut = Task.FromResult(validator);

        Action[] acts = [
            () => _ = sut.And(nullPredicate, _errorMessage),
            () => _ = sut.And(nullAsyncPredicate, _errorMessage)
        ];

        // Act & Assert
        Assert.Multiple(() =>
        {
            foreach (Action act in acts)
            {
                act.Should().Throw<ArgumentNullException>();
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTInvalidErrorMessages),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_InvalidMessage_ThrowsArgumentException<T>(
        Result<T> initial, string errorMessage)
    {
        // Arrange
        ResultValidator<T> validator = new(initial);
        Task<ResultValidator<T>> sut = Task.FromResult(validator);

        Action[] acts = [
            () => _ = sut.And(_predicate, errorMessage),
            () => _ = sut.And(_asyncPredicate, errorMessage)
        ];

        // Act & Assert
        Assert.Multiple(() =>
        {
            foreach (Action act in acts)
            {
                act.Should().Throw<ArgumentException>();
            }
        });
    }

    #endregion

    #region Ensure

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure<T>(Result<T> sut)
    {
        // Arrange
        object[] results = [
            sut.Ensure(_predicate, _errorMessage),
            sut.Ensure(_asyncPredicate, _errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (object result in results)
            {
                ResultValidator<T> validator = result
                    .Should().BeOfType<ResultValidator<T>>().Subject;
                validator.Validate().Should().BeSameAs(sut);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task EnsureAsync<T>(Result<T> initial)
    {
        // Arrange
        Task<Result<T>> sut = Task.FromResult(initial);

        // Act
        object[] results = [
            await sut.Ensure(_predicate, _errorMessage),
            await sut.Ensure(_asyncPredicate, _errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (object result in results)
            {
                ResultValidator<T> validator = result
                    .Should().BeOfType<ResultValidator<T>>().Subject;
                validator.Validate().Should().BeSameAs(initial);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_NullPredicate_ThrowsArgumentNullException<T>(
        Result<T> sut)
    {
        // Arrange
        Func<T, bool> nullPredicate = null!;
        Func<T, Task<bool>> nullAsyncPredicate = null!;

        Func<Task>[] acts = [
            () => _ = Task.Run(() => sut.Ensure(nullPredicate, _errorMessage)),
            () => _ = Task.Run(() => sut.Ensure(nullAsyncPredicate, _errorMessage)),
            () => _ = Task.FromResult(sut).Ensure(nullPredicate, _errorMessage),
            () => _ = Task.FromResult(sut.Ensure(nullAsyncPredicate, _errorMessage))
        ];

        // Act & Assert
        Assert.Multiple(async () =>
        {
            foreach (Func<Task> act in acts)
            {
                (await act.Should().ThrowExactlyAsync<ArgumentNullException>())
                    .And.ParamName.Should().NotBeNullOrWhiteSpace();
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTInvalidErrorMessages),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_InvalidMessage_ThrowsArgumentException<T>(
        Result<T> sut, string errorMessage)
    {
        // Arrange
        Func<Task>[] acts = [
            () => _ = Task.Run(() => sut.Ensure(_predicate, errorMessage)),
            () => _ = Task.Run(() => sut.Ensure(_asyncPredicate, errorMessage)),
            () => _ = Task.FromResult(sut).Ensure(_asyncPredicate, errorMessage),
            () => _ = Task.FromResult(sut.Ensure(_asyncPredicate, errorMessage))
        ];

        // Act & Assert
        Assert.Multiple(async () =>
        {
            foreach (Func<Task> act in acts)
            {
                (await act.Should().ThrowExactlyAsync<ArgumentException>())
                    .And.ParamName.Should().NotBeNullOrWhiteSpace();
            }
        });
    }

    #endregion

    #region Bind

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTBindTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task Bind<T>(Result<T> initial, Result nextResult)
    {
        // Arrange
        Result next(T _) => nextResult;
        Task<Result> asyncNext(T _) => Task.FromResult(next(_));
        ResultValidator<T> sut = initial.Ensure(_predicate, _errorMessage);

        // Act
        Result[] results = [
            sut.Bind(next),
            await sut.Bind(asyncNext),
            await Task.FromResult(sut).Bind(next),
            await Task.FromResult(sut).Bind(asyncNext)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (Result result in results)
            {
                if (initial.IsSuccess)
                {
                    result.Should().Be(nextResult);
                }
                else
                {
                    ResultTestUtility.AssertFailureResult(result, initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTBindTTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task BindT<T, U>(Result<T> initial, Result<U> nextResult)
    {
        // Arrange
        Result<U> next(T _) => nextResult;
        Task<Result<U>> asyncNext(T _) => Task.FromResult(next(_));
        ResultValidator<T> sut = initial.Ensure(_predicate, _errorMessage);

        // Act
        Result<U>[] results = [
            sut.Bind(next),
            await sut.Bind(asyncNext),
            await Task.FromResult(sut).Bind(next),
            await Task.FromResult(sut).Bind(asyncNext)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (Result<U> result in results)
            {
                if (initial.IsSuccess)
                {
                    result.Should().Be(nextResult);
                }
                else
                {
                    ResultTestUtility.AssertFailureResult(result, initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, Result> next = null!;
        Func<T, Task<Result>> nextAsync = null!;
        ResultValidator<T> sut = new(initial);

        Func<Task>[] acts = [
            () => Task.Run(() => sut.Bind(next)),
            () => sut.Bind(nextAsync),
            () => Task.FromResult(sut).Bind(next),
            () => Task.FromResult(sut).Bind(nextAsync)
        ];

        // Act & Assert
        Assert.Multiple(async () =>
        {
            foreach (Func<Task> act in acts)
            {
                await act.Should()
                    .ThrowExactlyAsync<ArgumentNullException>()
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.ParamName));
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, Result<object>> next = null!;
        Func<T, Task<Result<object>>> nextAsync = null!;
        ResultValidator<T> sut = new(initial);

        Func<Task>[] acts = [
            () => Task.Run(() => sut.Bind(next)),
            () => sut.Bind(nextAsync),
            () => Task.FromResult(sut).Bind(next),
            () => Task.FromResult(sut).Bind(nextAsync)
        ];

        // Act & Assert
        Assert.Multiple(async () =>
        {
            foreach (Func<Task> act in acts)
            {
                await act.Should()
                    .ThrowExactlyAsync<ArgumentNullException>()
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.ParamName));
            }
        });
    }

    #endregion
}
