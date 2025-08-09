using System.Diagnostics;
using DotNetSkills.Infrastructure.Common.Performance;

namespace DotNetSkills.API.Middleware;

/// <summary>
/// Middleware for logging request performance metrics.
/// Measures and logs the execution time of HTTP requests with detailed performance data.
/// </summary>
public class PerformanceLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceLoggingMiddleware> _logger;
    private readonly IPerformanceMonitoringService _performanceService;
    private readonly PerformanceLoggingOptions _options;

    /// <summary>
    /// Initializes a new instance of the PerformanceLoggingMiddleware.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="performanceService">The performance monitoring service.</param>
    /// <param name="options">Performance logging configuration options.</param>
    public PerformanceLoggingMiddleware(
        RequestDelegate next,
        ILogger<PerformanceLoggingMiddleware> logger,
        IPerformanceMonitoringService performanceService,
        PerformanceLoggingOptions options)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _performanceService = performanceService ?? throw new ArgumentNullException(nameof(performanceService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Invokes the middleware to measure and log request performance.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Skip performance logging for certain endpoints if configured
        if (ShouldSkipLogging(context))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;
        var operationName = $"{method} {path}";

        try
        {
            if (_options.LogRequestStart)
            {
                _logger.LogDebug(
                    "Starting request {Method} {Path} [{RequestId}]",
                    method,
                    path,
                    requestId);
            }

            await _next(context);

            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            var statusCode = context.Response.StatusCode;

            // Log request completion
            LogRequestCompletion(method, path, requestId, elapsed, statusCode, true);

            // Check for slow requests
            if (elapsed > _options.SlowRequestThreshold)
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} [{RequestId}] took {ElapsedMs}ms (Status: {StatusCode})",
                    method,
                    path,
                    requestId,
                    elapsed.TotalMilliseconds,
                    statusCode);
            }

            // Record performance metrics
            var tags = new Dictionary<string, string>
            {
                ["method"] = method,
                ["path"] = NormalizePath(path),
                ["status_code"] = statusCode.ToString(),
                ["status_class"] = GetStatusClass(statusCode)
            };

            _performanceService.RecordMetric("http.request.duration", elapsed.TotalMilliseconds, "ms", tags);
            _performanceService.RecordMetric("http.request.count", 1, "count", tags);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            LogRequestCompletion(method, path, requestId, stopwatch.Elapsed, context.Response.StatusCode, false, ex);
            throw;
        }
    }

    /// <summary>
    /// Determines whether to skip performance logging for a request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>True if logging should be skipped, otherwise false.</returns>
    private bool ShouldSkipLogging(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        if (path == null)
            return false;

        // Skip health check and monitoring endpoints
        return _options.SkipPaths.Any(skipPath => path.StartsWith(skipPath, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Logs the completion of a request with performance details.
    /// </summary>
    private void LogRequestCompletion(
        string method,
        string path,
        string requestId,
        TimeSpan elapsed,
        int statusCode,
        bool success,
        Exception? exception = null)
    {
        var logLevel = DetermineLogLevel(statusCode, elapsed, success);

        if (success)
        {
            _logger.Log(logLevel,
                "Completed request {Method} {Path} [{RequestId}] in {ElapsedMs}ms (Status: {StatusCode})",
                method,
                path,
                requestId,
                elapsed.TotalMilliseconds,
                statusCode);
        }
        else
        {
            _logger.Log(logLevel, exception,
                "Failed request {Method} {Path} [{RequestId}] after {ElapsedMs}ms (Status: {StatusCode})",
                method,
                path,
                requestId,
                elapsed.TotalMilliseconds,
                statusCode);
        }
    }

    /// <summary>
    /// Determines the appropriate log level for a request based on status and performance.
    /// </summary>
    private LogLevel DetermineLogLevel(int statusCode, TimeSpan elapsed, bool success)
    {
        if (!success || statusCode >= 500)
            return LogLevel.Error;

        if (statusCode >= 400)
            return LogLevel.Warning;

        if (elapsed > _options.SlowRequestThreshold)
            return LogLevel.Warning;

        return _options.DefaultLogLevel;
    }

    /// <summary>
    /// Normalizes the path for metrics by removing dynamic segments.
    /// </summary>
    private static string NormalizePath(string path)
    {
        // Replace GUIDs and numbers with placeholders for better metric aggregation
        var normalized = path.ToLowerInvariant();
        
        // Replace GUIDs with {id}
        normalized = System.Text.RegularExpressions.Regex.Replace(
            normalized, 
            @"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", 
            "{id}");
        
        // Replace numeric IDs with {id}
        normalized = System.Text.RegularExpressions.Regex.Replace(
            normalized,
            @"/\d+",
            "/{id}");

        return normalized;
    }

    /// <summary>
    /// Gets the status class (2xx, 3xx, 4xx, 5xx) for metrics grouping.
    /// </summary>
    private static string GetStatusClass(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => "2xx",
            >= 300 and < 400 => "3xx",
            >= 400 and < 500 => "4xx",
            >= 500 => "5xx",
            _ => "1xx"
        };
    }
}

/// <summary>
/// Configuration options for performance logging middleware.
/// </summary>
public class PerformanceLoggingOptions
{
    /// <summary>
    /// Gets or sets the threshold for considering a request as slow.
    /// Default is 2000ms (2 seconds).
    /// </summary>
    public TimeSpan SlowRequestThreshold { get; set; } = TimeSpan.FromMilliseconds(2000);

    /// <summary>
    /// Gets or sets whether to log request start events.
    /// Default is false to reduce log noise.
    /// </summary>
    public bool LogRequestStart { get; set; } = false;

    /// <summary>
    /// Gets or sets the default log level for successful requests.
    /// Default is Information.
    /// </summary>
    public LogLevel DefaultLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets the paths to skip for performance logging.
    /// Default includes health check and monitoring endpoints.
    /// </summary>
    public List<string> SkipPaths { get; set; } = new()
    {
        "/health",
        "/metrics",
        "/swagger",
        "/favicon.ico"
    };
}

/// <summary>
/// Extension methods for configuring performance logging middleware.
/// </summary>
public static class PerformanceLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds performance logging middleware to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="options">Optional configuration options.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UsePerformanceLogging(
        this IApplicationBuilder builder,
        PerformanceLoggingOptions? options = null)
    {
        options ??= new PerformanceLoggingOptions();
        
        return builder.UseMiddleware<PerformanceLoggingMiddleware>(options);
    }
}
