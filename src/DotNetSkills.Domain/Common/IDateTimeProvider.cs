namespace DotNetSkills.Domain.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}