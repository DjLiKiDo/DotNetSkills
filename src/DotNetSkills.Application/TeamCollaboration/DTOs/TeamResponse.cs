namespace DotNetSkills.Application.TeamCollaboration.DTOs;

/// <summary>
/// Response DTO for team operations.
/// This DTO represents team data returned from API endpoints.
/// </summary>
public record TeamResponse(
    Guid Id,
    string Name,
    string? Description,
    int MemberCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyList<TeamMemberResponse> Members)
{
    /// <summary>
    /// Creates a TeamResponse from domain entity.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public static TeamResponse FromDomain(Team team)
    {
        return new TeamResponse(
            team.Id.Value,
            team.Name,
            team.Description,
            team.MemberCount,
            team.CreatedAt,
            team.UpdatedAt,
            team.Members.Select(TeamMemberResponse.FromDomain).ToList().AsReadOnly());
    }
}

/// <summary>
/// Response DTO for team member data within team context.
/// </summary>
public record TeamMemberResponse(
    Guid UserId,
    string Role,
    DateTime JoinedAt)
{
    /// <summary>
    /// Creates a TeamMemberResponse from domain entity.
    /// TODO: Replace with AutoMapper when properly configured.
    /// TODO: User details will need to be loaded separately via repository or included in query.
    /// </summary>
    public static TeamMemberResponse FromDomain(TeamMember member)
    {
        return new TeamMemberResponse(
            member.UserId.Value,
            member.Role.ToString(),
            member.JoinedAt);
    }
}