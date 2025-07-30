using DotNetSkills.Domain.Common;

namespace DotNetSkills.Domain.ValueObjects;

public readonly record struct TeamId(Guid Value) : IStronglyTypedId<Guid>
{
    public static TeamId New() => new(Guid.NewGuid());
    public static TeamId Empty => new(Guid.Empty);

    public bool Equals(IStronglyTypedId<Guid>? other)
    {
        if (other is not TeamId otherId)
            return false;
        
        return Value.Equals(otherId.Value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(TeamId teamId) => teamId.Value;
    public static explicit operator TeamId(Guid guid) => new(guid);
}