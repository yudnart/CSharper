// Namespace to simulate EF Core proxy types
namespace CSharper.Tests.Types.Utilities.Castle.Proxies;

internal abstract class TestCastleProxyBase()
{
    // Intentionally blank
}

// Simulate an EF Core proxy type with Castle.Proxies namespace
internal class TestCastleProxyWithBase : TestCastleProxyBase { }

internal class TestCastleProxyNoBase { }