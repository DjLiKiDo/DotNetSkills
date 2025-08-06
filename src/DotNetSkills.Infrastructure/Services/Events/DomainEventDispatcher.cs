using MediatR;

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
    /// Dispatches a single domain event to its registered handlers.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async System.Threading.Tasks.Task DispatchAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) 
        where TDomainEvent : class, IDomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _logger.LogInformation(
            "Dispatching domain event {EventType} with CorrelationId {CorrelationId} occurred at {OccurredAt}",
            typeof(TDomainEvent).Name,
            domainEvent.CorrelationId,
            domainEvent.OccurredAt);

        try
        {
            // Publish the event via MediatR
            await _mediator.Publish(domainEvent, cancellationToken);
            
            _logger.LogDebug(
                "Successfully dispatched domain event {EventType} with CorrelationId {CorrelationId}",
                typeof(TDomainEvent).Name,
                domainEvent.CorrelationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to dispatch domain event {EventType} with CorrelationId {CorrelationId}",
                typeof(TDomainEvent).Name,
                domainEvent.CorrelationId);
            
            // Re-throw to allow upstream error handling
            throw;
        }
    }

    /// <summary>
    /// Dispatches multiple domain events sequentially.
    /// </summary>
    /// <param name="domainEvents">Collection of domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async System.Threading.Tasks.Task DispatchManyAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        var eventsList = domainEvents.ToList();
        if (!eventsList.Any())
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
                await _mediator.Publish(domainEvent, cancellationToken);
                dispatchedCount++;
                
                _logger.LogDebug(
                    "Dispatched domain event {EventType} with CorrelationId {CorrelationId} ({Current}/{Total})",
                    domainEvent.GetType().Name,
                    domainEvent.CorrelationId,
                    dispatchedCount,
                    eventsList.Count);
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

        if (failedEvents.Any())
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
    public async System.Threading.Tasks.Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _logger.LogInformation(
            "Dispatching domain event {EventType} with CorrelationId {CorrelationId} occurred at {OccurredAt}",
            domainEvent.GetType().Name,
            domainEvent.CorrelationId,
            domainEvent.OccurredAt);

        try
        {
            // Publish the event via MediatR
            await _mediator.Publish(domainEvent, cancellationToken);
            
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
            
            // Re-throw to allow upstream error handling
            throw;
        }
    }

    /// <summary>
    /// Dispatches multiple domain events (interface implementation).
    /// </summary>
    /// <param name="domainEvents">Collection of domain events to dispatch</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Task representing the asynchronous operation</returns>
    public async System.Threading.Tasks.Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
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
    public async System.Threading.Tasks.Task DispatchFromAggregateAsync<TId>(AggregateRoot<TId> aggregateRoot, CancellationToken cancellationToken = default)
        where TId : IStronglyTypedId<Guid>
    {
        ArgumentNullException.ThrowIfNull(aggregateRoot);

        var domainEvents = aggregateRoot.DomainEvents.ToList();
        
        if (!domainEvents.Any())
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
    public async System.Threading.Tasks.Task DispatchFromAggregatesAsync(IEnumerable<AggregateRoot<IStronglyTypedId<Guid>>> aggregateRoots, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(aggregateRoots);

        var aggregatesList = aggregateRoots.ToList();
        if (!aggregatesList.Any())
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
            if (events.Any())
            {
                allDomainEvents.AddRange(events);
                aggregatesWithEvents.Add(aggregate);
            }
        }

        if (!allDomainEvents.Any())
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
