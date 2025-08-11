
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetSkills.Infrastructure.HealthChecks;

/// <summary>
/// Health check for memory cache functionality.
/// Verifies that the memory cache is operational by performing basic set/get operations.
/// </summary>
public class CacheHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheHealthCheck> _logger;
    private readonly string _testKey = "__health_check_test_key__";

    public CacheHealthCheck(IMemoryCache cache, ILogger<CacheHealthCheck> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var testValue = Guid.NewGuid().ToString();
            
            // Test cache set operation
            _cache.Set(_testKey, testValue, TimeSpan.FromSeconds(1));
            
            // Test cache get operation
            var retrievedValue = _cache.Get<string>(_testKey);
            
            stopwatch.Stop();
            
            if (retrievedValue != testValue)
            {
                _logger.LogWarning("Cache health check failed: retrieved value does not match set value");
                return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Unhealthy("Cache is not functioning correctly"));
            }
            
            // Clean up test key
            _cache.Remove(_testKey);
            
            var data = new Dictionary<string, object>
            {
                ["cache_type"] = "MemoryCache",
                ["operation_time_ms"] = stopwatch.ElapsedMilliseconds
            };

            _logger.LogDebug("Cache health check completed successfully in {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);
            return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Healthy("Cache is healthy", data));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache health check failed");
            return System.Threading.Tasks.Task.FromResult(HealthCheckResult.Unhealthy("Cache is unhealthy", ex));
        }
    }
}