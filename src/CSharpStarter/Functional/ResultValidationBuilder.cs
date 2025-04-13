using CSharpStarter.Results;
using CSharpStarter.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpStarter.Functional;

/// <summary>
/// A builder for chaining validation predicates on a <see cref="Result{T}"/> to evaluate its value and collect errors.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
public class ResultValidationBuilder<T>
{
    private readonly Result<T> _initialResult;
    private readonly List<(Func<T, bool>, Error)> _predicates;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultValidationBuilder{T}"/> class with an initial result.
    /// </summary>
    /// <param name="initialResult">The initial <see cref="Result{T}"/> to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="initialResult"/> is null.</exception>
    internal ResultValidationBuilder(Result<T> initialResult)
    {
        _initialResult = initialResult
            ?? throw new ArgumentNullException(nameof(initialResult));
        _predicates = [];
    }

    /// <summary>
    /// Adds a predicate and associated error to the validation chain.
    /// </summary>
    /// <param name="predicate">The synchronous predicate to evaluate the result's value.</param>
    /// <param name="error">The error to include if the predicate fails.</param>
    /// <returns>The current <see cref="ResultValidationBuilder{T}"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public ResultValidationBuilder<T> Ensure(Func<T, bool> predicate, Error error)
    {
        predicate.ThrowIfNull(nameof(predicate));
        error.ThrowIfNull(nameof(error));
        _predicates.Add((predicate, error));
        return this;
    }

    /// <summary>
    /// Evaluates all predicates and returns the final <see cref="Result{T}"/> with collected errors, if any.
    /// </summary>
    /// <returns>
    /// The original <see cref="Result{T}"/> if successful and all predicates pass, 
    /// or a failed <see cref="Result{T}"/> with errors from failed predicates.
    /// </returns>
    public Result<T> Collect()
    {
        if (_initialResult.IsFailure)
        {
            return _initialResult;
        }

        Error[] errors = EvaluatePredicates(_initialResult.Value, _predicates).ToArray();

        return errors.Length == 0
            ? _initialResult
            : Result.Fail<T>(errors[0], [.. errors.Skip(1)]);
    }

    /// <summary>
    /// Evaluates predicates against a value and yields errors for those that fail.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to evaluate.</typeparam>
    /// <param name="value">The value to evaluate against predicates.</param>
    /// <param name="predicates">The collection of predicates and associated errors.</param>
    /// <returns>An enumerable of <see cref="Error"/> objects for predicates that fail.</returns>
    private static IEnumerable<Error> EvaluatePredicates<TValue>(TValue value, IEnumerable<(Func<TValue, bool>, Error)> predicates)
    {
        foreach ((Func<TValue, bool> predicate, Error error) in predicates)
        {
            if (!predicate(value))
            {
                yield return error;
            }
        }
    }
}