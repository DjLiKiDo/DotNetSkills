namespace DotNetSkills.Application.UserManagement.Projections;

/// <summary>
/// Lightweight projection for user summary information.
/// Used for read-only scenarios to minimize data transfer.
/// </summary>
public record UserSummaryProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required UserStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int TeamMembershipCount { get; init; }
}

/// <summary>
/// Projection for user dashboard information.
/// Optimized for dashboard and overview scenarios.
/// </summary>
public record UserDashboardProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required UserStatus Status { get; init; }
    public required int ActiveTeamCount { get; init; }
    public required int AssignedTaskCount { get; init; }
    public required int CompletedTaskCount { get; init; }
    public required DateTime? LastLoginAt { get; init; }
}

/// <summary>
/// Minimal projection for dropdown/selection scenarios.
/// Contains only essential identification information.
/// </summary>
public record UserSelectionProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public required bool IsActive { get; init; }
}
