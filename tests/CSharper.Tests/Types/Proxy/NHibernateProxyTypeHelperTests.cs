using CSharper.Results;
using CSharper.Types.Proxy;
using FluentAssertions;

namespace CSharper.Tests.Types.Proxy;

[Collection(nameof(SequentialTests))]
[Trait("Category", "Unit")]
[Trait("TestFor", nameof(NHibernateProxyTypeHelper))]
public sealed class NHibernateProxyTypeHelperTests : IDisposable
{
    public NHibernateProxyTypeHelperTests()
    {
        NHibernateProxyTypeHelper.Configure();
    }

    public void Dispose()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_ProxyTypeWithBase_ReturnsBaseType()
    {
        // Arrange
        TestWithBaseProxy proxyObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(proxyObject);

        // Assert
        result.Should().Be(typeof(TestProxyBase));
    }

    [Fact]
    public void GetUnproxiedTypeDelegate_ProxyTypeNoBase_ReturnsOriginalType()
    {
        // Arrange
        TestNoBaseProxy proxyObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(proxyObject);

        // Assert
        result.Should().Be(typeof(TestNoBaseProxy));
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

    // Helper class for non-proxy type
    private class TestProxyBase { }

    // Simulate an NHibernate proxy type with a valid base type
    private class TestWithBaseProxy : TestProxyBase { }

    // Simulate a proxy type with object as base type
    public class TestNoBaseProxy { }
}
