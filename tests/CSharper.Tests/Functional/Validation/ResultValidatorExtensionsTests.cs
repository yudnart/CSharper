using CSharper.Functional;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Errors;
using CSharper.Tests.Results;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit.Sdk;
using TestData = CSharper.Tests.Functional.Validation.ResultValidatorTestData;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultValidatorExtensions))]
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
                result.Should().BeOfType<ResultValidator>();
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
        nameof(TestData.InvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public void And_InvalidMessage_ThrowsArgumentException(
        string errorMessage)
    {
        // Arrange
        Task<ResultValidator> sut = Task
            .FromResult(new ResultValidator(Result.Ok()));

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
                result.Should().BeOfType<ResultValidator>();
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
                result.Should().BeOfType<ResultValidator>();
            }
        });
    }

    #endregion

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_NullPredicate_ThrowsArgumentNullException<T>(
        Result initial)
    {
        // Arrange
        Func<bool> nullPredicate = null!;
        Func<Task<bool>> nullAsyncPredicate = null!;

        Func<Task>[] acts = [
            () => _ = Task.Run(() => initial.Ensure(nullPredicate, _errorMessage)),
            () => _ = Task.Run(() => initial.Ensure(nullAsyncPredicate, _errorMessage)),
            () => _ = Task.FromResult(initial).Ensure(nullPredicate, _errorMessage),
            () => _ = Task.FromResult(initial.Ensure(nullAsyncPredicate, _errorMessage))
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
        nameof(FunctionalResultTestData.ResultBindTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Bind(Result initial, Result nextResult)
    {
        // Arrange
        Result next() => nextResult;
        ResultValidator sut = initial.Ensure(() => true, "Test error");

        // Act
        Result result = sut.Bind(next);

        // Assert
        result.Should().Be(initial.IsSuccess ? nextResult : initial);
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.BindTTestCases),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void BindT<T>(Result initial, Result<T> nextResult)
    {
        // Arrange
        Result<T> next() => nextResult;
        ResultValidator sut = initial.Ensure(() => true, "Test error");

        // Act
        Result<T> result = sut.Bind(next);

        // Assert
        if (initial.IsSuccess)
        {
            result.Should().Be(nextResult);
        }
        else
        {
            ResultTestUtility.AssertFailureResult(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultBindTestData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task BindAsync(Result initial, Result nextResult)
    {
        // Arrange
        Result next() => nextResult;
        Task<Result> nextAsync() => Task.FromResult(nextResult);
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

        // Act
        Result[] results = [
            // Act
            // Test 1:
            // Bind(this ResultValidator validator, Func<Task<Result>> next)
            await sut.Bind(nextAsync),

            // Test 2:
            // Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
            await Task.FromResult(sut).Bind(next),

            // Test 3:
            // Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
            await Task.FromResult(sut).Bind(nextAsync)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (Result result in results)
            {
                result.Should().Be(initial.IsSuccess ? nextResult : initial);
            }
        });
    }


    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.BindTTestCases),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task BindTAsync<T>(Result initial, Result<T> nextResult)
    {
        // Arrange
        Result<T> next() => nextResult;
        Task<Result<T>> nextAsync() => Task.FromResult(nextResult);
        ResultValidator sut = initial.Ensure(_predicate, _errorMessage);

        Result<T>[] results = [
            // Act
            // Test 1:
            // Bind(this ResultValidator validator, Func<Task<Result>> next)
            await sut.Bind(nextAsync),

            // Test 2:
            // Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
            await Task.FromResult(sut).Bind(next),

            // Test 3:
            // Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
            await Task.FromResult(sut).Bind(nextAsync)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Length; idx++)
            {
                Result<T> result = results[idx];
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
}
