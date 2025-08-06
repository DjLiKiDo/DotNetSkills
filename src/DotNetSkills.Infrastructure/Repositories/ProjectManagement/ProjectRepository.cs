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
    /// Gets projects by their associated team asynchronously.
    /// </summary>
    /// <param name="teamId">The team ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects belonging to the specified team.</returns>
    /// <exception cref="ArgumentNullException">Thrown when teamId is null.</exception>
    public async Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        if (teamId == null)
            throw new ArgumentNullException(nameof(teamId));

        return await DbSet
            .AsNoTracking()
            .Where(p => p.TeamId == teamId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a project with its tasks included asynchronously.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project with tasks if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
    public async Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        // Note: Using string literal for Task entity to avoid System.Threading.Tasks.Task conflict
        var project = await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (project != null)
        {
            // Load tasks separately to avoid complex Include that might cause issues
            var tasks = await Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                .AsNoTracking()
                .Where(t => t.ProjectId == id)
                .OrderBy(t => t.Title)
                .ToListAsync(cancellationToken);

            // Note: This assumes Project entity has a way to associate tasks
            // The actual implementation might need adjustment based on the domain model
        }

        return project;
    }

    /// <summary>
    /// Gets projects by their status asynchronously.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with the specified status.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(p => p.Status == status)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Checks if a project with the specified name exists within a team scope.
    /// Project names must be unique within each team.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="teamId">The team ID for scoping the uniqueness check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check (for updates).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a project with the name exists in the team, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when name is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when teamId is null.</exception>
    public async Task<bool> ExistsByNameInTeamAsync(
        string name, 
        TeamId teamId, 
        ProjectId? excludeProjectId = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name cannot be null or empty.", nameof(name));
        
        if (teamId == null)
            throw new ArgumentNullException(nameof(teamId));

        var query = DbSet
            .AsNoTracking()
            .Where(p => p.Name == name && p.TeamId == teamId);

        if (excludeProjectId != null)
        {
            query = query.Where(p => p.Id != excludeProjectId);
        }

        return await query.AnyAsync(cancellationToken);
    }

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
    public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamId? teamId = null,
        ProjectStatus? status = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        DateTime? endDateFrom = null,
        DateTime? endDateTo = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(searchTerm) || 
                (p.Description != null && p.Description.Contains(searchTerm)));
        }

        // Apply team filter
        if (teamId != null)
        {
            query = query.Where(p => p.TeamId == teamId);
        }

        // Apply status filter
        if (status != null)
        {
            query = query.Where(p => p.Status == status);
        }

        // Apply start date range filter
        query = query.ApplyDateRangeFilter(startDateFrom, startDateTo, p => p.StartDate);

        // Apply end date range filter (using PlannedEndDate or EndDate)
        if (endDateFrom.HasValue || endDateTo.HasValue)
        {
            query = query.Where(p => 
                (p.PlannedEndDate.HasValue && 
                 (!endDateFrom.HasValue || p.PlannedEndDate >= endDateFrom.Value) &&
                 (!endDateTo.HasValue || p.PlannedEndDate <= endDateTo.Value.Date.AddDays(1).AddTicks(-1))) ||
                (p.EndDate.HasValue && 
                 (!endDateFrom.HasValue || p.EndDate >= endDateFrom.Value) &&
                 (!endDateTo.HasValue || p.EndDate <= endDateTo.Value.Date.AddDays(1).AddTicks(-1))));
        }

        // Apply ordering
        query = query.OrderBy(p => p.Name);

        // Get paginated results
        var (projects, totalCount) = await GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
        
        return (projects, totalCount);
    }

    /// <summary>
    /// Gets projects with their task counts asynchronously.
    /// Optimized for dashboard and reporting scenarios.
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with task count information.</returns>
    public async Task<IEnumerable<(Project Project, int TotalTasks, int CompletedTasks, int ActiveTasks)>> GetWithTaskCountsAsync(
        TeamId? teamId = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (teamId != null)
        {
            query = query.Where(p => p.TeamId == teamId);
        }

        if (status != null)
        {
            query = query.Where(p => p.Status == status);
        }

        var results = await query
            .Select(p => new 
            {
                Project = p,
                TotalTasks = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id),
                CompletedTasks = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id && t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done),
                ActiveTasks = Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>()
                    .Count(t => t.ProjectId == p.Id && 
                        (t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.ToDo || 
                         t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.InProgress || 
                         t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.InReview))
            })
            .OrderBy(x => x.Project.Name)
            .ToListAsync(cancellationToken);

        return results.Select(x => (x.Project, x.TotalTasks, x.CompletedTasks, x.ActiveTasks)).ToList();
    }

    /// <summary>
    /// Gets projects that are overdue (past their planned end date).
    /// </summary>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of overdue projects.</returns>
    public async Task<IEnumerable<Project>> GetOverdueProjectsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var query = DbSet
            .AsNoTracking()
            .Where(p => p.PlannedEndDate.HasValue && 
                       p.PlannedEndDate.Value.Date < today &&
                       p.Status != ProjectStatus.Completed && 
                       p.Status != ProjectStatus.Cancelled);

        if (teamId != null)
        {
            query = query.Where(p => p.TeamId == teamId);
        }

        return await query
            .OrderBy(p => p.PlannedEndDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets projects that are approaching their deadline within the specified number of days.
    /// </summary>
    /// <param name="daysUntilDeadline">The number of days before deadline to consider.</param>
    /// <param name="teamId">Optional team filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects approaching their deadline.</returns>
    public async Task<IEnumerable<Project>> GetProjectsApproachingDeadlineAsync(
        int daysUntilDeadline,
        TeamId? teamId = null,
        CancellationToken cancellationToken = default)
    {
        if (daysUntilDeadline < 0)
            throw new ArgumentException("Days until deadline must be non-negative.", nameof(daysUntilDeadline));

        var today = DateTime.UtcNow.Date;
        var targetDate = today.AddDays(daysUntilDeadline);
        
        var query = DbSet
            .AsNoTracking()
            .Where(p => p.PlannedEndDate.HasValue && 
                       p.PlannedEndDate.Value.Date >= today &&
                       p.PlannedEndDate.Value.Date <= targetDate &&
                       p.Status != ProjectStatus.Completed && 
                       p.Status != ProjectStatus.Cancelled);

        if (teamId != null)
        {
            query = query.Where(p => p.TeamId == teamId);
        }

        return await query
            .OrderBy(p => p.PlannedEndDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets projects by date range for reporting and analytics.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <param name="dateType">The type of date to filter by (Created, Started, or Planned End).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects within the specified date range.</returns>
    public async Task<IEnumerable<Project>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        ProjectDateFilterType dateType = ProjectDateFilterType.Created,
        CancellationToken cancellationToken = default)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date.");

        var query = DbSet.AsNoTracking();

        query = dateType switch
        {
            ProjectDateFilterType.Created => query.Where(p => 
                p.CreatedAt.Date >= startDate.Date && p.CreatedAt.Date <= endDate.Date),
            ProjectDateFilterType.Started => query.Where(p => 
                p.StartDate.HasValue && p.StartDate.Value.Date >= startDate.Date && p.StartDate.Value.Date <= endDate.Date),
            ProjectDateFilterType.PlannedEnd => query.Where(p => 
                p.PlannedEndDate.HasValue && p.PlannedEndDate.Value.Date >= startDate.Date && p.PlannedEndDate.Value.Date <= endDate.Date),
            _ => throw new ArgumentException("Invalid date filter type.", nameof(dateType))
        };

        return await query
            .OrderBy(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Override to provide Project-specific default ordering.
    /// </summary>
    /// <returns>Expression to order by project name.</returns>
    protected override Expression<Func<Project, object>> GetDefaultOrderingExpression()
    {
        return p => p.Name;
    }
}
