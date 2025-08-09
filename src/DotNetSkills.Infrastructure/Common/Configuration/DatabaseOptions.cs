using System.ComponentModel.DataAnnotations;

namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Configuration options for database connection and behavior.
/// </summary>
public class DatabaseOptions
{
    /// <summary>
    /// Gets or sets the database connection string.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command timeout in seconds.
    /// </summary>
    [Range(1, 300)]
    public int CommandTimeout { get; set; } = 30;

    /// <summary>
    /// Gets or sets the maximum retry count for database operations.
    /// </summary>
    [Range(0, 10)]
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the maximum retry delay in seconds.
    /// </summary>
    [Range(1, 60)]
    public int MaxRetryDelaySeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets whether sensitive data logging is enabled.
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets whether detailed errors are enabled.
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;

    /// <summary>
    /// Gets or sets whether query logging is enabled.
    /// </summary>
    public bool EnableQueryLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets the migrations assembly name.
    /// </summary>
    public string? MigrationsAssembly { get; set; }
}
