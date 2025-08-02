namespace DotNetSkills.Application.TeamCollaboration.DTOs;

/// <summary>
/// Response DTO for team members listing.
/// This DTO represents all members of a team with their details.
/// </summary>
public record TeamMembersResponse(
    Guid TeamId,
    string TeamName,
    int MemberCount,
    int MaxMembers,
    IReadOnlyList<TeamMemberResponse> Members)
{
    /// <summary>
    /// Gets whether the team is at capacity.
    /// </summary>
    public bool IsAtCapacity => MemberCount >= MaxMembers;

    /// <summary>
    /// Creates a TeamMembersResponse from domain entity.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public static TeamMembersResponse FromDomain(Team team)
    {
        return new TeamMembersResponse(
            team.Id.Value,
            team.Name,
            team.MemberCount,
            Team.MaxMembers,
            team.Members.Select(TeamMemberResponse.FromDomain).ToList().AsReadOnly());
    }
}