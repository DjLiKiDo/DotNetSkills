namespace DotNetSkills.API.Middleware;

/// <summary>
/// Middleware for handling exceptions and converting them to appropriate HTTP responses.
/// Provides centralized error handling with proper status codes and problem details.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred. RequestId: {RequestId}, Path: {Path}",
            context.TraceIdentifier, context.Request.Path);

        var problemDetails = exception switch
        {
            DomainException domainEx => new ProblemDetails
            {
                Title = "Domain Rule Violation",
                Detail = domainEx.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path
            },
            ArgumentException argEx => new ProblemDetails
            {
                Title = "Invalid Argument",
                Detail = argEx.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "You are not authorized to access this resource",
                Status = StatusCodes.Status401Unauthorized,
                Instance = context.Request.Path
            },
            KeyNotFoundException => new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = "The requested resource was not found",
                Status = StatusCodes.Status404NotFound,
                Instance = context.Request.Path
            },
            InvalidOperationException invalidOpEx => new ProblemDetails
            {
                Title = "Invalid Operation",
                Detail = invalidOpEx.Message,
                Status = StatusCodes.Status422UnprocessableEntity,
                Instance = context.Request.Path
            },
            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            }
        };

        // Add additional context information
        problemDetails.Extensions["requestId"] = context.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        // Include stack trace in development
        if (_environment.IsDevelopment() && exception is not DomainException)
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
}

/// <summary>
/// Extension methods for registering the exception handling middleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adds exception handling middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder to configure.</param>
    /// <returns>The application builder for method chaining.</returns>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}