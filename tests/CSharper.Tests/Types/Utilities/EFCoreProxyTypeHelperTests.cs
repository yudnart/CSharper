using CSharper.Results;
using CSharper.Tests.Types.Utilities.Castle.Proxies;
using CSharper.Types.Utilities;
using FluentAssertions;

namespace CSharper.Tests.Types.Utilities;

[Collection(nameof(SequentialTests))]
[Trait("Category", "Unit")]
[Trait("TestFor", nameof(EFCoreProxyTypeHelper))]
public sealed class EFCoreProxyTypeHelperTests : IDisposable
{
    public EFCoreProxyTypeHelperTests()
    {
        EFCoreProxyTypeHelper.Configure();
    }

    public void Dispose()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }
 
    [Fact]
    public void GetUnproxiedTypeDelegate_ProxyTypeWithBase_ReturnsBaseType()
    {
        // Arrange
        TestCastleProxyWithBase proxyObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(proxyObject);

        // Assert
        result.Should().Be(typeof(TestCastleProxyBase));
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_ProxyTypeNoBase_ReturnsOriginalType()
    {
        // Arrange
        TestCastleProxyNoBase proxyObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(proxyObject);

        // Assert
        result.Should().Be(typeof(TestCastleProxyNoBase));
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_NonProxy_ReturnsOriginalType()
    {
        // Arrange
        Result nonProxyObject = Result.Ok();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(nonProxyObject);

        // Assert
        result.Should().Be(typeof(Result));
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_Object_ReturnsObject()
    {
        // Arrange
        object objectInstance = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(objectInstance);

        // Assert
        result.Should().Be(typeof(object));
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        object? nullObject = null;

        // Act
        Action act = () => ProxyTypeHelper.GetUnproxiedType(nullObject!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }
}