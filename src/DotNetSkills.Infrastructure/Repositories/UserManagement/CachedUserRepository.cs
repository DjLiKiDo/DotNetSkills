using DotNetSkills.Application.UserManagement.Projections;

/// <summary>
/// Cached implementation of the IUserRepository interface using the Decorator pattern.
/// Provides in-memory caching for frequently accessed user data to improve performance.
/// Falls back to the underlying repository for cache misses and write operations.
/// </summary>
public class CachedUserRepository : CachedRepositoryBase<User, UserId, IUserRepository>, IUserRepository
{
    /// <summary>
    /// The entity name used for cache key generation.
    /// </summary>
    protected override string EntityName => "user";

    /// <summary>
    /// Initializes a new instance of the CachedUserRepository class.
    /// </summary>
    /// <param name="innerRepository">The underlying repository implementation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when innerRepository or cache is null.</exception>
    public CachedUserRepository(IUserRepository innerRepository, IMemoryCache cache) 
        : base(innerRepository, cache)
    {
    }

    #region IUserRepository Specific Methods

    /// <summary>
    /// Gets a user by their email with caching support.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    public async Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{EntityName}:email:{email.Value.ToLowerInvariant()}";
        
        return await GetCachedAsync(cacheKey, async () =>
        {
            var user = await InnerRepository.GetByEmailAsync(email, cancellationToken).ConfigureAwait(false);
            
            if (user != null)
            {
                // Also cache by ID for consistency
                Cache.Set(GetIdCacheKey(user.Id), user, DefaultCacheOptions);
            }
            
            return user;
        });
    }

    /// <summary>
    /// Checks if a user exists by email with caching support.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if a user with the email exists, otherwise false.</returns>
    public async Task<bool> ExistsByEmailAsync(EmailAddress email, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{EntityName}:email:exists:{email.Value.ToLowerInvariant()}";
        
        return await GetCachedBoolAsync(cacheKey, () => InnerRepository.ExistsByEmailAsync(email, cancellationToken));
    }

    /// <summary>
    /// Gets users by role with caching support.
    /// </summary>
    /// <param name="role">The user role to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified role.</returns>
    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("role", role);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByRoleAsync(role, cancellationToken));
    }

    /// <summary>
    /// Gets users by status with caching support.
    /// </summary>
    /// <param name="status">The user status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of users with the specified status.</returns>
    public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("status", status);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByStatusAsync(status, cancellationToken));
    }

    /// <summary>
    /// Gets paginated users. This method bypasses cache due to dynamic nature of pagination.
    /// </summary>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        UserRole? role = null,
        UserStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, role, status, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets users by team membership. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<User>> GetByTeamMembershipAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetByTeamMembershipAsync(teamId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets users by role as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<User> GetByRoleAsyncEnumerable(UserRole role, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByRoleAsyncEnumerable(role, cancellationToken);
    }

    /// <summary>
    /// Gets users by status as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<User> GetByStatusAsyncEnumerable(UserStatus status, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByStatusAsyncEnumerable(status, cancellationToken);
    }

    /// <summary>
    /// Gets active users as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<User> GetActiveUsersAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetActiveUsersAsyncEnumerable(cancellationToken);
    }

    /// <summary>
    /// Gets user summaries with caching support.
    /// </summary>
    public async Task<IEnumerable<UserSummaryProjection>> GetUserSummariesAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("summaries");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetUserSummariesAsync(cancellationToken));
    }

    /// <summary>
    /// Gets user dashboard data with caching support.
    /// </summary>
    public async Task<IEnumerable<UserDashboardProjection>> GetUserDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("dashboard");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetUserDashboardDataAsync(cancellationToken));
    }

    /// <summary>
    /// Gets user selections with caching support.
    /// </summary>
    public async Task<IEnumerable<UserSelectionProjection>> GetUserSelectionsAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("selections", activeOnly.ToString());
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetUserSelectionsAsync(activeOnly, cancellationToken));
    }

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates entity-specific cache entries.
    /// Called automatically by the base class after add, update, or remove operations.
    /// </summary>
    /// <param name="user">The user entity that was modified.</param>
    protected override void InvalidateEntitySpecificCaches(User user)
    {
        // Email-based caches
        InvalidateCaches(
            $"{EntityName}:email:{user.Email.Value.ToLowerInvariant()}",
            $"{EntityName}:email:exists:{user.Email.Value.ToLowerInvariant()}"
        );
        
        // Role and status based caches
        InvalidateCaches(
            GetFilterCacheKey("role", user.Role),
            GetFilterCacheKey("status", user.Status)
        );
        
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