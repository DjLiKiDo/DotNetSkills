namespace DotNetSkills.Domain.Common;

public static class DateTimeService
{
    private static IDateTimeProvider _provider = new DefaultDateTimeProvider();

    public static void SetProvider(IDateTimeProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public static DateTime UtcNow => _provider.UtcNow;
}