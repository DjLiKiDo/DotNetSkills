using DotNetSkills.Domain.Common;

namespace DotNetSkills.Domain.ValueObjects;

public readonly record struct ProjectId(Guid Value) : IStronglyTypedId<Guid>
{
    public static ProjectId New() => new(Guid.NewGuid());
    public static ProjectId Empty => new(Guid.Empty);

    public bool Equals(IStronglyTypedId<Guid>? other)
    {
        if (other is not ProjectId otherId)
            return false;
        
        return Value.Equals(otherId.Value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ProjectId projectId) => projectId.Value;
    public static explicit operator ProjectId(Guid guid) => new(guid);
}