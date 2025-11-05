# CSharper.Types.Entity

CSharper�s `Entity` types provide a robust foundation for modeling domain entities in a Domain-Driven Design (DDD) 
context, enabling objects with unique identity and lifecycle management.

## Overview

The `Entity<TId>` and `Entity` classes represent domain entities, which are objects defined by their unique identifier 
and lifecycle. `Entity<TId>` is a generic base class supporting any identifier type, while `Entity` specializes in 
string identifiers. These classes support domain events for capturing state changes and equality based on identity.
## Installation

To use `CSharper.Types.Entity`, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Then, include the `CSharper.Types` namespace:

```csharp
using CSharper.Types;
```

## Usage Examples

### Basic Usage with `Entity<TId>`

Create an entity with a numeric ID and queue a domain event:

```csharp
public class OrderEntity : Entity<int>
{
    public OrderEntity(int id)
    {
        Id = id;
    }

    public void Complete()
    {
        QueueEvent(new OrderCompletedEvent(Id));
    }
}

public class OrderCompletedEvent : DomainEvent
{
    public int OrderId { get; }

    public OrderCompletedEvent(int orderId)
    {
        OrderId = orderId;
    }
}

// Usage
var order = new OrderEntity(123);
order.Complete();
var events = order.FlushEvents();
foreach (var evt in events)
{
    Console.WriteLine($"Event at {evt.Timestamp}: {evt.GetType().Name}"); // Output: Event at [UTC time]: OrderCompletedEvent
}
```

### Using `Entity` with String ID

Create an entity with a string ID and check its persistence state:

```csharp
public class UserEntity : Entity
{
    public UserEntity(string id)
    {
        Id = id;
    }
}

// Usage
var user = new UserEntity("user123");
Console.WriteLine(user.IsTransient() ? "Transient" : "Persisted"); // Output: Persisted
var transientUser = new UserEntity("");
Console.WriteLine(transientUser.IsTransient() ? "Transient" : "Persisted"); // Output: Transient
```

### Integration with `CSharper.Results`

Combine entity operations with result handling:

```csharp
public class UserEntity : Entity
{
    public string Name { get; private set; }

    public UserEntity(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public Result UpdateName(string newName)
    {
        if (string.IsNullOrEmpty(newName))
            return Result.Fail("Name cannot be empty", path: "name");
        Name = newName;
        QueueEvent(new UserNameUpdatedEvent(Id, newName));
        return Result.Ok();
    }
}

// Usage
var user = new UserEntity("user123", "Alice");
var result = user.UpdateName("");
if (result.IsSuccess)
    Console.WriteLine("Name updated");
else
    Console.WriteLine($"Error: {result.Errors[0]}"); // Output: Error: Path: name - Name cannot be empty
```

## Features

- `Entity<TId>`: A generic base class for entities with a typed identifier (`TId`).
- `Entity`: A specialized class for entities with string identifiers, inheriting from `Entity<string>`.
- `Id`: A protected property representing the entity�s unique identifier.
- `IsTransient()`: Checks if the entity is not persisted (e.g., `Id` is null, default, or an empty/whitespace string for string IDs).
- `QueueEvent(DomainEvent)`: Queues a domain event for later dispatching, supporting DDD event-driven patterns.
- `FlushEvents()`: Returns and clears all queued domain events as an enumerable, enabling integration with event handlers.
- **Equality**:
  - Equality is based on the entity�s type and `Id`, ignoring other properties.
  - `operator ==` and `operator !=`: Compare entities for equality.
  - `Equals(object?)`: Implements value-based equality for entities.
  - `GetHashCode()`: Generates a hash code based on the entity�s type and `Id`.
- **DomainEvent** **Type** (from `CSharper.Events`):
  - Inherited from `IEvent`, with a `Timestamp` property set to `DateTimeOffset.UtcNow`.
  - Ensures domain events are scoped to the domain and have consistent metadata.

## Integrating Domain Events with Mediator

CSharper's domain events can be seamlessly integrated with the Mediator pattern for event-driven architectures. Here's a complete example:

### 1. Define Your Domain Events

