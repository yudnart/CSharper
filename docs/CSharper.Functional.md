# CSharper.Functional

CSharper's functional extensions enhance `Result`, `Result<T>`, `Task<Result>`, and `Task<Result<T>>` with methods for functional-style task composition, supporting both synchronous and asynchronous operations for chaining, transformation, error handling, and side-effects in a declarative manner.

## Overview

The `CSharper.Functional` namespace provides extension methods to enable functional programming patterns. These methods facilitate task composition, error propagation, transformation, and side-effect handling without mutating state, making code predictable and easier to reason about.

## Installation

Install the `dht.csharper` package:

```bash
dotnet add package dht.csharper
```

Include the namespace:

```csharp
using CSharper.Functional;
```

## Usage Examples

### Bind: Chaining Operations

`Bind` chains operations, executing the next step only if the current result is successful, propagating errors otherwise.

**Synchronous Example**:

```csharp
Result<string> ValidateInput(string input) => string.IsNullOrEmpty(input) 
    ? Result.Fail<string>("Input is empty", code: "EMPTY", path: "input") 
    : Result.Ok(input);
Result<int> ParseNumber(string input) => int.TryParse(input, out var number) 
    ? Result.Ok(number) 
    : Result.Fail<int>("Invalid number", code: "INVALID_NUM", path: "input");

Result<int> result = ValidateInput("42").Bind(ParseNumber);
Console.WriteLine(result.IsSuccess ? $"Parsed: {result.Value}" : $"Error: {result.Error}");
// Output: Parsed: 42
```

**Asynchronous Example**:

```csharp
async Task<Result<string>> ValidateInputAsync(string input) => await Task.FromResult(ValidateInput(input));
async Task<Result<int>> ParseNumberAsync(string input) => await Task.FromResult(ParseNumber(input));

Task<Result<int>> result = ValidateInputAsync("42").Bind(ParseNumberAsync);
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Parsed: {final.Value}" : $"Error: {final.Error}");
// Output: Parsed: 42
```

### Map: Transforming Values

`Map` transforms a successful `Result<T>` value into a new `Result<U>`, preserving errors on failure.

**Synchronous Example**:

```csharp
Result<int> number = Result.Ok(42);
Result<string> result = number.Map(n => $"Number: {n}");
Console.WriteLine(result.IsSuccess ? result.Value : $"Error: {result.Error}");
// Output: Number: 42
```

**Asynchronous Example**:

```csharp
Task<Result<int>> number = Task.FromResult(Result.Ok(42));
Task<Result<string>> result = number.Map(n => $"Number: {n}");
Result<string> final = await result;
Console.WriteLine(final.IsSuccess ? final.Value : $"Error: {final.Error}");
// Output: Number: 42
```

### MapError: Mapping Errors

`MapError` converts a failed `Result` or `Result<T>` to a non-typed `Result` or `Result<U>`, preserving errors.

**Synchronous Example**:

```csharp
Result<string> failed = Result.Fail<string>("Invalid input", code: "INVALID", path: "input");
Result<int> result = failed.MapError<string, int>();
Console.WriteLine(result.IsSuccess ? "Unexpected success" : $"Error: {result.Error}");
// Output: Error: Path: input - Invalid input (INVALID)
```

**Asynchronous Example**:

```csharp
Task<Result<string>> failed = Task.FromResult(Result.Fail<string>("Invalid input", code: "INVALID", path: "input"));
Task<Result<int>> result = failed.MapError<string, int>();
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? "Unexpected success" : $"Error: {final.Error}");
// Output: Error: Path: input - Invalid input (INVALID)
```

### Match: Handling Success or Failure

`Match` executes handlers based on the result's state, returning a value for success or failure.

**Synchronous Example**:

```csharp
Result<int> number = Result.Ok(42);
string result = number.Match(
    n => $"Success: {n}",
    e => $"Error: {e}");
Console.WriteLine(result);
// Output: Success: 42
```

**Asynchronous Example**:

