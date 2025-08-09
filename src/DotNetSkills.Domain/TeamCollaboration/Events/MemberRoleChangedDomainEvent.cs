namespace DotNetSkills.Domain.TeamCollaboration.Events;

/// <summary>
/// Domain event raised when a team member's role changes.
/// </summary>
/// <param name="UserId">The ID of the user whose role changed.</param>
/// <param name="TeamId">The ID of the team.</param>
/// <param name="PreviousRole">The previous role the user had.</param>
/// <param name="NewRole">The new role assigned to the user.</param>
/// <param name="ChangedByUserId">The ID of the user who performed the change.</param>
public record MemberRoleChangedDomainEvent(
    UserId UserId,
    TeamId TeamId,
    TeamRole PreviousRole,
    TeamRole NewRole,
    UserId ChangedByUserId) : BaseDomainEvent;
