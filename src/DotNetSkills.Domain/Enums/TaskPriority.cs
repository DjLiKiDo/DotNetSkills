namespace DotNetSkills.Domain.Enums;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public static class TaskPriorityExtensions
{
    public static string GetDisplayName(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "Low",
        TaskPriority.Medium => "Medium",
        TaskPriority.High => "High",
        TaskPriority.Critical => "Critical",
        _ => priority.ToString()
    };

    public static string GetColorCode(this TaskPriority priority) => priority switch
    {
        TaskPriority.Low => "#28a745",      // Green
        TaskPriority.Medium => "#ffc107",   // Yellow
        TaskPriority.High => "#fd7e14",     // Orange
        TaskPriority.Critical => "#dc3545", // Red
        _ => "#6c757d"                      // Gray (default)
    };
}