```csharp
Task<Result<int>> number = Task.FromResult(Result.Ok(42));
Task<string> result = number.Match(
    async n => await Task.FromResult($"Success: {n}"),
    async e => await Task.FromResult($"Error: {e}"));
Console.WriteLine(await result);
// Output: Success: 42
```

### Recover: Handling Errors

`Recover` handles errors in a failed result, returning a successful `Result` or `Result<T>` with a fallback.

**Synchronous Example**:

```csharp
Result<int> failed = Result.Fail<int>("Operation failed", code: "OP_FAIL", path: "process");
Result<int> result = failed.Recover(e => 0);
Console.WriteLine(result.IsSuccess ? $"Recovered: {result.Value}" : $"Error: {result.Error}");
// Output: Recovered: 0
```

**Asynchronous Example**:

```csharp
Result<int> failed = Result.Fail<int>("Operation failed", code: "OP_FAIL", path: "process");
Task<Result<int>> result = failed.Recover(async e => await Task.FromResult(0));
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Recovered: {final.Value}" : $"Error: {final.Error}");
// Output: Recovered: 0
```

### Tap: Performing Side-Effects on Success

`Tap` performs a side-effect on success, returning the original result.

**Synchronous Example**:

```csharp
Result<int> number = Result.Ok(42);
Result<int> result = number.Tap(n => Console.WriteLine($"Success: {n}"));
Console.WriteLine(result.IsSuccess ? $"Value: {result.Value}" : $"Error: {result.Error}");
// Output: Success: 42
//         Value: 42
```

**Asynchronous Example**:

```csharp
Task<Result<int>> number = Task.FromResult(Result.Ok(42));
Task<Result<int>> result = number.Tap(async n => await Task.Run(() => Console.WriteLine($"Success: {n}")));
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Value: {final.Value}" : $"Error: {final.Error}");
// Output: Success: 42
//         Value: 42
```

### TapError: Performing Side-Effects on Failure

`TapError` performs a side-effect on failure, returning the original result.

**Synchronous Example**:

```csharp
Result<int> failed = Result.Fail<int>("Invalid input", code: "INVALID", path: "input");
Result<int> result = failed.TapError(e => Console.WriteLine($"Error: {e}"));
Console.WriteLine(result.IsSuccess ? "Unexpected success" : $"Error: {result.Error}");
// Output: Error: Path: input - Invalid input (INVALID)
//         Error: Path: input - Invalid input (INVALID)
```

**Asynchronous Example**:

```csharp
Task<Result<int>> failed = Task.FromResult(Result.Fail<int>("Invalid input", code: "INVALID", path: "input"));
Task<Result<int>> result = failed.TapError(async e => await Task.Run(() => Console.WriteLine($"Error: {e}")));
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? "Unexpected success" : $"Error: {final.Error}");
// Output: Error: Path: input - Invalid input (INVALID)
//         Error: Path: input - Invalid input (INVALID)
```

### Combined Workflow

Combine sync and async methods for a realistic workflow:

```csharp
async Task<Result<int>> ProcessInputAsync(string input)
{
    return await ValidateInputAsync(input)
        .Bind(ParseNumberAsync)
        .Map(n => n * 2)
        .Tap(n => Console.WriteLine($"Doubled: {n}"))
        .TapError(e => Console.WriteLine($"Error: {e}"))
        .Recover(e => 0);
}

Task<Result<int>> result = ProcessInputAsync("42");
Result<int> final = await result;
Console.WriteLine(final.IsSuccess ? $"Result: {final.Value}" : $"Error: {final.Error}");
// Output: Doubled: 84
//         Result: 84
```

## Features

### Bind: Chaining Operations
- **Synchronous**:
  - `Result.Bind(Func<Result>)`: Chains to non-typed result.
  - `Result.Bind<T>(Func<Result<T>>)`: Chains to typed result.
  - `Result<T>.Bind(Func<T, Result>)`: Chains typed to non-typed result.
  - `Result<T>.Bind<U>(Func<T, Result<U>>)`: Chains typed to typed result.
