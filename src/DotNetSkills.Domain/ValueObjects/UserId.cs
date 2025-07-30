using DotNetSkills.Domain.Common;

namespace DotNetSkills.Domain.ValueObjects;

public readonly record struct UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);

    public bool Equals(IStronglyTypedId<Guid>? other)
    {
        if (other is not UserId otherId)
            return false;
        
        return Value.Equals(otherId.Value);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static explicit operator UserId(Guid guid) => new(guid);
}