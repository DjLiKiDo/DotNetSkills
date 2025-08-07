namespace DotNetSkills.Application.ProjectManagement.Contracts;

/// <summary>
/// Repository interface specific to Project entities.
/// Extends the generic repository with Project-specific query methods.
/// </summary>
public interface IProjectRepository : IRepository<Project, ProjectId>
{
    /// <summary>
    /// Gets a project by its name asynchronously.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, otherwise null.</returns>
    Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a project with the specified name exists.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a project with the name exists, otherwise false.</returns>
    Task<bool> ExistsByNameAsync(string name, ProjectId? excludeProjectId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by team ID asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects belonging to the specified team.</returns>
    Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by their status asynchronously.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with the specified status.</returns>
    Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects with pagination support asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter by name or description.</param>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated collection of projects.</returns>
    Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamId? teamId = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by team ID as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many projects for a specific team.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of projects for the specified team.</returns>
    IAsyncEnumerable<Project> GetByTeamIdAsyncEnumerable(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects by their status as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many projects with the specified status.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of projects with the specified status.</returns>
    IAsyncEnumerable<Project> GetByStatusAsyncEnumerable(ProjectStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active projects as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of active projects.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of active projects.</returns>
    IAsyncEnumerable<Project> GetActiveProjectsAsyncEnumerable(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project with its tasks eagerly loaded.
    /// Prevents N+1 query problems when accessing task data.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with tasks if found, otherwise null.</returns>
    Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a project with its team information eagerly loaded.
    /// Optimized for scenarios that need team context.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with team information if found, otherwise null.</returns>
    Task<Project?> GetWithTeamAsync(ProjectId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets projects with their task counts for dashboard scenarios.
    /// Prevents N+1 problems when displaying project statistics.
    /// </summary>
    /// <param name="teamId">Optional team ID filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Projects with task count information.</returns>
    Task<IEnumerable<(Project Project, int TaskCount)>> GetWithTaskCountsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default);
}
