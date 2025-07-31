using DotNetSkills.Domain.Common;

namespace DotNetSkills.Infrastructure.Common;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}