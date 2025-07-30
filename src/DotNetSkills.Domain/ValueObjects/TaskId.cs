using DotNetSkills.Domain.Common;

namespace DotNetSkills.Domain.ValueObjects;

public readonly record struct TaskId(Guid Value) : IStronglyTypedId<Guid>
{
    public static TaskId New() => new(Guid.NewGuid());
    public static TaskId Empty => new(Guid.Empty);

    public bool Equals(IStronglyTypedId<Guid>? other)
    {
        if (other is not TaskId otherId)
            return false;
        
        return Value.Equals(otherId.Value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(TaskId taskId) => taskId.Value;
    public static explicit operator TaskId(Guid guid) => new(guid);
}