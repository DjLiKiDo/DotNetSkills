namespace DotNetSkills.Application.Common.Interfaces;

/// <summary>
/// Defines the contract for database migration operations.
/// This interface provides methods for managing database schema migrations
/// in a framework-agnostic way, supporting enterprise deployment scenarios.
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// Applies all pending database migrations asynchronously.
    /// This method should be called during application startup to ensure
    /// the database schema is up to date with the current application version.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous migration operation.</returns>
    /// <remarks>
    /// Implementation should:
    /// - Check for pending migrations
    /// - Apply migrations in correct order
    /// - Log progress and results
    /// - Handle errors gracefully
    /// - Support cancellation for graceful shutdowns
    /// </remarks>
    System.Threading.Tasks.Task MigrateAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if the database exists and is accessible.
    /// Useful for health checks and startup validation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>True if the database exists and is accessible, false otherwise.</returns>
    System.Threading.Tasks.Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the current migration status including applied and pending migrations.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>The migration status information.</returns>
    System.Threading.Tasks.Task<MigrationStatus> GetMigrationStatusAsync(CancellationToken cancellationToken = default);
}