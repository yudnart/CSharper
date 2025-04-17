using CSharper.Results;
using CSharper.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSharper.Functional;

/// <summary>
/// A builder for chaining validation predicates on a <see cref="Result{T}"/> to evaluate its value and collect errors.
/// </summary>
/// <typeparam name="T">The type of the successful result value.</typeparam>
public class ResultValidationChain<T>
{
    private readonly Result<T> _initialResult;
    private readonly List<(Func<T, bool>, Error)> _predicates;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultValidationChain{T}"/> class with an initial result.
    /// </summary>
    /// <param name="initialResult">The initial <see cref="Result{T}"/> to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="initialResult"/> is null.</exception>
    internal ResultValidationChain(Result<T> initialResult)
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
    /// <returns>The current <see cref="ResultValidationChain{T}"/> for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> or <paramref name="error"/> is null.</exception>
    public ResultValidationChain<T> And(Func<T, bool> predicate, Error error)
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

/// <summary>
/// Provides extension methods for handling <see cref="ResultValidationChain{T}"/> and <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result{T}}"/> 
/// operations in a functional programming style.
/// </summary>
public static class ResultValidationChainExtensions
{
    /// <summary>
    /// Binds the validated result to a function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="builder">The <see cref="ResultValidationChain{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="next"/> is null.</exception>
    public static Result Bind<T>(this ResultValidationChain<T> builder, Func<T, Result> next)
    {
        builder.ThrowIfNull(nameof(builder));
        next.ThrowIfNull(nameof(next));
        return builder.Collect().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to a function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="builder">The <see cref="ResultValidationChain{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result{U}"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="next"/> is null.</exception>
    public static Result<U> Bind<T, U>(this ResultValidationChain<T> builder, Func<T, Result<U>> next)
    {
        builder.ThrowIfNull(nameof(builder));
        next.ThrowIfNull(nameof(next));
        return builder.Collect().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to an asynchronous function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="builder">The <see cref="ResultValidationChain{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="next"/> is null.</exception>
    public static Task<Result> Bind<T>(this ResultValidationChain<T> builder, Func<T, Task<Result>> next)
    {
        builder.ThrowIfNull(nameof(builder));
        next.ThrowIfNull(nameof(next));
        return builder.Collect().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to an asynchronous function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="builder">The <see cref="ResultValidationChain{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result{U}}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="next"/> is null.</exception>
    public static Task<Result<U>> Bind<T, U>(this ResultValidationChain<T> builder, Func<T, Task<Result<U>>> next)
    {
        builder.ThrowIfNull(nameof(builder));
        next.ThrowIfNull(nameof(next));
        return builder.Collect().Bind(next);
    }

    /// <summary>
    /// Adds a predicate and associated error to the validation chain of an asynchronous <see cref="ResultValidationChain{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncBuilder">The asynchronous <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the validation builder.</param>
    /// <param name="predicate">The synchronous predicate to evaluate the result's value.</param>
    /// <param name="error">The error to include if the predicate fails.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the updated validation builder.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncBuilder"/>, <paramref name="predicate"/>, or <paramref name="error"/> is null.</exception>
    public static async Task<ResultValidationChain<T>> And<T>(this Task<ResultValidationChain<T>> asyncBuilder,
        Func<T, bool> predicate, Error error)
    {
        asyncBuilder.ThrowIfNull(nameof(asyncBuilder));
        predicate.ThrowIfNull(nameof(predicate));
        error.ThrowIfNull(nameof(error));
        ResultValidationChain<T> builder = await asyncBuilder;
        return builder.And(predicate, error);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidationChain{T}"/> to a function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncBuilder">The asynchronous <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the validation builder.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncBuilder"/> or <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind<T>(this Task<ResultValidationChain<T>> asyncBuilder, Func<T, Result> next)
    {
        asyncBuilder.ThrowIfNull(nameof(asyncBuilder));
        next.ThrowIfNull(nameof(next));
        ResultValidationChain<T> builder = await asyncBuilder;
        return builder.Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidationChain{T}"/> to a function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncBuilder">The asynchronous <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the validation builder.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result{U}}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncBuilder"/> or <paramref name="next"/> is null.</exception>
    public static async Task<Result<U>> Bind<T, U>(this Task<ResultValidationChain<T>> asyncBuilder, Func<T, Result<U>> next)
    {
        asyncBuilder.ThrowIfNull(nameof(asyncBuilder));
        next.ThrowIfNull(nameof(next));
        ResultValidationChain<T> builder = await asyncBuilder;
        return builder.Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidationChain{T}"/> to an asynchronous function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncBuilder">The asynchronous <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the validation builder.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncBuilder"/> or <paramref name="next"/> is null.</exception>
    public static async Task<Result> Bind<T>(this Task<ResultValidationChain<T>> asyncBuilder, Func<T, Task<Result>> next)
    {
        asyncBuilder.ThrowIfNull(nameof(asyncBuilder));
        next.ThrowIfNull(nameof(next));
        ResultValidationChain<T> builder = await asyncBuilder;
        return await builder.Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidationChain{T}"/> to an asynchronous function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncBuilder">The asynchronous <see cref="System.Threading.Tasks.Task`1{CSharper.Functional.ResultValidationBuilder{T}}"/> containing the validation builder.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task`1{CSharper.Results.Result{U}}"/> representing the asynchronous outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="asyncBuilder"/> or <paramref name="next"/> is null.</exception>
    public static async Task<Result<U>> Bind<T, U>(this Task<ResultValidationChain<T>> asyncBuilder, Func<T, Task<Result<U>>> next)
    {
        asyncBuilder.ThrowIfNull(nameof(asyncBuilder));
        next.ThrowIfNull(nameof(next));
        ResultValidationChain<T> builder = await asyncBuilder;
        return await builder.Bind(next);
    }
}