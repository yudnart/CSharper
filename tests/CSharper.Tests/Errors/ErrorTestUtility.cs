using CSharper.Errors;
using FluentAssertions;

namespace CSharper.Tests.Errors;

public static class ErrorTestUtility
{
    public static void AssertError(Error error, string code, string? message)
    {
        Assert.Multiple(() => AssertErrorCommon(error, code, message));
    }

    private static void AssertErrorCommon(Error error, string code, string? message)
    {
        error.Should().NotBeNull();
        error.Code.Should().Be(code);
        error.Message.Should().Be(message);
    }
}
