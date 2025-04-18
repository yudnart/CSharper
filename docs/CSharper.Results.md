# CSharper.Results

CSharper's `Result` types provide a functional approach to error handling, avoiding exceptions for control flow and making error states explicit.

## Overview

The `Result` and `Result<T>` types encapsulate success or failure states, enabling robust and predictable error handling. `Result` is used for operations without a return value, while `Result<T>` carries a typed value on success. Both types support multiple `Error` objects, allowing detailed failure reporting with contextual information.

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
        return Result.Fail<int>("Division by zero", code: "DIV_ZERO", path: "divisor");
    return Result.Ok(a / b);
}

// Usage
Result<int> result = Divide(10, 0);
if (result.IsSuccess)
    Console.WriteLine($"Result: {result.Value}");
else
    Console.WriteLine($"Error: {result.Errors[0]}"); // Output: Error: Path: divisor - Division by zero (DIV_ZERO)
```

### Non-Typed `Result` with Multiple Errors

Validate an operation with multiple error details:

```csharp
public Result ValidateUser(string userId, string email)
{
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(userId))
            errors.Add(new Error("User ID cannot be empty", path: "userId"));
        if (string.IsNullOrEmpty(email))
            errors.Add(new Error("Email cannot be empty", path: "email"));
        return Result.Fail(errors[0], errors.Skip(1).ToArray());
    }
    return Result.Ok();
}

// Usage
Result result = ValidateUser("", "");
Console.WriteLine(result.IsSuccess ? "Valid user" : $"Errors: {string.Join(", ", result.Errors)}");
// Output: Errors: Path: userId - User ID cannot be empty, Path: email - Email cannot be empty
```

### Aggregating Results with `Collect`

Combine multiple results into a single outcome:

```csharp
Result<int> GetAge() => Result.Ok(25);
Result<string> GetName() => Result.Fail<string>("Name is missing", code: "NAME_MISSING", path: "name");
Result Validate() => Result.Ok();

Result combined = Result.Collect(new ResultLike[] { GetAge(), GetName(), Validate() });
Console.WriteLine(combined.IsSuccess ? "All succeeded" : $"Errors: {string.Join(", ", combined.Errors)}");
// Output: Errors: Path: name - Name is missing (NAME_MISSING)
```

## Features

- `Result.Ok()`: Creates a successful `Result` for operations without a return value.
- `Result.Ok<T>(T value)`: Creates a successful `Result<T>` with a typed value.
- `Result.Fail(Error causedBy, params Error[] details)`: Creates a failed `Result` with a primary `Error` and optional additional errors.
- `Result.Fail<T>(Error causedBy, params Error[] details)`: Creates a failed `Result<T>` with errors.
- `Result.Fail(string message, string? code = null, string? path = null)`: Creates a failed `Result` with an `Error` defined by a message, optional code, and path.
- `Result.Fail<T>(string message, string? code = null, string? path = null)`: Creates a failed `Result<T>` with an `Error`.
- `Result.Collect(IEnumerable<ResultLike> results)`: Aggregates multiple `Result` or `Result<T>` instances, succeeding only if all are successful, otherwise collecting all `Error` objects from failed results.
- **Properties**:
  - `IsSuccess`: Indicates if the result is successful.
  - `IsFailure`: Indicates if the result is a failure.
  - `Errors`: Provides a read-only collection of `Error` objects for failed results.
  - `Value` (for `Result<T>`): Returns the typed value for successful results.
- `Error` **Type**:
  - `Message`: A descriptive string explaining the error (required).
  - `Code`: An optional identifier for programmatic handling (e.g., "DIV_ZERO").
  - `Path`: An optional context indicator (e.g., field name like "divisor").
  - `ToString()`: Formats the error as `[Path: {path} - ]{message} [({code})]`, including `Path` and `Code` only if non-empty.

## Best Practices

- Use `Result` for operations without a return value (e.g., validation) and `Result<T>` for operations returning a value.
- Reserve exceptions for unrecoverable errors (e.g., network failures) and use `Result` for expected errors or business logic failures.
- Use `Error.Code` to categorize errors for programmatic handling (e.g., switch on codes) and `Error.Path` to pinpoint error locations (e.g., form fields).
- Log or display `Error.ToString()` for user-friendly error messages with full context.
- Combine with `CSharper.Functional` extension methods (`Bind`, `Match`, etc.) for composable workflows.
- Use `Result.Collect` to aggregate multiple operations, ensuring all succeed before proceeding.

## Related Docs

- [**CSharper.Functional**](CSharper.Functional.md)
