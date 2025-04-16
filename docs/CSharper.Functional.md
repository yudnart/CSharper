# CSharper.Functional

CSharper's functional extensions enhance the `Result` and `Result<T>` types with methods for functional-style task composition, supporting synchronous and asynchronous operations for chaining, validation, error handling, and side-effects in a declarative manner.

## Overview

The `CSharper.Functional` namespace provides a set of extension methods for `Result`, `Result<T>`, `Task<Result>`, and `Task<Result<T>>`, enabling functional programming patterns. These methods facilitate task composition, error propagation, validation, and side-effect handling without mutating state, making code predictable and easier to reason about.

## Installation

To join the `CSharper.Functional` journey, install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Then, include the `CSharper.Functional` namespace:

```csharp
using CSharper.Functional;
```

## Usage Examples

### Chaining Operations with `Bind`

Chain synchronous operations on `Result<T>`:

```csharp
Result<string> ValidateInput(string input) => string.IsNullOrEmpty(input) ? Result.Fail<string>("Input is empty", code: "EMPTY", path: "input") : Result.Ok(input);
Result<int> ParseNumber(string input) => int.TryParse(input, out var number) ? Result.Ok(number) : Result.Fail<int>("Invalid number", code: "INVALID_NUM", path: "input");

Result<int> result = ValidateInput("42")
    .Bind(ParseNumber);

Console.WriteLine(result.IsSuccess ? $"Parsed: {result.Value}" : $"Error: {result.Errors[0]}");
// Output: Parsed: 42
```

### Mapping Values with `Map`

Transform a `Result<T>` value synchronously:

```csharp
Result<int> number = Result.Ok(42);
Result<string> result = number.Map(n => $"Number: {n}");

Console.WriteLine(result.IsSuccess ? result.Value : $"Error: {result.Errors[0]}");
// Output: Number: 42
```

### Validating with `Ensure`

Validate a `Result<T>` with chained predicates:

```csharp
Result<string> input = Result.Ok("test");
Result<string> result = input
    .Ensure(s => s.Length > 3, new Error("Input too short", code: "SHORT", path: "input"))
    .Ensure(s => s.All(char.IsLetter), new Error("Input must be letters", code: "INVALID", path: "input"))
    .Build();

Console.WriteLine(result.IsSuccess ? $"Valid: {result.Value}" : $"Error: {result.Errors[0]}");
// Output: Error: Path: input - Input too short (SHORT)
```

### Asynchronous Chaining with `Bind`

Chain asynchronous operations on `Task<Result<T>>`:

```csharp
async Task<Result<string>> ValidateInputAsync(string input) => await Task.FromResult(ValidateInput(input));
async Task<Result<int>> ParseNumberAsync(string input) => await Task.FromResult(ParseNumber(input));

Task<Result<int>> result = ValidateInputAsync("")
    .Bind(ParseNumberAsync);

Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Parsed: {final.Value}" : $"Error: {final.Errors[0]}");
// Output: Error: Path: input - Input is empty (EMPTY)
```

### Recovering from Errors with `Recover`

Recover from a failed `Result<T>` asynchronously:

```csharp
Result<int> operation = Result.Fail<int>("Operation failed", code: "OP_FAIL", path: "process");
Task<Result<int>> recovered = operation.Recover(async errors =>
{
    Console.WriteLine($"Logging: {errors[0]}");
    return await Task.FromResult(Result.Ok(0));
});

Result<int> result = await recovered;
Console.WriteLine(result.IsSuccess ? $"Recovered: {result.Value}" : $"Error: {result.Errors[0]}");
// Output: Logging: Path: process - Operation failed (OP_FAIL)
//         Recovered: 0
```

### Performing Side-Effects with `Tap` and `TapError`

Perform asynchronous side-effects on `Task<Result>`:

