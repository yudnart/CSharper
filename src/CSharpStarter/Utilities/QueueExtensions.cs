using System;
using System.Collections.Generic;

namespace CSharpStarter.Utilities;

/// <summary>
/// Provides internal extension methods for <see cref="Queue{T}"/> to support additional functionality
/// in .NET Standard 2.0 and compatible frameworks.
/// </summary>
public static class QueueExtensions
{
    /// <summary>
    /// Attempts to dequeue an item from the queue, returning a boolean indicating success.
    /// </summary>
    /// <typeparam name="T">The type of elements in the queue.</typeparam>
    /// <param name="queue">The queue to dequeue from.</param>
    /// <param name="result">When this method returns, contains the dequeued item if successful,
    /// or the default value for <typeparamref name="T"/> if the queue is empty.</param>
    /// <returns><c>true</c> if an item was dequeued; otherwise, <c>false</c> if the queue was empty.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="queue"/> is null.</exception>
    /// <remarks>
    /// This method is necessary for .NET Standard 2.0 compatibility, where <see cref="Queue{T}"/>
    /// does not provide a native <c>TryDequeue</c> method. It enables safe dequeuing without
    /// exceptions for empty queues and is used internally. This method is not
    /// thread-safe; use synchronization for concurrent access.
    /// </remarks>
    public static bool TryDequeue<T>(this Queue<T> queue, out T? result)
    {
        if (queue == null)
        {
            throw new ArgumentNullException(nameof(queue));
        }

        if (queue.Count == 0)
        {
            result = default;
            return false;
        }

        result = queue.Dequeue();
        return true;
    }
}