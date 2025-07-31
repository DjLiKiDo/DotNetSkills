namespace DotNetSkills.Domain.Repositories;

/// <summary>
/// Unit of Work pattern contract for coordinating transactions across multiple repositories.
/// Ensures data consistency and transactional integrity in complex operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves all changes made in the current unit of work to the underlying data store.
    /// This should dispatch domain events and handle all aggregate changes atomically.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>The number of affected records.</returns>
    System.Threading.Tasks.Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begins a new database transaction.
    /// Use this for operations that span multiple repository calls and need explicit transaction control.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    System.Threading.Tasks.Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commits the current transaction.
    /// Should be called after BeginTransactionAsync and all repository operations are complete.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    System.Threading.Tasks.Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rolls back the current transaction.
    /// Should be called if any error occurs during a transactional operation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    System.Threading.Tasks.Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the user repository instance.
    /// Ensures all repositories share the same database context/transaction.
    /// </summary>
    IUserRepository Users { get; }
    
    /// <summary>
    /// Gets the team repository instance.
    /// Ensures all repositories share the same database context/transaction.
    /// </summary>
    ITeamRepository Teams { get; }
    
    /// <summary>
    /// Gets the project repository instance.
    /// Ensures all repositories share the same database context/transaction.
    /// </summary>
    IProjectRepository Projects { get; }
    
    /// <summary>
    /// Gets the task repository instance.
    /// Ensures all repositories share the same database context/transaction.
    /// </summary>
    ITaskRepository Tasks { get; }
}
