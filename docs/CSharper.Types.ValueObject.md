# CSharper.Types.ValueObject

CSharper’s `ValueObject` type provides a robust foundation for modeling immutable, identity-less objects in a 
Domain-Driven Design (DDD) context, ensuring value-based equality and comparison.

## Overview

The `ValueObject` class is an abstract base class for value objects, which are immutable objects defined by 
their properties rather than a unique identity. It supports value-based equality, hash code generation, and 
comparison, making it ideal for domain concepts like `Money`, `Address`, or `DateRange`. Designed for DDD, 
`ValueObject` integrates with CSharper’s Results, Mediator, and Functional extensions, promoting type-safe, 
composable, and testable domain models.

## Installation

To use `CSharper.Types.ValueObject`, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Then, include the `CSharper.Types` namespace:

```csharp
using CSharper.Types;
```

## Usage Examples

### Basic Usage with `ValueObject`

Create a `Money` value object with value-based equality:

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency ?? throw new ArgumentNullException(nameof(currency));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

// Usage
var money1 = new Money(100.50m, "USD");
var money2 = new Money(100.50m, "USD");
var money3 = new Money(200.00m, "USD");
Console.WriteLine(money1 == money2); // True (same Amount and Currency)
Console.WriteLine(money1 == money3); // False (different Amount)
```

### Comparison with `ValueObject`

Compare value objects using `CompareTo`:

```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }

    public Address(string street, string city)
    {
        Street = street ?? throw new ArgumentNullException(nameof(street));
        City = city ?? throw new ArgumentNullException(nameof(city));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
    }
}

// Usage
var addr1 = new Address("123 Main St", "Springfield");
var addr2 = new Address("456 Oak St", "Springfield");
Console.WriteLine(addr1.CompareTo(addr2)); // Negative (123 Main St < 456 Oak St)
```

### Integration with `CSharper.Results`

Validate a value object with error handling:

```csharp
public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrEmpty(value))
            return Result.Fail<Email>("Email cannot be empty", path: "email");
        if (!value.Contains("@"))
            return Result.Fail<Email>("Invalid email format", path: "email");
        return Result.Ok(new Email(value));
    }
}

// Usage
var result = Email.Create("invalid");
if (result.IsSuccess)
    Console.WriteLine($"Valid email: {result.Value}");
else
    Console.WriteLine($"Error: {result.Errors[0]}"); // Output: Error: Path: email - Invalid email format
```

## Features

- `ValueObject`: An abstract base class for immutable value objects, implementing `IComparable` and `IComparable<ValueObject>`.
- `GetEqualityComponents()`: Abstract method to be implemented by derived classes, returning the properties that define equality.
- **Equality**:
  - `Equals(object?)`: Compares value objects based on their `GetEqualityComponents()`, ensuring value-based equality.
  - `operator ==` and `operator !=`: Provide convenient equality comparisons.
  - `GetHashCode()`: Generates a cached hash code from equality components for efficient dictionary or set operations.
- **Comparison**:
  - `CompareTo(object?)` and `CompareTo(ValueObject?)`: Compare value objects by their equality components, supporting
  sorting and ordering.
  - Handles nulls, type mismatches, and non-comparable components gracefully.
- **Type Resolution**:
  - Uses `TypeHelper.GetUnproxiedType` to handle ORM proxies (e.g., Entity Framework Core, NHibernate), ensuring correct 
  type comparisons for equality and ordering.
- **Immutability**:
  - Encourages immutable design by requiring derived classes to set properties in constructors and avoid setters.

## Best Practices

- Use `ValueObject` for domain concepts without identity, such as `Money`, `Address`, or `DateRange`, ensuring immutability 
by making properties read-only.
- Implement `GetEqualityComponents()` to include all relevant properties that define the value object’s equality, ensuring consistent 
equality and hash code behavior.
- Configure `TypeHelper` with `EFCoreProxyTypeHelper`, `NHibernateProxyTypeHelper`, or a custom delegate at application startup to
handle ORM proxies, ensuring correct equality checks.
- Combine with `CSharper.Results` to validate value object creation, returning `Result<T>` to handle errors explicitly (e.g., invalid email format).
- Leverage `CSharper.Functional` extension methods (e.g., `Bind`, `Map`) for composing operations with value objects in functional workflows.
- Test equality and comparison thoroughly, ensuring `GetEqualityComponents()` covers all relevant properties and handles edge cases (e.g., nulls).

## Related Docs

- [**CSharper.Results**][results]
- [**CSharper.Functional**][functional]

[results]: CSharper.Results.md
[functional]: CSharper.Functional.md