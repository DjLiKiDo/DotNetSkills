namespace DotNetSkills.Application.TeamCollaboration.Features.RemoveTeamMember;

/// <summary>
/// Command for removing a member from a team.
/// </summary>
public record RemoveTeamMemberCommand(TeamId TeamId, UserId UserId) : IRequest;
