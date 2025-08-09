namespace DotNetSkills.Application.TeamCollaboration.Features.AddTeamMember;

/// <summary>
/// Command for adding a member to a team.
/// Enforces Team.MaxMembers constraint and prevents duplicate memberships.
/// </summary>
public record AddTeamMemberCommand(TeamId TeamId, UserId UserId, TeamRole Role) : IRequest<TeamMemberResponse>;
