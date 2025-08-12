using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotNetSkills.Application.Common.Interfaces;
using DotNetSkills.Application.Common.Models;
using DotNetSkills.Infrastructure.Persistence.Context;

namespace DotNetSkills.Infrastructure.Services;

/// <summary>
/// Implementation of database migration operations using Entity Framework Core.
/// Provides enterprise-grade migration capabilities with proper logging and error handling.
/// </summary>
public class DatabaseMigrator : IDatabaseMigrator
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseMigrator> _logger;

    public DatabaseMigrator(ApplicationDbContext context, ILogger<DatabaseMigrator> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting database migration process...");
            
            // Get pending migrations count for logging
            var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
            var pendingCount = pendingMigrations?.Count() ?? 0;
            
            if (pendingCount > 0)
            {
                _logger.LogInformation("Found {PendingMigrationsCount} pending migrations. Applying migrations...", pendingCount);
                
                // Apply all pending migrations
                // This uses the existing connection string and retry policies configured in DependencyInjection
                await _context.Database.MigrateAsync(cancellationToken);
                
                _logger.LogInformation("Database migration completed successfully. Applied {AppliedMigrationsCount} migrations.", pendingCount);
            }
            else
            {
                _logger.LogInformation("Database is up to date. No pending migrations found.");
            }
            
            // Verify database connection is working
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            if (canConnect)
            {
                _logger.LogInformation("Database connection verified successfully.");
            }
            else
            {
                _logger.LogWarning("Database migration completed but connection verification failed.");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Database migration was cancelled during application startup.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Database migration failed during application startup. " +
                "Application will continue to start, but database operations may fail. " +
                "Check connection string, database server availability, and migration scripts.");
            
            // Re-throw to prevent application startup with invalid database state
            // This ensures fail-fast behavior in containerized environments
            throw new InvalidOperationException(
                "Database migration failed during application startup. See logs for details.", ex);
        }
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task<bool> CanConnectToDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to database during connectivity check.");
            return false;
        }
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task<MigrationStatus> GetMigrationStatusAsync(CancellationToken cancellationToken = default)
    {
        var appliedMigrations = await _context.Database.GetAppliedMigrationsAsync(cancellationToken);
        var pendingMigrations = await _context.Database.GetPendingMigrationsAsync(cancellationToken);
        
        return new MigrationStatus
        {
            AppliedMigrations = [.. appliedMigrations],
            PendingMigrations = [.. pendingMigrations],
            IsUpToDate = !pendingMigrations.Any()
        };
    }
}