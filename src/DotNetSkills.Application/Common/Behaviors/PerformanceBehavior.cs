using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for monitoring and logging performance of commands and queries.
/// Measures execution time and logs slow operations based on configurable thresholds.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly PerformanceBehaviorOptions _options;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IOptions<PerformanceBehaviorOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = GetRequestName(request);
        
        // Skip monitoring for excluded patterns
        if (ShouldExcludeRequest(requestName))
        {
            return await next().ConfigureAwait(false);
        }

        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            _logger.LogDebug("Starting execution of {RequestName}", requestName);
            
            var response = await next().ConfigureAwait(false);
            
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            
            LogPerformanceResult(requestName, elapsedMilliseconds, success: true);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            
            LogPerformanceResult(requestName, elapsedMilliseconds, success: false, ex);
            
            throw;
        }
    }

    private static string GetRequestName(TRequest request)
    {
        var requestType = request.GetType();
        return requestType.Name;
    }

    private bool ShouldExcludeRequest(string requestName)
    {
        if (_options.ExcludePatterns?.Any() != true)
            return false;

        return _options.ExcludePatterns.Any(pattern => 
            requestName.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private void LogPerformanceResult(
        string requestName, 
        long elapsedMilliseconds, 
        bool success, 
        Exception? exception = null)
    {
        var requestCategory = GetRequestCategory(requestName);
        
        if (elapsedMilliseconds > _options.SlowOperationThresholdMs)
        {
            if (success)
            {
                _logger.LogWarning(
                    "Slow operation detected: {RequestName} took {ElapsedMilliseconds}ms (Category: {RequestCategory})",
                    requestName, elapsedMilliseconds, requestCategory);
            }
            else
            {
                _logger.LogError(exception,
                    "Slow operation failed: {RequestName} took {ElapsedMilliseconds}ms before failing (Category: {RequestCategory})",
                    requestName, elapsedMilliseconds, requestCategory);
            }
        }
        else
        {
            if (success)
            {
                _logger.LogDebug(
                    "Completed {RequestName} in {ElapsedMilliseconds}ms (Category: {RequestCategory})",
                    requestName, elapsedMilliseconds, requestCategory);
            }
            else
            {
                _logger.LogError(exception,
                    "Failed {RequestName} after {ElapsedMilliseconds}ms (Category: {RequestCategory})",
                    requestName, elapsedMilliseconds, requestCategory);
            }
        }
    }

    private static string GetRequestCategory(string requestName)
    {
        return requestName switch
        {
            var name when name.Contains("Command", StringComparison.OrdinalIgnoreCase) => "Command",
            var name when name.Contains("Query", StringComparison.OrdinalIgnoreCase) => "Query",
            var name when name.Contains("Handler", StringComparison.OrdinalIgnoreCase) => "Handler",
            _ => "Unknown"
        };
    }
}

/// <summary>
/// Configuration options for performance monitoring behavior.
/// </summary>
public class PerformanceBehaviorOptions
{
    /// <summary>
    /// Threshold in milliseconds above which operations are considered slow.
    /// Default is 500ms.
    /// </summary>
    public long SlowOperationThresholdMs { get; set; } = 500;

    /// <summary>
    /// Patterns to exclude from performance monitoring.
    /// Operations matching these patterns will not be measured.
    /// </summary>
    public string[]? ExcludePatterns { get; set; }

    /// <summary>
    /// Whether to include detailed timing information in logs.
    /// Default is false for production environments.
    /// </summary>
    public bool IncludeDetailedTiming { get; set; } = false;
}
