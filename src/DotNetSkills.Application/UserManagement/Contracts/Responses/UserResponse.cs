namespace DotNetSkills.Application.UserManagement.Contracts.Responses;

/// <summary>
/// Response DTO for user operations.
/// This DTO represents user data returned from API endpoints.
/// </summary>
public record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    UserStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int TeamMembershipsCount)
{
}
