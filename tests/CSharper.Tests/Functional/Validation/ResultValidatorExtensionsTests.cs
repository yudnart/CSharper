using CSharper.Functional;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Results;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.Validation.ResultValidatorTestData;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultValidatorExtensions))]
public sealed class ResultValidatorExtensionsTests
{
    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_ValidParams_ReturnsSameValidator(Result initial)
    {
        // Arrange
        ResultValidator sut = new(initial);

        // Act
        ResultValidator result = sut.And(() => true, "Validation error");

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().NotBeNull();
            result.Validate().Should().BeSameAs(initial);
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
        ResultValidator sut = new(initial);

        Func<bool> predicate = null!;
        Func<Task<bool>> asyncPredicate = null!;

        Action act1 = () => _ = sut.And(predicate, "Validation error");
        Action act2 = () => _ = sut.And(asyncPredicate, "Validation error");

        // Act & Assert
        Assert.Multiple(() =>
        {
            act1.Should().Throw<ArgumentNullException>();
            act2.Should().Throw<ArgumentNullException>();
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.InvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public void And_InvalidMessage_ThrowsArgumentException(
        string message)
    {
        // Arrange
        Result initial = Result.Ok();
        ResultValidator sut = new(initial);
        Action act = () => _ = sut.And(() => true, message);

        // Act & Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            act.Should().Throw<ArgumentException>();
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void AndAsync_ValidParams_ReturnsAsyncValidator(Result initial)
    {
        // Arrange
        Task<ResultValidator> sut = Task.FromResult(new ResultValidator(initial));

        // Act
        Task<ResultValidator> result = sut.And(() => true, "Validation error");

        // Assert
        Assert.Multiple(async () =>
        {
            result.Should().NotBeNull();
            (await result).Validate().Should().BeSameAs(initial);
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void AndAsync_NullPredicate_ThrowsArgumentNullException(Result initial)
    {
        // Arrange
        Task<ResultValidator> sut = Task.FromResult(new ResultValidator(initial));

        Func<bool> predicate = null!;
        Func<Task<bool>> asyncPredicate = null!;

        Func<Task> act1 = () => sut.And(predicate, "Validation error");
        Func<Task> act2 = () => sut.And(asyncPredicate, "Validation error");

        // Act & Assert
        Assert.Multiple(async () =>
        {
            await act1.Should().ThrowAsync<ArgumentNullException>();
            await act2.Should().ThrowAsync<ArgumentNullException>();
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.InvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public void AndAsync_InvalidMessage_ThrowsArgumentException(
        string message)
    {
        // Arrange
        Result initial = Result.Ok();
        Task<ResultValidator> sut = Task.FromResult(new ResultValidator(initial));
        Func<Task> act = () => sut.And(() => true, message);

        // Act & Assert
        Assert.Multiple(async () =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            await act.Should().ThrowAsync<ArgumentException>();
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
        List<Result> results = [];

        Result next() => nextResult;
        Task<Result> nextAsync() => Task.FromResult(nextResult);
        ResultValidator sut = initial.Ensure(() => true, "Test error");

        // Act
        // Test 1:
        // Bind(this ResultValidator validator, Func<Task<Result>> next)
        results.Add(await sut.Bind(nextAsync));

        // Test 2:
        // Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
        results.Add(await Task.FromResult(sut).Bind(next));

        // Test 3:
        // Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
        results.Add(await Task.FromResult(sut).Bind(nextAsync));

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
        List<Result<T>> results = [];

        Result<T> next() => nextResult;
        Task<Result<T>> nextAsync() => Task.FromResult(nextResult);
        ResultValidator sut = initial.Ensure(() => true, "Test error");

        // Act
        // Test 1:
        // Bind(this ResultValidator validator, Func<Task<Result>> next)
        results.Add(await sut.Bind(nextAsync));

        // Test 2:
        // Bind(this Task<ResultValidator> asyncValidator, Func<Result> next)
        results.Add(await Task.FromResult(sut).Bind(next));

        // Test 3:
        // Bind(this Task<ResultValidator> asyncValidator, Func<Task<Result>> next)
        results.Add(await Task.FromResult(sut).Bind(nextAsync));

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
        ResultValidator sut = initial.Ensure(() => true, "Test error");

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
        ResultValidator sut = initial.Ensure(() => true, "Test error");

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
