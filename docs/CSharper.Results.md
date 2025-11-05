# CSharper.Results

CSharper's `Result` types provide a functional approach to error handling, avoiding exceptions for control flow and making error states explicit.

## Overview

The `Result` and `Result<T>` types encapsulate success or failure states, enabling robust and predictable error handling. `Result` is used for operations without a return value, while `Result<T>` carries a typed value on success. Both types support a single `Error` with optional contextual data for detailed failure reporting.

## Installation

To use `CSharper.Results`, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Then, include the `CSharper.Results` namespace:

```csharp
using CSharper.Results;
```

## Usage Examples

### Basic Usage with `Result<T>`

Handle a division operation with error details:

```csharp
public Result<int> Divide(int a, int b)
{
    if (b == 0)
        return Result.Fail<int>("DIV_ZERO", "Division by zero");
    return Result.Ok(a / b);
}

// Usage
Result<int> result = Divide(10, 0);
if (result.IsSuccess)
    Console.WriteLine($"Result: {result.Value}");
else
    Console.WriteLine($"Error: {result.Error}"); 
    // Output: Error: Type=Error, Code=DIV_ZERO, Message=Division by zero
```

### Non-Typed `Result` with Error Context Data

Validate an operation with contextual error data:

```csharp
public Result ValidateUser(string userId, string email)
{
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
    {
        var data = new 
        {
            UserId = string.IsNullOrEmpty(userId) ? "User ID cannot be empty" : null,
            Email = string.IsNullOrEmpty(email) ? "Email cannot be empty" : null
        };
        return Result.Fail("VALIDATION_FAILED", "User validation failed", data);
    }
    return Result.Ok();
}

// Usage
Result result = ValidateUser("", "");
Console.WriteLine(result.IsSuccess ? "Valid user" : $"Error: {result.Error}");
// Output: Error: Type=Error, Code=VALIDATION_FAILED, Message=User validation failed
// Access error data: result.Error.Data["UserId"], result.Error.Data["Email"]
```

### Aggregating Results with `Sequence`

Combine multiple results into a single outcome:

```csharp
Result<int> GetAge() => Result.Ok(25);
Result<string> GetName() => Result.Fail<string>("NAME_MISSING", "Name is missing");
Result Validate() => Result.Ok();

Result combined = Result.Sequence(new ResultLike[] { GetAge(), GetName(), Validate() }, "VALIDATION_FAILED", "Validation failed");
Console.WriteLine(combined.IsSuccess ? "All succeeded" : $"Errors: {combined.Error}");
// Output: Errors: Type=Error, Code=VALIDATION_FAILED, Message=Validation failed
```

## Features

- `Result.Ok()`: Creates a successful `Result` for operations without a return value.
- `Result.Ok<T>(T value)`: Creates a successful `Result<T>` with a typed value.
- `Result.Fail(Error error)`: Creates a failed `Result` with an `Error`.
- `Result.Fail<T>(Error error)`: Creates a failed `Result<T>` with an `Error`.
- `Result.Fail(string code, string? message = null, object? data = null)`: Creates a failed `Result` with an `Error` defined by a code, optional message, and optional context data.
- `Result.Fail<T>(string code, string? message = null, object? data = null)`: Creates a failed `Result<T>` with an `Error`.
- `Result.Sequence(IEnumerable<ResultLike> results, string? code = null, string? message = null)`: Aggregates multiple `Result` or `Result<T>` instances, succeeding only if all are successful, otherwise collecting all errors from failed results.
- **Properties**:
  - `IsSuccess`: Indicates if the result is successful.
  - `IsFailure`: Indicates if the result is a failure.
  - `Error`: Provides the `Error` object for failed results.
  - `Value` (for `Result<T>`): Returns the typed value for successful results.
- `Error` **Type**:
  - `Code`: A required identifier for programmatic handling (e.g., "DIV_ZERO", "VALIDATION_FAILED").
  - `Message`: An optional descriptive string explaining the error.
  - `Data`: A dictionary containing optional contextual data as key-value pairs. Supports dictionaries, anonymous objects, and POCOs.
  - `ToString()`: Formats the error as `Type=Error, Code={code}[, Message={message}]`.

## Best Practices

- Use `Result` for operations without a return value (e.g., validation) and `Result<T>` for operations returning a value.
- Reserve exceptions for unrecoverable errors (e.g., network failures) and use `Result` for expected errors or business logic failures.
- Use `Error.Code` as the primary identifier for programmatic handling (e.g., switch statements, API error codes).
- Use `Error.Message` for human-readable descriptions.
- Use `Error.Data` to attach additional contextual information (e.g., field names, validation details, debug information).
- Log or display `Error.ToString()` for user-friendly error messages with full context.
- Combine with `CSharper.Functional` extension methods (`Bind`, `Match`, etc.) for composable workflows.
- Use `Result.Sequence` to aggregate multiple operations, ensuring all succeed before proceeding.

## Related Docs

- [**CSharper.Functional**](CSharper.Functional.md)