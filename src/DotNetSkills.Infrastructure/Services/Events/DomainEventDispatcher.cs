using MediatR;
using DotNetSkills.Application.Common.Events;
using Task = System.Threading.Tasks.Task;

namespace DotNetSkills.Infrastructure.Services.Events;

/// <summary>
/// Domain event dispatcher implementation using MediatR.
/// Handles dispatching domain events to their respective handlers.
/// </summary>
/// <remarks>
/// This implementation bridges the gap between domain events and MediatR notifications.
/// In the future, this can be enhanced with additional features like:
/// - Event ordering and prioritization
/// - Asynchronous event processing with queues
/// - Event sourcing capabilities
/// - Retry mechanisms for failed handlers
/// </remarks>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(
        IMediator mediator, 
        ILogger<DomainEventDispatcher> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a domain event notification wrapper for MediatR publishing.
    /// </summary>
    /// <param name="domainEvent">The domain event to wrap</param>
    /// <returns>The wrapped domain event notification</returns>
    /// <exception cref="InvalidOperationException">Thrown when wrapper creation fails</exception>
    private object CreateDomainEventNotificationWrapper(IDomainEvent domainEvent)
    {
        var wrapperType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var wrapper = Activator.CreateInstance(wrapperType, domainEvent);
        if (wrapper is null)
            throw new InvalidOperationException($"Failed to create notification wrapper for domain event type {domainEvent.GetType().Name}");
        return wrapper;
    }

    /// <summary>
    /// Core dispatch logic for a single domain event with consistent logging and error handling.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    private async Task DispatchSingleEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Dispatching domain event {EventType} with CorrelationId {CorrelationId} occurred at {OccurredAt}",
            domainEvent.GetType().Name,
            domainEvent.CorrelationId,
            domainEvent.OccurredAt);

        try
        {
            var wrapper = CreateDomainEventNotificationWrapper(domainEvent);
            await _mediator.Publish(wrapper, cancellationToken);
            
            _logger.LogDebug(
                "Successfully dispatched domain event {EventType} with CorrelationId {CorrelationId}",
                domainEvent.GetType().Name,
                domainEvent.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch domain event {EventType} with CorrelationId {CorrelationId}",
                domainEvent.GetType().Name,
                domainEvent.CorrelationId);
            throw;
        }
    }

    /// <summary>
    /// Dispatches a single domain event to its registered handlers.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) 
        where TDomainEvent : class, IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        await DispatchSingleEventAsync(domainEvent, cancellationToken);
    }

    /// <summary>
    /// Dispatches multiple domain events sequentially.
    /// </summary>
    /// <param name="domainEvents">Collection of domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        var eventsList = domainEvents.ToList();
        if (eventsList.Count == 0)
        {
            _logger.LogDebug("No domain events to dispatch");
            return;
        }

        _logger.LogInformation(
            "Dispatching {EventCount} domain events", 
            eventsList.Count);

        var dispatchedCount = 0;
        List<(IDomainEvent Event, Exception Exception)> failedEvents = [];

        foreach (var domainEvent in eventsList)
        {
            try
            {
                // Wrap the domain event in a notification wrapper for MediatR
                var wrapper = CreateDomainEventNotificationWrapper(domainEvent);
                
                await _mediator.Publish(wrapper, cancellationToken);
                dispatchedCount++;
                
                _logger.LogDebug(
                    "Dispatched domain event {EventType} with CorrelationId {CorrelationId} ({Current}/{Total})",
                    domainEvent.GetType().Name,
                    domainEvent.CorrelationId,
                    dispatchedCount,
                    eventsList.Count);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Failed to create notification wrapper"))
            {
                _logger.LogError(ex,
                    "Domain event wrapper creation failed for {EventType}",
                    domainEvent.GetType().Name);

                failedEvents.Add((domainEvent, ex));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to dispatch domain event {EventType} with CorrelationId {CorrelationId}",
                    domainEvent.GetType().Name,
                    domainEvent.CorrelationId);

                failedEvents.Add((domainEvent, ex));
            }
        }

        if (failedEvents.Count > 0)
        {
            _logger.LogWarning(
                "Successfully dispatched {SuccessCount} out of {TotalCount} domain events. {FailedCount} events failed",
                dispatchedCount,
                eventsList.Count,
                failedEvents.Count);

            // For now, we'll throw an aggregate exception
            // In the future, we could implement partial failure handling
            var exceptions = failedEvents.Select(f => f.Exception).ToArray();
            throw new AggregateException(
                $"Failed to dispatch {failedEvents.Count} domain events", 
                exceptions);
        }

        _logger.LogInformation(
            "Successfully dispatched all {EventCount} domain events",
            eventsList.Count);
    }

    /// <summary>
    /// Dispatches a single domain event (interface implementation).
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        await DispatchSingleEventAsync(domainEvent, cancellationToken);
    }

    /// <summary>
    /// Dispatches multiple domain events (interface implementation).
    /// </summary>
    /// <param name="domainEvents">Collection of domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        await DispatchManyAsync(domainEvents, cancellationToken);
    }

    /// <summary>
    /// Dispatches all domain events from an aggregate root.
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate root's ID</typeparam>
    /// <param name="aggregateRoot">The aggregate root containing domain events</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchFromAggregateAsync<TId>(AggregateRoot<TId> aggregateRoot, CancellationToken cancellationToken = default)
        where TId : IStronglyTypedId<Guid>
    {
        ArgumentNullException.ThrowIfNull(aggregateRoot);

        var domainEvents = aggregateRoot.DomainEvents.ToList();
        
        if (domainEvents.Count == 0)
        {
            _logger.LogDebug("No domain events to dispatch from aggregate {AggregateType} with ID {AggregateId}",
                aggregateRoot.GetType().Name, aggregateRoot.Id);
            return;
        }

        _logger.LogInformation(
            "Dispatching {EventCount} domain events from aggregate {AggregateType} with ID {AggregateId}",
            domainEvents.Count, aggregateRoot.GetType().Name, aggregateRoot.Id);

        try
        {
            await DispatchAsync(domainEvents, cancellationToken);
            
            // Clear events after successful dispatching
            aggregateRoot.ClearDomainEvents();
            
            _logger.LogDebug(
                "Successfully dispatched and cleared {EventCount} domain events from aggregate {AggregateType}",
                domainEvents.Count, aggregateRoot.GetType().Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch domain events from aggregate {AggregateType} with ID {AggregateId}",
                aggregateRoot.GetType().Name, aggregateRoot.Id);
            
            // Don't clear events if dispatch failed - they can be retried
            throw;
        }
    }

    /// <summary>
    /// Dispatches domain events from multiple aggregate roots.
    /// </summary>
    /// <param name="aggregateRoots">Collection of aggregate roots containing domain events</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async Task DispatchFromAggregatesAsync(IEnumerable<AggregateRoot<IStronglyTypedId<Guid>>> aggregateRoots, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(aggregateRoots);

        var aggregatesList = aggregateRoots.ToList();
        if (aggregatesList.Count == 0)
        {
            _logger.LogDebug("No aggregates provided for domain event dispatching");
            return;
        }

        // Collect all domain events from all aggregates
        List<IDomainEvent> allDomainEvents = [];
        List<AggregateRoot<IStronglyTypedId<Guid>>> aggregatesWithEvents = [];

        foreach (var aggregate in aggregatesList)
        {
            var events = aggregate.DomainEvents.ToList();
            if (events.Count > 0)
            {
                allDomainEvents.AddRange(events);
                aggregatesWithEvents.Add(aggregate);
            }
        }

        if (allDomainEvents.Count == 0)
        {
            _logger.LogDebug("No domain events found in any of the {AggregateCount} aggregates", aggregatesList.Count);
            return;
        }

        _logger.LogInformation(
            "Dispatching {EventCount} domain events from {AggregateCount} aggregates",
            allDomainEvents.Count, aggregatesWithEvents.Count);

        try
        {
            await DispatchAsync(allDomainEvents, cancellationToken);
            
            // Clear events from all aggregates after successful dispatching
            foreach (var aggregate in aggregatesWithEvents)
            {
                aggregate.ClearDomainEvents();
            }
            
            _logger.LogDebug(
                "Successfully dispatched and cleared domain events from {AggregateCount} aggregates",
                aggregatesWithEvents.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch domain events from aggregates. Events will not be cleared for retry");
            
            // Don't clear events if dispatch failed - they can be retried
            throw;
        }
    }
}
