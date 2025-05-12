using CSharper.Errors;
using CSharper.Results.Abstractions;
using System;

namespace CSharper.Results;

/// <summary>
/// Represents a result of an operation with a returned <typeparamref name="TValue"/>,
/// indicating success with a value or failure with an error.
/// </summary>
/// <typeparam name="TValue">The type of the value returned by a successful result.</typeparam>
/// <remarks>
/// Use <see cref="Result"/> for operations without a return value.
/// </remarks>
public sealed class Result<TValue> : ResultBase
{
    private readonly TValue _value;

    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    /// <value>The value of the result if <see cref="ResultBase.IsSuccess"/> is true.</value>
    /// <exception cref="InvalidOperationException">Thrown if accessed on a failed result.</exception>
    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>
    /// Initializes a new instance of a successful <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <remarks>
    /// This constructor is private to encourage use of the <see cref="Result.Ok{T}(T)"/> factory method.
    /// </remarks>
    private Result(TValue value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of a failed <see cref="Result{TValue}"/> with an error.
    /// </summary>
    /// <param name="error">The error causing the failure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// This constructor is private to encourage use of the <see cref="Result.Fail{TValue}(Error)"/> factory method.
    /// </remarks>
    private Result(Error error) : base(error)
    {
        _value = default!;
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>
    /// For a success result, returns the value's string representation or "Success: null" if the value is null.
    /// For a failure result, returns the base class's string representation.
    /// </returns>
    public override string ToString()
    {
        return IsSuccess ?
            Value?.ToString() ?? "Success: null" : base.ToString();
    }

    /// <summary>
    /// Creates a successful <see cref="Result{TValue}"/> instance with the specified value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a successful operation.</returns>
    /// <remarks>
    /// This method is internal to support factory methods in <see cref="Result"/> while restricting direct instantiation.
    /// </remarks>
    internal static Result<TValue> Ok(TValue value) => new(value);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> instance with the specified error.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// This method is internal to support factory methods in <see cref="Result"/> while restricting direct instantiation.
    /// </remarks>
    internal static Result<TValue> Fail(Error error) => new(error);
}