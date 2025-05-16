using System;
using System.Collections.Generic;

namespace CSharper.Extensions.Internal;

/// <summary>
/// Provides internal extension methods for <see cref="Queue{T}"/> to support additional dequeuing functionality.
/// </summary>
internal static class QueueExtensions
{
    /// <summary>
    /// Attempts to dequeue an item from the queue, returning a boolean indicating success.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    /// <param name="queue">The queue to dequeue from.</param>
    /// <param name="result">When this method returns, contains the dequeued item if successful,
    /// or the default value for <typeparamref name="T"/> if the queue is empty.</param>
    /// <returns><c>true</c> if an item was dequeued; otherwise, <c>false</c> if the queue is empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="queue"/> is null.</exception>
    /// <remarks>
    /// This method provides compatibility for .NET Standard 2.0, where the native
    /// <see cref="T:Queue{T}.TryDequeue"/> method is unavailable. In .NET 8.0 and later,
    /// it delegates to the native method for optimal performance. Used internally,
    /// for example, in event processing. This method is not thread-safe; synchronize
    /// access for concurrent scenarios.
    /// </remarks>
    public static bool TryDequeueCommon<T>(this Queue<T> queue, out T? result)
    {
#if NET8_0_OR_GREATER
        return queue.TryDequeue(out result);
#else
        queue.ThrowIfNull(nameof(queue));

        if (queue.Count == 0)
        {
            result = default;
            return false;
        }

        result = queue.Dequeue();
        return true;
#endif
    }
}