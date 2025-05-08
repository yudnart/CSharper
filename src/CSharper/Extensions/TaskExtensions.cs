using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Extensions;

public static class TaskExtensions
{
    [DebuggerStepThrough]
    public static FaultHandler<TTask> HandleFault<TTask>(this TTask task)
        where TTask : Task
    {
        return new FaultHandler<TTask>(task);
    }

    public readonly struct FaultHandler<TTask>
        where TTask : Task
    {
        private readonly TTask _task;

        public FaultHandler(TTask task)
        {
            _task = task;
        }

        [DebuggerStepThrough]
        public T Or<T>(Func<TTask, T> orElse)
        {
            if (_task.IsFaulted)
            {
                AggregateException ex = _task.Exception;
                throw ex.InnerExceptions.Count == 1 ? ex.InnerExceptions[0] : ex;
            }
            return orElse(_task);
        }
    }
}
