namespace DotNetSkills.Domain.UnitTests.Common;

/// <summary>
/// Test clock implementation for deterministic time testing.
/// </summary>
public class TestClock
{
    private DateTime _fixedTime = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the current test time.
    /// </summary>
    public DateTime Now => _fixedTime;

    /// <summary>
    /// Gets the current test time as UTC.
    /// </summary>
    public DateTime UtcNow => _fixedTime;

    /// <summary>
    /// Sets the test time to a specific value.
    /// </summary>
    /// <param name="dateTime">The time to set.</param>
    public void SetTime(DateTime dateTime)
    {
        _fixedTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// Advances the test time by the specified amount.
    /// </summary>
    /// <param name="timeSpan">The amount of time to advance.</param>
    public void Advance(TimeSpan timeSpan)
    {
        _fixedTime = _fixedTime.Add(timeSpan);
    }

    /// <summary>
    /// Resets the test time to the default value (2025-01-01 12:00:00 UTC).
    /// </summary>
    public void Reset()
    {
        _fixedTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    }
}