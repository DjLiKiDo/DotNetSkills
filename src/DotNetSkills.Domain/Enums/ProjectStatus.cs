namespace DotNetSkills.Domain.Enums;

public enum ProjectStatus
{
    Planning = 1,
    Active = 2,
    OnHold = 3,
    Completed = 4,
    Cancelled = 5
}

public static class ProjectStatusExtensions
{
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

    public static string GetDisplayName(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planning => "Planning",
        ProjectStatus.Active => "Active",
        ProjectStatus.OnHold => "On Hold",
        ProjectStatus.Completed => "Completed",
        ProjectStatus.Cancelled => "Cancelled",
        _ => status.ToString()
    };

    public static bool IsActive(this ProjectStatus status) =>
        status is ProjectStatus.Planning or ProjectStatus.Active or ProjectStatus.OnHold;

    public static bool IsFinalized(this ProjectStatus status) =>
        status is ProjectStatus.Completed or ProjectStatus.Cancelled;

    public static string GetColorCode(this ProjectStatus status) => status switch
    {
        ProjectStatus.Planning => "#6f42c1",   // Purple
        ProjectStatus.Active => "#28a745",     // Green
        ProjectStatus.OnHold => "#ffc107",     // Yellow
        ProjectStatus.Completed => "#17a2b8",  // Cyan
        ProjectStatus.Cancelled => "#dc3545",  // Red
        _ => "#6c757d"                         // Gray (default)
    };
}