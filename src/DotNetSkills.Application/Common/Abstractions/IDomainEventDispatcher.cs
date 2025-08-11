using Task = System.Threading.Tasks.Task;

namespace DotNetSkills.Application.Common.Abstractions;

/// <summary>
/// Interface for dispatching domain events.
/// Provides mechanisms to publish domain events for cross-aggregate communication
/// and integration with external systems.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a single domain event asynchronously.
    /// </summary>
    /// <param name="domainEvent">The domain event to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches multiple domain events asynchronously.
    /// Events are typically dispatched in the order they were raised.
    /// </summary>
    /// <param name="domainEvents">The collection of domain events to dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches all domain events from an aggregate root asynchronously.
    /// This method extracts domain events from the aggregate and dispatches them,
    /// then clears the events from the aggregate.
    /// </summary>
    /// <typeparam name="TId">The type of the aggregate root's ID.</typeparam>
    /// <param name="aggregateRoot">The aggregate root containing domain events.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchFromAggregateAsync<TId>(AggregateRoot<TId> aggregateRoot, CancellationToken cancellationToken = default)
        where TId : IStronglyTypedId<Guid>;

    /// <summary>
    /// Dispatches domain events from multiple aggregate roots asynchronously.
    /// This is useful when multiple aggregates are involved in a single operation.
    /// </summary>
    /// <param name="aggregateRoots">The collection of aggregate roots containing domain events.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DispatchFromAggregatesAsync(IEnumerable<AggregateRoot<IStronglyTypedId<Guid>>> aggregateRoots, CancellationToken cancellationToken = default);
}