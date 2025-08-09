namespace DotNetSkills.Application.TeamCollaboration.Features.GetTeamMembers;

/// <summary>
/// Query for retrieving all members of a team.
/// </summary>
public record GetTeamMembersQuery(TeamId TeamId) : IRequest<TeamMembersResponse>;
