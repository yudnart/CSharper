using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
    /// string Next() => "Success";
    /// Task&lt;string&gt; result = initial.Then(Next);
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    public static Task<T> Then<T>(this Task task, Func<T> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                CancelTask(t);
            }
            if (t.IsFaulted)
            {
                ThrowTaskException(t);
            }
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
    /// <remarks>
    /// This method is internal and intended for use within the CSharper library.
    /// </remarks>
    /// <example>
    /// <code>
    /// Task&lt;int&gt; initial = Task.FromResult(42);
    /// string Next(int value) => $"Value: {value}";
    /// Task&lt;string&gt; result = initial.Then(Next);
    /// </code>
    /// </example>
    [DebuggerStepThrough]
    internal static Task<U> Then<T, U>(this Task<T> task, Func<T, U> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsCanceled)
            {
                CancelTask(t);
            }
            if (t.IsFaulted)
            {
                ThrowTaskException(t);
            }
            return next(t.Result);
        });
    }

    /// <summary>
    /// Throws a <see cref="TaskCanceledException"/> for a canceled task, including task details.
    /// </summary>
    /// <param name="t">The canceled task.</param>
    /// <exception cref="TaskCanceledException">Always thrown with details about the canceled task.</exception>
    private static void CancelTask(Task t)
    {
        throw new TaskCanceledException(
            $@"Task canceled. TaskID={t.Id}, Status={t.Status}.");
    }

    /// <summary>
    /// Throws an exception for a faulted task, extracting the inner exception if singular or the <see cref="AggregateException"/> if multiple.
    /// </summary>
    /// <param name="task">The faulted task.</param>
    /// <exception cref="AggregateException">Thrown if the task has multiple inner exceptions.</exception>
    /// <exception cref="Exception">Thrown if the task has a single inner exception.</exception>
    /// <exception cref="ApplicationException">Thrown if the task's exception is null.</exception>
    [ExcludeFromCodeCoverage
#if NET8_0_OR_GREATER
        (Justification = "Defensive code unreachable.")
#endif
    ]
    private static void ThrowTaskException(Task task)
    {
        AggregateException ex = task.Exception
            ?? throw new ApplicationException("Task failed without providing an exception.");
        throw ex.InnerExceptions.Count == 1
            ? ex.InnerExceptions[0] : ex;
    }
}