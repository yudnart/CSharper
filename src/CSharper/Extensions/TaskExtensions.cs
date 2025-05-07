using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CSharper.Utilities;

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
                AggregateException ex = _task.Exception
                    ?? throw new InvalidOperationException("Result task threw an unspecified exception.");
                throw ex.InnerExceptions.Count > 0 ? ex.InnerExceptions[0] : ex;
            }
            return orElse(_task);
        }
    }
}
