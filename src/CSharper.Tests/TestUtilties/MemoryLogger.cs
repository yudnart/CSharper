using Microsoft.Extensions.Logging;

namespace CSharper.Tests.TestHelpers;

/// <summary>
/// A memory-based logger that captures log entries and scope state for testing.
/// </summary>
/// <typeparam name="T">The type of the logger category.</typeparam>
internal sealed class MemoryLogger<T> : ILogger<T>
{
    private readonly List<LogEntry> _logEntries = [];
    private readonly Stack<Dictionary<string, object>> _scopes = new();

    /// <summary>
    /// Gets all captured log entries.
    /// </summary>
    public IReadOnlyList<LogEntry> GetLogEntries() => _logEntries.AsReadOnly();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        Dictionary<string, object>? scopeDict = state as Dictionary<string, object>;
        if (scopeDict == null && state is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            scopeDict = kvps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        scopeDict = scopeDict ?? [];

        _scopes.Push(scopeDict);
        return new ScopeDisposable(_scopes);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        string message = formatter(state, exception);
        Dictionary<string, object>? stateDict = state as Dictionary<string, object>;
        if (stateDict == null && state is IEnumerable<KeyValuePair<string, object>> kvps)
        {
            stateDict = kvps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        stateDict = stateDict ?? [];

        // Merge scope states (most recent first)
        Dictionary<string, object> mergedState = [];
        foreach (Dictionary<string, object>? scope in _scopes.Reverse())
        {
            foreach (KeyValuePair<string, object> kvp in scope)
            {
                mergedState[kvp.Key] = kvp.Value;
            }
        }
        // Message state takes precedence
        foreach (KeyValuePair<string, object> kvp in stateDict)
        {
            mergedState[kvp.Key] = kvp.Value;
        }

        _logEntries.Add(new LogEntry(logLevel, message, mergedState, exception));
    }

    private class ScopeDisposable : IDisposable
    {
        private readonly Stack<Dictionary<string, object>> _scopes;

        public ScopeDisposable(Stack<Dictionary<string, object>> scopes)
        {
            _scopes = scopes;
        }

        public void Dispose()
        {
            if (_scopes.Count > 0)
            {
                _scopes.Pop();
            }
        }
    }
}

/// <summary>
/// Represents a captured log entry.
/// </summary>
internal sealed class LogEntry
{
    public LogLevel Level { get; }
    public string Message { get; }
    public Dictionary<string, object> State { get; }
    public Exception? Exception { get; }

    public LogEntry(LogLevel level, string message, Dictionary<string, object> state, Exception? exception)
    {
        Level = level;
        Message = message;
        State = state;
        Exception = exception;
    }
}