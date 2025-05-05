using CSharper.Errors;
using CSharper.Results;
using CSharper.Utilities;
using CSharper.Validation;
using System;
using System.Threading.Tasks;

namespace CSharper.Functional;

/// <summary>
/// Provides extension methods for handling both synchronous and asynchronous <see cref="ResultValidator{T}"/> 
/// operations in a functional programming style.
/// </summary>
public static class ResultValidatorExtensions
{
    #region ResultValidator

    public static Result Bind(this ResultValidator validator, Func<Result> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }
    public static Result<T> Bind<T>(this Validator validator, Func<Result<T>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    #endregion

    #region ResultValidator<T>

    /// <summary>
    /// Binds the validated result to a function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or next is null.</exception>
    public static Result Bind<T>(this ResultValidator<T> validator, Func<T, Result> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to a function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A <see cref="Result{U}"/> representing the outcome of the binding operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or next is null.</exception>
    public static Result<U> Bind<T, U>(this ResultValidator<T> validator, Func<T, Result<U>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to an asynchronous function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or next is null.</exception>
    public static Task<Result> Bind<T>(this ResultValidator<T> validator, Func<T, Task<Result>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validated result to an asynchronous function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="validator">The <see cref="ResultValidator{T}"/> containing the validation chain.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if validator or next is null.</exception>
    public static Task<Result<U>> Bind<T, U>(this ResultValidator<T> validator, Func<T, Task<Result<U>>> next)
    {
        validator.ThrowIfNull(nameof(validator));
        next.ThrowIfNull(nameof(next));
        return validator.Validate().Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidator{T}"/> to a function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncvalidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncvalidator or next is null.</exception>
    public static async Task<Result> Bind<T>(this Task<ResultValidator<T>> asyncvalidator, Func<T, Result> next)
    {
        asyncvalidator.ThrowIfNull(nameof(asyncvalidator));
        next.ThrowIfNull(nameof(next));
        return (await asyncvalidator).Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidator{T}"/> to a function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncvalidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncvalidator or next is null.</exception>
    public static async Task<Result<U>> Bind<T, U>(this Task<ResultValidator<T>> asyncvalidator, Func<T, Result<U>> next)
    {
        asyncvalidator.ThrowIfNull(nameof(asyncvalidator));
        next.ThrowIfNull(nameof(next));
        return (await asyncvalidator).Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous function that returns a non-typed <see cref="Result"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <param name="asyncvalidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncvalidator or next is null.</exception>
    public static async Task<Result> Bind<T>(this Task<ResultValidator<T>> asyncvalidator, Func<T, Task<Result>> next)
    {
        asyncvalidator.ThrowIfNull(nameof(asyncvalidator));
        next.ThrowIfNull(nameof(next));
        ResultValidator<T> validator = await asyncvalidator;
        return await validator.Bind(next);
    }

    /// <summary>
    /// Binds the validated result of an asynchronous <see cref="ResultValidator{T}"/> to an asynchronous function that returns a typed <see cref="Result{U}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the successful result value.</typeparam>
    /// <typeparam name="U">The type of the successful result value of the next operation.</typeparam>
    /// <param name="asyncvalidator">The Task containing the <see cref="ResultValidator{T}"/>.</param>
    /// <param name="next">The asynchronous function to execute if validation succeeds.</param>
    /// <returns>A Task containing a <see cref="Result{U}"/> representing the asynchronous outcome.</returns>
    /// <exception cref="ArgumentNullException">Thrown if asyncvalidator or next is null.</exception>
    public static async Task<Result<U>> Bind<T, U>(this Task<ResultValidator<T>> asyncvalidator, Func<T, Task<Result<U>>> next)
    {
        asyncvalidator.ThrowIfNull(nameof(asyncvalidator));
        next.ThrowIfNull(nameof(next));
        ResultValidator<T> validator = await asyncvalidator;
        return await validator.Bind(next);
    }

    #endregion
}