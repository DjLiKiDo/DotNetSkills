using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DotNetSkills.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that monitors and logs slow-performing operations.
/// Tracks execution time and logs warnings for operations that exceed the configured threshold (500ms).
/// Provides performance metrics and monitoring capabilities for identifying bottlenecks.
/// </summary>
/// <typeparam name="TRequest">The type of the request being monitored.</typeparam>
/// <typeparam name="TResponse">The type of the response from the handler.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    /// <summary>
    /// The threshold in milliseconds above which operations are considered slow.
    /// Operations exceeding this threshold will be logged as warnings.
    /// </summary>
    private const int SlowOperationThresholdMs = 500;

    /// <summary>
    /// Initializes a new instance of the PerformanceBehavior class.
    /// </summary>
    /// <param name="logger">The logger instance for performance monitoring.</param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    /// <summary>
    /// Monitors the execution time of the request and logs performance metrics.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _timer.Start();
        
        try
        {
            var response = await next().ConfigureAwait(false);
            
            _timer.Stop();
            var elapsedMs = _timer.ElapsedMilliseconds;
            
            LogPerformanceMetrics(requestName, elapsedMs);
            
            return response;
        }
        catch (Exception)
        {
            _timer.Stop();
            var elapsedMs = _timer.ElapsedMilliseconds;
            
            // Log performance even for failed operations
            LogPerformanceMetrics(requestName, elapsedMs);
            
            throw;
        }
        finally
        {
            _timer.Reset();
        }
    }

    /// <summary>
    /// Logs performance metrics based on execution time.
    /// Logs warnings for slow operations and information for normal operations.
    /// </summary>
    /// <param name="requestName">The name of the request.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds.</param>
    private void LogPerformanceMetrics(string requestName, long elapsedMs)
    {
        if (elapsedMs > SlowOperationThresholdMs)
        {
            _logger.LogWarning("Slow operation detected: {RequestName} took {ElapsedMs}ms to complete (threshold: {ThresholdMs}ms)", 
                requestName, elapsedMs, SlowOperationThresholdMs);
        }
        else
        {
            _logger.LogInformation("Request {RequestName} completed in {ElapsedMs}ms", 
                requestName, elapsedMs);
        }
    }
}