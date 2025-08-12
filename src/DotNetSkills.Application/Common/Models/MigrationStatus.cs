namespace DotNetSkills.Application.Common.Models;

/// <summary>
/// Represents the current migration status of the database.
/// Contains information about applied and pending migrations for monitoring and diagnostics.
/// </summary>
public record MigrationStatus
{
    /// <summary>
    /// Gets the list of migrations that have been applied to the database.
    /// </summary>
    public required List<string> AppliedMigrations { get; init; } = [];
    
    /// <summary>
    /// Gets the list of migrations that are pending application.
    /// </summary>
    public required List<string> PendingMigrations { get; init; } = [];
    
    /// <summary>
    /// Gets a value indicating whether the database is up to date (no pending migrations).
    /// </summary>
    public required bool IsUpToDate { get; init; }
    
    /// <summary>
    /// Gets the total number of applied migrations.
    /// </summary>
    public int AppliedCount => AppliedMigrations.Count;
    
    /// <summary>
    /// Gets the total number of pending migrations.
    /// </summary>
    public int PendingCount => PendingMigrations.Count;
}