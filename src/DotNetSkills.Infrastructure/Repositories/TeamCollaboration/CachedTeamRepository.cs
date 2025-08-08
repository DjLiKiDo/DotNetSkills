using DotNetSkills.Application.TeamCollaboration.Projections;

namespace DotNetSkills.Infrastructure.Repositories.TeamCollaboration;

/// <summary>
/// Cached implementation of the ITeamRepository interface using the Decorator pattern.
/// Provides in-memory caching for frequently accessed team data to improve performance.
/// Falls back to the underlying repository for cache misses and write operations.
/// </summary>
public class CachedTeamRepository : CachedRepositoryBase<Team, TeamId, ITeamRepository>, ITeamRepository
{
    /// <summary>
    /// The entity name used for cache key generation.
    /// </summary>
    protected override string EntityName => "team";

    /// <summary>
    /// Initializes a new instance of the CachedTeamRepository class.
    /// </summary>
    /// <param name="innerRepository">The underlying repository implementation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when innerRepository or cache is null.</exception>
    public CachedTeamRepository(ITeamRepository innerRepository, IMemoryCache cache) 
        : base(innerRepository, cache)
    {
    }

    #region ITeamRepository Specific Methods

    /// <summary>
    /// Gets a team by their name with caching support.
    /// </summary>
    /// <param name="name">The team name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The team if found, otherwise null.</returns>
    public async Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetNameCacheKey(name);
        
        return await GetCachedAsync(cacheKey, async () =>
        {
            var team = await InnerRepository.GetByNameAsync(name, cancellationToken).ConfigureAwait(false);
            
            if (team != null)
            {
                // Also cache by ID for consistency
                Cache.Set(GetIdCacheKey(team.Id), team, DefaultCacheOptions);
            }
            
            return team;
        });
    }

    /// <summary>
    /// Checks if a team exists by name with caching support.
    /// </summary>
    /// <param name="name">The team name to check.</param>
    /// <param name="excludeTeamId">Optional team ID to exclude from the check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a team with the name exists, otherwise false.</returns>
    public async Task<bool> ExistsByNameAsync(string name, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetNameExistsCacheKey(name, excludeTeamId);
        
        return await GetCachedBoolAsync(cacheKey, () => InnerRepository.ExistsByNameAsync(name, excludeTeamId, cancellationToken));
    }

    /// <summary>
    /// Gets a team with members. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<Team?> GetWithMembersAsync(TeamId id, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithMembersAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets teams by status with caching support.
    /// </summary>
    /// <param name="status">The team status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of teams with the specified status.</returns>
    public async Task<IEnumerable<Team>> GetByStatusAsync(DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus status, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("status", status);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByStatusAsync(status, cancellationToken));
    }

    /// <summary>
    /// Gets teams by user membership. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<Team>> GetByUserMembershipAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetByUserMembershipAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets paginated teams. This method bypasses cache due to dynamic nature of pagination.
    /// </summary>
    public async Task<(IEnumerable<Team> Teams, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, status, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets teams with member counts. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<(Team Team, int MemberCount)>> GetWithMemberCountsAsync(CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithMemberCountsAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets teams with capacity. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<Team>> GetTeamsWithCapacityAsync(CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetTeamsWithCapacityAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets team members. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(
        TeamId teamId, 
        TeamRole? role = null, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetTeamMembersAsync(teamId, role, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets teams by status as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Team> GetByStatusAsyncEnumerable(DotNetSkills.Domain.TeamCollaboration.Enums.TeamStatus status, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByStatusAsyncEnumerable(status, cancellationToken);
    }

    /// <summary>
    /// Gets teams by user membership as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Team> GetByUserMembershipAsyncEnumerable(UserId userId, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByUserMembershipAsyncEnumerable(userId, cancellationToken);
    }

    /// <summary>
    /// Gets active teams as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<Team> GetActiveTeamsAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetActiveTeamsAsyncEnumerable(cancellationToken);
    }

    /// <summary>
    /// Gets team summaries with caching support.
    /// </summary>
    public async Task<IEnumerable<TeamSummaryProjection>> GetTeamSummariesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("summaries");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTeamSummariesAsync(cancellationToken));
    }

    /// <summary>
    /// Gets team dashboard data with caching support.
    /// </summary>
    public async Task<IEnumerable<TeamDashboardProjection>> GetTeamDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("dashboard");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTeamDashboardDataAsync(cancellationToken));
    }

    /// <summary>
    /// Gets team selections with caching support.
    /// </summary>
    public async Task<IEnumerable<TeamSelectionProjection>> GetTeamSelectionsAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("selections", activeOnly.ToString());
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTeamSelectionsAsync(activeOnly, cancellationToken));
    }

    /// <summary>
    /// Gets user team memberships. This method bypasses cache due to user-specific nature.
    /// </summary>
    public async Task<IEnumerable<TeamMembershipProjection>> GetUserTeamMembershipsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetUserTeamMembershipsAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates entity-specific cache entries.
    /// Called automatically by the base class after add, update, or remove operations.
    /// </summary>
    /// <param name="team">The team entity that was modified.</param>
    protected override void InvalidateEntitySpecificCaches(Team team)
    {
        // Name-based caches
        InvalidateCaches(
            GetNameCacheKey(team.Name),
            GetNameExistsCacheKey(team.Name)
        );
        
        // Status-based caches
        InvalidateCache(GetFilterCacheKey("status", team.Status));
        
        // Aggregated data caches
        InvalidateCaches(
            GetProjectionCacheKey("summaries"),
            GetProjectionCacheKey("dashboard"),
            GetProjectionCacheKey("selections", "True"),
            GetProjectionCacheKey("selections", "False")
        );
    }

    #endregion
}