using DotNetSkills.Application.ProjectManagement.Projections;

namespace DotNetSkills.Infrastructure.Repositories.ProjectManagement;

/// <summary>
/// Entity Framework Core implementation of the IProjectRepository interface.
/// Provides data access operations for Project entities with team associations and task management.
/// </summary>
public class ProjectRepository : BaseRepository<Project, ProjectId>, IProjectRepository
{
    /// <summary>
    /// Initializes a new instance of the ProjectRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets a project by its name asynchronously.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, otherwise null.</returns>
    public async Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name == name, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Checks if a project with the specified name exists.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a project with the name exists, otherwise false.</returns>
    public async Task<bool> ExistsByNameAsync(string name, ProjectId? excludeProjectId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false;

        var query = DbSet.AsNoTracking().Where(p => p.Name == name);

        if (excludeProjectId != null)
            query = query.Where(p => p.Id != excludeProjectId);

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects by their associated team asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects belonging to the specified team.</returns>
    public async Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(p => p.TeamId == teamId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects by their status asynchronously.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with the specified status.</returns>
    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

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
    public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamId? teamId = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchTermValue = searchTerm!; // We know it's not null here
            query = query.Where(p => p.Name.Contains(searchTermValue) || 
                                     (p.Description != null && p.Description.Contains(searchTermValue)));
        }

        if (teamId != null)
        {
            query = query.Where(p => p.TeamId == teamId);
        }

        if (status != null)
        {
            query = query.Where(p => p.Status == status);
        }

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        // Apply pagination
        var projects = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (projects, totalCount);
    }

    /// <summary>
    /// Gets projects by team ID as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many projects for a specific team.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of projects for the specified team.</returns>
    public IAsyncEnumerable<Project> GetByTeamIdAsyncEnumerable(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(p => p.TeamId == teamId)
            .OrderBy(p => p.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets projects by their status as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many projects with the specified status.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of projects with the specified status.</returns>
    public IAsyncEnumerable<Project> GetByStatusAsyncEnumerable(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderBy(p => p.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets all active projects as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of active projects.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of active projects.</returns>
    public IAsyncEnumerable<Project> GetActiveProjectsAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(p => p.Status == ProjectStatus.Active)
            .OrderBy(p => p.Name)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets a project with its tasks eagerly loaded.
    /// Prevents N+1 query problems when accessing task data.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with tasks if found, otherwise null.</returns>
    public async Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                .Where(t => t.ProjectId == p.Id))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a project with its team information eagerly loaded.
    /// Optimized for scenarios that need team context.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with team information if found, otherwise null.</returns>
    public async Task<Project?> GetWithTeamAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => Context.Set<Team>().Where(t => t.Id == p.TeamId))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects with their task counts for dashboard scenarios.
    /// Prevents N+1 problems when displaying project statistics.
    /// </summary>
    /// <param name="teamId">Optional team ID filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Projects with task count information.</returns>
    public async Task<IEnumerable<(Project Project, int TaskCount)>> GetWithTaskCountsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
            query = query.Where(p => p.TeamId == teamId);

        var results = await query
            .Select(p => new
            {
                Project = p,
                TaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id)
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return results.Select(r => (r.Project, r.TaskCount));
    }

    // Projection Methods

    /// <summary>
    /// Gets project summaries with optimized projection for read-only scenarios.
    /// Minimizes data transfer by selecting only required fields.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of project summary projections.</returns>
    public async Task<IEnumerable<ProjectSummaryProjection>> GetProjectSummariesAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
            query = query.Where(p => p.TeamId == teamId);

        return await query
            .Select(p => new ProjectSummaryProjection
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                TeamId = p.TeamId.Value,
                TeamName = Context.Set<Team>()
                    .Where(t => t.Id == p.TeamId)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? "Unknown",
                TaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id)
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project dashboard information with aggregated data.
    /// Optimized for dashboard scenarios with minimal queries.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of project dashboard projections.</returns>
    public async Task<IEnumerable<ProjectDashboardProjection>> GetProjectDashboardDataAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
            query = query.Where(p => p.TeamId == teamId);

        return await query
            .Select(p => new ProjectDashboardProjection
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                TeamId = p.TeamId.Value,
                TeamName = Context.Set<Team>()
                    .Where(t => t.Id == p.TeamId)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? "Unknown",
                TotalTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id),
                CompletedTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done),
                InProgressTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.InProgress),
                PendingTaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.ToDo),
                CompletionPercentage = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Where(t => t.ProjectId == p.Id)
                    .Count() == 0 ? 0 :
                    (decimal)Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                        .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done) * 100 /
                    Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                        .Count(t => t.ProjectId == p.Id)
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project selection data for dropdowns and selection lists.
    /// Minimal projection for UI scenarios.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="activeOnly">Whether to return only active projects.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of project selection projections.</returns>
    public async Task<IEnumerable<ProjectSelectionProjection>> GetProjectSelectionsAsync(TeamId? teamId = null, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
            query = query.Where(p => p.TeamId == teamId);

        if (activeOnly)
            query = query.Where(p => p.Status == ProjectStatus.Active);

        return await query
            .Select(p => new ProjectSelectionProjection
            {
                Id = p.Id.Value,
                Name = p.Name,
                Status = p.Status.ToString(),
                IsActive = p.Status == ProjectStatus.Active,
                TeamId = p.TeamId.Value,
                TeamName = Context.Set<Team>()
                    .Where(t => t.Id == p.TeamId)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? "Unknown"
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project overview data with team context.
    /// Useful for project listing by team scenarios.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of project overview projections.</returns>
    public async Task<IEnumerable<ProjectOverviewProjection>> GetProjectOverviewsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
            query = query.Where(p => p.TeamId == teamId);

        return await query
            .Select(p => new ProjectOverviewProjection
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status.ToString(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                TeamName = Context.Set<Team>()
                    .Where(t => t.Id == p.TeamId)
                    .Select(t => t.Name)
                    .FirstOrDefault() ?? "Unknown",
                TaskCount = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id),
                ProgressPercentage = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Where(t => t.ProjectId == p.Id)
                    .Count() == 0 ? 0 :
                    (decimal)Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                        .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done) * 100 /
                    Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                        .Count(t => t.ProjectId == p.Id)
            })
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
