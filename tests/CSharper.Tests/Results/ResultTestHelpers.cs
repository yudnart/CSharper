using CSharper.Results;
using FluentAssertions;

namespace CSharper.Tests.Results;

internal static class ResultTestHelpers
{
    public static void AssertResult(ResultBase result, bool isSuccess = true)
    {
        if (isSuccess)
        {
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
            result.Errors.Should().BeEmpty();
        }
        else
        {
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
            result.Errors.Should().NotBeEmpty();
        }
    }

    public static void AssertResult<T>(Result<T> result, T value)
    {
        AssertResult(result);
        result.Value.Should().Be(value);
    }
}