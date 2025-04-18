﻿using CSharper.Mediator;
using CSharper.Results;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CSharper.Tests.Mediator;

public sealed class SimpleMediatorTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly List<string> _executionOrder;

    public SimpleMediatorTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _executionOrder = [];
    }

    [Fact]
    public void Ctor_NullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange
        IServiceProvider nullServiceProvider = null!;
        IEnumerable<IBehavior> behaviors = [];

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new SimpleMediator(nullServiceProvider, behaviors));
    }

    [Fact]
    public async Task Ctor_NullBehaviors_InitializesEmptyBehaviors()
    {
        // Arrange
        TestHandler handler = new(_executionOrder);
        SetupRequest(_serviceProviderMock, handler);
        SimpleMediator sut = new(_serviceProviderMock.Object, null!);
        TestRequest request = new();

        // Act
        Result result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _executionOrder.Should().Equal("H");
    }

    [Fact]
    public async Task Send_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        SimpleMediator sut = new(_serviceProviderMock.Object, new IBehavior[0]);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Send(null!, default));
    }

    [Fact]
    public async Task SendT_NullRequest_ThrowsArgumentNullException()
    {
        // Arrange
        SimpleMediator sut = new(_serviceProviderMock.Object, new IBehavior[0]);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Send<string>(null!, default));
    }

    [Fact]
    public async Task Send_NoBehaviorsNoHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>())).Returns(null!);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    [Fact]
    public async Task SendT_NoBehaviorsNoHandler_ThrowsInvalidOperationException()
    {
        // Arrange
        _serviceProviderMock.Setup(sp => sp.GetService(It.IsAny<Type>())).Returns(null!);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest<string> request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    [Fact]
    public async Task Send_NoBehaviorsSuccessfulHandler_ReturnsOk()
    {
        // Arrange
        TestHandler handler = new(_executionOrder);
        SetupRequest(_serviceProviderMock, handler);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest request = new();

        // Act
        Result result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _executionOrder.Should().Equal("H");
    }

    [Fact]
    public async Task SendT_NoBehaviorsSuccessfulHandler_ReturnsValue()
    {
        // Arrange
        TestHandlerTValue handler = new(_executionOrder);
        SetupRequest(_serviceProviderMock, handler);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest<string> request = new();

        // Act
        Result<string> result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Success");
        _executionOrder.Should().Equal("H");
    }

    [Fact]
    public async Task Send_WithBehaviorsSuccessfulPipeline_ReturnsOk()
    {
        // Arrange
        TestGlobalBehavior global1 = new("G1", _executionOrder);
        TestGlobalBehavior global2 = new("G2", _executionOrder);
        TestSpecificBehavior specific1 = new("S1", _executionOrder);
        TestSpecificBehavior specific2 = new("S2", _executionOrder);
        TestHandler handler = new(_executionOrder);

        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => global1);
        services.AddScoped<IBehavior>(_ => global2);
        services.AddScoped<IBehavior<TestRequest>>(_ => specific1);
        services.AddScoped<IBehavior<TestRequest>>(_ => specific2);
        services.AddScoped<IRequestHandler<TestRequest>, TestHandler>(_ => handler);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest request = new();

        // Act
        Result result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _executionOrder.Should().Equal("G1", "G2", "S1", "S2", "H");
    }

    [Fact]
    public async Task SendT_WithBehaviorsSuccessfulPipeline_ReturnsValue()
    {
        // Arrange
        TestGlobalBehavior global1 = new("G1", _executionOrder);
        TestGlobalBehavior global2 = new("G2", _executionOrder);
        TestSpecificBehaviorTValue specific1 = new("S1", _executionOrder);
        TestSpecificBehaviorTValue specific2 = new("S2", _executionOrder);
        TestHandlerTValue handler = new(_executionOrder);

        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => global1);
        services.AddScoped<IBehavior>(_ => global2);
        services.AddScoped<IBehavior<TestRequest<string>>>(_ => specific1);
        services.AddScoped<IBehavior<TestRequest<string>>>(_ => specific2);
        services.AddScoped<IRequestHandler<TestRequest<string>, string>, TestHandlerTValue>(_ => handler);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest<string> request = new();

        // Act
        Result<string> result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("Success");
        _executionOrder.Should().Equal("G1", "G2", "S1", "S2", "H");
    }

    [Fact]
    public async Task Send_BehaviorFails_ShortCircuits()
    {
        // Arrange
        FailingBehavior failingBehavior = new();
        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => failingBehavior);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest request = new();

        // Act
        Result result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Failed");
        _executionOrder.Should().BeEmpty();
    }

    [Fact]
    public async Task SendT_BehaviorFails_ShortCircuits()
    {
        // Arrange
        FailingBehavior failingBehavior = new();
        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => failingBehavior);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest<string> request = new();

        // Act
        Result<string> result = await sut.Send(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be("Failed");
        _executionOrder.Should().BeEmpty();
    }

    [Fact]
    public async Task Send_CancellationRequested_CancelsExecution()
    {
        // Arrange
        SlowHandler handler = new();
        SetupRequest(_serviceProviderMock, handler, []);

        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest request = new();
        CancellationTokenSource cts = new();

        // Act
        Task<Result> task = sut.Send(request, cts.Token);
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
    }

    [Fact]
    public async Task SendT_CancellationRequested_CancelsExecution()
    {
        // Arrange
        SlowHandlerTValue handler = new();
        SetupRequest(_serviceProviderMock, handler, []);

        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest<string> request = new();
        CancellationTokenSource cts = new();

        // Act
        Task<Result<string>> task = sut.Send(request, cts.Token);
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<TaskCanceledException>(() => task);
    }

    [Fact]
    public async Task Send_HandlerThrowsException_PropagatesException()
    {
        // Arrange
        ThrowingHandler handler = new();
        SetupRequest(_serviceProviderMock, handler);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    [Fact]
    public async Task SendT_HandlerThrowsException_PropagatesException()
    {
        // Arrange
        ThrowingHandlerT handler = new();
        SetupRequest(_serviceProviderMock, handler);
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest<string> request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    [Fact]
    public async Task Send_HandlerThrowsException_CascadesThroughBehaviors()
    {
        // Arrange
        TestGlobalBehavior globalBehavior = new("G1", _executionOrder);
        TestSpecificBehavior specificBehavior = new("S1", _executionOrder);
        ThrowingHandler handler = new();

        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => globalBehavior);
        services.AddScoped<IBehavior<TestRequest>>(_ => specificBehavior);
        services.AddScoped<IRequestHandler<TestRequest>, ThrowingHandler>(_ => handler);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
        _executionOrder.Should().Equal("G1", "S1");
    }

    [Fact]
    public async Task SendT_HandlerThrowsException_CascadesThroughBehaviors()
    {
        // Arrange
        TestGlobalBehavior globalBehavior = new("G1", _executionOrder);
        TestSpecificBehaviorTValue specificBehavior = new("S1", _executionOrder);
        ThrowingHandlerT handler = new();

        ServiceCollection services = new();
        services.AddScoped<IBehavior>(_ => globalBehavior);
        services.AddScoped<IBehavior<TestRequest<string>>>(_ => specificBehavior);
        services.AddScoped<IRequestHandler<TestRequest<string>, string>, ThrowingHandlerT>(_ => handler);
        IServiceProvider provider = services.BuildServiceProvider().CreateScope().ServiceProvider;

        SimpleMediator sut = new(provider, provider.GetServices<IBehavior>());
        TestRequest<string> request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
        _executionOrder.Should().Equal("G1", "S1");
    }

    [Fact]
    public async Task Send_HandlerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _serviceProviderMock.Setup(sp => sp
            .GetService(typeof(IEnumerable<IBehavior<TestRequest>>)))
            .Returns(Enumerable.Empty<IBehavior<TestRequest>>());
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    [Fact]
    public async Task SendT_HandlerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _serviceProviderMock.Setup(sp => sp
            .GetService(typeof(IEnumerable<IBehavior<TestRequest>>)))
            .Returns(Enumerable.Empty<IBehavior<TestRequest>>());
        SimpleMediator sut = new(_serviceProviderMock.Object, []);
        TestRequest<string> request = new();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => sut.Send(request));
    }

    #region Test Helpers

    private static void SetupRequest<TRequest>(
        Mock<IServiceProvider> serviceProvider,
        IRequestHandler<TRequest> handler,
        params IBehavior<TRequest>[] behaviors)
        where TRequest : IRequest
    {
        serviceProvider.Setup(sp => sp
            .GetService(typeof(IRequestHandler<TRequest>)))
            .Returns(handler);
        serviceProvider.Setup(sp => sp
            .GetService(typeof(IEnumerable<IBehavior<TRequest>>)))
            .Returns(behaviors);
    }

    private static void SetupRequest<TRequest, TValue>(
        Mock<IServiceProvider> serviceProvider,
        IRequestHandler<TRequest, TValue> handler,
        params IBehavior<TRequest>[] behaviors)
        where TRequest : IRequest<TValue>
    {
        serviceProvider.Setup(sp => sp
            .GetService(typeof(IRequestHandler<TRequest, TValue>)))
            .Returns(handler);
        serviceProvider.Setup(sp => sp
            .GetService(typeof(IEnumerable<IBehavior<TRequest>>)))
            .Returns(behaviors);
    }

    #endregion
}