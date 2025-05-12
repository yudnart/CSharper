using CSharper.Types.Proxy;
using FluentAssertions;

namespace CSharper.Tests.Types.Proxy;

[Collection(nameof(SequentialTests))]
[Trait("Category", "Unit")]
[Trait("TestFor", nameof(ProxyTypeHelper))]
public sealed class ProxyTypeHelperTests : IDisposable
{
    public ProxyTypeHelperTests()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    public void Dispose()
    {
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
    }

    [Fact]
    public void ConfigureGetUnproxiedTypeDelegate_SetsCustomDelegate_UsesCustomDelegate()
    {
        // Arrange
        Func<object, Type> customDelegate = obj => typeof(string);
        object testObject = new object();

        // Act
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(customDelegate);
        Type result = ProxyTypeHelper.GetUnproxiedType(testObject);

        // Assert
        result.Should().Be(typeof(string));
    }

    [Fact]
    public void ConfigureGetUnproxiedTypeDelegate_NullDelegate_ThrowsArgumentNullException()
    {
        // Arrange
        Func<object, Type>? nullDelegate = null;

        // Act
        Action act = () => ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(nullDelegate!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ResetGetUnproxiedTypeDelegate_AfterCustomDelegate_RevertsToDefaultDelegate()
    {
        // Arrange
        object testObject = new();

        static Type customDelegate(object obj) => typeof(string);
        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(customDelegate);

        // Act
        ProxyTypeHelper.ResetGetUnproxiedTypeDelegate();
        Type result = ProxyTypeHelper.GetUnproxiedType(testObject);

        // Assert
        result.Should().Be(typeof(object));
    }

    [Fact]
    public void GetUnproxiedType_DefaultDelegate_ReturnsRuntimeType()
    {
        // Arrange
        DerivedClass testObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(testObject);

        // Assert
        result.Should().Be(typeof(DerivedClass));
    }

    [Fact]
    public void GetUnproxiedType_CustomDelegate_ReturnsBaseType()
    {
        // Arrange
        DerivedClass testObject = new();

        static Type customDelegate(object obj) => obj.GetType().BaseType ?? obj.GetType();

        ProxyTypeHelper.ConfigureGetUnproxiedTypeDelegate(customDelegate);

        Type expected = customDelegate(testObject);

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(testObject);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetUnproxiedType_SystemObject_ReturnsObjectType()
    {
        // Arrange
        object testObject = new();

        // Act
        Type result = ProxyTypeHelper.GetUnproxiedType(testObject);

        // Assert
        result.Should().Be(typeof(object));
    }

    [Fact]
    public void GetUnproxiedType_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        object? nullObject = null;

        // Act
        Action act = () => ProxyTypeHelper.GetUnproxiedType(nullObject!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("obj");
    }

    // Helper classes for testing
    private class BaseClass { }
    private class DerivedClass : BaseClass { }
}
