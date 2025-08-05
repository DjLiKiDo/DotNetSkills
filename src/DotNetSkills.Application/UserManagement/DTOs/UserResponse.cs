namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Response DTO for user operations.
/// This DTO represents user data returned from API endpoints.
/// </summary>
public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int TeamMembershipsCount)
{
    /// <summary>
    /// Creates a UserResponse from domain entity.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public static UserResponse FromDomain(User user)
    {
        return new UserResponse(
            user.Id.Value,
            user.Name,
            user.Email.Value,
            user.Role.ToString(),
            user.Status.ToString(),
            user.CreatedAt,
            user.UpdatedAt,
            user.TeamMemberships.Count);
    }
}
