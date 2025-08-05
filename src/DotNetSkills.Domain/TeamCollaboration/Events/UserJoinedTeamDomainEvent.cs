namespace DotNetSkills.Domain.TeamCollaboration.Events;

/// <summary>
/// Domain event raised when a user joins a team.
/// </summary>
/// <param name="UserId">The ID of the user joining the team.</param>
/// <param name="TeamId">The ID of the team being joined.</param>
/// <param name="Role">The role assigned to the user in the team.</param>
public record UserJoinedTeamDomainEvent(
    UserId UserId,
    TeamId TeamId,
    TeamRole Role) : BaseDomainEvent;
