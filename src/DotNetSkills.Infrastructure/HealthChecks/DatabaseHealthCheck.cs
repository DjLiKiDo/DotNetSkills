using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DotNetSkills.Infrastructure.HealthChecks;

/// <summary>
/// Health check for database connectivity and basic operations.
/// Verifies that the application can connect to and query the database.
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(ApplicationDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Test basic database connectivity with a simple query
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken)
                .ConfigureAwait(false);
            
            stopwatch.Stop();
            
            var responseTime = stopwatch.ElapsedMilliseconds;
            var data = new Dictionary<string, object>
            {
                ["database"] = _context.Database.GetConnectionString() != null ? "connected" : "disconnected",
                ["response_time_ms"] = responseTime
            };

            // Consider slow response times as degraded
            if (responseTime > 1000) // 1 second threshold
            {
                _logger.LogWarning("Database health check took {ResponseTime}ms, which is above the 1000ms threshold", responseTime);
                return HealthCheckResult.Degraded($"Database is responding slowly ({responseTime}ms)", data: data);
            }

            _logger.LogDebug("Database health check completed successfully in {ResponseTime}ms", responseTime);
            return HealthCheckResult.Healthy("Database is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
        }
    }
}