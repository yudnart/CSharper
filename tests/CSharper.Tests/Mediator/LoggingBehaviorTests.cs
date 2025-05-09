using CSharper.AppContext;
using CSharper.Errors;
using CSharper.Mediator;
using CSharper.Results;
using CSharper.Tests.TestUtilities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace CSharper.Tests.Mediator;

[Trait("Category", "Unit")]
[Trait("TestOf", nameof(LoggingBehavior))]
public sealed class LoggingBehaviorTests
{
    private readonly MemoryLogger<LoggingBehavior> _logger;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IAppContext> _appContextMock;
    private const string _errorStateKey = "Error";
    private const string _requestTypeStateKey = "RequestType";
    private const string _requestStateKey = "Request";

    public LoggingBehaviorTests()
    {
        _logger = new MemoryLogger<LoggingBehavior>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _appContextMock = new Mock<IAppContext>();
    }

    [Fact]
    public void Ctor_ValidParams_Succeeds()
    {
        // Act
        LoggingBehavior sut = new(_logger, _serviceProviderMock.Object);

        // Assert
        sut.Should().NotBeNull();
    }

    [Fact]
    public void Ctor_NullLogger_Throws()
    {
        // Arrange
        ILogger<LoggingBehavior> nullLogger = null!;
        Action act = () => _ = new LoggingBehavior(nullLogger, _serviceProviderMock.Object);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>()
            .And.ParamName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_SuccessfulRequest_LogsStartAndSuccess()
    {
        // Arrange
        LoggingBehavior sut = new(_logger, _serviceProviderMock.Object);

        IRequest request = TestRequest.Instance;
        Result nextResult = Result.Ok();
        Task<Result> next(IRequest r, CancellationToken c) => Task.FromResult(nextResult);

        string requestType = request.GetType().Name;
        string requestData = JsonSerializer.Serialize(request, typeof(TestRequest));

        // Act
        Result result = await sut.Handle(request, next, CancellationToken.None);
        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(nextResult);

            logs.Should().HaveCount(2);

            logs[0].Level.Should().Be(LogLevel.Information);
            logs[0].State[_requestTypeStateKey].Should().Be(requestType);
            logs[0].State[_requestStateKey].Should().Be(requestData);

            logs[1].Level.Should().Be(LogLevel.Information);
            logs[1].State[_requestTypeStateKey].Should().Be(requestType);
        });
    }

    [Fact]
    public async Task Handle_RequestWithError_LogsStartAndFailure()
    {
        // Arrange
        LoggingBehavior sut = new(_logger, _serviceProviderMock.Object);

        IRequest request = TestRequest.Instance;
        string errorMessage = "Test error";
        Error error = new(errorMessage);
        Result nextResult = Result.Fail(error);
        Task<Result> next(IRequest r, CancellationToken c) => Task.FromResult(nextResult);

        string requestType = request.GetType().Name;
        string requestData = JsonSerializer.Serialize(request, typeof(TestRequest));

        // Act
        Result result = await sut.Handle(request, next, CancellationToken.None);
        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();

        // Assert
        Assert.Multiple(() =>
        {
            result.Should().Be(nextResult);

            logs.Should().HaveCount(2);

            logs[0].Level.Should().Be(LogLevel.Information);
            logs[0].State[_requestTypeStateKey].Should().Be(requestType);
            logs[0].State[_requestStateKey].Should().Be(requestData);

            logs[1].Level.Should().Be(LogLevel.Warning);
            logs[1].Message.Should().Contain(errorMessage);
            logs[1].State[_requestTypeStateKey].Should().Be(requestType);
            logs[1].State[_errorStateKey].Should().Be(error);
        });
    }

    [Fact]
    public void Handle_RequestThrowsException_LogsExceptionAndRethrows()
    {
        // Arrange
        LoggingBehavior sut = new(_logger, _serviceProviderMock.Object);

        IRequest request = TestRequest.Instance;
        string exceptionMessage = "Test exception";
        InvalidOperationException exception = new(exceptionMessage);
        Task<Result> next(IRequest r, CancellationToken c) => throw exception;

        string requestType = request.GetType().Name;
        string requestData = JsonSerializer.Serialize(request, typeof(TestRequest));

        // Act
        Func<Task> act = async () => await sut.Handle(request, next, CancellationToken.None);
        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();

        // Assert
        Assert.Multiple(async () =>
        {
            await act.Should()
                .ThrowExactlyAsync<InvalidOperationException>()
                .WithMessage(exceptionMessage);

            logs.Should().HaveCount(2);
            
            logs[0].Level.Should().Be(LogLevel.Information);
            logs[0].State[_requestTypeStateKey].Should().Be(requestType);
            logs[0].State[_requestStateKey].Should().Be(requestData);

            logs[1].Level.Should().Be(LogLevel.Error);
            logs[1].Exception.Should().Be(exception);
        });
    }

    [Fact]
    public async Task Handle_SerializationFails_LogsWarningAndUsesToString()
    {
        // Arrange;
        Result result = Result.Ok();
        BehaviorDelegate next = (_, _) => Task.FromResult(result);
        LoggingBehavior behavior = new(_logger, _serviceProviderMock.Object);

        IRequest request = UnserializableTestRequest.Instance;
        string requestData = request.ToString()!;

        // Act
        Result outcome = await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        Assert.Multiple(() =>
        {
            outcome.Should().Be(result);
            IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
            logs.Should().HaveCount(3);
            logs[0].Level.Should().Be(LogLevel.Warning);
            logs[0].Message.Should().NotBeNullOrWhiteSpace();
            logs[0].Exception.Should().BeOfType<JsonException>();
            logs[1].Level.Should().Be(LogLevel.Information);
            logs[1].Message.Should().NotBeNullOrWhiteSpace();
            logs[1].State.Should().ContainKey(_requestStateKey).WhoseValue.Should().Be(requestData);
            logs[2].Level.Should().Be(LogLevel.Information);
            logs[2].Message.Should().NotBeNullOrWhiteSpace();
        });
    }

    //[Fact]
    //public async Task Handle_LargeRequestJson_TruncatesAndLogsWarning()
    //{
    //    // Arrange
    //    TestRequest largeRequest = new TestRequest { Data = new string('A', LoggingBehavior._maxRequestJsonLength + 10) };
    //    _requestMock.Setup(r => r.GetType()).Returns(largeRequest.GetType());
    //    Result result = Result.Ok();
    //    BehaviorDelegate next = (_, _) => Task.FromResult(result);
    //    LoggingBehavior behavior = new LoggingBehavior(_logger, _serviceProviderMock.Object);
    //    string requestTruncated = LoggingBehavior._requestTruncated;
    //    string requestProcessing = LoggingBehavior._requestProcessing;
    //    string requestCompleted = LoggingBehavior._requestCompleted;

    //    // Act
    //    Result outcome = await behavior.Handle(_testRequest, next, CancellationToken.None);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        outcome.Should().Be(result);
    //        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
    //        logs.Should().HaveCount(3);
    //        logs[0].Level.Should().Be(LogLevel.Warning);
    //        logs[0].Message.Should().Contain(requestTruncated);
    //        logs[1].Level.Should().Be(LogLevel.Information);
    //        logs[1].Message.Should().Contain(requestProcessing);
    //        logs[1].State.Should().ContainKey(_requestKey).WhoseValue.As<string>().Length.Should().Be(LoggingBehavior._maxRequestJsonLength);
    //        logs[2].Level.Should().Be(LogLevel.Information);
    //        logs[2].Message.Should().Contain(requestCompleted);
    //    });
    //}

    //[Fact]
    //public async Task Handle_WithAppContext_LogsContextProperties()
    //{
    //    // Arrange
    //    string requestId = "req123";
    //    string correlationId = "corr456";
    //    string userId = "user789";
    //    string tenantId = "tenant101";
    //    string clientIpAddress = "192.168.1.1";
    //    string userAgent = "Mozilla/5.0";
    //    string requestPath = "/api/test";
    //    string ext1Key = "_Ext1";
    //    string ext1Value = "Value1";
    //    string ext2Key = "_Ext2";
    //    int ext2Value = 42;
    //    _appContextMock.Setup(a => a.RequestId).Returns(requestId);
    //    _appContextMock.Setup(a => a.CorrelationId).Returns(correlationId);
    //    _appContextMock.Setup(a => a.CurrentUser).Returns(new UserContext(userId, tenantId));
    //    _appContextMock.Setup(a => a.ClientIpAddress).Returns(clientIpAddress);
    //    _appContextMock.Setup(a => a.UserAgent).Returns(userAgent);
    //    _appContextMock.Setup(a => a.RequestPath).Returns(requestPath);
    //    _appContextMock.Setup(a => a.Extensions).Returns(new Dictionary<string, object>
    //    {
    //        { "Ext1", ext1Value },
    //        { "Ext2", ext2Value }
    //    });
    //    _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAppContext))).Returns(_appContextMock.Object);
    //    Result result = Result.Ok();
    //    BehaviorDelegate next = (_, _) => Task.FromResult(result);
    //    LoggingBehavior behavior = new LoggingBehavior(_logger, _serviceProviderMock.Object);
    //    string requestProcessing = LoggingBehavior._requestProcessing;
    //    string requestCompleted = LoggingBehavior._requestCompleted;

    //    // Act
    //    Result outcome = await behavior.Handle(_testRequest, next, CancellationToken.None);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        outcome.Should().Be(result);
    //        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
    //        logs.Should().HaveCount(2);
    //        logs[0].Level.Should().Be(LogLevel.Information);
    //        logs[0].Message.Should().Contain(requestProcessing);
    //        logs[0].State.Should().ContainKeys(_requestTypeKey, nameof(IAppContext.RequestId), nameof(IAppContext.CorrelationId), nameof(IAppContext.CurrentUser.UserId), nameof(IAppContext.CurrentUser.TenantId), nameof(IAppContext.ClientIpAddress), nameof(IAppContext.UserAgent), nameof(IAppContext.RequestPath), ext1Key, ext2Key);
    //        logs[0].State[nameof(IAppContext.RequestId)].Should().Be(requestId);
    //        logs[0].State[nameof(IAppContext.CorrelationId)].Should().Be(correlationId);
    //        logs[0].State[nameof(IAppContext.CurrentUser.UserId)].Should().Be(userId);
    //        logs[0].State[nameof(IAppContext.CurrentUser.TenantId)].Should().Be(tenantId);
    //        logs[0].State[nameof(IAppContext.ClientIpAddress)].Should().Be(clientIpAddress);
    //        logs[0].State[nameof(IAppContext.UserAgent)].Should().Be(userAgent);
    //        logs[0].State[nameof(IAppContext.RequestPath)].Should().Be(requestPath);
    //        logs[0].State[ext1Key].Should().Be(ext1Value);
    //        logs[0].State[ext2Key].Should().Be(ext2Value);
    //        logs[1].Level.Should().Be(LogLevel.Information);
    //        logs[1].Message.Should().Contain(requestCompleted);
    //    });
    //}

    //[Fact]
    //public async Task Handle_TooManyExtensions_LogsWarningAndLimits()
    //{
    //    // Arrange
    //    Dictionary<string, object> extensions = new Dictionary<string, object>();
    //    for (int i = 0; i < LoggingBehavior._maxExtensions + 10; i++)
    //    {
    //        extensions[$"Ext{i}"] = i;
    //    }
    //    _appContextMock.Setup(a => a.Extensions).Returns(extensions);
    //    _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAppContext))).Returns(_appContextMock.Object);
    //    Result result = Result.Ok();
    //    BehaviorDelegate next = (_, _) => Task.FromResult(result);
    //    LoggingBehavior behavior = new LoggingBehavior(_logger, _serviceProviderMock.Object);
    //    string extensionsLimited = LoggingBehavior._extensionsLimited;
    //    string requestProcessing = LoggingBehavior._requestProcessing;
    //    string requestCompleted = LoggingBehavior._requestCompleted;
    //    string lastValidExtensionKey = $"_Ext{LoggingBehavior._maxExtensions - 1}";
    //    string firstInvalidExtensionKey = $"_Ext{LoggingBehavior._maxExtensions}";

    //    // Act
    //    Result outcome = await behavior.Handle(_testRequest, next, CancellationToken.None);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        outcome.Should().Be(result);
    //        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
    //        logs.Should().HaveCount(3);
    //        logs[0].Level.Should().Be(LogLevel.Warning);
    //        logs[0].Message.Should().Contain(extensionsLimited);
    //        logs[1].Level.Should().Be(LogLevel.Information);
    //        logs[1].Message.Should().Contain(requestProcessing);
    //        logs[1].State.Should().ContainKey(lastValidExtensionKey).WhoseValue.Should().Be(LoggingBehavior._maxExtensions - 1);
    //        logs[1].State.Should().NotContainKey(firstInvalidExtensionKey);
    //        logs[2].Level.Should().Be(LogLevel.Information);
    //        logs[2].Message.Should().Contain(requestCompleted);
    //    });
    //}

    //[Fact]
    //public async Task Handle_NoAppContext_LogsGeneratedCorrelationId()
    //{
    //    // Arrange
    //    _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAppContext))).Returns(null);
    //    Result result = Result.Ok();
    //    BehaviorDelegate next = (_, _) => Task.FromResult(result);
    //    LoggingBehavior behavior = new LoggingBehavior(_logger, _serviceProviderMock.Object);
    //    string requestProcessing = LoggingBehavior._requestProcessing;
    //    string requestCompleted = LoggingBehavior._requestCompleted;

    //    // Act
    //    Result outcome = await behavior.Handle(_testRequest, next, CancellationToken.None);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        outcome.Should().Be(result);
    //        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
    //        logs.Should().HaveCount(2);
    //        logs[0].Level.Should().Be(LogLevel.Information);
    //        logs[0].Message.Should().Contain(requestProcessing);
    //        logs[0].State.Should().ContainKey(nameof(IAppContext.CorrelationId)).WhoseValue.As<string>().Should().Match(s => Guid.TryParse(s, out _));
    //        logs[0].State.Should().NotContainKeys(nameof(IAppContext.RequestId), nameof(IAppContext.CurrentUser.UserId), nameof(IAppContext.ClientIpAddress), nameof(IAppContext.UserAgent), nameof(IAppContext.RequestPath));
    //        logs[1].Level.Should().Be(LogLevel.Information);
    //        logs[1].Message.Should().Contain(requestCompleted);
    //    });
    //}

    //[Fact]
    //public async Task Handle_NullAppContextProperties_LogsNotApplicable()
    //{
    //    // Arrange
    //    string correlationId = "corr456";
    //    string notApplicable = LoggingBehavior._notApplicable;
    //    string anonymousUserId = LoggingBehavior._anonymousUserId;
    //    _appContextMock.Setup(a => a.RequestId).Returns((string)null);
    //    _appContextMock.Setup(a => a.CorrelationId).Returns(correlationId);
    //    _appContextMock.Setup(a => a.CurrentUser).Returns((UserContext)null);
    //    _appContextMock.Setup(a => a.ClientIpAddress).Returns((string)null);
    //    _appContextMock.Setup(a => a.UserAgent).Returns((string)null);
    //    _appContextMock.Setup(a => a.RequestPath).Returns((string)null);
    //    _serviceProviderMock.Setup(sp => sp.GetService(typeof(IAppContext))).Returns(_appContextMock.Object);
    //    Result result = Result.Ok();
    //    BehaviorDelegate next = (_, _) => Task.FromResult(result);
    //    LoggingBehavior behavior = new LoggingBehavior(_logger, _serviceProviderMock.Object);
    //    string requestProcessing = LoggingBehavior._requestProcessing;
    //    string requestCompleted = LoggingBehavior._requestCompleted;

    //    // Act
    //    Result outcome = await behavior.Handle(_testRequest, next, CancellationToken.None);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        outcome.Should().Be(result);
    //        IReadOnlyList<LogEntry> logs = _logger.GetLogEntries();
    //        logs.Should().HaveCount(2);
    //        logs[0].Level.Should().Be(LogLevel.Information);
    //        logs[0].Message.Should().Contain(requestProcessing);
    //        logs[0].State.Should().ContainKeys(_requestTypeKey, nameof(IAppContext.RequestId), nameof(IAppContext.CorrelationId), nameof(IAppContext.CurrentUser.UserId), nameof(IAppContext.ClientIpAddress), nameof(IAppContext.UserAgent), nameof(IAppContext.RequestPath));
    //        logs[0].State[nameof(IAppContext.RequestId)].Should().Be(notApplicable);
    //        logs[0].State[nameof(IAppContext.CorrelationId)].Should().Be(correlationId);
    //        logs[0].State[nameof(IAppContext.CurrentUser.UserId)].Should().Be(anonymousUserId);
    //        logs[0].State[nameof(IAppContext.ClientIpAddress)].Should().Be(notApplicable);
    //        logs[0].State[nameof(IAppContext.UserAgent)].Should().Be(notApplicable);
    //        logs[0].State[nameof(IAppContext.RequestPath)].Should().Be(notApplicable);
    //        logs[1].Level.Should().Be(LogLevel.Information);
    //        logs[1].Message.Should().Contain(requestCompleted);
    //    });
    //}
}