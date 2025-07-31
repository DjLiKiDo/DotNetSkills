namespace DotNetSkills.Domain.Common;

internal class DefaultDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}