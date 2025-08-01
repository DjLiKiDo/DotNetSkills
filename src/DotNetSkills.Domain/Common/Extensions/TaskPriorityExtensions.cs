namespace DotNetSkills.Domain.Common.Extensions;

/// <summary>
/// Extension methods for TaskPriority enum providing business logic and utility methods.
/// </summary>
public static class TaskPriorityExtensions
{
    /// <summary>
    /// Gets the display name for the task priority.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "Low",
        TaskPriority.Medium => "Medium",
        TaskPriority.High => "High",
        TaskPriority.Critical => "Critical",
        _ => priority.ToString()
    };

    /// <summary>
    /// Gets the color code associated with the task priority for UI purposes.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>A hexadecimal color code.</returns>
    public static string GetColorCode(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "#28a745",      // Green
        TaskPriority.Medium => "#ffc107",   // Yellow
        TaskPriority.High => "#fd7e14",     // Orange
        TaskPriority.Critical => "#dc3545", // Red
        _ => "#6c757d"                      // Gray (default)
    };

    /// <summary>
    /// Gets the numeric weight for priority-based sorting and calculations.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>A numeric weight where higher values indicate higher priority.</returns>
    public static int GetWeight(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => 1,
        TaskPriority.Medium => 2,
        TaskPriority.High => 3,
        TaskPriority.Critical => 4,
        _ => 0
    };

    /// <summary>
    /// Gets the SLA (Service Level Agreement) days based on priority.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>The number of days within which the task should be completed.</returns>
    public static int GetSlaInDays(this TaskPriority priority) => priority switch
    {
        TaskPriority.Critical => 1,  // Must be done within 1 day
        TaskPriority.High => 3,      // Must be done within 3 days
        TaskPriority.Medium => 7,    // Must be done within 1 week
        TaskPriority.Low => 14,      // Must be done within 2 weeks
        _ => 30                      // Default fallback
    };

    /// <summary>
    /// Gets an icon name associated with the task priority for UI purposes.
    /// </summary>
    /// <param name="priority">The task priority.</param>
    /// <returns>An icon name string (using common icon library conventions).</returns>
    public static string GetIconName(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "arrow-down",
        TaskPriority.Medium => "minus",
        TaskPriority.High => "arrow-up",
        TaskPriority.Critical => "exclamation-triangle",
        _ => "question"
    };
}
