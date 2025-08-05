namespace DotNetSkills.Application.UserManagement.DTOs;

/// <summary>
/// Lightweight user response DTO for lists, dropdowns, and summary displays.
/// Contains only essential user information for performance optimization.
/// </summary>
public record UserSummaryResponse(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string Status);