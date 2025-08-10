using DotNetSkills.Application.Common.Events;

namespace DotNetSkills.Application.TaskExecution.EventHandlers;

/// <summary>
/// Domain event handler for TaskAssignedDomainEvent.
/// Demonstrates how domain events are processed after successful command execution.
/// In a real application, this might send notifications, update read models, or trigger integrations.
/// </summary>
public class TaskAssignedDomainEventHandler : INotificationHandler<DomainEventNotification<TaskAssignedDomainEvent>>
{
    private readonly ILogger<TaskAssignedDomainEventHandler> _logger;

    public TaskAssignedDomainEventHandler(ILogger<TaskAssignedDomainEventHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the TaskAssignedDomainEvent.
    /// This method is called automatically by MediatR when the event is dispatched.
    /// </summary>
    /// <param name="notification">The domain event notification wrapper.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(DomainEventNotification<TaskAssignedDomainEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        
        _logger.LogInformation(
            "Processing TaskAssignedDomainEvent: Task {TaskId} assigned to user {AssigneeId} by user {AssignedBy} at {OccurredAt} (CorrelationId: {CorrelationId})",
            domainEvent.TaskId,
            domainEvent.AssigneeId,
            domainEvent.AssignedBy,
            domainEvent.OccurredAt,
            domainEvent.CorrelationId);

        try
        {
            // Simulate processing time
            await Task.Delay(100, cancellationToken);

            // In a real application, you might:
            // 1. Send notification emails or push notifications
            // 2. Update read models or projections
            // 3. Trigger integrations with external systems
            // 4. Update analytics or reporting data
            // 5. Create audit log entries

            _logger.LogInformation(
                "Successfully processed TaskAssignedDomainEvent for Task {TaskId}",
                domainEvent.TaskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process TaskAssignedDomainEvent for Task {TaskId}",
                domainEvent.TaskId);
            
            // In production, you might want to:
            // - Implement retry logic
            // - Send events to a dead letter queue
            // - Create compensating actions
            // For this example, we'll re-throw to demonstrate error handling
            throw;
        }
    }
}
