using CSharper.Mediator;
using CSharper.Results;

namespace CSharper.Tests.Mediator;

internal sealed class TestRequest : IRequest
{
    public string Id { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Description { get; set; } = default!;
}

internal sealed class TestRequest<T> : IRequest<T>
{
    public string Id { get; set; } = string.Empty;
    public int Value { get; set; }
}

internal sealed class UnserializableTestRequest : IRequest
{
    public string Id { get; set; } = string.Empty;
    public object CircularReference => this;

    public override string ToString() => $"TestRequestWithSerializationFailure{{Id={Id}}}";
}

internal sealed class TestGlobalBehavior : IBehavior
{
    private readonly string _id;
    private readonly List<string> _order;

    public TestGlobalBehavior(string id, List<string> order)
    {
        _id = id;
        _order = order;
    }

    public async Task<Result> Handle(IRequest request, BehaviorDelegate next, CancellationToken ct)
    {
        _order.Add(_id);
        return await next(request, ct);
    }
}

internal sealed class TestSpecificBehavior : IBehavior<TestRequest>
{
    private readonly string _id;
    private readonly List<string> _order;

    public TestSpecificBehavior(string id, List<string> order)
    {
        _id = id;
        _order = order;
    }

    public async Task<Result> Handle(TestRequest request, BehaviorDelegate next, CancellationToken ct)
    {
        _order.Add(_id);
        return await next(request, ct);
    }
}

internal sealed class TestSpecificBehaviorTValue : IBehavior<TestRequest<string>>
{
    private readonly string _id;
    private readonly List<string> _order;

    public TestSpecificBehaviorTValue(string id, List<string> order)
    {
        _id = id;
        _order = order;
    }

    public async Task<Result> Handle(TestRequest<string> request, BehaviorDelegate next, CancellationToken ct)
    {
        _order.Add(_id);
        return await next(request, ct);
    }
}

internal sealed class FailingBehavior : IBehavior
{
    public Task<Result> Handle(IRequest request, BehaviorDelegate next, CancellationToken ct)
    {
        return Task.FromResult(Result.Fail("Failed"));
    }
}

internal sealed class TestHandler : IRequestHandler<TestRequest>
{
    private readonly List<string> _order;

    public TestHandler(List<string> order)
    {
        _order = order;
    }

    public Task<Result> Handle(TestRequest request, CancellationToken ct)
    {
        _order.Add("H");
        return Task.FromResult(Result.Ok());
    }
}

internal sealed class TestHandlerTValue : IRequestHandler<TestRequest<string>, string>
{
    private readonly List<string> _order;

    public TestHandlerTValue(List<string> order)
    {
        _order = order;
    }

    public Task<Result<string>> Handle(TestRequest<string> request, CancellationToken ct)
    {
        _order.Add("H");
        return Task.FromResult(Result.Ok("Success"));
    }
}

internal sealed class SlowHandler : IRequestHandler<TestRequest>
{
    public async Task<Result> Handle(TestRequest request, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        return Result.Ok();
    }
}

internal sealed class SlowHandlerTValue : IRequestHandler<TestRequest<string>, string>
{
    public async Task<Result<string>> Handle(TestRequest<string> request, CancellationToken ct)
    {
        await Task.Delay(1000, ct);
        return Result.Ok("Success");
    }
}

internal sealed class ThrowingHandler : IRequestHandler<TestRequest>
{
    public Task<Result> Handle(TestRequest request, CancellationToken ct)
    {
        throw new InvalidOperationException("Handler failed");
    }
}

internal sealed class ThrowingHandlerT : IRequestHandler<TestRequest<string>, string>
{
    public Task<Result<string>> Handle(TestRequest<string> request, CancellationToken ct)
    {
        throw new InvalidOperationException("Handler failed");
    }
}
