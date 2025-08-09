namespace DotNetSkills.Application.TeamCollaboration.Features.UpdateMemberRole;

/// <summary>
/// Command for updating a team member's role.
/// </summary>
public record UpdateMemberRoleCommand(TeamId TeamId, UserId UserId, TeamRole NewRole) : IRequest<TeamMemberResponse>;
