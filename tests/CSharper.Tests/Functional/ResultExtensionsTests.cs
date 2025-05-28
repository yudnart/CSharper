using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.FunctionalResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Functional;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ResultExtensions))]
public sealed class ResultExtensionsTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTestData),
        MemberType = typeof(TestData)
    )]
    public void Bind(Result sut, Result nextResult)
    {
        // Act
        Result result = sut.Bind(() => nextResult);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(
                sut.IsSuccess ? nextResult : sut);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTestData),
        MemberType = typeof(TestData)
    )]
    public void Bind_Chain(Result sut, Result final)
    {
        // Act
        Result result = sut
            .Bind(NextValue)
            .Bind(_ => Result.Ok())
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(_ => Result.Ok(false))
            .Bind(_ => final);

        // Assert
        if (sut.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }
    public Result<int> NextValue()
    {
        return Result.Ok(42); // Debugger steps here
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException(
        Result sut)
    {
        // Arrange
        Func<Result> next = null!;
        Action act = () => sut.Bind(next);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTTestData),
        MemberType = typeof(TestData)
    )]
    public void BindT<T>(Result sut, Result<T> nextResult)
    {
        // Act
        Result<T> result = sut.Bind(() => nextResult);

        // Assert
        if (sut.IsSuccess)
        {
            result.Should().Be(nextResult);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultBindTTestData),
        MemberType = typeof(TestData)
    )]
    public void BindT_Chain<T>(Result sut, Result<T> final)
    {
        // Act
        Result<T> result = sut
            .Bind(() => Result.Ok(42))
            .Bind(_ => Result.Ok())
            .Bind(() => Result.Ok("Hello world!"))
            .Bind(_ => Result.Ok(false))
            .Bind(_ => final);

        // Assert
        if (sut.IsSuccess)
        {
            result.Should().Be(final);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException<T>(
        Result sut)
    {
        // Arrange
        Func<Result<int>> next = null!;
        Action act = () => sut.Bind(next);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void MapError(Result sut)
    {
        // Arrange
        Func<Result<string>> act = sut.MapError<string>;

        // Act & Assert
        Assert.Multiple(() =>
        {
            if (sut.IsSuccess)
            {
                act.Should()
                    .ThrowExactly<InvalidOperationException>()
                    .And.Message.Should().NotBeNullOrWhiteSpace();
            }
            else
            {
                Result<string> result = act();
                TestUtility.AssertFailureResult(result, sut.Error);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultMatchTestCases),
        MemberType = typeof(TestData)
    )]
    public void Match<T>(Result sut, T successValue, T errorValue)
    {
        // Arrange
        T onSuccess() => successValue;

        Error onFailureError = default!;
        T onFailure(Error error)
        {
            onFailureError = error;
            return errorValue;
        }

        // Act
        T? result1 = sut.Match(onSuccess);
        T result2 = sut.Match(onSuccess, onFailure);

        // Assert
        Assert.Multiple(() =>
        {
            if (sut.IsSuccess)
            {
                onFailureError.Should().Be(default(Error));

                result1.Should().Be(successValue);
                result2.Should().Be(successValue);
            }
            else
            {
                onFailureError.Should().Be(sut.Error);

                result1.Should().Be(default(T));
                result2.Should().Be(errorValue);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void Recover(Result sut)
    {
        // Arrange
        Error error = default!;
        Result result = sut.Recover(sutError =>
        {
            error = sutError;
        });

        // Assert
        Assert.Multiple(() =>
        {
            TestUtility.AssertSuccessResult(result);
            if (sut.IsSuccess)
            {
                result.Should().Be(sut);
                error.Should().Be(default(Error));
            }
            else
            {
                result.Should().NotBe(sut);
                error.Should().Be(sut.Error);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void Tap(Result sut)
    {
        // Arrange
        bool tapCalled = false;
        void action()
        {
            tapCalled = true;
        }

        // Act
        Result result = sut.Tap(action);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(sut);
            tapCalled.Should().Be(sut.IsSuccess ? true : false);
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultData),
        MemberType = typeof(TestData)
    )]
    public void TapError(Result sut)
    {
        // Arrange
        Error error = default!;

        // Act
        Result result = sut.TapError(sutError =>
        {
            error = sutError;
        });

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(sut);
            if (sut.IsSuccess)
            {
                error.Should().Be(default(Error));
            }
            else
            {
                error.Should().Be(sut.Error);
            }
        });
    }
}
