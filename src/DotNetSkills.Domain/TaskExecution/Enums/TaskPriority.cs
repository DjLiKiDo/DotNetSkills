namespace DotNetSkills.Domain.TaskExecution.Enums;

/// <summary>
/// Represents the priority level of a task.
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Low priority task.
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium priority task.
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High priority task.
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical priority task requiring immediate attention.
    /// </summary>
    Critical = 4
}
