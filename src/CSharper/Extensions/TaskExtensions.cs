using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CSharper.Extensions;

public static class TaskExtensions
{
    [DebuggerStepThrough]
    internal static Task<T> Then<T>(this Task task, Func<T> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                ThrowTaskException(t);
            }
            return next();
        });
    }

    [DebuggerStepThrough]
    internal static Task<U> Then<T, U>(this Task<T> task, Func<T, U> next)
    {
        task.ThrowIfNull(nameof(task));
        next.ThrowIfNull(nameof(next));
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                ThrowTaskException(t);
            }
            return next(t.Result);
        });
    }

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
