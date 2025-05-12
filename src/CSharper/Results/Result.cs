using CSharper.Errors;
using CSharper.Results.Abstractions;
using System;

namespace CSharper.Results;

/// <summary>
/// Represents a non-generic result of an operation, indicating success or failure with associated errors.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="ResultBase"/> to provide a type-safe way to represent operations without a return value.
/// Use <see cref="Result{T}"/> for operations that return a value.
/// </remarks>
public sealed partial class Result : ResultBase
{
    private static readonly Result _success = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class as a success result.
    /// </summary>
    /// <remarks>
    /// This constructor is private to encourage use of factory methods like <see cref="Result.Ok"/> for creating success results.
    /// </remarks>
    private Result() : base()
    {
        // Intentionally blank
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class as a failure result with an error.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// This constructor is private to encourage use of factory methods like <see cref="Result.Fail(Error)"/> for creating failure results.
    /// </remarks>
    private Result(Error error) : base(error)
    {
        // Intentionally blank
    }
}