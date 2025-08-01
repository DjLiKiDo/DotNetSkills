namespace DotNetSkills.Domain.TeamCollaboration.Events;

/// <summary>
/// Domain event raised when a team is created.
/// </summary>
/// <param name="TeamId">The ID of the created team.</param>
/// <param name="Name">The name of the created team.</param>
/// <param name="CreatedBy">The ID of the user who created the team.</param>
public record TeamCreatedDomainEvent(
    TeamId TeamId,
    string Name,
    UserId CreatedBy) : BaseDomainEvent;
