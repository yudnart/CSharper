using FluentAssertions;

namespace CSharper.Tests.TestUtilities;

internal static class AssertHelper
{
    public static void AssertException<T>(Action act, Action<T>? additionalAssertion = null)
        where T : Exception
    {
        T exception = act.Should().Throw<T>().Which;
        exception.Message.Should().NotBeNullOrWhiteSpace();

        additionalAssertion?.Invoke(exception);
    }

    public static async Task AssertException<T>(Func<Task> act, Action<T>? additionalAssertion = null)
        where T : Exception
    {
        T exception = (await act.Should().ThrowAsync<T>()).Which;
        exception.Message.Should().NotBeNullOrWhiteSpace();

        additionalAssertion?.Invoke(exception);
    }

    public static void AssertException(Action act, Action<Exception>? additionalAssertion = null) 
        => AssertException<Exception>(act, additionalAssertion);

    public static Task AssertException(Func<Task> act, Action<Exception>? additionalAssertion = null)
        => AssertException<Exception>(act, additionalAssertion);

    public static void AssertArgumentException<T>(Action act)
        where T : ArgumentException
    {
        AssertException<T>(act, ex =>
        {
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        });
    }

    public static Task AssertArgumentException<T>(Func<Task> act, Action<T>? additionalAssertion = null)
        where T : ArgumentException
    {
        return AssertException<T>(act, ex =>
        {
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        });
    }

    public static void AssertArgumentException(Action act) 
        => AssertArgumentException<ArgumentNullException>(act);

    public static Task AssertArgumentException(Func<Task> act)
        => AssertArgumentException<ArgumentNullException>(act);
}
