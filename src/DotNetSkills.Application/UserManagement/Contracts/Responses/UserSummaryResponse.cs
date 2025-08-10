namespace DotNetSkills.Application.UserManagement.Contracts.Responses;

/// <summary>
/// Lightweight user response DTO for lists, dropdowns, and summary displays.
/// Contains only essential user information for performance optimization.
/// </summary>
public record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    UserStatus Status);