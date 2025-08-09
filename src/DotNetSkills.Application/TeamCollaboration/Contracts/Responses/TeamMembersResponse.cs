namespace DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

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

}
