using DotNetSkills.Application.Common.Events;
using DotNetSkills.Application.TaskExecution.EventHandlers;
using DotNetSkills.Domain.TaskExecution.Events;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetSkills.Application.UnitTests.TaskExecution.EventHandlers;

/// <summary>
/// Unit tests for TaskAssignedDomainEventHandler.
/// Tests the processing of TaskAssignedDomainEvent notifications.
/// </summary>
public class TaskAssignedDomainEventHandlerTests
{
    private readonly Mock<ILogger<TaskAssignedDomainEventHandler>> _loggerMock;
    private readonly TaskAssignedDomainEventHandler _handler;

    public TaskAssignedDomainEventHandlerTests()
    {
        _loggerMock = new Mock<ILogger<TaskAssignedDomainEventHandler>>();
        _handler = new TaskAssignedDomainEventHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidEvent_ShouldProcessSuccessfully()
    {
        // Arrange
        var taskId = TaskId.New();
        var assigneeId = UserId.New();
        var assignedBy = UserId.New();
        
        var domainEvent = new TaskAssignedDomainEvent(taskId, assigneeId, assignedBy);
        var notification = new DomainEventNotification<TaskAssignedDomainEvent>(domainEvent);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        // Verify that the handler completed without throwing
        // In a real scenario, you might verify that specific actions were taken
        // such as notifications sent, read models updated, etc.
        
        // Verify logging calls were made
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Processing TaskAssignedDomainEvent")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Successfully processed TaskAssignedDomainEvent")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var taskId = TaskId.New();
        var assigneeId = UserId.New();
        var assignedBy = UserId.New();
        
        var domainEvent = new TaskAssignedDomainEvent(taskId, assigneeId, assignedBy);
        var notification = new DomainEventNotification<TaskAssignedDomainEvent>(domainEvent);
        
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            () => _handler.Handle(notification, cts.Token));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new TaskAssignedDomainEventHandler(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("logger");
    }
}
