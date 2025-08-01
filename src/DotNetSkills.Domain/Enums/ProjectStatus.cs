namespace DotNetSkills.Domain.Enums;

/// <summary>
/// Represents the current status of a project.
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// The project is currently active and work is ongoing.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The project has been completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// The project is temporarily on hold.
    /// </summary>
    OnHold = 3,

    /// <summary>
    /// The project has been cancelled.
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// The project is in planning phase.
    /// </summary>
    Planning = 5
}
