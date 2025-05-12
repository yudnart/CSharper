# CSharper.Results

CSharper's `Result` types provide a functional approach to error handling, avoiding exceptions for control flow and making error states explicit.

## Overview

The `Result` and `Result<T>` types encapsulate success or failure states, enabling robust and predictable error handling. `Result` is used for operations without a return value, while `Result<T>` carries a typed value on success. Both types support a single `Error` with optional `ErrorDetail` objects for detailed failure reporting.

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
        return Result.Fail<int>("Division by zero", code: "DIV_ZERO");
    return Result.Ok(a / b);
}

// Usage
Result<int> result = Divide(10, 0);
if (result.IsSuccess)
    Console.WriteLine($"Result: {result.Value}");
else
    Console.WriteLine($"Error: {result.Error}"); // Output: Error: Division by zero, Code=DIV_ZERO
```

### Non-Typed `Result` with Error Details

Validate an operation with multiple error details:

```csharp
public Result ValidateUser(string userId, string email)
{
    if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
    {
        var details = new List<ErrorDetail>();
        if (string.IsNullOrEmpty(userId))
            details.Add(new ErrorDetail("User ID cannot be empty"));
        if (string.IsNullOrEmpty(email))
            details.Add(new ErrorDetail("Email cannot be empty"));
        return Result.Fail("User validation failed", errorDetails: details.ToArray());
    }
    return Result.Ok();
}

// Usage
Result result = ValidateUser("", "");
Console.WriteLine(result.IsSuccess ? "Valid user" : $"Errors: {result.Error}");
// Output: Errors: User validation failed> User ID cannot be empty> Email cannot be empty
```

### Aggregating Results with `Sequence`

Combine multiple results into a single outcome:

```csharp
Result<int> GetAge() => Result.Ok(25);
Result<string> GetName() => Result.Fail<string>("Name is missing", code: "NAME_MISSING");
Result Validate() => Result.Ok();

Result combined = Result.Sequence(new ResultLike[] { GetAge(), GetName(), Validate() }, "Validation failed");
Console.WriteLine(combined.IsSuccess ? "All succeeded" : $"Errors: {combined.Error}");
// Output: Errors: Validation failed> Name is missing, Code=NAME_MISSING
```

## Features

- `Result.Ok()`: Creates a successful `Result` for operations without a return value.
- `Result.Ok<T>(T value)`: Creates a successful `Result<T>` with a typed value.
- `Result.Fail(Error error)`: Creates a failed `Result` with an `Error`.
- `Result.Fail<T>(Error error)`: Creates a failed `Result<T>` with an `Error`.
- `Result.Fail(string message, string? code = null)`: Creates a failed `Result` with an `Error` defined by a message and optional code.
- `Result.Fail<T>(string message, string? code = null)`: Creates a failed `Result<T>` with an `Error`.
- `Result.Sequence(IEnumerable<ResultLike> results, string message, string? code = null)`: Aggregates multiple `Result` or `Result<T>` instances, succeeding only if all are successful, otherwise collecting all `ErrorDetail` objects from failed results.
- **Properties**:
  - `IsSuccess`: Indicates if the result is successful.
  - `IsFailure`: Indicates if the result is a failure.
  - `Error`: Provides the `Error` object for failed results.
  - `Value` (for `Result<T>`): Returns the typed value for successful results.
- `Error` **Type**:
  - `Message`: A descriptive string explaining the error (required).
  - `Code`: An optional identifier for programmatic handling (e.g., "DIV_ZERO").
  - `ErrorDetails`: A read-only list of `ErrorDetail` objects for additional context.
  - `ToString()`: Formats the error as `{message}[, Code={code}]` followed by indented `ErrorDetail` messages prefixed with "> ".
- `ErrorDetail` **Type**:
  - `Message`: A descriptive string for the detail (required).
  - `Code`: An optional identifier for the detail.
  - `ToString()`: Formats as `{message}[, Code={code}]`.

## Best Practices

- Use `Result` for operations without a return value (e.g., validation) and `Result<T>` for operations returning a value.
- Reserve exceptions for unrecoverable errors (e.g., network failures) and use `Result` for expected errors or business logic failures.
- Use `Error.Code` to categorize errors for programmatic handling (e.g., switch on codes) and `ErrorDetail` for additional context (e.g., specific field errors).
- Log or display `Error.ToString()` for user-friendly error messages with full context.
- Combine with `CSharper.Functional` extension methods (`Bind`, `Match`, etc.) for composable workflows.
- Use `Result.Sequence` to aggregate multiple operations, ensuring all succeed before proceeding.

## Related Docs

- [**CSharper.Functional**](CSharper.Functional.md)