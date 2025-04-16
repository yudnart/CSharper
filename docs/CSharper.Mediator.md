# CSharper.Mediator

CSharper’s Mediator enables decoupled request handling, supporting commands, queries, and extensible behaviors for modular business logic across contexts like controllers and services.

## Overview

The `CSharper.Mediator` package provides a Mediator implementation that centralizes request processing, reducing dependencies between components. It supports commands for executing actions, queries for retrieving data, and behaviors for shared logic like validation or logging, integrating with `CSharper.Results` for robust error handling. Designed for versatility, it fits seamlessly into web controllers, service layers, or other application structures, promoting clean and testable code.

## Installation

To use `CSharper.Mediator`, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Configure the Mediator in your dependency injection container:

```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddMediator();
```

Include the `CSharper.Mediator` namespace:

```csharp
using CSharper.Mediator;
```

## Usage Examples

### Handling a Command in a Controller

Dispatch a command in a controller with a validation behavior:

```csharp
using CSharper.Mediator;
using CSharper.Results;
using Microsoft.AspNetCore.Mvc;

// Request DTO
public record CreateUserInfo(string Name);

// Command
public record CreateUserCommand(CreateUserInfo CreateUserInfo) : IRequest;

// Validation behavior
public class ValidationBehavior<TRequest> : IBehavior<TRequest> where TRequest : IRequest
{
    public async Task<Result> Handle(TRequest request, BehaviorDelegate next, CancellationToken cancellationToken)
    {
        if (request is CreateUserCommand cmd && string.IsNullOrEmpty(cmd.CreateUserInfo.Name))
        {
            return Result.Fail("Name cannot be empty", code: "EMPTY_NAME", path: "CreateUserInfo.Name");
        }
        return await next(request, cancellationToken);
    }
}

// Handler
public class CreateUserHandler : IRequestHandler<CreateUserCommand>
{
    public Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Simulate database save
        return Task.FromResult(Result.Ok());
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserInfo createUserInfo, CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(createUserInfo);
        Result result = await _mediator.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Errors[0].ToString());
    }
}

// Client call
// POST /api/users { "Name": "" }
// Response: 400 Bad Request - "Path: CreateUserInfo.Name - Name cannot be empty (EMPTY_NAME)"
```

### Handling a Query in a Controller

Retrieve data with a query in a controller:

```csharp
using CSharper.Mediator;
using CSharper.Results;
using Microsoft.AspNetCore.Mvc;

// Query
public record GetUserQuery(int UserId) : IRequest<Result<User>>;

// Response DTO
public record User(int Id, string Name);

// Handler
public class GetUserHandler : IRequestHandler<GetUserQuery, Result<User>>
{
    public Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        if (request.UserId <= 0)
        {
            return Task.FromResult(Result.Fail<User>("Invalid user ID", code: "INVALID_ID", path: "UserId"));
        }
        var user = new User(request.UserId, "Alice");
        return Task.FromResult(Result.Ok(user));
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<User>> Get(int userId, CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(userId);
        Result<User> result = await _mediator.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors[0].ToString());
    }
}

// Client call
// GET /api/users/1
// Response: 200 OK - { "Id": 1, "Name": "Alice" }
```

## Features

- **Command Handling**: Process actions via `IRequest`, returning a `Result` for success or failure.
- **Query Handling**: Retrieve data with `IRequest<T>`, yielding a typed `Result<T>`.
- **Pipeline Behaviors**: Apply `IBehavior<TRequest>` for specific requests or `IBehavior` for all, supporting validation, logging, or other shared logic.
- **Request Dispatching**: Send requests through `IMediator.Send`, suitable for controllers, services, or other components.
- **Asynchronous Support**: Handle requests asynchronously with `Task<Result>` or `Task<Result<T>>`, using `CancellationToken` for cancellation.
- **Error Integration**: Use `CSharper.Results` to return `Result` or `Result<T>` with `Error` objects, including `Message`, `Code`, and `Path`.

## Best Practices

- Create focused commands (e.g., `CreateUserCommand`) for actions and queries (e.g., `GetUserQuery`) for data retrieval, each with a single purpose.
- Use behaviors to manage cross-cutting concerns like validation or logging, calling `next` to proceed unless halting the pipeline.
- Return `Result` or `Result<T>` with `Error.Code` (e.g., “INVALID_ID”) and `Error.Path` (e.g., “UserId”) for precise error details.
- Map `Result` to HTTP responses in controllers (e.g., `Ok()`, `BadRequest()`) for clear API interactions.
- Leverage `CSharper.Functional` methods (e.g., `Bind`, `Ensure`) in handlers to compose complex logic cleanly.
- Validate inputs in handlers or behaviors, using `Result.Fail` to avoid exceptions.
- Include `CancellationToken` in async requests to support cancellation.
- Test handlers and behaviors independently, mocking `IMediator` to isolate logic.
- Halt pipelines early in behaviors for invalid requests to optimize performance.
- Use `Error.ToString()` for readable error messages, including `Path` and `Code`.

## Related Docs

- [**CSharper.Results**](CSharper.Results.md)
- [**CSharper.Functional**](CSharper.Functional.md)