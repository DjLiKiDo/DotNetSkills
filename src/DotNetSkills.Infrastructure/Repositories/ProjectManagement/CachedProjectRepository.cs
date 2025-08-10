using DotNetSkills.Application.ProjectManagement.Projections;

namespace DotNetSkills.Infrastructure.Repositories.ProjectManagement;

/// <summary>
/// Cached implementation of the IProjectRepository interface using the Decorator pattern.
/// Provides in-memory caching for frequently accessed project data to improve performance.
/// Falls back to the underlying repository for cache misses and write operations.
/// </summary>
public class CachedProjectRepository : CachedRepositoryBase<Project, ProjectId, IProjectRepository>, IProjectRepository
{
    /// <summary>
    /// The entity name used for cache key generation.
    /// </summary>
    protected override string EntityName => "project";

    /// <summary>
    /// Initializes a new instance of the CachedProjectRepository class.
    /// </summary>
    /// <param name="innerRepository">The underlying repository implementation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when innerRepository or cache is null.</exception>
    public CachedProjectRepository(IProjectRepository innerRepository, IMemoryCache cache) 
        : base(innerRepository, cache)
    {
    }

    #region IProjectRepository Specific Methods

    /// <summary>
    /// Gets a project by their name with caching support.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The project if found, otherwise null.</returns>
    public async Task<Project?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetNameCacheKey(name);
        
        return await GetCachedAsync(cacheKey, async () =>
        {
            var project = await InnerRepository.GetByNameAsync(name, cancellationToken).ConfigureAwait(false);
            
            if (project != null)
            {
                Cache.Set(GetIdCacheKey(project.Id), project, DefaultCacheOptions);
            }
            
            return project;
        });
    }

    /// <summary>
    /// Checks if a project exists by name with caching support.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a project with the name exists, otherwise false.</returns>
    public async Task<bool> ExistsByNameAsync(string name, ProjectId? excludeProjectId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetNameExistsCacheKey(name, excludeProjectId);
        
        return await GetCachedBoolAsync(cacheKey, () => InnerRepository.ExistsByNameAsync(name, excludeProjectId, cancellationToken));
    }

    /// <summary>
    /// Gets projects by team ID. This method bypasses cache due to team-specific nature.
    /// </summary>
    public async Task<IEnumerable<Project>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetByTeamIdAsync(teamId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects by status with caching support.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of projects with the specified status.</returns>
    public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("status", status);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByStatusAsync(status, cancellationToken));
    }

    /// <summary>
    /// Gets paginated projects. This method bypasses cache due to dynamic nature of pagination.
    /// </summary>
    public async Task<(IEnumerable<Project> Projects, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        TeamId? teamId = null,
        ProjectStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, teamId, status, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects by team ID as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Project> GetByTeamIdAsyncEnumerable(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByTeamIdAsyncEnumerable(teamId, cancellationToken);
    }

    /// <summary>
    /// Gets projects by status as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Project> GetByStatusAsyncEnumerable(ProjectStatus status, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByStatusAsyncEnumerable(status, cancellationToken);
    }

    /// <summary>
    /// Gets active projects as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Project> GetActiveProjectsAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetActiveProjectsAsyncEnumerable(cancellationToken);
    }

    /// <summary>
    /// Gets project with tasks. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<Project?> GetWithTasksAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithTasksAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project with team. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<Project?> GetWithTeamAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithTeamAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets projects with task counts. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<(Project Project, int TaskCount)>> GetWithTaskCountsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithTaskCountsAsync(teamId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project summaries with caching support.
    /// </summary>
    public async Task<IEnumerable<ProjectSummaryProjection>> GetProjectSummariesAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:summaries:{teamId?.Value}";
        
        if (Cache.TryGetValue(cacheKey, out IEnumerable<ProjectSummaryProjection>? cachedSummaries))
            return cachedSummaries!;
            
        var summaries = await InnerRepository.GetProjectSummariesAsync(teamId, cancellationToken).ConfigureAwait(false);
        Cache.Set(cacheKey, summaries, ShortCacheOptions);
        
        return summaries;
    }

    /// <summary>
    /// Gets project dashboard data with caching support.
    /// </summary>
    public async Task<IEnumerable<ProjectDashboardProjection>> GetProjectDashboardDataAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:dashboard:{teamId?.Value}";
        
        if (Cache.TryGetValue(cacheKey, out IEnumerable<ProjectDashboardProjection>? cachedDashboard))
            return cachedDashboard!;
            
        var dashboard = await InnerRepository.GetProjectDashboardDataAsync(teamId, cancellationToken).ConfigureAwait(false);
        Cache.Set(cacheKey, dashboard, ShortCacheOptions);
        
        return dashboard;
    }

    /// <summary>
    /// Gets project selections with caching support.
    /// </summary>
    public async Task<IEnumerable<ProjectSelectionProjection>> GetProjectSelectionsAsync(TeamId? teamId = null, bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:selections:{teamId?.Value}:{activeOnly}";
        
        if (Cache.TryGetValue(cacheKey, out IEnumerable<ProjectSelectionProjection>? cachedSelections))
            return cachedSelections!;
            
        var selections = await InnerRepository.GetProjectSelectionsAsync(teamId, activeOnly, cancellationToken).ConfigureAwait(false);
        Cache.Set(cacheKey, selections, ShortCacheOptions);
        
        return selections;
    }

    /// <summary>
    /// Gets project overviews with caching support.
    /// </summary>
    public async Task<IEnumerable<ProjectOverviewProjection>> GetProjectOverviewsAsync(TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"project:overviews:{teamId?.Value}";
        
        if (Cache.TryGetValue(cacheKey, out IEnumerable<ProjectOverviewProjection>? cachedOverviews))
            return cachedOverviews!;
            
        var overviews = await InnerRepository.GetProjectOverviewsAsync(teamId, cancellationToken).ConfigureAwait(false);
        Cache.Set(cacheKey, overviews, ShortCacheOptions);
        
        return overviews;
    }

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates entity-specific cache entries.
    /// Called automatically by the base class after add, update, or remove operations.
    /// </summary>
    /// <param name="project">The project entity that was modified.</param>
    protected override void InvalidateEntitySpecificCaches(Project project)
    {
        // Name-based caches
        InvalidateCaches(
            GetNameCacheKey(project.Name),
            GetNameExistsCacheKey(project.Name)
        );
        
        // Status-based caches
        InvalidateCache(GetFilterCacheKey("status", project.Status));
        
        // Team-based caches
        if (project.TeamId != null)
        {
            InvalidateCaches(
                GetProjectionCacheKey("summaries", project.TeamId.Value.ToString()),
                GetProjectionCacheKey("dashboard", project.TeamId.Value.ToString()),
                GetProjectionCacheKey("selections", $"{project.TeamId.Value}:True"),
                GetProjectionCacheKey("selections", $"{project.TeamId.Value}:False"),
                GetProjectionCacheKey("overviews", project.TeamId.Value.ToString())
            );
        }
        
        // General aggregated caches
        InvalidateCaches(
            GetProjectionCacheKey("summaries"),
            GetProjectionCacheKey("dashboard"),
            GetProjectionCacheKey("selections", "True"),
            GetProjectionCacheKey("selections", "False"),
            GetProjectionCacheKey("overviews")
        );
    }

    #endregion
}