using Microsoft.Extensions.Logging;

namespace DotNetSkills.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that dispatches domain events after successful command execution.
/// This behavior ensures that domain events are published only after the command handler
/// completes successfully, maintaining consistency and proper event ordering.
/// Only applies to commands (requests that don't return data) to avoid dispatching events for queries.
/// </summary>
/// <typeparam name="TRequest">The type of the request being processed.</typeparam>
/// <typeparam name="TResponse">The type of the response from the handler.</typeparam>
public class DomainEventDispatchBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IDomainEventDispatcher _eventDispatcher;
    private readonly IDomainEventCollectionService _eventCollectionService;
    private readonly ILogger<DomainEventDispatchBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the DomainEventDispatchBehavior class.
    /// </summary>
    /// <param name="eventDispatcher">The domain event dispatcher service.</param>
    /// <param name="eventCollectionService">The domain event collection service.</param>
    /// <param name="logger">The logger instance for event dispatching operations.</param>
    public DomainEventDispatchBehavior(
        IDomainEventDispatcher eventDispatcher,
        IDomainEventCollectionService eventCollectionService,
        ILogger<DomainEventDispatchBehavior<TRequest, TResponse>> logger)
    {
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _eventCollectionService = eventCollectionService ?? throw new ArgumentNullException(nameof(eventCollectionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the handler and dispatches domain events after successful completion.
    /// Domain events are only dispatched for successful operations to maintain consistency.
    /// </summary>
    /// <param name="request">The request being processed.</param>
    /// <param name="next">The next handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the next handler.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogDebug("Processing {RequestName} in DomainEventDispatchBehavior", requestName);
        
        // Execute the handler first
        var response = await next().ConfigureAwait(false);
        
        // Only dispatch events for successful operations and commands (not queries)
        if (IsSuccessfulCommand(response) && IsCommand(requestName))
        {
            try
            {
                // Collect domain events from tracked aggregate roots
                var modifiedAggregates = _eventCollectionService.GetModifiedAggregates();
                
                if (modifiedAggregates.Any())
                {
                    _logger.LogInformation(
                        "Found {AggregateCount} modified aggregates with domain events for {RequestName}",
                        modifiedAggregates.Count,
                        requestName);

                    // Collect all domain events from all modified aggregates
                    var allDomainEvents = modifiedAggregates
                        .SelectMany(aggregate => aggregate.DomainEvents)
                        .ToList();

                    if (allDomainEvents.Any())
                    {
                        _logger.LogInformation(
                            "Dispatching {EventCount} domain events for {RequestName}",
                            allDomainEvents.Count,
                            requestName);

                        // Dispatch all domain events
                        await _eventDispatcher.DispatchAsync(allDomainEvents, cancellationToken)
                            .ConfigureAwait(false);

                        // Clear domain events from all aggregates after successful dispatch
                        foreach (var aggregate in modifiedAggregates)
                        {
                            aggregate.ClearDomainEvents();
                        }

                        // Clear tracked aggregates
                        _eventCollectionService.ClearTrackedAggregates();

                        _logger.LogInformation(
                            "Successfully dispatched {EventCount} domain events for {RequestName}",
                            allDomainEvents.Count,
                            requestName);
                    }
                    else
                    {
                        _logger.LogDebug(
                            "No domain events found in {AggregateCount} tracked aggregates for {RequestName}",
                            modifiedAggregates.Count,
                            requestName);
                    }
                }
                else
                {
                    _logger.LogDebug("No modified aggregates found for domain event dispatching in {RequestName}", requestName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Failed to dispatch domain events for {RequestName}: {ErrorMessage}", 
                    requestName, ex.Message);
                
                // Decision: Should we fail the entire operation if event dispatching fails?
                // For this implementation, we'll log the error but not fail the operation
                // This maintains consistency with the primary operation while allowing for event system issues
                // In production, consider implementing a retry mechanism or dead letter queue
            }
        }
        else
        {
            _logger.LogDebug(
                "Skipping domain event dispatch for {RequestName} (IsSuccessful: {IsSuccessful}, IsCommand: {IsCommand})",
                requestName,
                IsSuccessfulCommand(response),
                IsCommand(requestName));
        }
        
        return response;
    }

    /// <summary>
    /// Determines if the response indicates a successful operation.
    /// Checks Result and Result{T} types for success status.
    /// </summary>
    /// <param name="response">The response to check.</param>
    /// <returns>True if the operation was successful, false otherwise.</returns>
    private static bool IsSuccessfulCommand(TResponse response)
    {
        // Check if response is a Result type
        if (response is Result result)
        {
            return result.IsSuccess;
        }
        
        // For non-Result responses, assume success (queries typically don't use Result pattern)
        return true;
    }

    /// <summary>
    /// Determines if the request is a command (as opposed to a query).
    /// Commands typically modify state and should dispatch domain events.
    /// Queries typically don't modify state and shouldn't dispatch events.
    /// </summary>
    /// <param name="requestName">The name of the request type.</param>
    /// <returns>True if the request is a command, false if it's a query.</returns>
    private static bool IsCommand(string requestName)
    {
        // Commands typically end with "Command", queries with "Query"
        return requestName.EndsWith("Command", StringComparison.OrdinalIgnoreCase);
    }
}