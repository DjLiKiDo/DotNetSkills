namespace DotNetSkills.Infrastructure.Services.Events;

/// <summary>
/// Implementation of domain event collection service using AsyncLocal for thread-safe tracking.
/// This service tracks aggregate roots that have been modified during a request lifecycle
/// to enable proper domain event dispatching.
/// </summary>
/// <remarks>
/// Uses AsyncLocal to ensure thread-safe operation across async operations
/// while maintaining request-scoped state. This is crucial for scenarios where
/// domain events need to be dispatched after successful business operations.
/// </remarks>
public class DomainEventCollectionService : IDomainEventCollectionService
{
    private static readonly AsyncLocal<HashSet<IAggregateRoot>> _modifiedAggregates = new();
    private readonly ILogger<DomainEventCollectionService> _logger;

    public DomainEventCollectionService(ILogger<DomainEventCollectionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets or creates the collection of modified aggregates for the current async context.
    /// </summary>
    private HashSet<IAggregateRoot> ModifiedAggregates
    {
        get
        {
            _modifiedAggregates.Value ??= new HashSet<IAggregateRoot>();
            return _modifiedAggregates.Value;
        }
    }

    /// <summary>
    /// Registers an aggregate root that has been modified during the current request.
    /// The aggregate root will be tracked for domain event dispatching.
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate root's ID.</typeparam>
    /// <param name="aggregateRoot">The aggregate root to track.</param>
    public void RegisterModifiedAggregate<TId>(AggregateRoot<TId> aggregateRoot) where TId : IStronglyTypedId<Guid>
    {
        ArgumentNullException.ThrowIfNull(aggregateRoot);

        var wasAdded = ModifiedAggregates.Add(aggregateRoot);
        
        if (wasAdded)
        {
            _logger.LogDebug(
                "Registered modified aggregate {AggregateType} with ID {AggregateId} for domain event tracking",
                aggregateRoot.GetTypeName(),
                aggregateRoot.GetId());
        }
        else
        {
            _logger.LogDebug(
                "Aggregate {AggregateType} with ID {AggregateId} was already tracked",
                aggregateRoot.GetTypeName(),
                aggregateRoot.GetId());
        }
    }

    /// <summary>
    /// Gets all aggregate roots that have been modified during the current request.
    /// This method is typically called by the DomainEventDispatchBehavior to collect events.
    /// </summary>
    /// <returns>A collection of modified aggregate roots.</returns>
    public IReadOnlyCollection<IAggregateRoot> GetModifiedAggregates()
    {
        var aggregates = ModifiedAggregates.ToList().AsReadOnly();
        
        _logger.LogDebug(
            "Retrieved {AggregateCount} modified aggregates for domain event processing",
            aggregates.Count);
        
        return aggregates;
    }

    /// <summary>
    /// Clears all tracked aggregate roots.
    /// This should be called after successful domain event dispatching.
    /// </summary>
    public void ClearTrackedAggregates()
    {
        var count = ModifiedAggregates.Count;
        ModifiedAggregates.Clear();
        
        _logger.LogDebug(
            "Cleared {AggregateCount} tracked aggregates after domain event processing",
            count);
    }

    /// <summary>
    /// Gets all domain events from tracked aggregate roots.
    /// This is a convenience method that collects events from all tracked aggregates.
    /// </summary>
    /// <returns>A collection of domain events from all tracked aggregates.</returns>
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        var allEvents = ModifiedAggregates
            .SelectMany(aggregate => aggregate.DomainEvents)
            .ToList()
            .AsReadOnly();
        
        _logger.LogDebug(
            "Collected {EventCount} domain events from {AggregateCount} tracked aggregates",
            allEvents.Count,
            ModifiedAggregates.Count);
        
        return allEvents;
    }
}
