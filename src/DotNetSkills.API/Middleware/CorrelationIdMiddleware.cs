namespace DotNetSkills.API.Middleware;

/// <summary>
/// Middleware for handling correlation IDs in HTTP requests and responses.
/// Adds X-Correlation-Id header to all responses for request tracing.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    
    public const string CorrelationIdHeaderName = "X-Correlation-Id";
    public const string CorrelationIdKey = "CorrelationId";

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from incoming request header
        var correlationId = GetOrCreateCorrelationId(context);
        
        // Store correlation ID in HttpContext items for access throughout the request pipeline
        context.Items[CorrelationIdKey] = correlationId;
        
        // Add correlation ID to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        
        // Add correlation ID to logging scope
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        });
        
        await _next(context);
    }

    /// <summary>
    /// Gets the correlation ID from the request header or creates a new one if not present.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The correlation ID.</returns>
    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        // Check if correlation ID is provided in request headers
        if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdValues) &&
            !string.IsNullOrWhiteSpace(correlationIdValues.FirstOrDefault()))
        {
            return correlationIdValues.First()!;
        }

        // Generate new correlation ID if not provided
        return Guid.NewGuid().ToString("D");
    }
}

/// <summary>
/// Extension methods for registering correlation ID middleware.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    /// <summary>
    /// Adds correlation ID middleware to the application pipeline.
    /// Should be added early in the pipeline to ensure correlation ID is available for all subsequent middleware.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <returns>The application builder for method chaining.</returns>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}
