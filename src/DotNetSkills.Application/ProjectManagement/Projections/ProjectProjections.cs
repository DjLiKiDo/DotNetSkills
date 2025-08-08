namespace DotNetSkills.Application.ProjectManagement.Projections;

/// <summary>
/// Lightweight projection for project summary information.
/// Used for read-only scenarios to minimize data transfer.
/// </summary>
public record ProjectSummaryProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ProjectStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? StartDate { get; init; }
    public required DateTime? EndDate { get; init; }
    public required Guid TeamId { get; init; }
    public required string TeamName { get; init; }
    public required int TaskCount { get; init; }
}

/// <summary>
/// Projection for project dashboard information.
/// Optimized for dashboard and overview scenarios.
/// </summary>
public record ProjectDashboardProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ProjectStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime? StartDate { get; init; }
    public required DateTime? EndDate { get; init; }
    public required Guid TeamId { get; init; }
    public required string TeamName { get; init; }
    public required int TotalTaskCount { get; init; }
    public required int CompletedTaskCount { get; init; }
    public required int InProgressTaskCount { get; init; }
    public required int PendingTaskCount { get; init; }
    public required decimal CompletionPercentage { get; init; }
}

/// <summary>
/// Minimal projection for dropdown/selection scenarios.
/// Contains only essential identification information.
/// </summary>
public record ProjectSelectionProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required ProjectStatus Status { get; init; }
    public required bool IsActive { get; init; }
    public required Guid TeamId { get; init; }
    public required string TeamName { get; init; }
}

/// <summary>
/// Projection for project overview with team context.
/// Useful for listing projects by team.
/// </summary>
public record ProjectOverviewProjection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ProjectStatus Status { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string TeamName { get; init; }
    public required int TaskCount { get; init; }
    public required decimal ProgressPercentage { get; init; }
}
