using CSharper.AppContext;
using CSharper.Results;
using CSharper.Tests.Mediator;
using CSharper.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace CSharper.Mediator.Tests;

/// <summary>
/// Contains unit tests for the <see cref="LoggingBehavior"/> class.
/// </summary>
public sealed class LoggingBehaviorTests
{
    private readonly MemoryLogger<LoggingBehavior> _logger;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IAppContext> _appContextMock;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehaviorTests"/> class.
    /// </summary>
    public LoggingBehaviorTests()
    {
        _logger = new MemoryLogger<LoggingBehavior>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _appContextMock = new Mock<IAppContext>();
    }

    /// <summary>
    /// Tests that a request without app context logs with a generated correlation ID.
    /// </summary>
    [Fact]
    public async Task Handle_LogsRequest()
    {
        // Arrange
        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());

        LoggingBehavior sut = CreateLoggingBehavior();
        TestRequest test = new() { Id = "test-id", Value = 42 };

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry? requestLog = logEntries.FirstOrDefault(e => e.Message == "Processing request TestRequest");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);

        Dictionary<string, object> state = requestLog.State
            .Should().BeOfType<Dictionary<string, object>>().Subject;
        AssertRequestLogged(state);
    }

    /// <summary>
    /// Tests that a request without app context logs with a generated correlation ID.
    /// </summary>
    [Fact]
    public async Task Handle_WithoutAppContext_LogsRequestWithGeneratedCorrelationId()
    {
        // Arrange
        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());

        LoggingBehavior sut = CreateLoggingBehavior();
        TestRequest test = new() { Id = "test-id", Value = 42 };

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry? requestLog = logEntries.FirstOrDefault(e => e.Message == "Processing request TestRequest");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);

        Dictionary<string, object> state = requestLog.State
            .Should().BeOfType<Dictionary<string, object>>().Subject;

        AssertRequestLogged(state);
        
        state.Should().ContainKey("CorrelationId").WhoseValue.As<string>()
            .Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");

        LogEntry? successLog = logEntries.FirstOrDefault(e => e.Message == "Request TestRequest completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    /// <summary>
    /// Tests that a request with app context logs structured properties correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithAppContext_LogsRequestWithContext()
    {
        // Arrange
        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());

        LoggingBehavior sut = CreateLoggingBehavior(TestAppContext.GetDefaultAppContext());
        TestRequest test = new() { Id = "test-id", Value = 42 };

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry? requestLog = logEntries.FirstOrDefault(e => e.Message == "Processing request TestRequest");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);

        Dictionary<string, object> state = requestLog.State
            .Should().BeOfType<Dictionary<string, object>>().Subject;
        
        AssertRequestLogged(state);

        state.Should().ContainKey(nameof(IAppContext.RequestId)).WhoseValue.Should().Be(TestAppContext.DefaultRequestId);
        state.Should().ContainKey(nameof(IAppContext.CorrelationId)).WhoseValue.Should().Be(TestAppContext.DefaultCorrelationId);
        state.Should().ContainKey(nameof(IUserContext.UserId)).WhoseValue.Should().Be(TestAppContext.DefaultUserId);
        state.Should().ContainKey(nameof(IUserContext.TenantId)).WhoseValue.Should().Be(TestAppContext.DefaultTenantId);
        state.Should().ContainKey(nameof(IAppContext.ClientIpAddress)).WhoseValue.Should().Be(TestAppContext.DefaultClientIpAddress);
        state.Should().ContainKey(nameof(IAppContext.UserAgent)).WhoseValue.Should().Be(TestAppContext.DefaultUserAgent);
        state.Should().ContainKey(nameof(IAppContext.RequestPath)).WhoseValue.Should().Be(TestAppContext.DefaultRequestPath);

        foreach (KeyValuePair<string, object> extension in TestAppContext.DefaultExtensions)
        {
            string extensionKey = $"{LoggingBehavior._extensionPrefix}{extension.Key}";
            state.Should().ContainKey(extensionKey).WhoseValue.Should().Be(extension.Value);
        }

        LogEntry? successLog = logEntries.FirstOrDefault(e => e.Message == "Request TestRequest completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    /// <summary>
    /// Tests that a successful request logs the success message.
    /// </summary>
    [Fact]
    public async Task Handle_SuccessfulRequest_LogsSuccess()
    {
        // Arrange
        LoggingBehavior sut = CreateLoggingBehavior();
        TestRequest test = new() { Id = "test-id", Value = 42 };
        Mock<BehaviorDelegate> next = new();
        next.Setup(n => n(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok());

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry? successLog = logEntries
            .FirstOrDefault(e => e.Message == "Request TestRequest completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    /// <summary>
    /// Tests that a failed request logs errors correctly.
    /// </summary>
    [Fact]
    public async Task Handle_FailedRequest_LogsErrors()
    {
        // Arrange
        LoggingBehavior sut = CreateLoggingBehavior();
        TestRequest test = new() { Id = "test-id", Value = 42 };

        Error[] errors = [new Error("Error1"), new Error("Error2")];
        Mock<BehaviorDelegate> next = new();
        next.Setup(n => n(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(errors[0], errors[1]));

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry failureLog = logEntries[logEntries.Count - 1];
        failureLog.Should().NotBeNull();
        failureLog!.Level.Should().Be(LogLevel.Warning);
    }

    /// <summary>
    /// Tests that an exception with app context logs with the context's correlation ID.
    /// </summary>
    [Fact]
    public async Task Handle_ExceptionWithAppContext_LogsException()
    {
        // Arrange
        InvalidOperationException exception = new("Test error");
        Mock<BehaviorDelegate> next = SetupNextDelegate(exception);

        TestRequest test = new() { Id = "test-id", Value = 42 };
        LoggingBehavior sut = CreateLoggingBehavior(TestAppContext.GetDefaultAppContext());

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.Handle(test, next.Object, CancellationToken.None));

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(2);

        LogEntry? exceptionLog = logEntries[logEntries.Count - 1];
        exceptionLog.Should().NotBeNull();
        exceptionLog!.Level.Should().Be(LogLevel.Error);
        exceptionLog.Exception.Should().Be(exception);
    }

    [Fact]
    public async Task Handle_SerializationFailure_LogsWarningAndUsesToString()
    {
        // Arrange
        string requestType = nameof(UnserializableTestRequest);
        UnserializableTestRequest test = new()
        {
            Id = "test-id"
        };

        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());
        LoggingBehavior sut = CreateLoggingBehavior();

        // Act
        await sut.Handle(test, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(3); // Request start, warning, success

        LogEntry? warningLog = logEntries
            .FirstOrDefault(e =>
                e.Message == $"Failed to serialize request {nameof(UnserializableTestRequest)}. Using ToString instead.");
        warningLog.Should().NotBeNull();
        warningLog!.Level.Should().Be(LogLevel.Warning);
        warningLog.Exception.Should().NotBeNull();

        LogEntry? requestLog = logEntries
            .FirstOrDefault(e => e.Message == $"Processing request {requestType}");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);
        requestLog.State.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().ContainKey("Request").WhoseValue.As<string>()
            .Should().Be("TestRequestWithSerializationFailure{Id=test-id}");

        LogEntry? successLog = logEntries
            .FirstOrDefault(e => e.Message == $"Request {requestType} completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task Handle_LargeRequestJson_TruncatesAndLogsWarning()
    {
        // Arrange
        string largeString = new('A', 15_000);
        TestRequest request = new() { Id = "test-id", Value = 42, Description = largeString };
        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());
        LoggingBehavior sut = CreateLoggingBehavior();

        // Act
        await sut.Handle(request, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(3);

        LogEntry? warningLog = logEntries
            .FirstOrDefault(e => e.Message == "Request JSON truncated to 10000 characters.");
        warningLog.Should().NotBeNull();
        warningLog!.Level.Should().Be(LogLevel.Warning);

        LogEntry? requestLog = logEntries
            .FirstOrDefault(e => e.Message == "Processing request TestRequest");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);

        Dictionary<string, object> state = requestLog.State
            .Should().BeOfType<Dictionary<string, object>>().Subject;
        requestLog.State.Should().BeOfType<Dictionary<string, object>>()
            .Which.Should().ContainKey("Request").WhoseValue.As<string>()
            .Should().HaveLength(10_000);

        LogEntry? successLog = logEntries
            .FirstOrDefault(e => e.Message == "Request TestRequest completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task Handle_TooManyExtensions_LimitsAndLogsWarning()
    {
        // Arrange
        Dictionary<string, object> extensions = Enumerable.Range(1, 60)
            .ToDictionary(i => $"Key{i}", i => (object)$"Value{i}");

        Mock<BehaviorDelegate> next = SetupNextDelegate(Result.Ok());
        IAppContext appContext = TestAppContext.GetAppContext(extensions);
        LoggingBehavior sut = CreateLoggingBehavior(appContext);
        TestRequest request = new() { Id = "test-id", Value = 42 };

        // Act
        await sut.Handle(request, next.Object, CancellationToken.None);

        // Assert
        IReadOnlyList<LogEntry> logEntries = _logger.GetLogEntries();
        logEntries.Should().HaveCount(3); // Request start, extension warning, success

        LogEntry? warningLog = logEntries
            .FirstOrDefault(e => e.Message == "Extensions limited to 50 entries.");
        warningLog.Should().NotBeNull();
        warningLog!.Level.Should().Be(LogLevel.Warning);

        LogEntry? requestLog = logEntries
            .FirstOrDefault(e => e.Message == "Processing request TestRequest");
        requestLog.Should().NotBeNull();
        requestLog!.Level.Should().Be(LogLevel.Information);
        var state = requestLog.State.Should().BeOfType<Dictionary<string, object>>().Subject;
        int appContextAttributeCount = 9; // Properties from app context
        int maxExtensionCount = 50;

        // +1 for message format.
        state.Should().HaveCount(appContextAttributeCount + maxExtensionCount + 1);
        for (int i = 1; i <= 50; i++)
        {
            state.Should().ContainKey($"_Key{i}").WhoseValue.Should().Be($"Value{i}");
        }
        state.Should().NotContainKey("_Key51"); // Beyond limit

        var successLog = logEntries.FirstOrDefault(e => e.Message == "Request TestRequest completed successfully.");
        successLog.Should().NotBeNull();
        successLog!.Level.Should().Be(LogLevel.Information);
    }

    #region Helpers

    private static void AssertRequestLogged(Dictionary<string, object> state)
    {
        state.Should().ContainKey("RequestType").WhoseValue.Should().Be("TestRequest");
        state.Should().ContainKey("Request").WhoseValue.As<string>()
            .Should().Contain("\"Id\":\"test-id\"")
            .And.Contain("\"Value\":42")
            .And.Contain("\"Description\":null");
    }

    private LoggingBehavior CreateLoggingBehavior(IAppContext? appContext = null)
    {
        if (appContext != null)
        {
            // Configure service provider to return app context
            _serviceProviderMock.Setup(sp => sp
                .GetService(It.Is<Type>(t => t == typeof(IAppContext))))
                .Returns(appContext);
        }
        return new LoggingBehavior(_logger, _serviceProviderMock.Object);
    }

    private static Mock<BehaviorDelegate> SetupNextDelegate(Result result)
    {
        var next = new Mock<BehaviorDelegate>();
        next.Setup(n => n(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return next;
    }

    private static Mock<BehaviorDelegate> SetupNextDelegate(Exception exception)
    {
        var next = new Mock<BehaviorDelegate>();
        next.Setup(n => n(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);
        return next;
    }

    #endregion
}
