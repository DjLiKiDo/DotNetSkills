namespace DotNetSkills.Application.TeamCollaboration.Features.UpdateTeam;

/// <summary>
/// Command for updating an existing team.
/// </summary>
public record UpdateTeamCommand(TeamId TeamId, string Name, string? Description) : IRequest<TeamResponse>;
