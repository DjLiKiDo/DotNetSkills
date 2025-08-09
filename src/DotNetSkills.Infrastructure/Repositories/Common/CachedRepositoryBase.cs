namespace DotNetSkills.Infrastructure.Repositories.Common;

/// <summary>
/// Base class for cached repository implementations using the decorator pattern.
/// Provides common caching functionality and patterns to reduce code duplication.
/// </summary>
/// <typeparam name="TEntity">The entity type that must be an aggregate root.</typeparam>
/// <typeparam name="TId">The strongly-typed ID type that must implement IStronglyTypedId&lt;Guid&gt;.</typeparam>
/// <typeparam name="TRepository">The repository interface type.</typeparam>
public abstract class CachedRepositoryBase<TEntity, TId, TRepository>
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
    where TRepository : IRepository<TEntity, TId>
{
    protected readonly TRepository InnerRepository;
    protected readonly IMemoryCache Cache;

    /// <summary>
    /// Default cache options for entity data (longer expiration).
    /// </summary>
    protected static readonly MemoryCacheEntryOptions DefaultCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
        SlidingExpiration = TimeSpan.FromMinutes(2),
        Priority = CacheItemPriority.Normal
    };

    /// <summary>
    /// Short cache options for query results and existence checks.
    /// </summary>
    protected static readonly MemoryCacheEntryOptions ShortCacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
        SlidingExpiration = TimeSpan.FromMinutes(1),
        Priority = CacheItemPriority.Normal
    };

    /// <summary>
    /// The entity name used for cache key generation.
    /// </summary>
    protected abstract string EntityName { get; }

    /// <summary>
    /// Initializes a new instance of the cached repository base class.
    /// </summary>
    /// <param name="innerRepository">The underlying repository implementation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when innerRepository or cache is null.</exception>
    protected CachedRepositoryBase(TRepository innerRepository, IMemoryCache cache)
    {
        InnerRepository = innerRepository ?? throw new ArgumentNullException(nameof(innerRepository));
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    #region Cache Key Generation

    /// <summary>
    /// Generates a cache key for entity lookup by ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <returns>The cache key.</returns>
    protected string GetIdCacheKey(TId id) => $"{EntityName}:id:{id.Value}";

    /// <summary>
    /// Generates a cache key for entity existence check by ID.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <returns>The cache key.</returns>
    protected string GetExistsCacheKey(TId id) => $"{EntityName}:exists:{id.Value}";

    /// <summary>
    /// Generates a cache key for entity lookup by name.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <returns>The cache key.</returns>
    protected string GetNameCacheKey(string name) => $"{EntityName}:name:{name.ToLowerInvariant()}";

    /// <summary>
    /// Generates a cache key for entity existence check by name.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <param name="excludeId">Optional ID to exclude from the check.</param>
    /// <returns>The cache key.</returns>
    protected string GetNameExistsCacheKey(string name, TId? excludeId = default) => 
        $"{EntityName}:name:exists:{name.ToLowerInvariant()}:{excludeId?.Value}";

    /// <summary>
    /// Generates a cache key for filtered entity collections.
    /// </summary>
    /// <param name="filterType">The filter type (e.g., "status", "role").</param>
    /// <param name="filterValue">The filter value.</param>
    /// <returns>The cache key.</returns>
    protected string GetFilterCacheKey(string filterType, object filterValue) => 
        $"{EntityName}:{filterType}:{filterValue}";

    /// <summary>
    /// Generates a cache key for projection queries.
    /// </summary>
    /// <param name="projectionType">The projection type (e.g., "summaries", "dashboard").</param>
    /// <param name="additionalKey">Optional additional key for parameterized projections.</param>
    /// <returns>The cache key.</returns>
    protected string GetProjectionCacheKey(string projectionType, string? additionalKey = null) => 
        string.IsNullOrEmpty(additionalKey) 
            ? $"{EntityName}:{projectionType}" 
            : $"{EntityName}:{projectionType}:{additionalKey}";

    #endregion

    #region Caching Patterns

    /// <summary>
    /// Gets an entity from cache or executes the factory function and caches the result.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="factory">The factory function to execute on cache miss.</param>
    /// <param name="cacheOptions">Optional cache options (uses DefaultCacheOptions if not specified).</param>
    /// <returns>The cached or newly fetched entity.</returns>
    protected async Task<T?> GetCachedAsync<T>(
        string cacheKey, 
        Func<Task<T?>> factory, 
        MemoryCacheEntryOptions? cacheOptions = null) where T : class
    {
        if (Cache.TryGetValue(cacheKey, out T? cachedValue))
            return cachedValue;

        var value = await factory().ConfigureAwait(false);
        
        if (value != null)
        {
            Cache.Set(cacheKey, value, cacheOptions ?? DefaultCacheOptions);
        }
        
        return value;
    }

    /// <summary>
    /// Gets a collection from cache or executes the factory function and caches the result.
    /// </summary>
    /// <typeparam name="T">The collection item type.</typeparam>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="factory">The factory function to execute on cache miss.</param>
    /// <param name="cacheOptions">Optional cache options (uses ShortCacheOptions if not specified).</param>
    /// <returns>The cached or newly fetched collection.</returns>
    protected async Task<IEnumerable<T>> GetCachedCollectionAsync<T>(
        string cacheKey, 
        Func<Task<IEnumerable<T>>> factory, 
        MemoryCacheEntryOptions? cacheOptions = null)
    {
        if (Cache.TryGetValue(cacheKey, out IEnumerable<T>? cachedValue))
            return cachedValue!;

        var value = await factory().ConfigureAwait(false);
        Cache.Set(cacheKey, value, cacheOptions ?? ShortCacheOptions);
        
        return value;
    }

    /// <summary>
    /// Gets a boolean result from cache or executes the factory function and caches the result.
    /// </summary>
    /// <param name="cacheKey">The cache key.</param>
    /// <param name="factory">The factory function to execute on cache miss.</param>
    /// <param name="cacheOptions">Optional cache options (uses ShortCacheOptions if not specified).</param>
    /// <returns>The cached or newly fetched boolean result.</returns>
    protected async Task<bool> GetCachedBoolAsync(
        string cacheKey, 
        Func<Task<bool>> factory, 
        MemoryCacheEntryOptions? cacheOptions = null)
    {
        if (Cache.TryGetValue(cacheKey, out bool cachedValue))
            return cachedValue;

        var value = await factory().ConfigureAwait(false);
        Cache.Set(cacheKey, value, cacheOptions ?? ShortCacheOptions);
        
        return value;
    }

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates cache entries related to a specific entity.
    /// Called after add, update, or remove operations.
    /// </summary>
    /// <param name="entity">The entity that was modified.</param>
    protected virtual void InvalidateEntityCaches(TEntity entity)
    {
        // Basic entity caches
        Cache.Remove(GetIdCacheKey(entity.Id));
        Cache.Remove(GetExistsCacheKey(entity.Id));
        
        // Override in derived classes to add entity-specific invalidations
        InvalidateEntitySpecificCaches(entity);
    }

    /// <summary>
    /// Invalidates entity-specific cache entries.
    /// Override in derived classes to implement custom invalidation logic.
    /// </summary>
    /// <param name="entity">The entity that was modified.</param>
    protected abstract void InvalidateEntitySpecificCaches(TEntity entity);

    /// <summary>
    /// Invalidates a single cache entry.
    /// </summary>
    /// <param name="cacheKey">The cache key to invalidate.</param>
    protected void InvalidateCache(string cacheKey)
    {
        Cache.Remove(cacheKey);
    }

    /// <summary>
    /// Invalidates multiple cache entries.
    /// </summary>
    /// <param name="cacheKeys">The cache keys to invalidate.</param>
    protected void InvalidateCaches(params string[] cacheKeys)
    {
        foreach (var cacheKey in cacheKeys)
        {
            Cache.Remove(cacheKey);
        }
    }

    #endregion

    #region IRepository<TEntity, TId> Base Implementation

    /// <summary>
    /// Gets an entity by ID with caching support.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetIdCacheKey(id);
        return await GetCachedAsync(cacheKey, () => InnerRepository.GetByIdAsync(id, cancellationToken));
    }

    /// <summary>
    /// Checks if an entity exists by ID with caching support.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetExistsCacheKey(id);
        return await GetCachedBoolAsync(cacheKey, () => InnerRepository.ExistsAsync(id, cancellationToken));
    }

    /// <summary>
    /// Gets all entities. Bypasses cache due to potentially large result set.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all entities as an async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of entities.</returns>
    public virtual IAsyncEnumerable<TEntity> GetAllAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetAllAsyncEnumerable(cancellationToken);
    }

    /// <summary>
    /// Gets entities in batches. Bypasses cache for streaming scenarios.
    /// </summary>
    /// <param name="batchSize">The size of each batch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of entity batches.</returns>
    public virtual IAsyncEnumerable<IEnumerable<TEntity>> GetBatchedAsync(int batchSize = 1000, CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetBatchedAsync(batchSize, cancellationToken);
    }

    /// <summary>
    /// Adds a new entity and invalidates related cache entries.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public virtual void Add(TEntity entity)
    {
        InnerRepository.Add(entity);
        InvalidateEntityCaches(entity);
    }

    /// <summary>
    /// Updates an existing entity and invalidates related cache entries.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public virtual void Update(TEntity entity)
    {
        InnerRepository.Update(entity);
        InvalidateEntityCaches(entity);
    }

    /// <summary>
    /// Removes an entity and invalidates related cache entries.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public virtual void Remove(TEntity entity)
    {
        InnerRepository.Remove(entity);
        InvalidateEntityCaches(entity);
    }

    #endregion
}