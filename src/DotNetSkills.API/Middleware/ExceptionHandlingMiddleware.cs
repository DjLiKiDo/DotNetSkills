using DotNetSkills.Application.Common.Exceptions;

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
        // Log with appropriate level based on exception type
        LogException(context, exception);

    var problemDetails = MapToProblemDetails(context, exception);

        // Ensure validation errors are always surfaced in serialized JSON (System.Text.Json may omit
        // derived properties when serializing through base ProblemDetails reference).
        if (problemDetails is ValidationProblemDetails vpd && !problemDetails.Extensions.ContainsKey("errors"))
        {
            problemDetails.Extensions["errors"] = vpd.Errors.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        // Provide a normalized errorCode for clients (snake_case) even when Title is derived differently.
        if (!problemDetails.Extensions.ContainsKey("errorCode"))
        {
            var errorCode = exception switch
            {
                ApplicationExceptionBase appEx => string.IsNullOrWhiteSpace(appEx.ErrorCode) ? DeriveErrorCode(problemDetails.Title) : appEx.ErrorCode,
                DomainException => "domain_rule_violation",
                FluentValidation.ValidationException => "validation_failed",
                ArgumentException => "invalid_argument",
                UnauthorizedAccessException => "unauthorized",
                KeyNotFoundException => "not_found",
                InvalidOperationException => "invalid_operation",
                _ => "internal_server_error"
            };
            problemDetails.Extensions["errorCode"] = errorCode;
        }

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

    /// <summary>
    /// Logs the exception with appropriate level and structured data based on exception type.
    /// </summary>
    /// <param name="context">The HTTP context for additional logging context.</param>
    /// <param name="exception">The exception to log.</param>
    private void LogException(HttpContext context, Exception exception)
    {
        var requestId = context.TraceIdentifier;
        var requestPath = context.Request.Path.Value;
        var requestMethod = context.Request.Method;
        var userAgent = context.Request.Headers.UserAgent.ToString();

        switch (exception)
        {
            case DomainException domainEx:
                _logger.LogWarning(domainEx,
                    "Domain rule violation occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}, Message: {Message}",
                    requestId, requestPath, requestMethod, domainEx.Message);
                break;

            case FluentValidation.ValidationException validationEx:
                _logger.LogWarning(validationEx,
                    "Validation failed. RequestId: {RequestId}, Path: {Path}, Method: {Method}, ValidationErrors: {ValidationErrors}",
                    requestId, requestPath, requestMethod,
                    string.Join("; ", validationEx.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}")));
                break;

            case ApplicationExceptionBase appEx:
                _logger.LogWarning(appEx,
                    "Application exception occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}, StatusCode: {StatusCode}, ErrorCode: {ErrorCode}, Message: {Message}",
                    requestId, requestPath, requestMethod, appEx.StatusCode, appEx.ErrorCode, appEx.Message);
                break;

            case UnauthorizedAccessException:
                _logger.LogWarning(exception,
                    "Unauthorized access attempt. RequestId: {RequestId}, Path: {Path}, Method: {Method}, UserAgent: {UserAgent}",
                    requestId, requestPath, requestMethod, userAgent);
                break;

            case KeyNotFoundException:
                _logger.LogInformation(exception,
                    "Resource not found. RequestId: {RequestId}, Path: {Path}, Method: {Method}",
                    requestId, requestPath, requestMethod);
                break;

            case ArgumentException:
            case InvalidOperationException:
                _logger.LogWarning(exception,
                    "Client error occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}, Message: {Message}",
                    requestId, requestPath, requestMethod, exception.Message);
                break;

            default:
                _logger.LogError(exception,
                    "Unhandled server error occurred. RequestId: {RequestId}, Path: {Path}, Method: {Method}, UserAgent: {UserAgent}, ExceptionType: {ExceptionType}",
                    requestId, requestPath, requestMethod, userAgent, exception.GetType().Name);
                break;
        }
    }

    /// <summary>
    /// Creates a problem details response for FluentValidation exceptions with structured validation errors.
    /// </summary>
    /// <param name="validationException">The validation exception containing validation failures.</param>
    /// <param name="requestPath">The request path for context.</param>
    /// <returns>A ValidationProblemDetails object with structured validation errors.</returns>
    private static ValidationProblemDetails CreateValidationProblemDetails(
        FluentValidation.ValidationException validationException,
        string requestPath)
    {
        var validationProblem = new ValidationProblemDetails
        {
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred",
            Status = StatusCodes.Status400BadRequest,
            Instance = requestPath
        };

        // Group validation errors by property name
        foreach (var error in validationException.Errors)
        {
            if (validationProblem.Errors.ContainsKey(error.PropertyName))
            {
                var existingErrors = validationProblem.Errors[error.PropertyName].ToList();
                existingErrors.Add(error.ErrorMessage);
                validationProblem.Errors[error.PropertyName] = existingErrors.ToArray();
            }
            else
            {
                validationProblem.Errors[error.PropertyName] = [error.ErrorMessage];
            }
        }

        return validationProblem;
    }

    private ProblemDetails MapToProblemDetails(HttpContext context, Exception exception)
    {
        return exception switch
        {
            ApplicationExceptionBase appEx => new ProblemDetails
            {
                Title = FormatTitle(appEx.ErrorCode),
                Detail = appEx.Message,
                Status = appEx.StatusCode,
                Instance = context.Request.Path
            },
            DomainException domainEx => new ProblemDetails
            {
                Title = "Domain Rule Violation",
                Detail = domainEx.Message,
                Status = StatusCodes.Status400BadRequest,
                Instance = context.Request.Path
            },
            FluentValidation.ValidationException validationEx => CreateValidationProblemDetails(validationEx, context.Request.Path),
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
    }

    private static string FormatTitle(string errorCode)
    {
        if (string.IsNullOrWhiteSpace(errorCode)) return "Error";
        // Convert snake_case to Title Case: not_found -> Not Found
        var words = errorCode.Split(['_', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Select(w => char.ToUpperInvariant(w[0]) + w[1..]);
        return string.Join(' ', words);
    }

    private static string DeriveErrorCode(string? title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "error";
        // Convert Title Case / spaced phrase into snake_case
        var span = title.Trim();
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < span.Length; i++)
        {
            var c = span[i];
            if (char.IsWhiteSpace(c) || c == '-' || c == '/')
            {
                if (sb.Length > 0 && sb[^1] != '_') sb.Append('_');
            }
            else if (char.IsUpper(c) && i > 0 && char.IsLetterOrDigit(span[i - 1]) && char.IsLower(span[i - 1]))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }
        var result = sb.ToString().Trim('_');
        return string.IsNullOrWhiteSpace(result) ? "error" : result;
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