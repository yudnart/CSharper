using CSharper.Mediator;
using Moq;

namespace CSharper.Tests.Mediator;

internal static class SimpleMediatorTestHelper
{
    public static void SetupRequest<TRequest>(
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

    public static void SetupRequest<TRequest, TValue>(
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
}
