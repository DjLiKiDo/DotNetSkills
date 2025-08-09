namespace DotNetSkills.Application.TeamCollaboration.Projections;

/// <summary>
/// Lightweight projection for team summary information.
/// Used for read-only scenarios to minimize data transfer.
/// </summary>
public record TeamSummaryProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required TeamStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int MemberCount { get; init; }
    public required int ProjectCount { get; init; }
}

/// <summary>
/// Projection for team dashboard information.
/// Optimized for dashboard and overview scenarios.
/// </summary>
public record TeamDashboardProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required TeamStatus Status { get; init; }
    public required int MemberCount { get; init; }
    public required int ActiveProjectCount { get; init; }
    public required int CompletedProjectCount { get; init; }
    public required int ActiveTaskCount { get; init; }
    public required int CompletedTaskCount { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}

/// <summary>
/// Minimal projection for dropdown/selection scenarios.
/// Contains only essential identification information.
/// </summary>
public record TeamSelectionProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required TeamStatus Status { get; init; }
    public required bool IsActive { get; init; }
    public required int MemberCount { get; init; }
}

/// <summary>
/// Projection for team membership information.
/// Shows team details with member context.
/// </summary>
public record TeamMembershipProjection
{
    public required Guid TeamId { get; init; }
    public required string TeamName { get; init; }
    public required TeamStatus TeamStatus { get; init; }
    public required TeamRole MemberRole { get; init; }
    public required DateTime JoinedAt { get; init; }
    public required int TotalMembers { get; init; }
}
