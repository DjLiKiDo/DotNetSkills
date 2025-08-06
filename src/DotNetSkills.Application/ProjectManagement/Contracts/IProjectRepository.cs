namespace DotNetSkills.Application.ProjectManagement.Contracts;

/// <summary>
/// Repository interface specific to Project entities.
/// Extends the generic repository with Project-specific query methods for project management operations.
/// </summary>
public interface IProjectRepository : IRepository<Project, ProjectId>
{
    /// <summary>
    /// Gets projects by their associated team asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects belonging to the specified team.</returns>
    Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project with its tasks included asynchronously.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with tasks if found, otherwise null.</returns>
    Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by their status asynchronously.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with the specified status.</returns>
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a project with the specified name exists within a team scope.
    /// Project names must be unique within each team.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="teamId">The team ID for scoping the uniqueness check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a project with the name exists in the team, otherwise false.</returns>
    Task<bool> ExistsByNameInTeamAsync(
        string name, 
        TeamId teamId, 
        ProjectId? excludeProjectId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects with pagination support asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by name or description.</param>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="startDateFrom">Optional start date range filter (from).</param>
    /// <param name="startDateTo">Optional start date range filter (to).</param>
    /// <param name="endDateFrom">Optional end date range filter (from).</param>
    /// <param name="endDateTo">Optional end date range filter (to).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of projects with task counts.</returns>
    Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamId? teamId = null,
        ProjectStatus? status = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        DateTime? endDateFrom = null,
        DateTime? endDateTo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects with their task counts asynchronously.
    /// Optimized for dashboard and reporting scenarios.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with task count information.</returns>
    Task<IEnumerable<(Project Project, int TotalTasks, int CompletedTasks, int ActiveTasks)>> GetWithTaskCountsAsync(
        TeamId? teamId = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects that are overdue (past their planned end date).
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of overdue projects.</returns>
    Task<IEnumerable<Project>> GetOverdueProjectsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects that are approaching their deadline within the specified number of days.
    /// </summary>
    /// <param name="daysUntilDeadline">The number of days before deadline to consider.</param>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects approaching their deadline.</returns>
    Task<IEnumerable<Project>> GetProjectsApproachingDeadlineAsync(
        int daysUntilDeadline,
        TeamId? teamId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by date range for reporting and analytics.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="dateType">The type of date to filter by (Created, Started, or Planned End).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects within the specified date range.</returns>
    Task<IEnumerable<Project>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        ProjectDateFilterType dateType = ProjectDateFilterType.Created,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Enumeration for different date filter types when querying projects.
/// </summary>
public enum ProjectDateFilterType
{
    /// <summary>
    /// Filter by project creation date.
    /// </summary>
    Created,

    /// <summary>
    /// Filter by project start date.
    /// </summary>
    Started,

    /// <summary>
    /// Filter by project planned end date.
    /// </summary>
    PlannedEnd
}
