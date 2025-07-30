namespace DotNetSkills.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}