- **Asynchronous**:
  - `Result.Bind(Func<Task<Result>>)`: Chains to async non-typed result.
  - `Result.Bind<T>(Func<Task<Result<T>>>)`: Chains to async typed result.
  - `Result<T>.Bind(Func<T, Task<Result>>)`: Chains typed to async non-typed result.
  - `Result<T>.Bind<U>(Func<T, Task<Result<U>>>)`: Chains typed to async typed result.
  - `Task<Result>.Bind(Func<Result>)`: Chains async to sync non-typed result.
  - `Task<Result>.Bind<T>(Func<Result<T>>)`: Chains async to sync typed result.
  - `Task<Result>.Bind(Func<Task<Result>>)`: Chains async to async non-typed result.
  - `Task<Result>.Bind<T>(Func<Task<Result<T>>>)`: Chains async to async typed result.
  - `Task<Result<T>>.Bind(Func<T, Result>)`: Chains async typed to sync non-typed result.
  - `Task<Result<T>>.Bind<U>(Func<T, Result<U>>)`: Chains async typed to sync typed result.
  - `Task<Result<T>>.Bind(Func<T, Task<Result>>)`: Chains async typed to async non-typed result.
  - `Task<Result<T>>.Bind<U>(Func<T, Task<Result<U>>>)`: Chains async typed to async typed result.

### Map: Transforming Values
- **Synchronous**:
  - `Result<T>.Map<U>(Func<T, U>)`: Transforms value to `Result<U>`.
- **Asynchronous**:
  - `Task<Result<T>>.Map<U>(Func<T, U>)`: Transforms async value to `Result<U>`.

### MapError: Mapping Errors
- **Synchronous**:
  - `Result.MapError<T>()`: Maps failed result to `Result<T>`.
  - `Result<T>.MapError()`: Maps failed typed result to non-typed `Result`.
  - `Result<T>.MapError<U>()`: Maps failed typed result to `Result<U>`.
- **Asynchronous**:
  - `Task<Result>.MapError<T>()`: Maps async failed result to `Result<T>`.
  - `Task<Result<T>>.MapError()`: Maps async failed typed result to non-typed `Result`.
  - `Task<Result<T>>.MapError<U>()`: Maps async failed typed result to `Result<U>`.

### Match: Handling Success or Failure
- **Synchronous**:
  - `Result.Match<T>(Func<T>)`: Executes success handler, returns default on failure.
  - `Result.Match<T>(Func<T>, Func<Error, T>)`: Executes success or failure handler.
  - `Result<T>.Match<U>(Func<T, U>)`: Executes success handler with value, returns default on failure.
  - `Result<T>.Match<U>(Func<T, U>, Func<Error, U>)`: Executes success or failure handler with value.
- **Asynchronous**:
  - `Result.Match<T>(Func<Task<T>>)`: Executes async success handler, returns default on failure.
  - `Result.Match<T>(Func<Task<T>>, Func<Error, Task<T>>)`: Executes async success or failure handler.
  - `Result<T>.Match<U>(Func<T, Task<U>>)`: Executes async success handler with value, returns default on failure.
  - `Result<T>.Match<U>(Func<T, Task<U>>, Func<Error, Task<U>>)`: Executes async success or failure handler with value.
  - `Task<Result>.Match<T>(Func<T>)`: Executes sync success handler on async result.
  - `Task<Result>.Match<T>(Func<T>, Func<Error, T>)`: Executes sync success or failure handler on async result.
  - `Task<Result>.Match<T>(Func<Task<T>>)`: Executes async success handler on async result.
  - `Task<Result>.Match<T>(Func<Task<T>>, Func<Error, Task<T>>)`: Executes async success or failure handler on async result.
  - `Task<Result<T>>.Match<U>(Func<T, U>)`: Executes sync success handler on async typed result.
  - `Task<Result<T>>.Match<U>(Func<T, U>, Func<Error, U>)`: Executes sync success or failure handler on async typed result.
  - `Task<Result<T>>.Match<U>(Func<T, Task<U>>)`: Executes async success handler on async typed result.
  - `Task<Result<T>>.Match<U>(Func<T, Task<U>>, Func<Error, Task<U>>)`: Executes async success or failure handler on async typed result.

