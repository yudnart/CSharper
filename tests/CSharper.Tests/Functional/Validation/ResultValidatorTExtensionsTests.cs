using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Validation;
using CSharper.Tests.Errors;
using CSharper.Tests.Results;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.Validation.ResultValidatorTestData;

namespace CSharper.Tests.Functional.Validation;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(ResultValidatorTExtensions))]
public sealed class ResultValidatorTExtensionsTests
{
    private static readonly string _errorMessage = ErrorTestData.Error.Message;

    private static bool _predicate() => true;
    private static Task<bool> _asyncPredicate() => Task.FromResult(_predicate());

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_ValidParams_ReturnsSameValidator<T>(Result<T> initial)
    {
        // Arrange
        ResultValidator<T> sut = new(initial);

        // Act
        ResultValidator<T> result = sut
            .And(_ => true, "Validation error");

        // Assert
        result.Validate().Should().BeSameAs(initial);
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void And_NullPredicate_ThrowsArgumentException<T>(Result<T> initial)
    {
        // Arrange
        ResultValidator<T> sut = new(initial);

        Func<T, bool> predicate = null!;
        Func<T, Task<bool>> asyncPredicate = null!;

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
        Result<int> initial = Result.Ok(42);
        ResultValidator<int> sut = new(initial);
        Action act = () => _ = sut.And(_ => true, message);

        // Act & Assert
        Assert.Multiple(() =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            act.Should().Throw<ArgumentException>();
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void AndAsync_ValidParams_ReturnsAsyncValidator<T>(Result<T> initial)
    {
        // Arrange
        Task<ResultValidator<T>> sut = Task
            .FromResult(new ResultValidator<T>(initial));

        // Act
        Task<ResultValidator<T>> result = sut.And(_ => true, "Validation error");

        // Assert
        Assert.Multiple(async () =>
        {
            result.Should().NotBeNull();
            (await result).Validate().Should().BeSameAs(initial);
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public async Task AndAsync_NullPredicate_ThrowsArgumentNullException<T>(Result<T> initial)
    {
        // Arrange
        Func<T, bool> nullPredicate = null!;
        Func<T, Task<bool>> nullAsyncPredicate = null!;

        Task<ResultValidator<T>> sut = Task.FromResult(new ResultValidator<T>(initial));

        Func<Task>[] acts = [
            async () => _ = await sut.And(nullPredicate, _errorMessage),
            async () => _ = await sut.And(nullAsyncPredicate, _errorMessage)
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
        nameof(TestData.InvalidErrorMessages),
        MemberType = typeof(TestData)
    )]
    public void AndAsync_InvalidMessage_ThrowsArgumentException(
        string message)
    {
        // Arrange
        Result<string> initial = Result.Ok("Value");
        Task<ResultValidator<string>> sut = Task
            .FromResult(new ResultValidator<string>(initial));
        Func<Task> act = () => sut.And(_ => true, message);

        // Act & Assert
        Assert.Multiple(async () =>
        {
            ResultTestUtility.AssertSuccessResult(initial);
            await act.Should().ThrowAsync<ArgumentException>();
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure<T>(Result<T> sut)
    {
        // Arrange
        static bool predicate(T _) => true;
        Error error = ErrorTestData.Error;

        // Act
        ResultValidator<T> result = sut
            .Ensure(predicate, error.Message, error.Code);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<ResultValidator<T>>();
        });
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTData),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_WithNullPredicate_ThrowsArgumentNullException<T>(
        Result<T> sut)
    {
        // Arrange
        Func<T, bool> predicate = null!;
        Error error = ErrorTestData.Error;

        Action act = () => sut
            .Ensure(predicate, error.Message, error.Code);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(FunctionalResultTestData.ResultTEnsureInvalidTestCases),
        MemberType = typeof(FunctionalResultTestData)
    )]
    public void Ensure_InvalidParams_ThrowsArgumentNullException<T>(
        Result<T> sut, string message, string? code)
    {
        // Arrange
        static bool predicate(T x) => true;
        Action act = () => sut.Ensure(predicate, message, code);

        // Act & Assert
        act.Should().Throw<ArgumentException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }
}
