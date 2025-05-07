using CSharper.Errors;
using CSharper.Results.Abstractions;
using System;

namespace CSharper.Results;

/// <summary>
/// Represents a result of an operation with a returned <typeparamref name="T"/>,
/// indicating success with a value or failure with an error.
/// </summary>
/// <typeparam name="T">The type of the value returned by a successful result.</typeparam>
/// <remarks>
/// Use <see cref="Result"/> for operations without a return value.
/// </remarks>
public sealed class Result<T> : ResultBase
{
    private readonly T _value;

    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    /// <value>The value of the result if <see cref="ResultBase.IsSuccess"/> is <see langword="true"/>.</value>
    /// <exception cref="InvalidOperationException">Thrown if accessed on a failed result.</exception>
    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>
    /// Initializes a new instance of a success <see cref="Result{TValue}"/>.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <remarks>
    /// This constructor is private to encourage use of the <see cref="Ok(T)"/> factory method.
    /// </remarks>
    private Result(T value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of a failure <see cref="Result{TValue}"/> with error.
    /// </summary>
    /// <param name="error">The error causing the failure.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// Use <see cref="Result.Fail(Error, Error[])"/> factory method.
    /// </remarks>
    private Result(Error error) : base(error)
    {
        _value = default!;
    }

    /// <returns>
    /// <c>"{Value}"</c> or <c>"Success: {Value}"</c> for a success result, or a formatted string for a failure result.
    /// </returns>
    /// <inheritdoc/>
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
    internal static Result<T> Ok(T value) => new(value);

    /// <summary>
    /// Creates a failed <see cref="Result{TValue}"/> instance with the specified errors.
    /// </summary>
    /// <param name="error">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    /// <remarks>
    /// This method is internal to support factory methods in <see cref="Result"/> while restricting direct instantiation.
    /// </remarks>
    internal static Result<T> Fail(Error error) => new(error);
}
