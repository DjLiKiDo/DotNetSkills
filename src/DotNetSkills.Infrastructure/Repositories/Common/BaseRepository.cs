namespace DotNetSkills.Infrastructure.Repositories.Common;

/// <summary>
/// Base repository implementation providing common CRUD operations for aggregate roots.
/// Implements the generic IRepository interface with Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">The entity type that must be an aggregate root.</typeparam>
/// <typeparam name="TId">The strongly-typed ID type that must implement IStronglyTypedId&lt;Guid&gt;.</typeparam>
public abstract class BaseRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
{
    /// <summary>
    /// The Entity Framework Core database context.
    /// </summary>
    protected readonly ApplicationDbContext Context;

    /// <summary>
    /// The DbSet for the entity type.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Initializes a new instance of the BaseRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    protected BaseRepository(ApplicationDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Gets an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all entities.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .OrderBy(GetDefaultOrderingExpression())
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    public virtual void Add(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        DbSet.Add(entity);
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    public virtual void Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // Attach the entity if it's not being tracked
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        Context.Entry(entity).State = EntityState.Modified;
    }

    /// <summary>
    /// Removes an entity from the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
    public virtual void Remove(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // If the entity is not being tracked, attach it first
        if (Context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }

        DbSet.Remove(entity);
    }

    /// <summary>
    /// Checks if an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
    public virtual async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await DbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets the default ordering expression for queries.
    /// Override this method in derived classes to provide entity-specific ordering.
    /// </summary>
    /// <returns>The default ordering expression.</returns>
    protected virtual Expression<Func<TEntity, object>> GetDefaultOrderingExpression()
    {
        // Default to ordering by CreatedAt if it exists, otherwise by Id
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        
        // Check if entity has CreatedAt property
        var createdAtProperty = typeof(TEntity).GetProperty("CreatedAt");
        if (createdAtProperty != null)
        {
            var property = Expression.Property(parameter, createdAtProperty);
            var convert = Expression.Convert(property, typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(convert, parameter);
        }

        // Fall back to Id
        var idProperty = Expression.Property(parameter, "Id");
        var convertId = Expression.Convert(idProperty, typeof(object));
        return Expression.Lambda<Func<TEntity, object>>(convertId, parameter);
    }

    /// <summary>
    /// Creates a queryable for the entity with optional tracking.
    /// Use this method in derived classes for complex queries.
    /// </summary>
    /// <param name="asNoTracking">Whether to use AsNoTracking for read-only scenarios.</param>
    /// <returns>An IQueryable for the entity.</returns>
    protected IQueryable<TEntity> Query(bool asNoTracking = true)
    {
        return asNoTracking 
            ? DbSet.AsNoTracking() 
            : DbSet;
    }

    /// <summary>
    /// Creates a queryable for the entity with Include operations for related data.
    /// Use this method for queries that need related entities loaded.
    /// </summary>
    /// <param name="includeExpressions">The Include expressions for related data.</param>
    /// <returns>An IQueryable with the specified includes.</returns>
    protected IQueryable<TEntity> QueryWithIncludes(params Expression<Func<TEntity, object>>[] includeExpressions)
    {
        var query = DbSet.AsNoTracking();
        
        foreach (var includeExpression in includeExpressions)
        {
            query = query.Include(includeExpression);
        }

        return query;
    }

    /// <summary>
    /// Gets a paginated result set with the specified query.
    /// </summary>
    /// <param name="query">The base query to paginate.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple containing the items and total count.</returns>
    protected async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync<T>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) where T : class
    {
        if (pageNumber <= 0)
            throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));
        
        if (pageSize <= 0)
            throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ((IEnumerable<TEntity>)items, totalCount);
    }
}
