using System;

namespace CSharper.Results;

/// <summary>
/// Represents a generic result of an operation, indicating success with a value or failure 
/// with associated errors.
/// </summary>
/// <typeparam name="TValue">The type of the value returned by a successful result.</typeparam>
/// <remarks>
/// Represent operations with a return value.
/// Use <see cref="Result"/> for operations without a return value.
/// </remarks>
public sealed class Result<TValue> : ResultBase
{
    private readonly TValue _value;

    /// <summary>
    /// Gets the value of a successful result.
    /// </summary>
    /// <value>The value of the result if <see cref="ResultBase.IsSuccess"/> is <c>true</c>.</value>
    /// <exception cref="InvalidOperationException">Thrown if accessed on a failed result.</exception>
    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class as a success result with a value.
    /// </summary>
    /// <param name="value">The value of the successful result.</param>
    /// <remarks>
    /// This constructor is private to encourage use of the <see cref="Ok(TValue)"/> factory method.
    /// </remarks>
    private Result(TValue value) : base()
    {
        _value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class as a failure result with errors.
    /// </summary>
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    /// <remarks>
    /// This constructor is private to encourage use of the <see cref="Fail(Error, Error[])"/> factory method.
    /// The value is set to the default for <typeparamref name="TValue"/>.
    /// </remarks>
    private Result(Error causedBy, params Error[] details)
        : base(causedBy, details)
    {
        _value = default!;
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
    /// Creates a failed <see cref="Result{TValue}"/> instance with the specified errors.
    /// </summary>
    /// <param name="causedBy">The primary error causing the failure.</param>
    /// <param name="details">Additional error details, if any.</param>
    /// <returns>A new <see cref="Result{TValue}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="causedBy"/> is null.</exception>
    /// <remarks>
    /// This method is internal to support factory methods in <see cref="Result"/> while restricting direct instantiation.
    /// </remarks>
    internal static Result<TValue> Fail(Error causedBy, params Error[] details)
        => new(causedBy, details);
}
