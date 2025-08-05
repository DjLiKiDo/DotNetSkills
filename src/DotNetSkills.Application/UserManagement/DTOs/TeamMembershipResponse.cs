namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Response DTO for team membership relationships.
/// Contains detailed information about a user's membership in a team.
/// </summary>
public record TeamMembershipResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string UserEmail,
    Guid TeamId,
    string TeamName,
    string TeamDescription,
    string Role,
    DateTime JoinedAt,
    DateTime? LeftAt,
    bool IsActive)
{
    /// <summary>
    /// Indicates if the membership is currently active (user is still a member).
    /// </summary>
    public bool IsCurrentMember => LeftAt == null && IsActive;
};