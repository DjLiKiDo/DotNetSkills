namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeam;

/// <summary>
/// Query for retrieving a team by ID with member details.
/// </summary>
public record GetTeamByIdQuery(TeamId TeamId) : IRequest<TeamResponse>;
