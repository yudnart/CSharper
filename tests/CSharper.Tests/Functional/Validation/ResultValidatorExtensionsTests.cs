using CSharper.Functional;
using CSharper.Functional.Validation;
using CSharper.Results;
using CSharper.Tests.Errors;
using CSharper.Tests.Results;
using FluentAssertions;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ResultValidatorExtensions))]
public sealed class ResultValidatorExtensionsTests
{
    private static readonly string _errorMessage = ErrorTestData.Error.Message;

    private static bool _predicate() => true;
    private static Task<bool> _asyncPredicate() => Task.FromResult(_predicate());

    #region And

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task And(Result initial)
    {
        // Arrange
        Task<ResultValidator> sut = Task.FromResult(new ResultValidator(initial));

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
                ResultValidator validator = result
                    .Should().BeOfType<ResultValidator>().Subject;
                validator.Validate().Should().BeSameAs(initial);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_NullPredicate_ThrowsArgumentException(Result initial)
    {
        // Arrange
        Func<bool> nullPredicate = null!;
        Func<Task<bool>> nullAsyncPredicate = null!;

        Task<ResultValidator> sut = Task.FromResult(new ResultValidator(initial));

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
        nameof(FunctionalResultTestData.ResultInvalidErrorMessages),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_InvalidMessage_ThrowsArgumentException(
        Result initial, string errorMessage)
    {
        // Arrange
        Task<ResultValidator> sut = Task
            .FromResult(new ResultValidator(initial));

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
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure(Result sut)
    {
        // Act
        object[] results = [
            sut.Ensure(_predicate, _errorMessage),
            sut.Ensure(_asyncPredicate, _errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (object result in results)
            {
                ResultValidator validator = result
                    .Should().BeOfType<ResultValidator>().Subject;
                validator.Validate().Should().BeSameAs(sut);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task EnsureAsync(Result initial)
    {
        // Arrange
        Task<Result> sut = Task.FromResult(initial);

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
                ResultValidator validator = result
                    .Should().BeOfType<ResultValidator>().Subject;
                validator.Validate().Should().BeSameAs(initial);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_NullPredicate_ThrowsArgumentNullException<T>(
        Result sut)
    {
        // Arrange
        Func<bool> nullPredicate = null!;
        Func<Task<bool>> nullAsyncPredicate = null!;

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
        nameof(FunctionalResultTestData.ResultInvalidErrorMessages),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_InvalidMessage_ThrowsArgumentException(
        Result sut, string errorMessage)
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
        nameof(FunctionalResultTestData.ResultBindTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task Bind(Result initial, Result nextResult)
    {
        // Arrange
        Result next() => nextResult;
        Task<Result> asyncNext() => Task.FromResult(next());
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

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
        nameof(FunctionalResultTestData.ResultBindTTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task BindT<T>(Result initial, Result<T> nextResult)
    {
        // Arrange
        Result<T> next() => nextResult;
        Task<Result<T>> asyncNext() => Task.FromResult(next());
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

        // Act
        Result<T>[] results = [
            sut.Bind(next),
            await sut.Bind(asyncNext),
            await Task.FromResult(sut).Bind(next),
            await Task.FromResult(sut).Bind(asyncNext)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (Result<T> result in results)
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
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException(Result initial)
    {
        // Arrange
        Func<Result> next = null!;
        Func<Task<Result>> nextAsync = null!;
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

        Func<Task>[] acts = [
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
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException(Result initial)
    {
        // Arrange
        Func<Result<string>> next = null!;
        Func<Task<Result<string>>> nextAsync = null!;
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

        List<Func<Task>> acts = [
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
