namespace DotNetSkills.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }

    Guid CorrelationId { get; }
}
