using Microsoft.Extensions.Logging;

namespace DotNetSkills.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that provides structured logging for requests and responses.
/// Logs request execution with correlation IDs, operation context, and execution details.
/// Supports both successful operations and exception handling with proper log levels.
/// </summary>
/// <typeparam name="TRequest">The type of the request being logged.</typeparam>
/// <typeparam name="TResponse">The type of the response from the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the LoggingBehavior class.
    /// </summary>
    /// <param name="logger">The logger instance for structured logging.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs the request execution with structured logging and correlation tracking.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid();
        
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName,
            ["RequestType"] = typeof(TRequest).FullName ?? requestName
        });

        _logger.LogInformation("Handling request {RequestName} with correlation ID {CorrelationId}", 
            requestName, correlationId);

        try
        {
            var response = await next().ConfigureAwait(false);
            
            LogResponse(requestName, correlationId, response);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Request {RequestName} failed with correlation ID {CorrelationId}: {ErrorMessage}", 
                requestName, correlationId, ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Logs successful request completion. Failure scenarios are logged in the catch block of Handle.
    /// (Result wrapper pattern removed per ADR-0001; all failures now expressed via exceptions.)
    /// </summary>
    private void LogResponse(string requestName, Guid correlationId, TResponse _)
    {
        _logger.LogInformation("Request {RequestName} completed successfully with correlation ID {CorrelationId}",
            requestName, correlationId);
    }
}