namespace DotNetSkills.Domain.ProjectManagement.Events;

/// <summary>
/// Domain event raised when a project status changes.
/// </summary>
/// <param name="ProjectId">The ID of the project whose status changed.</param>
/// <param name="PreviousStatus">The previous status of the project.</param>
/// <param name="NewStatus">The new status of the project.</param>
/// <param name="ChangedBy">The ID of the user who changed the status.</param>
public record ProjectStatusChangedDomainEvent(
    ProjectId ProjectId,
    ProjectStatus PreviousStatus,
    ProjectStatus NewStatus,
    UserId ChangedBy) : BaseDomainEvent;