### Recover: Handling Errors
- **Synchronous**:
  - `Result.Recover(Action<Error>)`: Executes failure handler, returns successful `Result`.
  - `Result<T>.Recover(Func<Error, T>)`: Executes failure handler, returns successful `Result<T>`.
- **Asynchronous**:
  - `Result.Recover(Func<Error, Task>)`: Executes async failure handler, returns successful `Result`.
  - `Result<T>.Recover(Func<Error, Task<T>>)`: Executes async failure handler, returns successful `Result<T>`.
  - `Task<Result>.Recover(Action<Error>)`: Executes sync failure handler on async result.
  - `Task<Result>.Recover(Func<Error, Task>)`: Executes async failure handler on async result.
  - `Task<Result<T>>.Recover(Func<Error, T>)`: Executes sync failure handler on async typed result.
  - `Task<Result<T>>.Recover(Func<Error, Task<T>>)`: Executes async failure handler on async typed result.

### Tap: Performing Side-Effects on Success
- **Synchronous**:
  - `Result.Tap(Action)`: Performs side-effect on success, returns original `Result`.
  - `Result<T>.Tap(Action<T>)`: Performs side-effect on success with value, returns original `Result<T>`.
- **Asynchronous**:
  - `Result.Tap(Func<Task>)`: Performs async side-effect on success, returns original `Result`.
  - `Result<T>.Tap(Func<T, Task>)`: Performs async side-effect on success with value, returns original `Result<T>`.
  - `Task<Result>.Tap(Action)`: Performs sync side-effect on async success, returns original `Result`.
  - `Task<Result>.Tap(Func<Task>)`: Performs async side-effect on async success, returns original `Result`.
  - `Task<Result<T>>.Tap(Action<T>)`: Performs sync side-effect on async typed success, returns original `Result<T>`.
  - `Task<Result<T>>.Tap(Func<T, Task>)`: Performs async side-effect on async typed success, returns original `Result<T>`.

### TapError: Performing Side-Effects on Failure
- **Synchronous**:
  - `Result.TapError(Action<Error>)`: Performs side-effect on failure, returns original `Result`.
  - `Result<T>.TapError(Action<Error>)`: Performs side-effect on failure, returns original `Result<T>`.
- **Asynchronous**:
  - `Result.TapError(Func<Error, Task>)`: Performs async side-effect on failure, returns original `Result`.
  - `Result<T>.TapError(Func<Error, Task>)`: Performs async side-effect on failure, returns original `Result<T>`.
  - `Task<Result>.TapError(Action<Error>)`: Performs sync side-effect on async failure, returns original `Result`.
  - `Task<Result>.TapError(Func<Error, Task>)`: Performs async side-effect on async failure, returns original `Result`.
  - `Task<Result<T>>.TapError(Action<Error>)`: Performs sync side-effect on async typed failure, returns original `Result<T>`.
  - `Task<Result<T>>.TapError(Func<Error, Task>)`: Performs async side-effect on async typed failure, returns original `Result<T>`.

## Best Practices

- Use `Bind` to chain dependent operations, ensuring errors propagate without explicit checks.
- Apply `Map` for value transformations, keeping pipelines concise.
- Leverage `Match` for branching logic, combining success and failure handling.
- Use `Recover` to handle errors gracefully, providing fallbacks or logging.
- Apply `Tap` and `TapError` for side-effects like logging or notifications without altering the `Result`.
- Use asynchronous methods for I/O-bound operations, ensuring non-blocking workflows.
- Combine with `CSharper.Results` for robust workflows, using `Error.Code` for programmatic handling and `Error.Path` for context.
- Display `Error.ToString()` for user-friendly error messages, including `Path` and `Code`.
- Chain methods to build declarative pipelines, improving readability and maintainability.

## Related Docs

- [**CSharper.Results**](../CSharper.Results/CSharper.Results.md)