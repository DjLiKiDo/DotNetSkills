using DotNetSkills.Domain.UserManagement.Events;
using DotNetSkills.Infrastructure.Services.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace DotNetSkills.Infrastructure.UnitTests.Services.Events;

/// <summary>
/// Unit tests for DomainEventDispatcher.
/// Tests wrapper creation, dispatching, and error handling scenarios.
/// </summary>
public class DomainEventDispatcherTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<DomainEventDispatcher>> _loggerMock;
    private readonly DomainEventDispatcher _dispatcher;

    public DomainEventDispatcherTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<DomainEventDispatcher>>();
        _dispatcher = new DomainEventDispatcher(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task DispatchAsync_GenericMethod_WithValidEvent_ShouldCreateWrapperAndPublish()
    {
        // Arrange
        var domainEvent = new UserCreatedDomainEvent(
            new UserId(Guid.NewGuid()),
            new EmailAddress("test@example.com"),
            "Test User",
            UserRole.Developer,
            null);

        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _dispatcher.DispatchAsync(domainEvent);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DispatchAsync_InterfaceMethod_WithValidEvent_ShouldCreateWrapperAndPublish()
    {
        // Arrange
        IDomainEvent domainEvent = new UserCreatedDomainEvent(
            new UserId(Guid.NewGuid()),
            new EmailAddress("test@example.com"),
            "Test User",
            UserRole.Developer,
            null);

        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _dispatcher.DispatchAsync(domainEvent);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DispatchManyAsync_WithMultipleValidEvents_ShouldDispatchAllSuccessfully()
    {
        // Arrange
        var events = new List<IDomainEvent>
        {
            new UserCreatedDomainEvent(
                new UserId(Guid.NewGuid()),
                new EmailAddress("test1@example.com"),
                "Test User 1",
                UserRole.Developer,
                null),
            new UserCreatedDomainEvent(
                new UserId(Guid.NewGuid()),
                new EmailAddress("test2@example.com"),
                "Test User 2",
                UserRole.ProjectManager,
                null)
        };

        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _dispatcher.DispatchManyAsync(events);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task DispatchManyAsync_WithEmptyEventsList_ShouldNotCallMediator()
    {
        // Arrange
        var events = new List<IDomainEvent>();

        // Act
        await _dispatcher.DispatchManyAsync(events);

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DispatchManyAsync_WithMediatorFailure_ShouldContinueProcessingAndThrowAggregateException()
    {
        // Arrange
        var events = new List<IDomainEvent>
        {
            new UserCreatedDomainEvent(
                new UserId(Guid.NewGuid()),
                new EmailAddress("test1@example.com"),
                "Test User 1",
                UserRole.Developer,
                null)
        };

        var exception = new InvalidOperationException("Mediator failure");
        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var aggregateException = await Assert.ThrowsAsync<AggregateException>(
            () => _dispatcher.DispatchManyAsync(events));

        aggregateException.InnerExceptions.Should().HaveCount(1);
        aggregateException.InnerExceptions.Should().AllBeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task DispatchAsync_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _dispatcher.DispatchAsync<UserCreatedDomainEvent>(null!));
    }

    [Fact]
    public async Task DispatchAsync_Interface_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _dispatcher.DispatchAsync((IDomainEvent)null!));
    }

    [Fact]
    public async Task DispatchManyAsync_WithNullEventsList_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _dispatcher.DispatchManyAsync(null!));
    }

    [Fact]
    public async Task DispatchManyAsync_WithPublishFailure_ShouldLogErrorsAndThrowAggregateException()
    {
        // Arrange
        var events = new List<IDomainEvent>
        {
            new UserCreatedDomainEvent(
                new UserId(Guid.NewGuid()),
                new EmailAddress("test1@example.com"),
                "Test User 1",
                UserRole.Developer,
                null),
            new UserCreatedDomainEvent(
                new UserId(Guid.NewGuid()),
                new EmailAddress("test2@example.com"),
                "Test User 2",
                UserRole.ProjectManager,
                null)
        };

        var publishException = new InvalidOperationException("Publish failure");
        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(publishException);

        // Act & Assert
        var aggregateException = await Assert.ThrowsAsync<AggregateException>(
            () => _dispatcher.DispatchManyAsync(events));

        // Verify that errors were logged for both events
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed to dispatch domain event")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(2));

        aggregateException.InnerExceptions.Should().HaveCount(2);
        aggregateException.InnerExceptions.Should().AllBeOfType<InvalidOperationException>();
    }
}