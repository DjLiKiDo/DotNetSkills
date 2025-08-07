using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DotNetSkills.Infrastructure.Common.Performance;

/// <summary>
/// Service for monitoring and logging performance metrics across the application.
/// Provides centralized performance tracking for repositories, services, and operations.
/// </summary>
public interface IPerformanceMonitoringService
{
    /// <summary>
    /// Measures the execution time of an async operation.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to measure.</param>
    /// <param name="operationName">The name of the operation for logging.</param>
    /// <param name="context">Additional context for the operation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    System.Threading.Tasks.Task<T> MeasureAsync<T>(
        Func<System.Threading.Tasks.Task<T>> operation,
        string operationName,
        string? context = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Measures the execution time of an async operation without return value.
    /// </summary>
    /// <param name="operation">The operation to measure.</param>
    /// <param name="operationName">The name of the operation for logging.</param>
    /// <param name="context">Additional context for the operation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    System.Threading.Tasks.Task MeasureAsync(
        Func<System.Threading.Tasks.Task> operation,
        string operationName,
        string? context = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a performance warning when an operation exceeds the threshold.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="elapsed">The elapsed time.</param>
    /// <param name="threshold">The warning threshold.</param>
    /// <param name="context">Additional context.</param>
    void LogSlowOperation(string operationName, TimeSpan elapsed, TimeSpan threshold, string? context = null);

    /// <summary>
    /// Records a performance metric for aggregation and analysis.
    /// </summary>
    /// <param name="metricName">The name of the metric.</param>
    /// <param name="value">The metric value.</param>
    /// <param name="unit">The unit of measurement.</param>
    /// <param name="tags">Additional tags for the metric.</param>
    void RecordMetric(string metricName, double value, string unit, IDictionary<string, string>? tags = null);
}

/// <summary>
/// Implementation of the performance monitoring service.
/// Provides comprehensive performance tracking and logging capabilities.
/// </summary>
public class PerformanceMonitoringService : IPerformanceMonitoringService
{
    private readonly ILogger<PerformanceMonitoringService> _logger;
    private readonly PerformanceMonitoringOptions _options;

    /// <summary>
    /// Initializes a new instance of the PerformanceMonitoringService.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">Performance monitoring configuration options.</param>
    public PerformanceMonitoringService(
        ILogger<PerformanceMonitoringService> logger,
        PerformanceMonitoringOptions options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Measures the execution time of an async operation.
    /// </summary>
    public async System.Threading.Tasks.Task<T> MeasureAsync<T>(
        Func<System.Threading.Tasks.Task<T>> operation,
        string operationName,
        string? context = null,
        CancellationToken cancellationToken = default)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));
        
        if (string.IsNullOrWhiteSpace(operationName))
            throw new ArgumentException("Operation name cannot be null or empty.", nameof(operationName));

        var stopwatch = Stopwatch.StartNew();
        var operationId = Guid.NewGuid().ToString("N")[..8];

        try
        {
            if (_options.LogOperationStart)
            {
                _logger.LogDebug(
                    "Starting operation {OperationName} [{OperationId}] {Context}",
                    operationName,
                    operationId,
                    context ?? string.Empty);
            }

            var result = await operation().ConfigureAwait(false);
            
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;

            // Log performance metrics
            LogOperationCompletion(operationName, operationId, elapsed, context, true);

            // Check for slow operations
            if (elapsed > _options.SlowOperationThreshold)
            {
                LogSlowOperation(operationName, elapsed, _options.SlowOperationThreshold, context);
            }

            // Record metrics
            RecordMetric($"operation.{operationName}.duration", elapsed.TotalMilliseconds, "ms", 
                context != null ? new Dictionary<string, string> { ["context"] = context } : null);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogOperationCompletion(operationName, operationId, stopwatch.Elapsed, context, false, ex);
            throw;
        }
    }

    /// <summary>
    /// Measures the execution time of an async operation without return value.
    /// </summary>
    public async System.Threading.Tasks.Task MeasureAsync(
        Func<System.Threading.Tasks.Task> operation,
        string operationName,
        string? context = null,
        CancellationToken cancellationToken = default)
    {
        await MeasureAsync(async () =>
        {
            await operation().ConfigureAwait(false);
            return System.Threading.Tasks.Task.CompletedTask;
        }, operationName, context, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Logs a performance warning when an operation exceeds the threshold.
    /// </summary>
    public void LogSlowOperation(string operationName, TimeSpan elapsed, TimeSpan threshold, string? context = null)
    {
        _logger.LogWarning(
            "Slow operation detected: {OperationName} took {ElapsedMs}ms (threshold: {ThresholdMs}ms) {Context}",
            operationName,
            elapsed.TotalMilliseconds,
            threshold.TotalMilliseconds,
            context ?? string.Empty);
    }

    /// <summary>
    /// Records a performance metric for aggregation and analysis.
    /// </summary>
    public void RecordMetric(string metricName, double value, string unit, IDictionary<string, string>? tags = null)
    {
        if (!_options.EnableMetricRecording)
            return;

        var tagsString = tags != null && tags.Count > 0 
            ? string.Join(", ", tags.Select(kvp => $"{kvp.Key}={kvp.Value}"))
            : string.Empty;

        _logger.LogInformation(
            "Metric recorded: {MetricName} = {Value} {Unit} {Tags}",
            metricName,
            value,
            unit,
            tagsString);

        // In a production environment, you would send this to a metrics backend
        // like Application Insights, Prometheus, or similar
    }

    /// <summary>
    /// Logs the completion of an operation with performance details.
    /// </summary>
    private void LogOperationCompletion(
        string operationName,
        string operationId,
        TimeSpan elapsed,
        string? context,
        bool success,
        Exception? exception = null)
    {
        var logLevel = success ? LogLevel.Debug : LogLevel.Warning;

        if (success)
        {
            _logger.Log(logLevel,
                "Completed operation {OperationName} [{OperationId}] in {ElapsedMs}ms {Context}",
                operationName,
                operationId,
                elapsed.TotalMilliseconds,
                context ?? string.Empty);
        }
        else
        {
            _logger.Log(logLevel, exception,
                "Failed operation {OperationName} [{OperationId}] after {ElapsedMs}ms {Context}",
                operationName,
                operationId,
                elapsed.TotalMilliseconds,
                context ?? string.Empty);
        }
    }
}

/// <summary>
/// Configuration options for performance monitoring.
/// </summary>
public class PerformanceMonitoringOptions
{
    /// <summary>
    /// Gets or sets the threshold for considering an operation as slow.
    /// Default is 1000ms (1 second).
    /// </summary>
    public TimeSpan SlowOperationThreshold { get; set; } = TimeSpan.FromMilliseconds(1000);

    /// <summary>
    /// Gets or sets whether to log operation start events.
    /// Default is false to reduce log noise.
    /// </summary>
    public bool LogOperationStart { get; set; } = false;

    /// <summary>
    /// Gets or sets whether to enable metric recording.
    /// Default is true.
    /// </summary>
    public bool EnableMetricRecording { get; set; } = true;

    /// <summary>
    /// Gets or sets the threshold for database query warnings.
    /// Default is 500ms.
    /// </summary>
    public TimeSpan DatabaseQueryWarningThreshold { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets the threshold for repository operation warnings.
    /// Default is 2000ms (2 seconds).
    /// </summary>
    public TimeSpan RepositoryOperationWarningThreshold { get; set; } = TimeSpan.FromMilliseconds(2000);
}
