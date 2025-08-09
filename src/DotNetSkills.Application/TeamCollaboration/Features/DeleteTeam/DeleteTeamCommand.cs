namespace DotNetSkills.Application.TeamCollaboration.Features.DeleteTeam;

/// <summary>
/// Command for deleting a team.
/// </summary>
public record DeleteTeamCommand(TeamId TeamId) : IRequest;
