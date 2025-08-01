namespace DotNetSkills.Domain.Common;

public interface IStronglyTypedId<out T>
{
    T Value { get; }
}
