using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.FunctionalResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Functional;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(AsyncResultTExtensions))]
public sealed class AsyncResultTExtensionsTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTestData),
        MemberType = typeof(TestData)
    )]
    public async Task Bind<T>(Result<T> initial, Result nextResult)
    {
        // Arrange
        List<T> bindParams = [];

        Result next(T value)
        {
            bindParams.Add(value);
            return nextResult;
        }
        Task<Result> nextAsync(T value) => Task.FromResult(next(value));

        // Act
        Result[] results = [
            // Test 1:
            // Bind<T>(this Result<T> result, Func<T, Task<Result>> next)
            await initial.Bind(nextAsync),

            // Test 2:
            // Bind<T>(this Task<Result<T>> asyncResult, Func<T, Result> next)
            await Task.FromResult(initial).Bind(next),

            // Test 3:
            // Bind<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<Result<U>>> next)
            await Task.FromResult(initial).Bind(nextAsync)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Length; idx++)
            {
                Result result = results[idx];
                if (initial.IsSuccess)
                {
                    result.Should().Be(nextResult);

                    T? bindParam = bindParams[idx];
                    bindParam.Should().Be(initial.Value);
                }
                else
                {
                    TestUtility.AssertFailure(result, initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTestData),
        MemberType = typeof(TestData)
    )]
    public async Task Bind_Chain<T>(Result<T> initial, Result final)
    {
        // Act
        Result result = await initial
            .Bind(value => Result.Ok(42))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok())
            .Bind(() => Task.FromResult(Result.Ok()))
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok(false))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(_ => final);

        // Assert
        if (initial.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailure(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, Result> next = null!;
        Func<T, Task<Result>> nextAsync = null!;

        Func<Task>[] acts = [
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
        nameof(TestData.ResultTBindTTestData),
        MemberType = typeof(TestData)
    )]
    public async Task BindT<T, U>(Result<T> initial, Result<U> nextResult)
    {
        // Arrange
        List<T> bindParams = [];

        Result<U> next(T value)
        {
            bindParams.Add(value);
            return nextResult;
        }
        Task<Result<U>> nextAsync(T value) => Task.FromResult(next(value));

        // Act
        Result<U>[] results = [
                // Test 1:
            // Bind<T, U>(this Result<T> result, Func<T, Task<Result<U>>> next)
            await initial.Bind(nextAsync),

            // Test 2:
            // Bind<T, U>(this Task<Result<T>> asyncResult,
            //     Func<T, Result<U>> next)
            await Task.FromResult(initial).Bind(next),

            // Test 3:
            // Bind<T, U>(this Task<Result<T>> asyncResult,
            //     Func<T, Task<Result<U>>> next)
            await Task.FromResult(initial).Bind(nextAsync)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Length; idx++)
            {
                Result<U> result = results[idx];
                if (initial.IsSuccess)
                {
                    result.Should().Be(nextResult);

                    T? bindParam = bindParams[idx];
                    bindParam.Should().Be(initial.Value);
                }
                else
                {
                    TestUtility.AssertFailure(result, initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTTestData),
        MemberType = typeof(TestData)
    )]
    public async Task BindT_Chain<T, U>(Result<T> initial, Result<U> final)
    {
        // Act
        Result<U> result = await initial
            .Bind(value => Result.Ok(42))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok())
            .Bind(() => Task.FromResult(Result.Ok()))
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(value => Result.Ok(false))
            .Bind(value => Task.FromResult(Result.Ok(value)))
            .Bind(_ => final);

        // Assert
        if (initial.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailure(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, Result<double>> next = null!;
        Func<T, Task<Result<bool>>> nextAsync = null!;

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
        nameof(TestData.ResultTMapTestCases),
        MemberType = typeof(TestData)
    )]
    public async Task Map<T, U>(Result<T> initial, U value)
    {
        // Arrange
        List<T> mapParams = [];
        List<Result<U>> results = [];

        U transform(T _value)
        {
            mapParams.Add(_value);
            return value;
        }
        Task<U> transformAsync(T _value) => Task.FromResult(transform(_value));

        // Act

        // Test 1:
        // Map(this Result<T>, Func<T, Task<U>> transform)
        results.Add(await initial.Map(transformAsync));

        // Test 2:
        // Map(this Task<Result<T>>, Func<T, U> transform)
        results.Add(await Task.FromResult(initial).Map(transform));

        // Test 3:
        // Map(this Task<Result<T>>, Func<T, Task<U>> transform)
        results.Add(await Task.FromResult(initial).Map(transformAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<U> result = results[idx];
                if (initial.IsSuccess)
                {
                    TestUtility.AssertSuccess(result, value);
                    mapParams[idx].Should().Be(initial.Value);
                }
                else
                {
                    TestUtility.AssertFailure(result, initial.Error);
                    mapParams.Should().BeEmpty();
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task Map_WithNullMap_ThrowsArgumentNullException<T>(Result<T> sut)
    {
        // Arrange
        Func<T, string> map = null!;
        Func<T, Task<string>> mapAsync = null!;
        Func<Task>[] acts = [
            () => sut.Map(mapAsync),
            () => Task.FromResult(sut).Map(map),
            () => Task.FromResult(sut).Map(mapAsync)
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
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task MapError<T>(Result<T> initial)
    {
        // Arrange
        Task<Result<T>> sut = Task.FromResult(initial);
        Func<Task<Result>> act = () => sut.MapError();

        // Act & Assert
        if (initial.IsSuccess)
        {
            await act.Should().ThrowExactlyAsync<InvalidOperationException>()
                .Where(ex => !string.IsNullOrWhiteSpace(ex.Message));
        }
        else
        {
            Result result = await act();
            TestUtility.AssertFailure(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task MapErrorT<T>(Result<T> initial)
    {
        // Arrange
        Task<Result<T>> sut = Task.FromResult(initial);
        Func<Task<Result<string>>> act = sut.MapError<T, string>;

        // Act & Assert
        if (initial.IsSuccess)
        {
            await act.Should().ThrowExactlyAsync<InvalidOperationException>()
                .Where(ex => !string.IsNullOrWhiteSpace(ex.Message));
        }
        else
        {
            Result<string> result = await act();
            TestUtility.AssertFailure(result, initial.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTMatchTestCases),
        MemberType = typeof(TestData)
    )]
    public async Task Match<T, U>(Result<T> initial, U successValue, U errorValue)
    {
        // Arrange
        List<T?> onSuccessParams = [];
        List<Error> onFailureParams = [];
        List<U?> results = [];

        U onSuccess(T value)
        {
            onSuccessParams.Add(value);
            return successValue;
        }

        Task<U> onSuccessAsync(T value) => Task.FromResult(onSuccess(value));

        U onFailure(Error error)
        {
            onFailureParams.Add(error);
            return errorValue;
        }

        Task<U> onFailureAsync(Error error) => Task.FromResult(onFailure(error));

        // Test 1:
        // Match<T, U>(this Result<T> result, Func<T, Task<U>> onSuccess)
        results.Add(await initial.Match(onSuccessAsync));

        // Test 2:
        // Match<T, U>(this Result<T> result,
        //     Func<T, Task<U>> onSuccess, Func< Error[], Task < U >> onFailure)
        results.Add(await initial.Match(onSuccessAsync, onFailureAsync));

        // Test 3:
        // Match<T, U>(this Task<Result<T>> asyncResult, Func<T, U> onSuccess)
        results.Add(await Task.FromResult(initial).Match(onSuccess));

        // Test 4:
        // Match<T, U>(this Task<Result<T>> asyncResult,
        //     Func<T, U> onSuccess, Func< Error[], U > onFailure)
        results.Add(await Task.FromResult(initial).Match(onSuccess, onFailure));

        // Test 5:
        // Match<T, U>(this Task<Result<T>> asyncResult, Func<T, Task<U>> onSuccess)
        results.Add(await Task.FromResult(initial).Match(onSuccessAsync));

        // Test 6:
        // Match<T, U>(this Task<Result<T>> asyncResult,
        //     Func<T, Task<U>> onSuccess, Func< Error[], Task < U >> onFailure)
        results.Add(await Task.FromResult(initial).Match(onSuccessAsync, onFailureAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                U? result = results[idx];

                if (initial.IsSuccess)
                {
                    T? onSuccessParam = onSuccessParams[idx];
                    onSuccessParam.Should().Be(initial.Value);
                    result.Should().Be(successValue);
                }
                else
                {
                    result.Should().BeOneOf(default(U), errorValue);
                }
            }

            if (initial.IsFailure)
            {
                foreach (Error onFailureParam in onFailureParams)
                {
                    onFailureParam.Should().Be(initial.Error);
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTRecoverTestCases),
        MemberType = typeof(TestData)
    )]
    public async Task Recover<T>(Result<T> initial, T fallbackValue)
    {
        // Arrange
        List<Result<T>> results = [];
        List<Error> fallbackParams = [];

        Result<T> fallback(Error error)
        {
            fallbackParams.Add(error);
            return Result.Ok(fallbackValue);
        }

        Task<Result<T>> fallbackAsync(Error error)
            => Task.FromResult(fallback(error));

        // Test 1:
        // Recover(this Result<T>, Func<Error[], Task<T>> fallback)
        results.Add(await initial.Recover(fallbackAsync));

        // Test 2:
        // Recover(this Task<Result<T>>, Func<Error[], T> fallback)
        results.Add(await Task.FromResult(initial).Recover(fallback));

        // Test 3:
        // Recover(this Task<Result<T>>, Func<Error[], Task<T>> fallback)
        results.Add(await Task.FromResult(initial).Recover(fallbackAsync));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<T> result = results[idx];
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
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task Tap<T>(Result<T> initial)
    {
        // Arrange
        List<T> actionParams = [];
        List<Result<T>> results = [];

        void action(T value)
        {
            actionParams.Add(value);
        }

        Task asyncAction(T value)
        {
            action(value);
            return Task.CompletedTask;
        }

        // Act

        // Test 1:
        // Tap(this Result<T>, Func<T, Task> action)
        results.Add(await initial.Tap(asyncAction));

        // Test 2:
        // Tap(this Task<Result<T>>, Action<T> action)
        results.Add(await Task.FromResult(initial).Tap(action));

        // Test 3:
        // Tap(this Task<Result<T>>, Func<T, Task> action)
        results.Add(await Task.FromResult(initial).Tap(asyncAction));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<T> result = results[idx];
                result.Should().Be(initial);
                if (initial.IsSuccess)
                {
                    T actionParam = actionParams[idx];
                    actionParam.Should().Be(initial.Value);
                }
                else
                {
                    actionParams.Should().BeEmpty();
                }
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task TapError<T>(Result<T> initial)
    {
        // Arrange
        List<Error> actionParams = [];
        List<Result<T>> results = [];

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
        // TapError(this Result<T>, Func<Error[], Task> action)
        results.Add(await initial.TapError(asyncAction));

        // Test 2:
        // TapError(this Task<Result<T>>, Action<Error[]> action)
        results.Add(await Task.FromResult(initial).TapError(action));

        // Test 3:
        // TapError(this Task<Result<T>>, Func<Error[], Task> action)
        results.Add(await Task.FromResult(initial).TapError(asyncAction));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<T> result = results[idx];
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

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task Ensure_WhenPredicatePasses_ReturnsOriginal<T>(Result<T> initial)
    {
        // Arrange
        const string errorCode = "ENSURE_FAILED";
        const string errorMessage = "Predicate failed";
        List<T> predicateParams = [];
        List<Result<T>> results = [];

        bool predicate(T value)
        {
            predicateParams.Add(value);
            return true;
        }
        Task<bool> predicateAsync(T value) => Task.FromResult(predicate(value));

        // Act

        // Test 1:
        // Ensure(this Result<T>, Func<T, Task<bool>>, ...)
        results.Add(await initial.Ensure(predicateAsync, errorCode, errorMessage));

        // Test 2:
        // Ensure(this Task<Result<T>>, Func<T, bool>, ...)
        results.Add(await Task.FromResult(initial).Ensure(predicate, errorCode, errorMessage));

        // Test 3:
        // Ensure(this Task<Result<T>>, Func<T, Task<bool>>, ...)
        results.Add(await Task.FromResult(initial).Ensure(predicateAsync, errorCode, errorMessage));

        // Assert
        Assert.Multiple(() =>
        {
            for (int idx = 0; idx < results.Count; idx++)
            {
                Result<T> result = results[idx];
                result.Should().Be(initial);
                if (initial.IsSuccess)
                {
                    predicateParams[idx].Should().Be(initial.Value);
                }
                else
                {
                    predicateParams.Should().BeEmpty();
                }
            }
        });
    }

    [Theory]
    [InlineData(42)]
    [InlineData("value")]
    [InlineData(true)]
    public async Task Ensure_WhenPredicateFails_ReturnsFailure<T>(T value)
    {
        // Arrange
        const string errorCode = "ENSURE_FAILED";
        const string errorMessage = "Predicate failed";
        Result<T> initial = Result.Ok(value);

        bool predicate(T _) => false;
        Task<bool> predicateAsync(T _) => Task.FromResult(false);

        // Act
        Result<T>[] results = [
            await initial.Ensure(predicateAsync, errorCode, errorMessage),
            await Task.FromResult(initial).Ensure(predicate, errorCode, errorMessage),
            await Task.FromResult(initial).Ensure(predicateAsync, errorCode, errorMessage)
        ];

        // Assert
        Assert.Multiple(() =>
        {
            foreach (Result<T> result in results)
            {
                TestUtility.AssertFailure(result);
                result.Error!.Code.Should().Be(errorCode);
                result.Error.Message.Should().Be(errorMessage);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public async Task Ensure_WithNullPredicate_ThrowsArgumentNullException<T>(Result<T> sut)
    {
        // Arrange
        Func<T, bool> predicate = null!;
        Func<T, Task<bool>> predicateAsync = null!;
        Func<Task>[] acts = [
            () => sut.Ensure(predicateAsync, "CODE"),
            () => Task.FromResult(sut).Ensure(predicate, "CODE"),
            () => Task.FromResult(sut).Ensure(predicateAsync, "CODE")
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
        nameof(TestData.ResultTInvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public async Task Ensure_WithInvalidErrorCode_ThrowsArgumentException<T>(
        Result<T> sut, string errorCode)
    {
        // Arrange
        static bool predicate(T _) => true;
        static Task<bool> predicateAsync(T _) => Task.FromResult(true);
        Func<Task>[] acts = [
            () => sut.Ensure(predicateAsync, errorCode),
            () => Task.FromResult(sut).Ensure(predicate, errorCode),
            () => Task.FromResult(sut).Ensure(predicateAsync, errorCode)
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
}
