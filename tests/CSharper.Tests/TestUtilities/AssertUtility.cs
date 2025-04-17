using FluentAssertions;

namespace CSharper.Tests.TestUtilities;

internal static class AssertUtility
{
    public static void AssertException<T>(Action act, Action<T>? additionalAssertion = null)
        where T : Exception
    {
        T exception = act.Should().Throw<T>().Which;
        exception.Message.Should().NotBeNullOrWhiteSpace();

        if (additionalAssertion != null)
        {
            additionalAssertion(exception)  ;
        }
    }

    public static void AssertException(Action act, Action<Exception>? additionalAssertion = null) 
        => AssertException<Exception>(act, additionalAssertion);

    public static void AssertArgumentException<T>(Action act)
        where T : ArgumentException
    {
        AssertException<T>(act, ex =>
        {
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        });
    }

    public static void AssertArgumentException(Action act) 
        => AssertArgumentException<ArgumentNullException>(act);
}