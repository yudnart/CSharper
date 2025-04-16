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
    /// This constructor is private to encourage use of factory methods like <see cref="Ok"/> for creating success results.
    /// </remarks>
    private Result() : base()
    {
        // Intentionally blank
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class as a failure result with errors.
    /// </summary>
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    /// <remarks>
    /// This constructor is private to encourage use of factory methods like <see cref="Fail(Error, Error[])"/> for creating failure results.
    /// </remarks>
    private Result(Error causedBy, params Error[] details)
        : base(causedBy, details)
    {
        // Intentionally blank
    }
}
