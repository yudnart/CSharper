using CSharper.Errors;
using CSharper.Results;
using CSharper.Results.Abstractions;
using FluentAssertions;

namespace CSharper.Tests.Results;

public static class ResultTestUtility
{
    public static void AssertFailureResult(ResultBase result, Error? error = null)
    {
        Assert.Multiple(() =>
        {
            AssertFailureResultInternal(result, error);
        });
    }

    public static void AssertFailureResult<T>(Result<T> result, Error? error = null)
    {
        Assert.Multiple(() =>
        {
            AssertFailureResultInternal(result, error);

            Action getValue = () => _ = result.Value;
            getValue.Should().
                ThrowExactly<InvalidOperationException>()
                .And.Message.Should().NotBeNullOrWhiteSpace();
        });
    }

    public static void AssertFailureResult(
        ResultBase result, params string[] detailMessages)
    {
        Assert.Multiple(() =>
        {
            AssertFailureResultInternal(result, detailMessages);
        });
    }

    public static void AssertFailureResult<T>(
        Result<T> result, params string[] detailMessages)
    {
        Assert.Multiple(() =>
        {
            AssertFailureResultInternal(result, detailMessages);

            Action getValue = () => _ = result.Value;
            getValue.Should().
                ThrowExactly<InvalidOperationException>()
                .And.Message.Should().NotBeNullOrWhiteSpace();
        });
    }

    public static void AssertSuccessResult(ResultBase result)
    {
        Assert.Multiple(() =>
        {
            AssertSuccessResultInternal(result);
        });
    }

    public static void AssertSuccessResult<T>(Result<T> result, T value)
    {
        Assert.Multiple(() =>
        {
            AssertSuccessResultInternal(result);
            result.Value.Should().Be(value);
        });
    }

    #region Internal

    private static void AssertFailureResultCommon(ResultBase result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    private static void AssertFailureResultInternal(ResultBase result, Error? error)
    {
        AssertFailureResultCommon(result);

        if (error != null)
        {
            result.Error.Should().Be(error);
            result.Error.Message.Should().Be(error.Message);
            result.Error.Code.Should().Be(error.Code);
            result.Error.ErrorDetails.Should()
                .ContainInOrder(error.ErrorDetails);
        }
    }

    private static void AssertFailureResultInternal(
        ResultBase result, params string[] detailMessages)
    {
        AssertFailureResultCommon(result);

        if (detailMessages?.Length > 0)
        {
            result.Error!.ErrorDetails
                .Select(e => e.Message)
                .Should().ContainInOrder(detailMessages);
        }
    }

    private static void AssertSuccessResultInternal(ResultBase result)
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();

        Action getError = () => _ = result.Error;
        getError.Should()
            .ThrowExactly<InvalidOperationException>()
            .And.Message.Should().NotBeNullOrWhiteSpace();
    }

    #endregion
}
