using CSharper.Errors;
using FluentAssertions;

namespace CSharper.Tests.Errors;

public static class ErrorTestUtility
{
    public static void AssertError(Error error, string message, string? code)
    {
        Assert.Multiple(() =>
        {
            AssertErrorInternal(error, message, code);
        });
    }

    private static void AssertErrorInternal(Error error, string message, string? code)
    {
        error.Should().NotBeNull();
        error.Message.Should().Be(message);
        error.Code.Should().Be(code);
    }
}
