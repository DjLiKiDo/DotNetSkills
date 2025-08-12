using DotNetSkills.Application.Common.Interfaces;

namespace DotNetSkills.API.Extensions;

/// <summary>
/// Extension methods for WebApplication to handle database migration operations.
/// Provides a clean API for integrating database migrations into application startup.
/// </summary>
public static class DatabaseMigrationExtensions
{
    /// <summary>
    /// Runs database migrations if the RUN_MIGRATIONS environment variable is set to "true".
    /// This method follows DevOps best practices by making migrations configurable via environment variables.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous migration operation.</returns>
    /// <remarks>
    /// This extension method:
    /// - Checks the RUN_MIGRATIONS environment variable (case-insensitive)
    /// - Only runs migrations when explicitly enabled
    /// - Uses the registered IDatabaseMigrator service
    /// - Supports cancellation for graceful shutdowns
    /// - Logs migration decisions and results
    /// </remarks>
    public static async Task RunDatabaseMigrationsAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        
        // Check if migrations should be run based on environment variable
        var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS");
        var shouldRunMigrations = string.Equals(runMigrations, "true", StringComparison.OrdinalIgnoreCase);
        
        if (!shouldRunMigrations)
        {
            logger.LogInformation("Database migrations skipped. Set RUN_MIGRATIONS=true to enable automatic migrations.");
            return;
        }
        
        logger.LogInformation("RUN_MIGRATIONS=true detected. Starting database migration process...");
        
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
        
        try
        {
            await migrator.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migration process completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration failed. Application startup will be aborted.");
            throw;
        }
    }
    
    /// <summary>
    /// Checks if the database is accessible and optionally returns migration status.
    /// Useful for health checks and startup validation.
    /// </summary>
    /// <param name="app">The web application instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the database is accessible, false otherwise.</returns>
    public static async Task<bool> CheckDatabaseHealthAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>();
        
        return await migrator.CanConnectToDatabaseAsync(cancellationToken);
    }
}