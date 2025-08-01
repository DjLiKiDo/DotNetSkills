namespace DotNetSkills.Domain.Common.Extensions;

/// <summary>
/// Extension methods for ProjectStatus enum providing business logic and utility methods.
/// </summary>
public static class ProjectStatusExtensions
{
    /// <summary>
    /// Determines if the current project status can transition to the specified new status.
    /// </summary>
    /// <param name="currentStatus">The current project status.</param>
    /// <param name="newStatus">The target status to transition to.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    public static bool CanTransitionTo(this ProjectStatus currentStatus, ProjectStatus newStatus)
    {
        return currentStatus switch
        {
            ProjectStatus.Planning => newStatus is ProjectStatus.Active or ProjectStatus.Cancelled,
            ProjectStatus.Active => newStatus is ProjectStatus.OnHold or ProjectStatus.Completed or ProjectStatus.Cancelled,
            ProjectStatus.OnHold => newStatus is ProjectStatus.Active or ProjectStatus.Cancelled,
            ProjectStatus.Completed => false, // Completed projects cannot be changed
            ProjectStatus.Cancelled => false, // Cancelled projects cannot be changed
            _ => false
        };
    }

    /// <summary>
    /// Gets the display name for the project status.
    /// </summary>
    /// <param name="status">The project status.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planning => "Planning",
        ProjectStatus.Active => "Active",
        ProjectStatus.OnHold => "On Hold",
        ProjectStatus.Completed => "Completed",
        ProjectStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };

    /// <summary>
    /// Checks if the project status represents an active state.
    /// </summary>
    /// <param name="status">The project status.</param>
    /// <returns>True if the project is in an active state, false otherwise.</returns>
    public static bool IsActive(this ProjectStatus status) =>
        status is ProjectStatus.Planning or ProjectStatus.Active or ProjectStatus.OnHold;

    /// <summary>
    /// Checks if the project status represents a finalized state.
    /// </summary>
    /// <param name="status">The project status.</param>
    /// <returns>True if the project is in a finalized state, false otherwise.</returns>
    public static bool IsFinalized(this ProjectStatus status) =>
        status is ProjectStatus.Completed or ProjectStatus.Cancelled;

    /// <summary>
    /// Gets the color code associated with the project status for UI purposes.
    /// </summary>
    /// <param name="status">The project status.</param>
    /// <returns>A hexadecimal color code.</returns>
    public static string GetColorCode(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planning => "#6f42c1",   // Purple
        ProjectStatus.Active => "#28a745",     // Green
        ProjectStatus.OnHold => "#ffc107",     // Yellow
        ProjectStatus.Completed => "#17a2b8",  // Cyan
        ProjectStatus.Cancelled => "#dc3545",  // Red
        _ => "#6c757d"                         // Gray (default)
    };

    /// <summary>
    /// Gets the progress weight for project completion calculations.
    /// </summary>
    /// <param name="status">The project status.</param>
    /// <returns>A value between 0 and 1 representing completion progress.</returns>
    public static decimal GetProgressWeight(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planning => 0.0m,
        ProjectStatus.Active => 0.5m,
        ProjectStatus.OnHold => 0.5m,      // Same as active since work is paused
        ProjectStatus.Completed => 1.0m,
        ProjectStatus.Cancelled => 0.0m,   // No progress for cancelled
        _ => 0.0m
    };
}
