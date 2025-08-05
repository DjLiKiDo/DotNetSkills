namespace DotNetSkills.Domain.TaskExecution.Enums;

/// <summary>
/// Extension methods for TaskStatus enum providing business logic and utility methods.
/// </summary>
public static class TaskStatusExtensions
{
    /// <summary>
    /// Gets the display name for the task status.
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this TaskStatus status) => status switch
    {
        TaskStatus.ToDo => "To Do",
        TaskStatus.InProgress => "In Progress",
        TaskStatus.InReview => "In Review",
        TaskStatus.Done => "Done",
        TaskStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };

    /// <summary>
    /// Checks if the task status represents a completed state.
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>True if the task is completed, false otherwise.</returns>
    public static bool IsComplete(this TaskStatus status) =>
        status == TaskStatus.Done;

    /// <summary>
    /// Checks if the task status represents an active working state.
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>True if the task is in an active working state, false otherwise.</returns>
    public static bool IsActive(this TaskStatus status) =>
        status is TaskStatus.ToDo or TaskStatus.InProgress or TaskStatus.InReview;

    /// <summary>
    /// Checks if the task status represents a finalized state (completed or cancelled).
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>True if the task is in a finalized state, false otherwise.</returns>
    public static bool IsFinalized(this TaskStatus status) =>
        status is TaskStatus.Done or TaskStatus.Cancelled;

    /// <summary>
    /// Gets the color code associated with the task status for UI purposes.
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>A hexadecimal color code.</returns>
    public static string GetColorCode(this TaskStatus status) => status switch
    {
        TaskStatus.ToDo => "#6c757d",        // Gray
        TaskStatus.InProgress => "#007bff",  // Blue
        TaskStatus.InReview => "#ffc107",    // Yellow
        TaskStatus.Done => "#28a745",        // Green
        TaskStatus.Cancelled => "#dc3545",   // Red
        _ => "#6c757d"                       // Gray (default)
    };

    /// <summary>
    /// Gets the progress percentage for the task status.
    /// </summary>
    /// <param name="status">The task status.</param>
    /// <returns>A percentage value between 0 and 100.</returns>
    public static int GetProgressPercentage(this TaskStatus status) => status switch
    {
        TaskStatus.ToDo => 0,
        TaskStatus.InProgress => 50,
        TaskStatus.InReview => 80,
        TaskStatus.Done => 100,
        TaskStatus.Cancelled => 0,
        _ => 0
    };

    // ...existing code...
}
