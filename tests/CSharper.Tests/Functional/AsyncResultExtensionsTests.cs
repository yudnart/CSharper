using CSharper.Errors;
using CSharper.Functional;
using CSharper.Functional.Validation;
using CSharper.Results;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.FunctionalResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Functional;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(AsyncResultExtensions))]
public sealed class AsyncResultExtensionsTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTestData),
        MemberType = typeof(TestData)
    )]
    public async Task Bind(Result initial, Result nextResult)
    {
        // Arrange
        List<Result> results = [];

        Result next() => nextResult;
        Task<Result> nextAsync() => Task.FromResult(nextResult);

        // Act
        // Test 1:
        // Bind(this Result result, Func<Task<Result>> next)
        results.Add(await initial.Bind(nextAsync));

        // Test 2:
        // Bind(this Task<Result> asyncResult, Func<Result> next)
        results.Add(await Task.FromResult(initial).Bind(next));

        // Test 3:
        // Bind(this Task<Result> asyncResult, Func<Task<Result>> next)
        results.Add(await Task.FromResult(initial).Bind(nextAsync));

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
        nameof(TestData.ResultBindTestData),
        MemberType = typeof(TestData)
    )]
    public async Task Bind_Chain(Result initial, Result final)
    {
        Result result = await initial
            .Bind(() => Result.Ok(42))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok())
            .Bind(() => Task.FromResult(Result.Ok()))
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok(false))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(_ => final);

        if (initial.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailureResult(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException(Result initial)
    {
        // Arrange
        Func<Result> next = null!;
        Func<Task<Result>> nextAsync = null!;

        List<Func<Task>> acts = [
            () => initial.Bind(nextAsync),
            () => Task.FromResult(initial).Bind(next),
            () => Task.FromResult(initial).Bind(nextAsync)
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
        nameof(TestData.ResultBindTTestData),
        MemberType = typeof(TestData)
    )]
    public async Task BindT<T>(Result initial, Result<T> nextResult)
    {
        // Arrange
        List<T> bindParams = [];
        List<Result<T>> results = [];

        Result<T> next() => nextResult;
        Task<Result<T>> nextAsync() => Task.FromResult(next());

        // Act

        // Test 1:
        // Bind<T>(this Result result, Func<Task<Result<T>>> next)
        results.Add(await initial.Bind(nextAsync));

        // Test 2:
        // Bind<T>(this Task<Result> asyncResult, Func<Result<T>> next)
        results.Add(await Task.FromResult(initial).Bind(next));

        // Test 3:
        // Bind<T>(this Task<Result> asyncResult, Func<Task<Result<T>>> next)
        results.Add(await Task.FromResult(initial).Bind(nextAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<T> result = results[idx];
                if (initial.IsSuccess)
                {
                    result.Should().Be(nextResult);
                }
                else
                {
                    TestUtility.AssertFailureResult(result, initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTTestData),
        MemberType = typeof(TestData)
    )]
    public async Task BindT_Chain<T>(Result initial, Result<T> final)
    {
        Result<T> result = await initial
            .Bind(() => Result.Ok(42))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok())
            .Bind(() => Task.FromResult(Result.Ok()))
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok(false))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(_ => final);

        if (initial.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailureResult(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException(Result initial)
    {
        // Arrange
        Func<Result<int>> next = null!;
        Func<Task<Result<string>>> nextAsync = null!;

        List<Func<Task>> acts = [
            () => initial.Bind(nextAsync),
            () => Task.FromResult(initial).Bind(next),
            () => Task.FromResult(initial).Bind(nextAsync)
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
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void Ensure_WithNullPredicate_ThrowsArgumentNullException<T>(
        Result initial)
    {
        // Arrange
        string message = ErrorTestData.Error.Message;
        Func<bool> predicate = null!;
        Func<Task<bool>> asyncPredicate = null!;

        List<Func<Task>> acts = [
            () => Task.FromResult(initial).Ensure(asyncPredicate, message),
            () => Task.FromResult(initial).Ensure(predicate, message),
            () => Task.FromResult(initial.Ensure(asyncPredicate, message))
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
        nameof(TestData.ResultInvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public void Ensure_InvalidParams_ThrowsArgumentNullException<T>(
        Result sut, string message)
    {
        // Arrange
        static bool predicate() => true;

        List<Func<Task>> acts = [
            () => Task.FromResult(sut).Ensure(predicate, message),
            () => Task.FromResult(sut).Ensure(() => false, message),
            () => Task.FromResult(sut.Ensure(() => Task.FromResult(true), message))
        ];

        // Act & Assert
        Assert.Multiple(async () =>
        {
            foreach (Func<Task> act in acts)
            {
                await act.Should()
                    .ThrowExactlyAsync<ArgumentException>()
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.ParamName));
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void MapError(Result initial)
    {
        // Arrange
        Task<Result> sut = Task.FromResult(initial);
        Func<Task<Result<string>>> act = sut.MapError<string>;

        // Act & Assert
        Assert.Multiple(async () =>
        {
            if (initial.IsSuccess)
            {
                await act.Should()
                    .ThrowExactlyAsync<InvalidOperationException>()
                    .Where(ex => !string.IsNullOrWhiteSpace(ex.Message));
            }
            else
            {
                Result<string> result = await act();
                TestUtility.AssertFailureResult(result, initial.Error);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultMatchTestCases),
        MemberType = typeof(TestData)
    )]
    public async Task Match<T>(Result initial, T successValue, T errorValue)
    {
        // Arrange
        List<Error> onFailureParams = [];
        List<T?> results = [];

        T onSuccess() => successValue;

        Task<T> onSuccessAsync() => Task.FromResult(onSuccess());

        T onFailure(Error error)
        {
            onFailureParams.Add(error);
            return errorValue;
        }

        Task<T> onFailureAsync(Error error) => Task.FromResult(onFailure(error));

        // Test 1:
        // Match<T>(this Result result, Func<Task<T>> onSuccess)
        results.Add(await initial.Match(onSuccessAsync));

        // Test 2:
        // Match<T>(this Result result,
        //     Func<Task<T>> onSuccess, Func< Error[], Task <T>> onFailure)
        results.Add(await initial.Match(onSuccessAsync, onFailureAsync));

        // Test 3:
        // Match<T>(this Task<Result> asyncResult, Func<T> onSuccess)
        results.Add(await Task.FromResult(initial).Match(onSuccess));

        // Test 4:
        // Match<T>(this Task<Result> asyncResult,
        //     Func<T> onSuccess, Func<Error[], T> onFailure)
        results.Add(await Task.FromResult(initial).Match(onSuccess, onFailure));

        // Test 5:
        // Match<T>(this Task<Result> asyncResult, Func<Task<T>> onSuccess)
        results.Add(await Task.FromResult(initial).Match(onSuccessAsync));

        // Test 6:
        // Match<T>(this Task<Result> asyncResult,
        //     Func<Task<T>> onSuccess, Func<Error[], Task<T>> onFailure)
        results.Add(await Task.FromResult(initial).Match(onSuccessAsync, onFailureAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                T? result = results[idx];

                if (initial.IsSuccess)
                {
                    result.Should().Be(successValue);
                }
                else
                {
                    result.Should().BeOneOf(default(T), errorValue);
                }
            }

            if (initial.IsFailure)
            {
                foreach (Error onFailureParam in onFailureParams)
                {
                    onFailureParam.Should().BeSameAs(initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public async Task Recover(Result initial)
    {
        // Arrange
        List<Result> results = [];
        List<Error> fallbackParams = [];

        void fallback(Error error)
        {
            fallbackParams.Add(error);
        }

        Task fallbackAsync(Error errors)
        {
            fallback(errors);
            return Task.CompletedTask;
        }

        // Test 1:
        // Recover(this Result result, Func<Error[], Task> fallback)
        results.Add(await initial.Recover(fallbackAsync));

        // Test 2:
        // Recover(this Task<Result> asyncResult, Action<Error[]> fallback)
        results.Add(await Task.FromResult(initial).Recover(fallback));

        // Test 3:
        // Recover(this Task<Result> asyncResult, Func<Error[], Task> fallback)
        results.Add(await Task.FromResult(initial).Recover(fallbackAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result result = results[idx];
                if (initial.IsSuccess)
                {
                    result.Should().Be(initial);
                    fallbackParams.Should().BeEmpty();
                }
                else
                {
                    Error fallbackParam = fallbackParams[idx];
                    fallbackParam.Should().Be(initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public async Task Tap(Result initial)
    {
        // Arrange
        int actionInvokeCounter = 0;
        List<Result> results = [];

        void action()
        {
            actionInvokeCounter++;
        }
        Task asyncAction()
        {
            action();
            return Task.CompletedTask;
        }

        // Act

        // Test 1:
        // Tap(this Result result, Func<Task> action)
        results.Add(await initial.Tap(asyncAction));

        // Test 2:
        // Tap(this Task<Result> asyncResult, Action action)
        results.Add(await Task.FromResult(initial).Tap(action));

        // Test 3:
        // Tap(this Task<Result> asyncResult, Func<Task> action)
        results.Add(await Task.FromResult(initial).Tap(asyncAction));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result result = results[idx];
                result.Should().Be(initial);
            }
            if (initial.IsSuccess)
            {
                actionInvokeCounter.Should().Be(results.Count);

            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public async Task TapError(Result initial)
    {
        // Arrange
        List<Error> actionParams = [];
        List<Result> results = [];

        void action(Error error)
        {
            actionParams.Add(error);
        }

        Task asyncAction(Error error)
        {
            actionParams.Add(error);
            return Task.CompletedTask;
        }

        // Act

        // Test 1:
        // TapError(this Result result, Func<Error[], Task> action)
        results.Add(await initial.TapError(asyncAction));

        // Test 2:
        // TapError(this Task<Result> asyncResult, Action<Error[]> action)
        results.Add(await Task.FromResult(initial).TapError(action));

        // Test 3:
        // TapError(this Task<Result> asyncResult, Func<Error[], Task> action)
        results.Add(await Task.FromResult(initial).TapError(asyncAction));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result result = results[idx];
                result.Should().Be(initial);
                if (initial.IsSuccess)
                {
                    actionParams.Should().BeEmpty();
                }
                else
                {
                    Error actionParam = actionParams[idx];
                    actionParam.Should().Be(initial.Error);
                }
            }
        });
    }
}
