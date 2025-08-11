namespace DotNetSkills.Application.Common.Abstractions;

/// <summary>
/// Service for collecting aggregate roots that have been modified during a request lifecycle.
/// This service tracks aggregates to enable domain event dispatching at the end of successful operations.
/// </summary>
public interface IDomainEventCollectionService
{
    /// <summary>
    /// Registers an aggregate root that has been modified during the current request.
    /// The aggregate root will be tracked for domain event dispatching.
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate root's ID.</typeparam>
    /// <param name="aggregateRoot">The aggregate root to track.</param>
    void RegisterModifiedAggregate<TId>(AggregateRoot<TId> aggregateRoot) where TId : IStronglyTypedId<Guid>;

    /// <summary>
    /// Gets all aggregate roots that have been modified during the current request.
    /// This method is typically called by the DomainEventDispatchBehavior to collect events.
    /// </summary>
    /// <returns>A collection of modified aggregate roots.</returns>
    IReadOnlyCollection<IAggregateRoot> GetModifiedAggregates();

    /// <summary>
    /// Clears all tracked aggregate roots.
    /// This should be called after successful domain event dispatching.
    /// </summary>
    void ClearTrackedAggregates();

    /// <summary>
    /// Gets all domain events from tracked aggregate roots.
    /// This is a convenience method that collects events from all tracked aggregates.
    /// </summary>
    /// <returns>A collection of domain events from all tracked aggregates.</returns>
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
}
