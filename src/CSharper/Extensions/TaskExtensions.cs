using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Extensions;

/// <summary>
/// Provides extension methods for working with <see cref="Task"/> and <see cref="Task{T}"/> in a functional style.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Continues a <see cref="Task"/> with a synchronous function, executing the function if the task completes successfully.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the continuation function.</typeparam>
    /// <param name="task">The task to continue.</param>
    /// <param name="next">The synchronous function to invoke if the task completes successfully.</param>
    /// <returns>A task representing the result of the continuation function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> or <paramref name="next"/> is null.</exception>
    /// <exception cref="TaskCanceledException">Thrown if the task is canceled.</exception>
    /// <exception cref="AggregateException">Thrown if the task faults, containing the inner exception(s).</exception>
    /// <example>
    /// <code>
    /// Task initial = Task.CompletedTask;
    /// string next() => "Success";
    /// Task&lt;string&gt; result = initial.Then(next);
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    public static Task<T> Then<T>(this Task task, Func<T> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsCanceled) throw new TaskCanceledException();
            if (t.IsFaulted) throw t.Exception!;
            return next();
        });
    }

    /// <summary>
    /// Continues a <see cref="Task{T}"/> with a synchronous function, passing the task's result to the function if the task completes successfully.
    /// </summary>
    /// <typeparam name="T">The type of the task's result.</typeparam>
    /// <typeparam name="U">The type of the value returned by the continuation function.</typeparam>
    /// <param name="task">The task to continue.</param>
    /// <param name="next">The synchronous function to invoke with the task's result if the task completes successfully.</param>
    /// <returns>A task representing the result of the continuation function.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> or <paramref name="next"/> is null.</exception>
    /// <exception cref="TaskCanceledException">Thrown if the task is canceled.</exception>
    /// <exception cref="AggregateException">Thrown if the task faults, containing the inner exception(s).</exception>
    /// <example>
    /// <code>
    /// Task&lt;int&gt; initial = Task.FromResult(42);
    /// string next(int value) => $"Value: {value}";
    /// Task&lt;string&gt; result = initial.Then(next);
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    public static Task<U> Then<T, U>(this Task<T> task, Func<T, U> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsCanceled) throw new TaskCanceledException();
            if (t.IsFaulted) throw t.Exception!;
            return next(t.Result);
        });
    }
}
