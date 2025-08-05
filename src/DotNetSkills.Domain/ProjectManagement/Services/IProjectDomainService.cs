namespace DotNetSkills.Domain.ProjectManagement.Services;

/// <summary>
/// Domain service for complex project management operations that require external dependencies
/// or cross-aggregate coordination. This service handles project-related business logic that
/// involves multiple aggregates or requires repository access.
/// </summary>
/// <remarks>
/// This interface defines contracts for operations that:
/// - Require database access to check project dependencies
/// - Involve cross-aggregate business logic (projects, teams, tasks)
/// - Orchestrate complex project management operations
///
/// Simple project business rules remain in BusinessRules.ProjectStatus for performance.
/// </remarks>
public interface IProjectDomainService
{
    /// <summary>
    /// Validates if a project can be safely deleted from the system.
    /// This involves checking for active tasks, team assignments, and other dependencies.
    /// </summary>
    /// <param name="projectId">The ID of the project to validate for deletion.</param>
    /// <param name="requestingUser">The user requesting the deletion (for authorization).</param>
    /// <returns>True if the project can be deleted, false otherwise.</returns>
    Task<bool> CanDeleteProjectAsync(ProjectId projectId, User requestingUser);

    /// <summary>
    /// Checks if a project has any active tasks that would prevent certain operations.
    /// </summary>
    /// <param name="projectId">The ID of the project to check.</param>
    /// <returns>True if the project has active tasks, false otherwise.</returns>
    Task<bool> HasActiveTasksAsync(ProjectId projectId);

    /// <summary>
    /// Validates if a project can transition to a new status considering current state
    /// and business dependencies.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="newStatus">The target status for the transition.</param>
    /// <param name="requestingUser">The user requesting the status change.</param>
    /// <returns>True if the status transition is allowed, false otherwise.</returns>
    Task<bool> CanTransitionToStatusAsync(ProjectId projectId, ProjectStatus newStatus, User requestingUser);

    /// <summary>
    /// Validates project name uniqueness within a team scope.
    /// </summary>
    /// <param name="name">The project name to validate.</param>
    /// <param name="teamId">The team ID for scoping the uniqueness check.</param>
    /// <param name="excludeProjectId">Optional project ID to exclude from the check (for updates).</param>
    /// <returns>True if the project name is unique within the team, false if it already exists.</returns>
    Task<bool> IsProjectNameUniqueInTeamAsync(string name, TeamId teamId, ProjectId? excludeProjectId = null);

    /// <summary>
    /// Calculates comprehensive project metrics including progress, resource allocation,
    /// and completion estimates.
    /// </summary>
    /// <param name="projectId">The ID of the project to analyze.</param>
    /// <returns>The project metrics analysis result.</returns>
    Task<ProjectMetricsResult> CalculateProjectMetricsAsync(ProjectId projectId);

    /// <summary>
    /// Validates if a project can be assigned to a different team considering
    /// current dependencies and business rules.
    /// </summary>
    /// <param name="projectId">The ID of the project to reassign.</param>
    /// <param name="newTeamId">The ID of the target team.</param>
    /// <param name="requestingUser">The user requesting the reassignment.</param>
    /// <returns>True if the project can be reassigned, false otherwise.</returns>
    Task<bool> CanReassignToTeamAsync(ProjectId projectId, TeamId newTeamId, User requestingUser);
}

/// <summary>
/// Result object for project metrics analysis.
/// </summary>
public class ProjectMetricsResult
{
    /// <summary>
    /// Gets the total number of tasks in the project.
    /// </summary>
    public int TotalTasks { get; init; }

    /// <summary>
    /// Gets the number of completed tasks.
    /// </summary>
    public int CompletedTasks { get; init; }

    /// <summary>
    /// Gets the number of active tasks.
    /// </summary>
    public int ActiveTasks { get; init; }

    /// <summary>
    /// Gets the project completion percentage (0-100).
    /// </summary>
    public double CompletionPercentage { get; init; }

    /// <summary>
    /// Gets the estimated total hours for the project.
    /// </summary>
    public int? EstimatedTotalHours { get; init; }

    /// <summary>
    /// Gets the actual hours spent on the project.
    /// </summary>
    public int? ActualHours { get; init; }

    /// <summary>
    /// Gets the number of team members assigned to the project.
    /// </summary>
    public int AssignedMembers { get; init; }

    /// <summary>
    /// Gets the current project status.
    /// </summary>
    public ProjectStatus CurrentStatus { get; init; }

    /// <summary>
    /// Gets the estimated completion date based on current progress.
    /// </summary>
    public DateTime? EstimatedCompletionDate { get; init; }

    /// <summary>
    /// Gets whether the project is on schedule.
    /// </summary>
    public bool IsOnSchedule { get; init; }

    /// <summary>
    /// Gets whether the project is over budget (if budget tracking is enabled).
    /// </summary>
    public bool? IsOverBudget { get; init; }

    /// <summary>
    /// Gets additional notes about the project metrics.
    /// </summary>
    public string? Notes { get; init; }
}
