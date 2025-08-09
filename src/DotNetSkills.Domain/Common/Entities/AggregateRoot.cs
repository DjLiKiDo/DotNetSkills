namespace DotNetSkills.Domain.Common.Entities;

public abstract class AggregateRoot<TId>(TId id) : BaseEntity<TId>(id) where TId : IStronglyTypedId<Guid>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Restores a domain event to the aggregate root.
    /// This method is used by the DbContext to restore domain events
    /// after a failed save operation to maintain transactional integrity.
    /// </summary>
    /// <param name="domainEvent">The domain event to restore.</param>
    public void RestoreDomainEvent(IDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        
        _domainEvents.Add(domainEvent);
    }
}
