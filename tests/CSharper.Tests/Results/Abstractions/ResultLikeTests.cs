using CSharper.Results;
using CSharper.Results.Abstractions;
using CSharper.Tests.Errors;
using FluentAssertions;
using TestUtility = CSharper.Tests.Results.ResultTestUtility;

namespace CSharper.Tests.Results.Abstractions;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ResultLike))]
public sealed class ResultLikeTests
{
    [Theory]
    [MemberData(nameof(ResultBaseData))]
    public void ResultLike_ValueMatchesInitialResult(ResultBase sut)
    {
        ResultLike result = sut;
        result.Value.Should().Be(sut);
    }

    [Theory]
    [MemberData(nameof(ResultBaseFactoryData))]
    public void ResultLike_ValueMatchesInitial(Func<ResultBase> sut)
    {
        ResultBase expected = sut();
        ResultLike result = sut;
        if (expected.IsSuccess)
        {
            TestUtility.AssertSuccessResult(result.Value);
        }
        else
        {
            TestUtility.AssertFailureResult(result.Value, expected.Error);
        }
    }

    [Fact]
    public void ImplicitOperator_FromResultBase_CreatesValidResultLike()
    {
        // Arrange
        ResultBase initial = Result.Ok();

        // Act
        ResultLike sut = initial;

        // Assert
        sut.Value.Should().Be(initial);
    }

    [Fact]
    public void ImplicitOperator_FromFactory_CreatesValidResultLike()
    {
        // Arrange
        Func<ResultBase> factory = Result.Ok;
        ResultBase expected = factory();

        // Act
        ResultLike sut = factory; // Implicit conversion

        // Assert
        sut.Value.Should().Be(expected);
    }

    [Fact]
    public void ImplicitOperator_NullResultBase_ThrowsArgumentNullException()
    {
        // Arrange
        ResultBase initial = null!;
        Action act = () =>
        {
            ResultLike test = initial;
        };

        // Act & Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ImplicitOperator_NullFactory_ThrowsArgumentNullException()
    {
        // Arrange
        Func<ResultBase> initial = null!;
        Action act = () =>
        {
            ResultLike test = initial;
        };

        // Act & Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    public static TheoryData<ResultBase> ResultBaseData()
    {
        Result test1 = Result.Ok();
        Result test2 = Result.Fail(ErrorTestData.Error);
        Result<int> test3 = Result.Ok(42);
        Result<string> test4 = Result.Fail<string>("Error");

        return new TheoryData<ResultBase>
        {
            { Result.Ok() },
            { Result.Fail(ErrorTestData.Error) },
            { Result.Ok(42) },
            { Result.Fail<string>("Error") },
        };
    }

    public static TheoryData<Func<ResultBase>> ResultBaseFactoryData()
    {
        Result test1 = Result.Ok();
        Result test2 = Result.Fail(ErrorTestData.Error);
        Result<int> test3 = Result.Ok(42);
        Result<string> test4 = Result.Fail<string>("Error");

        return new TheoryData<Func<ResultBase>>
        {
            { Result.Ok },
            { () => Result.Fail(ErrorTestData.Error) },
            { () => Result.Ok(42) },
            { () => Result.Fail<string>("Error") },
        };
    }
}
