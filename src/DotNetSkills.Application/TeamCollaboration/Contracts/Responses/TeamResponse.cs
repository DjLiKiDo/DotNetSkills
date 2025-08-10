namespace DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

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
}

/// <summary>
/// Response DTO for team member data within team context.
/// </summary>
public record TeamMemberResponse(
    Guid UserId,
    TeamRole Role,
    DateTime JoinedAt)
{
}
