using FluentAssertions;

namespace CSharper.Tests.TestUtilities;

internal static class AssertHelpers
{
    public static void AssertArgumentException<T>(Action act)
        where T : ArgumentException
    {
        act.Should().Throw<T>()
            .Which.Should().Match<T>(e =>
                !string.IsNullOrWhiteSpace(e.ParamName)
                && !string.IsNullOrWhiteSpace(e.Message));
    }
}