namespace DotNetSkills.Application.Common.Abstractions;

/// <summary>
/// Generic repository interface for entities with strongly-typed IDs.
/// Provides basic CRUD operations following the Repository pattern.
/// </summary>
/// <typeparam name="TEntity">The entity type that must be an aggregate root.</typeparam>
/// <typeparam name="TId">The strongly-typed ID type that must implement IStronglyTypedId&lt;Guid&gt;.</typeparam>
public interface IRepository<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : IStronglyTypedId<Guid>
{
    /// <summary>
    /// Gets an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity to the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Removes an entity from the repository.
    /// Note: Changes are not persisted until UnitOfWork.SaveChangesAsync is called.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Checks if an entity with the specified ID exists.
    /// </summary>
    /// <param name="id">The entity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity exists, otherwise false.</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}