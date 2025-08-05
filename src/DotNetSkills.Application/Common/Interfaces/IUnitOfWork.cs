namespace DotNetSkills.Application.Common.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions and coordinating changes across multiple repositories.
/// Implements the Unit of Work pattern to ensure data consistency and transaction boundaries.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the User repository.
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the underlying data store asynchronously.
    /// This method commits the current transaction and persists all pending changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entities that were affected by the save operation.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// Use this when you need explicit transaction control beyond the default SaveChanges behavior.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current database transaction asynchronously.
    /// Should be called after BeginTransactionAsync and successful operations.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current database transaction asynchronously.
    /// Should be called when an error occurs during transaction processing.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}