```csharp
using CSharper.Types;
using CSharper.Mediator;
using CSharper.Results;

// Domain event implementing IRequest for mediator integration
public class OrderCompletedEvent : DomainEvent, IRequest
{
    public int OrderId { get; }
    public decimal TotalAmount { get; }

    public OrderCompletedEvent(int orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}

// Event handler
public class OrderCompletedEventHandler : IRequestHandler<OrderCompletedEvent>
{
    public async Task<Result> Handle(OrderCompletedEvent request, CancellationToken cancellationToken)
    {
        // Send email notification
        Console.WriteLine($"Order {request.OrderId} completed with total ${request.TotalAmount}");
        
        // Additional async operations like sending emails, updating analytics, etc.
        await Task.CompletedTask;
        
        return Result.Ok();
    }
}
```

### 2. Create Entity with Events

```csharp
public class OrderEntity : Entity<int>
{
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }

    public OrderEntity(int id)
    {
        Id = id;
        Status = OrderStatus.Pending;
    }

    public Result Complete()
    {
        if (Status == OrderStatus.Completed)
            return Result.Fail("ALREADY_COMPLETED", "Order is already completed");

        Status = OrderStatus.Completed;
        QueueEvent(new OrderCompletedEvent(Id, TotalAmount));
        return Result.Ok();
    }
}

public enum OrderStatus { Pending, Completed, Cancelled }
```

### 3. Dispatch Events After Persistence

```csharp
using CSharper.Mediator;

public class OrderService
{
    private readonly IMediator _mediator;
    private readonly IOrderRepository _repository;

    public OrderService(IMediator mediator, IOrderRepository repository)
    {
        _mediator = mediator;
        _repository = repository;
    }

    public async Task<Result> CompleteOrder(int orderId, CancellationToken cancellationToken)
    {
        // 1. Load entity
        OrderEntity? order = await _repository.GetById(orderId);
        if (order == null)
            return Result.Fail("NOT_FOUND", "Order not found");

        // 2. Execute business logic
        Result result = order.Complete();
        if (result.IsFailure)
            return result;

        // 3. Persist changes
        await _repository.Save(order);

        // 4. Dispatch domain events through mediator
        foreach (DomainEvent evt in order.FlushEvents())
        {
            if (evt is IRequest request)
            {
                await _mediator.Send(request, cancellationToken);
            }
        }

        return Result.Ok();
    }
}
```

### 4. Unit of Work Pattern (Recommended)

For cleaner architecture, implement a Unit of Work that automatically dispatches events:

```csharp
public interface IUnitOfWork
{
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IMediator _mediator;

    public UnitOfWork(DbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Collect all domain events before saving
        var entities = _context.ChangeTracker.Entries<Entity<object>>()
            .Where(e => e.Entity != null)
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.FlushEvents()).ToList();

        // 2. Save changes to database
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Fail("SAVE_FAILED", ex.Message);
        }

        // 3. Dispatch events after successful persistence
        foreach (DomainEvent evt in events)
        {
            if (evt is IRequest request)
            {
                Result result = await _mediator.Send(request, cancellationToken);
                if (result.IsFailure)
                {
                    // Log error but don't fail the entire operation
                    Console.WriteLine($"Event dispatch failed: {result.Error}");
                }
            }
        }

        return Result.Ok();
    }
}
```

## Best Practices

- Use `Entity<TId>` for entities with non-string identifiers (e.g., `int`, `Guid`) and `Entity` for string-based 
identifiers.
- Implement business logic in derived entity classes (e.g., `UserEntity.UpdateName`) and use `QueueEvent` to 
capture state changes.
- **Always dispatch domain events AFTER successful persistence** to maintain consistency. Use the Unit of Work pattern for automatic event dispatching.
- **Implement domain events as `IRequest`** to enable mediator integration and decoupled event handling.
- **Use event handlers for side effects** like sending emails, updating read models, or triggering workflows.
- Ensure entities are persisted before comparing equality, as `IsTransient()` entities are considered unequal.
- Keep entity logic focused on domain behavior, delegating infrastructure concerns (e.g., persistence) to 
repositories or services.
- Combine with `CSharper.Results` to return `Result` or `Result<T>` for operations, reserving exceptions for 
unrecoverable errors.
- Leverage `CSharper.Functional` extension methods (e.g., `Bind`, `Match`) for composing entity operations in 
functional workflows.

## Related Docs

- [**CSharper.Results**](CSharper.Results.md)
- [**CSharper.Functional**](CSharper.Functional.md)