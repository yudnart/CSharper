using CSharper.Errors;
using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

internal static class ResultTestHelpers
{
    public static void AssertSuccessResult(ResultBase result)
    {
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Errors.Should().BeEmpty();
    }

    public static void AssertSuccessResult<T>(Result<T> result, T value)
    {
        AssertSuccessResult(result);
        result.Value.Should().Be(value);
    }

    public static void AssertFailureResult(Result result, params Error[] errors)
        => AssertFailureResultBase(result, errors);

    public static void AssertFailureResult<T>(Result<T> result, params Error[] errors)
    {
        AssertFailureResultBase(result, errors);
        Action act = () => _ = result.Value;
        act.Should().Throw<InvalidOperationException>()
            .And.Message.Should().NotBeNullOrWhiteSpace();
    }

    private static void AssertFailureResultBase(ResultBase result, params Error[] errors)
    {
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();

        if (errors?.Length > 0)
        {
            result.Errors.Should().HaveCount(errors.Length);
            result.Errors.Should().ContainInOrder(errors);
        }
    }
}