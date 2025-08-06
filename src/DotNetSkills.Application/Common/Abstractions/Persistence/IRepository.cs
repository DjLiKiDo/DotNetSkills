namespace DotNetSkills.Application.Common.Interfaces.Persistence;

/// <summary>
/// Base repository interface with common CRUD operations.
/// All entity repositories should inherit from this interface.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
public interface IRepository<TEntity, TId> 
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The entity identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The entity if found, null otherwise</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added entity</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">The entity to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}

/// <summary>
/// Read-only repository interface for query operations.
/// Use this for read-side operations in CQRS scenarios.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
public interface IReadOnlyRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Retrieves an entity by its identifier.
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all entities with optional pagination.
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if an entity exists with the given identifier.
    /// </summary>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Write-only repository interface for command operations.
/// Use this for write-side operations in CQRS scenarios.
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier type</typeparam>
public interface IWriteOnlyRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
}
