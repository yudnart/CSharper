# CSharper.Types

CSharper’s `Types` module provides foundational abstractions for Domain-Driven Design (DDD) in .NET, enabling robust 
domain modeling with entities, value objects, auditing interfaces, and extensible type resolution for ORM integration.

## Overview

The `CSharper.Types` namespace offers a comprehensive set of types and utilities for building DDD-compliant domain models. 
It includes base classes for entities (`Entity<TId>`, `Entity`) and value objects (`ValueObject`), interfaces for auditing
and soft deletion (`IAuditable`, `ISoftDelete`), and a flexible type resolution system (`TypeHelper`) that supports 
custom ORM proxy handling, with built-in configurations for Entity Framework Core and NHibernate. These components 
ensure type safety and immutability.

## Installation

To use `CSharper.Types`, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Then, include the `CSharper.Types` namespace:

```csharp
using CSharper.Types;
```

## Usage Examples

### Auditing with `IAuditable`

Implement auditing for an entity’s creation and modification:

```csharp
public class UserEntity : Entity, IAuditable
{
    public UserEntity(string id, string createdBy)
    {
        Id = id;
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }

    public void Update(string modifiedBy)
    {
        LastModifiedAt = DateTimeOffset.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}

// Usage
var user = new UserEntity("user123", "system");
user.Update("admin");
Console.WriteLine($"Created: {user.CreatedAt}, Modified: {user.LastModifiedAt} by {user.LastModifiedBy}");
```

### Soft Deletion with `ISoftDelete`

Implement soft deletion for an entity:

```csharp
public class ProductEntity : Entity, ISoftDelete
{
    public ProductEntity(string id, string createdBy)
    {
        Id = id;
        CreatedAt = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset? LastModifiedAt { get; private set; }
    public string? LastModifiedBy { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    public void Delete(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
    }
}

// Usage
var product = new ProductEntity("prod456", "system");
product.Delete("admin");
Console.WriteLine(product.IsDeleted ? $"Deleted at {product.DeletedAt} by {product.DeletedBy}" : "Not deleted");
```

### Custom ORM Type Resolution with `TypeHelper`

Configure a custom delegate to resolve unproxied types for a hypothetical ORM:

```csharp
// Configure at application startup for a custom ORM
TypeHelper.ConfigureGetUnproxiedTypeDelegate(obj =>
{
    Type type = obj.GetType();
    // Example: Custom ORM uses "MyORM.Proxy" namespace for proxies
    if (type.FullName!.Contains("MyORM.Proxy"))
    {
        return type.BaseType ?? type; // Return the base type (unproxied)
    }
    return type; // Return original type if not a proxy
});

// Usage with Entity equality
var user1 = new UserEntity("user123", "system"); // Proxied by custom ORM
var user2 = new UserEntity("user123", "system"); // Another proxied instance
Console.WriteLine(user1.Equals(user2)); // True, as TypeHelper resolves unproxied type
```

### EF Core Proxy Resolution with `TypeHelper`

Use the built-in EF Core proxy resolution:

```csharp
// Configure at application startup
EFCoreProxyTypeHelper.Configure();

// Usage with Entity equality
var user1 = new UserEntity("user123", "system"); // Proxied by EF Core
var user2 = new UserEntity("user123", "system"); // Another proxied instance
Console.WriteLine(user1.Equals(user2)); // True, as EFCoreProxyTypeHelper resolves unproxied type
```

## Features

- [**Entity**][entity]: A base class for entities with identity and lifecycle, supporting domain events and equality
based on `Id`.
- [**ValueObject**][valueobject]: A base class for immutable value objects, with equality and comparison based on 
properties.
- **Auditing Interfaces**:
  - `IAuditCreate`: Defines properties for creation auditing (`CreatedAt`, `CreatedBy`).
  - `IAuditUpdate`: Defines properties for modification auditing (`LastModifiedAt`, `LastModifiedBy`).
  - `IAuditable`: Combines `IAuditCreate` and `IAuditUpdate` for comprehensive auditing.
  - `ISoftDelete`: Extends `IAuditable` with soft deletion properties (`IsDeleted`, `DeletedAt`, `DeletedBy`).
- **Type Resolution Utilities**:
  - `TypeHelper`:
    - `ConfigureGetUnproxiedTypeDelegate`: Sets a custom delegate to resolve the unproxied type of an object, crucial 
    for handling proxies created by ORMs (e.g., Entity Framework Core, NHibernate) or other frameworks. This allows 
    developers to define logic to identify and strip proxy layers, returning the original domain type for accurate 
    equality checks and type comparisons in `Entity<TId>` and `ValueObject`. For example, a delegate might check a 
    type’s namespace or metadata to determine if it’s a proxy and return its base type.
    - `GetUnproxiedType`: Retrieves the unproxied type of an object using the configured delegate, defaulting to 
    `obj.GetType()` if no delegate is set. Throws `ArgumentNullException` for null objects.
  - `EFCoreProxyTypeHelper`:
    - `Configure`: Configures `TypeHelper` to handle Entity Framework Core proxies by checking for the `Castle.Proxies` 
    prefix in the type’s string representation, returning the base type for proxies.
    - `TypePrefix`: Constant (`"Castle.Proxies"`) for identifying EF Core proxy types.
  - `NHibernateProxyTypeHelper`:
    - `Configure`: Configures `TypeHelper` to handle NHibernate proxies by checking for the `Proxy` suffix in the type’s
    string representation, returning the base type for proxies.
    - `TypePrefix`: Constant (`"Proxy"`) for identifying NHibernate proxy types.

## Best Practices

- Use `Entity` or `Entity<TId>` for domain objects with unique identity (e.g., `User`, `Product`) and `ValueObject` for 
immutable, identity-less concepts (e.g., `Money`, `Address`).
- Implement `IAuditable` for entities requiring creation and modification tracking, and `ISoftDelete` for entities 
supporting soft deletion.
- Configure `TypeHelper` at application startup using `EFCoreProxyTypeHelper`, `NHibernateProxyTypeHelper`, or a custom 
delegate for your ORM or proxying system to ensure correct type resolution for equality and comparison in `Entity<TId>` 
and `ValueObject`.
- Define custom `TypeHelper` delegates for non-standard ORMs by checking type names, namespaces, or metadata 
(e.g., `FullName.Contains("MyORM.Proxy")`) to resolve unproxied types accurately.
- Combine with `CSharper.Results` to return `Result` or `Result<T>` for entity operations, handling errors explicitly.
- Leverage `CSharper.Functional` extension methods (e.g., `Bind`, `Map`) for composing operations involving entities or 
value objects.
- Keep entity logic focused on domain behavior, delegating persistence and infrastructure concerns to repositories or 
services.

## Related Docs

- [**CSharper.Types.Entity**][entity]
- [**CSharper.Types.ValueObject**][valueobject]
- [**CSharper.Results**][results]
- [**CSharper.Functional**][functional]

[entity]: CSharper.Types.Entity.md
[valueobject]: CSharper.Types.ValueObject.md
[results]: CSharper.Results.md
[functional]: CSharper.Functional.md