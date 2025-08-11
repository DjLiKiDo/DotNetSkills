using DotNetSkills.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetSkills.API.UnitTests.Middleware;

public class CorrelationIdMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<CorrelationIdMiddleware>> _loggerMock;
    private readonly CorrelationIdMiddleware _middleware;

    public CorrelationIdMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<CorrelationIdMiddleware>>();
        _middleware = new CorrelationIdMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenCorrelationIdProvidedInRequest_ShouldUseProvidedId()
    {
        // Arrange
        const string providedCorrelationId = "test-correlation-123";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName] = providedCorrelationId;

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString().Should().Be(providedCorrelationId);
        httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey].Should().Be(providedCorrelationId);
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        var responseCorrelationId = httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString();
        var contextCorrelationId = httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey]?.ToString();

        responseCorrelationId.Should().NotBeNullOrWhiteSpace();
        responseCorrelationId.Should().Be(contextCorrelationId);
        Guid.TryParse(responseCorrelationId, out _).Should().BeTrue("Generated correlation ID should be a valid GUID");
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenEmptyCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName] = "";

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        var responseCorrelationId = httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString();
        var contextCorrelationId = httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey]?.ToString();

        responseCorrelationId.Should().NotBeNullOrWhiteSpace();
        responseCorrelationId.Should().Be(contextCorrelationId);
        Guid.TryParse(responseCorrelationId, out _).Should().BeTrue("Generated correlation ID should be a valid GUID");
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenWhitespaceCorrelationIdProvided_ShouldGenerateNewId()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName] = "   ";

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        var responseCorrelationId = httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString();
        var contextCorrelationId = httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey]?.ToString();

        responseCorrelationId.Should().NotBeNullOrWhiteSpace();
        responseCorrelationId.Should().Be(contextCorrelationId);
        Guid.TryParse(responseCorrelationId, out _).Should().BeTrue("Generated correlation ID should be a valid GUID");
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenMultipleCorrelationIdsProvided_ShouldUseFirstOne()
    {
        // Arrange
        const string firstCorrelationId = "first-correlation-123";
        const string secondCorrelationId = "second-correlation-456";
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName] = new[] { firstCorrelationId, secondCorrelationId };

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString().Should().Be(firstCorrelationId);
        httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey].Should().Be(firstCorrelationId);
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Always_ShouldCallNextMiddleware()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        const string correlationId = "test-correlation-id";
        httpContext.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName] = correlationId;

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        _nextMock.Verify(next => next(httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_Always_ShouldSetResponseHeader()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Response.Headers.Should().ContainKey(CorrelationIdMiddleware.CorrelationIdHeaderName);
        httpContext.Response.Headers[CorrelationIdMiddleware.CorrelationIdHeaderName].ToString().Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task InvokeAsync_Always_ShouldSetContextItem()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _middleware.InvokeAsync(httpContext);

        // Assert
        httpContext.Items.Should().ContainKey(CorrelationIdMiddleware.CorrelationIdKey);
        httpContext.Items[CorrelationIdMiddleware.CorrelationIdKey].Should().NotBeNull();
    }
}
