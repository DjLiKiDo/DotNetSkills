namespace DotNetSkills.Domain.TeamCollaboration.Events;

/// <summary>
/// Domain event raised when a user leaves a team.
/// </summary>
/// <param name="UserId">The ID of the user leaving the team.</param>
/// <param name="TeamId">The ID of the team being left.</param>
/// <param name="Role">The role the user had in the team.</param>
public record UserLeftTeamDomainEvent(
    UserId UserId,
    TeamId TeamId,
    TeamRole Role) : BaseDomainEvent;
