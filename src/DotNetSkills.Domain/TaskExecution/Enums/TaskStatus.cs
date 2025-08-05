namespace DotNetSkills.Domain.TaskExecution.Enums;

/// <summary>
/// Represents the current status of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// The task is created but not yet started.
    /// </summary>
    ToDo = 1,

    /// <summary>
    /// The task is currently being worked on.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// The task is under review.
    /// </summary>
    InReview = 3,

    /// <summary>
    /// The task has been completed.
    /// </summary>
    Done = 4,

    /// <summary>
    /// The task has been cancelled or is no longer needed.
    /// </summary>
    Cancelled = 5
}
