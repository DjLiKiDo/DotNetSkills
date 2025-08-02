namespace DotNetSkills.Domain.UnitTests.Common;

/// <summary>
/// Helper class for testing domain events in aggregates.
/// </summary>
public class DomainEventTestHelper
{
    /// <summary>
    /// Asserts that an aggregate has raised a domain event of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to check for.</typeparam>
    /// <param name="aggregate">The aggregate to check.</param>
    /// <returns>The domain event that was found.</returns>
    public TEvent AssertEventRaised<TEvent>(AggregateRoot<UserId> aggregate) where TEvent : IDomainEvent
    {
        var domainEvent = aggregate.DomainEvents.OfType<TEvent>().SingleOrDefault();
        domainEvent.Should().NotBeNull($"Expected domain event of type {typeof(TEvent).Name} was not raised");
        return domainEvent!;
    }

    /// <summary>
    /// Asserts that an aggregate has raised a domain event of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to check for.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate to check.</param>
    /// <returns>The domain event that was found.</returns>
    public TEvent AssertEventRaised<TEvent, TId>(AggregateRoot<TId> aggregate) 
        where TEvent : IDomainEvent 
        where TId : IStronglyTypedId<Guid>
    {
        var domainEvent = aggregate.DomainEvents.OfType<TEvent>().SingleOrDefault();
        domainEvent.Should().NotBeNull($"Expected domain event of type {typeof(TEvent).Name} was not raised");
        return domainEvent!;
    }

    /// <summary>
    /// Asserts that an aggregate has not raised any domain events of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to check for.</typeparam>
    /// <param name="aggregate">The aggregate to check.</param>
    public void AssertEventNotRaised<TEvent>(AggregateRoot<UserId> aggregate) where TEvent : IDomainEvent
    {
        var domainEvent = aggregate.DomainEvents.OfType<TEvent>().SingleOrDefault();
        domainEvent.Should().BeNull($"Domain event of type {typeof(TEvent).Name} should not have been raised");
    }

    /// <summary>
    /// Asserts that an aggregate has not raised any domain events of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event to check for.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate to check.</param>
    public void AssertEventNotRaised<TEvent, TId>(AggregateRoot<TId> aggregate) 
        where TEvent : IDomainEvent 
        where TId : IStronglyTypedId<Guid>
    {
        var domainEvent = aggregate.DomainEvents.OfType<TEvent>().SingleOrDefault();
        domainEvent.Should().BeNull($"Domain event of type {typeof(TEvent).Name} should not have been raised");
    }

    /// <summary>
    /// Asserts that an aggregate has raised exactly the specified number of domain events.
    /// </summary>
    /// <param name="aggregate">The aggregate to check.</param>
    /// <param name="expectedCount">The expected number of domain events.</param>
    public void AssertEventCount<TId>(AggregateRoot<TId> aggregate, int expectedCount) where TId : IStronglyTypedId<Guid>
    {
        aggregate.DomainEvents.Should().HaveCount(expectedCount);
    }

    /// <summary>
    /// Asserts that an aggregate has not raised any domain events.
    /// </summary>
    /// <param name="aggregate">The aggregate to check.</param>
    public void AssertNoEventsRaised<TId>(AggregateRoot<TId> aggregate) where TId : IStronglyTypedId<Guid>
    {
        aggregate.DomainEvents.Should().BeEmpty();
    }

    /// <summary>
    /// Gets all domain events of a specific type from an aggregate.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event.</typeparam>
    /// <typeparam name="TId">The type of the aggregate ID.</typeparam>
    /// <param name="aggregate">The aggregate to get events from.</param>
    /// <returns>Collection of domain events.</returns>
    public IEnumerable<TEvent> GetEvents<TEvent, TId>(AggregateRoot<TId> aggregate) 
        where TEvent : IDomainEvent 
        where TId : IStronglyTypedId<Guid>
    {
        return aggregate.DomainEvents.OfType<TEvent>();
    }
}