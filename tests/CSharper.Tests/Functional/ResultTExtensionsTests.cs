using CSharper.Errors;
using CSharper.Functional;
using CSharper.Results;
using FluentAssertions;
using TestData = CSharper.Tests.Functional.FunctionalResultTestData;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Functional;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ResultTExtensions))]
public sealed class ResultTExtensionsTests
{
    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTestData),
        MemberType = typeof(TestData)
    )]
    public void Bind<T>(Result<T> sut, Result nextResult)
    {
        // Act
        T? bindValue = default!;
        Result result = sut.Bind(sutValue =>
        {
            bindValue = sutValue;
            return nextResult;
        });

        // Assert
        if (sut.IsSuccess)
        {
            result.Should().Be(nextResult);
            bindValue.Should().Be(sut.Value);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTestData),
        MemberType = typeof(TestData)
    )]
    public void Bind_Chain<T>(Result<T> sut, Result final)
    {
        // Act
        Result result = sut
            .Bind(_ => Result.Ok(42))
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
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void Bind_WithNullNext_ThrowsArgumentNullException<T>(
        Result<T> sut)
    {
        // Arrange
        Func<T, Result> next = null!;
        Action act = () => sut.Bind(next);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be(nameof(next));
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTTestData),
        MemberType = typeof(TestData)
    )]
    public void BindT<T, U>(Result<T> sut, Result<U> nextResult)
    {
        // Act
        T? bindParam = default!;
        Result<U> result = sut.Bind(_value =>
        {
            bindParam = _value;
            return nextResult;
        });

        // Assert
        if (sut.IsSuccess)
        {
            result.Should().Be(nextResult);
            bindParam.Should().Be(sut.Value);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTBindTTestData),
        MemberType = typeof(TestData)
    )]
    public void BindT_Chain<T, U>(Result<T> sut, Result<U> final)
    {
        // Act
        Result<U> result = sut
            .Bind(_ => Result.Ok(42))
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
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void BindT_WithNullNext_ThrowsArgumentNullException<T>(
        Result<T> sut)
    {
        // Arrange
        Func<T, Result<string>> next = null!;
        Action act = () => sut.Bind(next);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be(nameof(next));
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTMapTestCases),
        MemberType = typeof(TestData)
    )]
    public void Map<T, U>(Result<T> sut, U value)
    {
        // Arrange
        T? mapParam = default!;
        U mapDelegate(T _value)
        {
            mapParam = _value;
            return value;
        }

        // Act
        Result<U> result = sut.Map(mapDelegate);

        // Assert
        if (sut.IsSuccess)
        {
            TestUtility.AssertSuccessResult(result, value);
            mapParam.Should().Be(sut.Value);
        }
        else
        {
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void Map_WithNullMap_ThrowsArgumentNullException<T>(
        Result<T> sut)
    {
        // Arrange
        Func<T, string> map = null!;
        Func<Result<string>> act = () => sut.Map(map);

        // Act & Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void MapError<T>(Result<T> sut)
    {
        // Arrange
        Func<Result> act = () => sut.MapError();

        // Act & Assert
        if (sut.IsSuccess)
        {
            act.Should().ThrowExactly<InvalidOperationException>()
                .And.Message.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            Result result = act();
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void MapErrorT<T>(Result<T> sut)
    {
        // Arrange
        Func<Result<int>> act = sut.MapError<T, int>;

        // Act & Assert
        if (sut.IsSuccess)
        {
            act.Should().ThrowExactly<InvalidOperationException>()
            .And.Message.Should().NotBeNullOrWhiteSpace();
        }
        else
        {
            Result<int> result = act();
            TestUtility.AssertFailureResult(result, sut.Error);
        }
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTMatchTestCases),
        MemberType = typeof(TestData)
    )]
    public void Match<T, U>(Result<T> sut, U successValue, U errorValue)
    {
        // Arrange
        T onSuccessParam = default!;
        U onSuccess(T value)
        {
            onSuccessParam = value;
            return successValue;
        }

        Error onFailureParam = default!;
        U onFailure(Error error)
        {
            onFailureParam = error;
            return errorValue;
        }

        // Act
        U? result1 = sut.Match(onSuccess);
        U result2 = sut.Match(onSuccess, onFailure);

        // Assert
        Assert.Multiple(() =>
        {
            if (sut.IsSuccess)
            {
                onSuccessParam.Should().Be(sut.Value);

                onFailureParam.Should().Be(default);

                result1.Should().Be(successValue);
                result2.Should().Be(successValue);
            }
            else
            {
                onSuccessParam.Should().Be(default(T));

                onFailureParam.Should().Be(sut.Error);

                result1.Should().Be(default(U));
                result2.Should().Be(errorValue);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTRecoverTestCases),
        MemberType = typeof(TestData)
    )]
    public void Recover<T>(Result<T> sut, T fallbackValue)
    {
        // Arrange
        Error error = default!;

        // Act
        Result<T> result = sut.Recover(sutError =>
        {
            error = sutError;
            return Result.Ok(fallbackValue);
        });

        // Assert
        Assert.Multiple(() =>
        {
            T value = sut.IsSuccess
                ? sut.Value : result.Value;
            TestUtility.AssertSuccessResult(result, value);
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
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void Tap<T>(Result<T> sut)
    {
        // Arrange
        bool tapCalled = false;
        T actionParam = default!;
        void action(T value)
        {
            tapCalled = true;
            actionParam = value;
        }

        // Act
        Result<T> result = sut.Tap(action);

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(sut);
            tapCalled.Should().Be(sut.IsSuccess);
            if (sut.IsSuccess)
            {
                actionParam.Should().Be(sut.Value);
            }
        });
    }

    [Theory]
    [MemberData(
        nameof(TestData.ResultTData),
        MemberType = typeof(TestData)
    )]
    public void TapError<T>(Result<T> sut)
    {
        // Arrange
        Error error = default!;

        // Act
        Result<T> result = sut.TapError(sutError =>
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
