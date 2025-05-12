using CSharper.Mediator;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace CSharper.Tests.Mediator;

[Trait("Category", "Unit")]
[Trait("TestFor", nameof(MediatorModule))]
public sealed class MediatorModuleTests
{
    [Fact]
    public void AddSimpleMediator_RegistersSimpleMediator()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddSimpleMediator();

        // Assert
        services.Should().ContainSingle(descriptor =>
            descriptor.ServiceType == typeof(IMediator) &&
            descriptor.ImplementationType == typeof(SimpleMediator));
    }

    [Fact]
    public void AddSimpleMediator_NullServices_ThrowsArgumentNullException()
    {
        // Arrange
        ServiceCollection services = null!;
        void act() => services.AddSimpleMediator();

        // Act & Assert
        Assert.Multiple(() =>
        {
            ArgumentNullException ex = Assert
                .Throws<ArgumentNullException>(act);
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        });
    }

    [Fact] 
    public void AddLoggingBehavior_RegistersLoggingBehavior()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        services.AddLoggingBehavior();

        // Assert
        services.Should().ContainSingle(descriptor =>
            descriptor.ServiceType == typeof(IBehavior) &&
            descriptor.ImplementationType == typeof(LoggingBehavior));
    }

    [Fact]
    public void AddLoggingBehavior_NullServices_ThrowsArgumentNullException()
    {
        // Arrange
        ServiceCollection services = null!;
        void act() => services.AddLoggingBehavior();

        // Act & Assert
        Assert.Multiple(() =>
        {
            ArgumentNullException ex = Assert
                .Throws<ArgumentNullException>(act);
            ex.ParamName.Should().NotBeNullOrWhiteSpace();
        });
    }
}
