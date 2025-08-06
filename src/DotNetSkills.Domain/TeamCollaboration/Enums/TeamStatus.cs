namespace DotNetSkills.Domain.TeamCollaboration.Enums;

/// <summary>
/// Represents the different statuses a team can have in the system.
/// </summary>
/// <remarks>
/// Team status controls team visibility, member additions, and operational capabilities.
/// Status transitions should be handled through domain methods to maintain data integrity.
/// </remarks>
public enum TeamStatus
{
    /// <summary>
    /// Team is active and operational. Members can join, create projects, and perform all team activities.
    /// This is the default status for newly created teams.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Team is temporarily inactive but not deleted. 
    /// Team activities are limited, new members cannot join, but existing members retain access.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Team is archived for historical purposes.
    /// No new activities are allowed, but data is preserved for reporting and audit purposes.
    /// </summary>
    Archived = 2,

    /// <summary>
    /// Team is pending approval or setup completion.
    /// Limited functionality until team is activated by an administrator.
    /// </summary>
    Pending = 3
}

/// <summary>
/// Extension methods for the TeamStatus enum.
/// </summary>
public static class TeamStatusExtensions
{
    /// <summary>
    /// Determines if the team status allows new members to join.
    /// </summary>
    /// <param name="status">The team status to check.</param>
    /// <returns>True if new members can join; otherwise, false.</returns>
    public static bool AllowsMemberAddition(this TeamStatus status) => status switch
    {
        TeamStatus.Active => true,
        TeamStatus.Inactive => false,
        TeamStatus.Archived => false,
        TeamStatus.Pending => false,
        _ => false
    };

    /// <summary>
    /// Determines if the team status allows project creation.
    /// </summary>
    /// <param name="status">The team status to check.</param>
    /// <returns>True if projects can be created; otherwise, false.</returns>
    public static bool AllowsProjectCreation(this TeamStatus status) => status switch
    {
        TeamStatus.Active => true,
        TeamStatus.Inactive => false,
        TeamStatus.Archived => false,
        TeamStatus.Pending => false,
        _ => false
    };

    /// <summary>
    /// Determines if the team status allows operational activities.
    /// </summary>
    /// <param name="status">The team status to check.</param>
    /// <returns>True if operational activities are allowed; otherwise, false.</returns>
    public static bool AllowsOperations(this TeamStatus status) => status switch
    {
        TeamStatus.Active => true,
        TeamStatus.Inactive => true, // Existing members can still work
        TeamStatus.Archived => false,
        TeamStatus.Pending => false,
        _ => false
    };

    /// <summary>
    /// Gets a user-friendly description of the team status.
    /// </summary>
    /// <param name="status">The team status.</param>
    /// <returns>A descriptive string for the status.</returns>
    public static string GetDescription(this TeamStatus status) => status switch
    {
        TeamStatus.Active => "Active and operational",
        TeamStatus.Inactive => "Temporarily inactive",
        TeamStatus.Archived => "Archived for historical purposes",
        TeamStatus.Pending => "Pending approval or setup",
        _ => "Unknown status"
    };

    /// <summary>
    /// Gets all valid status transitions from the current status.
    /// </summary>
    /// <param name="current">The current team status.</param>
    /// <returns>A collection of valid next statuses.</returns>
    public static IEnumerable<TeamStatus> GetValidTransitions(this TeamStatus current) => current switch
    {
        TeamStatus.Active => [TeamStatus.Inactive, TeamStatus.Archived],
        TeamStatus.Inactive => [TeamStatus.Active, TeamStatus.Archived],
        TeamStatus.Archived => [], // Archived teams cannot transition to other statuses
        TeamStatus.Pending => [TeamStatus.Active, TeamStatus.Inactive],
        _ => []
    };

    /// <summary>
    /// Determines if a status transition is valid.
    /// </summary>
    /// <param name="from">The current status.</param>
    /// <param name="to">The target status.</param>
    /// <returns>True if the transition is valid; otherwise, false.</returns>
    public static bool CanTransitionTo(this TeamStatus from, TeamStatus to)
    {
        return from.GetValidTransitions().Contains(to);
    }
}
