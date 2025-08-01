namespace DotNetSkills.Domain.Common.Events;

public abstract record BaseDomainEvent(
    DateTime OccurredAt,
    Guid CorrelationId) : IDomainEvent
{
    protected BaseDomainEvent() : this(DateTime.UtcNow, Guid.NewGuid())
    {
    }
}
