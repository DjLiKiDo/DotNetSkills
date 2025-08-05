namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// User profile response DTO for profile management operations.
/// Contains detailed user information suitable for profile editing and management screens.
/// Excludes sensitive information like passwords.
/// </summary>
public record UserProfileResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int TeamMembershipsCount,
    IReadOnlyList<UserProfileResponse.TeamMembershipSummary> TeamMemberships)
{
    /// <summary>
    /// Summary information about a team membership for profile display.
    /// </summary>
    public record TeamMembershipSummary(
        Guid TeamId,
        string TeamName,
        string Role,
        DateTime JoinedAt);
};