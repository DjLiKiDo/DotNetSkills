namespace DotNetSkills.Domain.TeamCollaboration.Events;

/// <summary>
/// Domain event that is raised when a team's status changes.
/// </summary>
/// <param name="TeamId">The unique identifier of the team whose status changed.</param>
/// <param name="PreviousStatus">The previous status of the team.</param>
/// <param name="NewStatus">The new status of the team.</param>
/// <param name="ChangedBy">The ID of the user who initiated the status change.</param>
/// <param name="OccurredAt">The timestamp when the status change occurred.</param>
/// <param name="CorrelationId">The unique identifier for this event.</param>
public record TeamStatusChangedDomainEvent(
    TeamId TeamId,
    TeamStatus PreviousStatus,
    TeamStatus NewStatus,
    UserId ChangedBy,
    DateTime OccurredAt,
    Guid CorrelationId) : BaseDomainEvent(OccurredAt, CorrelationId)
{
    /// <summary>
    /// Initializes a new instance of the TeamStatusChangedDomainEvent record with auto-generated event metadata.
    /// </summary>
    /// <param name="teamId">The unique identifier of the team whose status changed.</param>
    /// <param name="previousStatus">The previous status of the team.</param>
    /// <param name="newStatus">The new status of the team.</param>
    /// <param name="changedBy">The ID of the user who initiated the status change.</param>
    public TeamStatusChangedDomainEvent(
        TeamId teamId,
        TeamStatus previousStatus,
        TeamStatus newStatus,
        UserId changedBy)
        : this(teamId, previousStatus, newStatus, changedBy, DateTime.UtcNow, Guid.NewGuid())
    {
    }

    /// <summary>
    /// Gets a description of what changed in this domain event.
    /// </summary>
    public string Description => 
        $"Team {TeamId} status changed from {PreviousStatus.GetDescription()} to {NewStatus.GetDescription()} by user {ChangedBy}";

    /// <summary>
    /// Determines if this status change represents a significant operational change.
    /// </summary>
    /// <returns>True if the change significantly affects team operations; otherwise, false.</returns>
    public bool IsSignificantChange()
    {
        // Status changes to/from archived are considered significant
        return PreviousStatus == TeamStatus.Archived || NewStatus == TeamStatus.Archived ||
               // Activating or deactivating a team is significant
               (PreviousStatus == TeamStatus.Active && NewStatus != TeamStatus.Active) ||
               (PreviousStatus != TeamStatus.Active && NewStatus == TeamStatus.Active);
    }

    /// <summary>
    /// Determines if this status change requires notifying team members.
    /// </summary>
    /// <returns>True if team members should be notified; otherwise, false.</returns>
    public bool RequiresNotification()
    {
        // Notify members when team becomes inactive or archived
        return NewStatus == TeamStatus.Inactive || NewStatus == TeamStatus.Archived ||
               // Notify when team is reactivated
               (PreviousStatus != TeamStatus.Active && NewStatus == TeamStatus.Active);
    }

    /// <summary>
    /// Gets the notification priority level for this status change.
    /// </summary>
    /// <returns>The priority level for notifications.</returns>
    public NotificationPriority GetNotificationPriority()
    {
        return NewStatus switch
        {
            TeamStatus.Archived => NotificationPriority.High,
            TeamStatus.Inactive => NotificationPriority.Medium,
            TeamStatus.Active when PreviousStatus != TeamStatus.Active => NotificationPriority.Medium,
            _ => NotificationPriority.Low
        };
    }
}

/// <summary>
/// Represents the priority level for notifications.
/// </summary>
public enum NotificationPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}
