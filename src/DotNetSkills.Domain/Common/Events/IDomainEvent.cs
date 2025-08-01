namespace DotNetSkills.Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }

    Guid CorrelationId { get; }
}