```csharp
async Task<Result> operation() => Result.Ok();
Task<Result> result = operation()
    .Tap(async () => await Task.Run(() => Console.WriteLine("Success!")))
    .TapError(async errors => await Task.Run(() => Console.WriteLine($"Errors: {string.Join(", ", errors)}")));

Result final = await result;
Console.WriteLine(final.IsSuccess ? "Completed" : "Failed");
// Output: Success!
//         Completed
```

### Combining Synchronous and Asynchronous Workflow

Combine methods for a realistic workflow:

```csharp
async Task<Result<int>> ProcessInputAsync(string input)
{
    return await ValidateInputAsync(input)
        .Ensure(async s => await Task.FromResult(s.Length > 3), new Error("Input too short", code: "SHORT", path: "input"))
        .Build()
        .Bind(ParseNumberAsync)
        .Map(n => n * 2)
        .Tap(n => Console.WriteLine($"Doubled: {n}"))
        .TapError(errors => Console.WriteLine($"Errors: {string.Join(", ", errors)}"))
        .Recover(errors => Result.Ok(0));
}

Task<Result<int>> result = ProcessInputAsync("42");
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Result: {final.Value}" : $"Error: {final.Errors[0]}");
// Output: Errors: Path: input - Input too short (SHORT)
//         Result: 0
```

## Features

- **Bind Operations**:
  - Chains `Result` or `Result<T>` to synchronous or asynchronous operations, executing the next step only if successful.
  - Supports non-typed to typed (`Result` to `Result<T>`), typed to non-typed (`Result<T>` to `Result`), and typed to typed (`Result<T>` to `Result<U>`) transitions.
  - Asynchronous variants for `Task<Result>` and `Task<Result<T>>`.
- **Mapping and Transformation**:
  - `Map` transforms `Result<T>` values synchronously or asynchronously to a `Result<U>`.
  - `MapError` converts failed `Result` or `Result<T>` to non-typed `Result` or `Result<U>`, preserving `Errors`.
  - Asynchronous `Map` and `MapError` for `Task<Result>` and `Task<Result<T>>`.
- **Validation**:
  - `Ensure` validates `Result<T>` with synchronous or asynchronous predicates, returning a builder for chained validations.
  - Asynchronous `Ensure` for `Task<Result<T>>` with synchronous or asynchronous predicates.
- **Matching**:
  - `Match` executes success or failure handlers for `Result` or `Result<T>`, returning a value.
  - Supports synchronous and asynchronous handlers for `Task<Result>` and `Task<Result<T>>`.
- **Recovery**:
  - `Recover` handles errors in failed `Result` or `Result<T>`, returning a successful `Result` or `Result<T>`.
  - Asynchronous `Recover` for `Task<Result>` and `Task<Result<T>>` with synchronous or asynchronous fallbacks.
- **Side-Effects**:
  - `Tap` performs side-effects on success for `Result`, `Result<T>`, `Task<Result>`, or `Task<Result<T>>`.
  - `TapError` performs side-effects on failure, preserving the original result.
  - Asynchronous `Tap` and `TapError` variants.

## Best Practices

- Use `Bind` to chain dependent operations, ensuring errors propagate without explicit checks.
- Apply `Map` for value transformations, keeping pipelines concise and expressive.
- Use `Ensure` with `ResultValidationBuilder<T>` for complex validation logic, chaining multiple predicates.
- Leverage `Match` for branching logic, combining success and failure handling in a single expression.
- Use `Recover` to handle errors gracefully, providing fallbacks or logging before continuing.
- Apply `Tap` and `TapError` for side-effects like logging or notifications without altering the `Result`.
- Use asynchronous methods for I/O-bound operations, ensuring non-blocking workflows.
- Combine with `CSharper.Results` to create robust workflows, using `Error.Code` for programmatic handling and `Error.Path` for context (e.g., field validation).
- Display `Error.ToString()` for user-friendly error messages, including `Path` and `Code` when available.
- Chain methods to build declarative pipelines, improving readability and maintainability.

## Related Docs

- [**CSharper.Results**](../CSharper.Results/CSharper.Results.md)
