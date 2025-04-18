using System;

namespace CSharper.Results;

/// <summary>
/// Wraps a result or result factory for aggregation in methods like <see cref="Result.Collect"/>.
/// </summary>
/// <remarks>
/// This class supports both <see cref="Result"/> and <see cref="Result{T}"/> instances, as well as
/// factory methods that produce results, enabling flexible result aggregation.
/// </remarks>
public sealed class ResultLike
{
    private readonly object _result;

    /// <summary>
    /// Gets the underlying <see cref="ResultBase"/> value, resolving factories if necessary.
    /// </summary>
    /// <value>The <see cref="ResultBase"/> instance representing the result.</value>
    /// <exception cref="NotSupportedException">Thrown if the wrapped object is not a <see cref="ResultBase"/> or a factory producing one.</exception>
    public ResultBase Value => GetResult();

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultLike"/> class with a result or factory.
    /// </summary>
    /// <param name="result">The <see cref="ResultBase"/> instance or a factory method producing one.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="result"/> is null.</exception>
    private ResultLike(object result)
    {
        _result = result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <summary>
    /// Resolves the wrapped object to a <see cref="ResultBase"/> instance.
    /// </summary>
    /// <returns>The resolved <see cref="ResultBase"/> instance.</returns>
    /// <exception cref="NotSupportedException">Thrown if the wrapped object is not a <see cref="ResultBase"/> or a factory producing one.</exception>
    private ResultBase GetResult()
    {
        return _result switch
        {
            ResultBase result => result,
            Func<ResultBase> factory => factory(),
            _ => throw new NotSupportedException($"Invalid result type. (${_result.GetType().FullName})")
        };
    }

    /// <summary>
    /// Implicitly converts a <see cref="ResultBase"/> instance to a <see cref="ResultLike"/>.
    /// </summary>
    /// <param name="result">The <see cref="ResultBase"/> instance to wrap.</param>
    /// <returns>A new <see cref="ResultLike"/> wrapping the result.</returns>
    public static implicit operator ResultLike(ResultBase result)
    {
        return new ResultLike(result);
    }

    /// <summary>
    /// Implicitly converts a factory method producing a <see cref="ResultBase"/> to a <see cref="ResultLike"/>.
    /// </summary>
    /// <param name="factory">The factory method that produces a <see cref="ResultBase"/>.</param>
    /// <returns>A new <see cref="ResultLike"/> wrapping the factory.</returns>
    public static implicit operator ResultLike(Func<ResultBase> factory)
    {
        return new ResultLike(factory);
    }
}
