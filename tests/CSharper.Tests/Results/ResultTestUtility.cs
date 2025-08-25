using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Abstractions;
using FluentAssertions;

namespace CSharper.Tests.Results;

public static class ResultTestUtility
{
    public static void AssertSuccess(ResultBase result)
    {
        Assert.Multiple(() => AssertSuccessCommon(result));
    }

    public static void AssertSuccess<T>(Result<T> result, T value)
    {
        Assert.Multiple(() =>
        {
            AssertSuccessCommon(result);
            result.Value.Should().Be(value);
        });
    }

    public static void AssertFailure(ResultBase result, Error? error = null)
    {
        Assert.Multiple(() =>
        {
            AssertFailureCommon(result, error);
        });
    }

    public static void AssertFailure<T>(Result<T> result, Error? error = null)
    {
        Assert.Multiple(() =>
        {
            AssertFailureCommon(result, error);

            Action getValue = () => _ = result.Value;
            getValue.Should().
                ThrowExactly<InvalidOperationException>()
                .And.Message.Should().NotBeNullOrWhiteSpace();
        });
    }

    #region Internal

    private static void AssertSuccessCommon(ResultBase result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();

        Action getError = () => _ = result.Error;
        getError.Should()
            .ThrowExactly<InvalidOperationException>()
            .And.Message.Should().NotBeNullOrWhiteSpace();
    }

    private static void AssertFailureCommon(ResultBase result, Error? error)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();


        if (error != null)
        {
            result.Error.Should().Be(error);
        }
    }

    #endregion
}
