namespace DotNetSkills.Domain.Common;

public interface IStronglyTypedId<T> : IEquatable<IStronglyTypedId<T>>
    where T : IEquatable<T>
{
    T Value { get; }
}