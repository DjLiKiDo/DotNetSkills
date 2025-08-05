namespace DotNetSkills.Domain.ProjectManagement.Entities;

/// <summary>
/// Represents a project in the DotNetSkills system.
/// This is an aggregate root that manages project lifecycle and status.
/// </summary>
public class Project : AggregateRoot<ProjectId>
{
    /// <summary>
    /// Gets the project name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the project description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the current status of the project.
    /// </summary>
    public ProjectStatus Status { get; private set; }

    /// <summary>
    /// Gets the ID of the team associated with this project.
    /// </summary>
    public TeamId TeamId { get; private set; }

    /// <summary>
    /// Gets the project start date.
    /// </summary>
    public DateTime? StartDate { get; private set; }

    /// <summary>
    /// Gets the project end date.
    /// </summary>
    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Gets the planned completion date.
    /// </summary>
    public DateTime? PlannedEndDate { get; private set; }

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Project() : base(ProjectId.New())
    {
        Name = string.Empty;
        TeamId = null!;
    }

    /// <summary>
    /// Initializes a new instance of the Project class.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description (optional).</param>
    /// <param name="teamId">The ID of the team associated with the project.</param>
    /// <param name="plannedEndDate">The planned completion date (optional).</param>
    /// <param name="createdBy">The user creating the project.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when teamId or createdBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public Project(string name, string? description, TeamId teamId, DateTime? plannedEndDate, User createdBy)
        : base(ProjectId.New())
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));
        Ensure.NotNull(teamId, nameof(teamId));
        Ensure.NotNull(createdBy, nameof(createdBy));
        Ensure.BusinessRule(createdBy.CanManageProjects(), ValidationMessages.Project.NoPermissionToCreate);

        if (plannedEndDate.HasValue)
        {
            Ensure.FutureDate(plannedEndDate.Value, nameof(plannedEndDate));
        }

        Name = name.Trim();
        Description = description?.Trim();
        TeamId = teamId;
        Status = ProjectStatus.Planning;
        PlannedEndDate = plannedEndDate;

        // Raise domain event
        RaiseDomainEvent(new ProjectCreatedDomainEvent(Id, Name, TeamId, createdBy.Id));
    }

    /// <summary>
    /// Creates a new project with the specified details.
    /// </summary>
    /// <param name="name">The project name.</param>
    /// <param name="description">The project description (optional).</param>
    /// <param name="teamId">The ID of the team associated with the project.</param>
    /// <param name="plannedEndDate">The planned completion date (optional).</param>
    /// <param name="createdBy">The user creating the project.</param>
    /// <returns>A new Project instance.</returns>
    public static Project Create(string name, string? description, TeamId teamId, DateTime? plannedEndDate, User createdBy)
    {
        return new Project(name, description, teamId, plannedEndDate, createdBy);
    }

    /// <summary>
    /// Updates the project information.
    /// </summary>
    /// <param name="name">The new project name.</param>
    /// <param name="description">The new project description (optional).</param>
    /// <param name="plannedEndDate">The new planned completion date (optional).</param>
    /// <param name="updatedBy">The user updating the project.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when updatedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void UpdateInfo(string name, string? description, DateTime? plannedEndDate, User updatedBy)
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));
        Ensure.NotNull(updatedBy, nameof(updatedBy));
        Ensure.BusinessRule(CanModifyProject(updatedBy), ValidationMessages.Project.NoPermissionToModify);
        Ensure.BusinessRule(Status != ProjectStatus.Completed, ValidationMessages.Project.CannotModifyCompleted);

        if (plannedEndDate.HasValue && Status != ProjectStatus.Completed)
        {
            Ensure.FutureDate(plannedEndDate.Value, nameof(plannedEndDate));
        }

        Name = name.Trim();
        Description = description?.Trim();
        PlannedEndDate = plannedEndDate;
    }

    /// <summary>
    /// Starts the project by changing its status to Active.
    /// </summary>
    /// <param name="startedBy">The user starting the project.</param>
    /// <exception cref="ArgumentNullException">Thrown when startedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Start(User startedBy)
    {
        Ensure.NotNull(startedBy, nameof(startedBy));
        Ensure.BusinessRule(CanModifyProject(startedBy), ValidationMessages.Project.NoPermissionToStart);
        Ensure.BusinessRule(CanTransitionTo(ProjectStatus.Active),
            ValidationMessages.Formatting.Format(ValidationMessages.Common.InvalidStatusTransition, "project", Status.ToString(), ProjectStatus.Active.ToString()));

        var previousStatus = Status;
        Status = ProjectStatus.Active;
        StartDate = DateTime.UtcNow;

        // Raise domain event
        RaiseDomainEvent(new ProjectStatusChangedDomainEvent(Id, previousStatus, Status, startedBy.Id));
    }

    /// <summary>
    /// Puts the project on hold.
    /// </summary>
    /// <param name="pausedBy">The user putting the project on hold.</param>
    /// <exception cref="ArgumentNullException">Thrown when pausedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void PutOnHold(User pausedBy)
    {
        Ensure.NotNull(pausedBy, nameof(pausedBy));
        Ensure.BusinessRule(CanModifyProject(pausedBy), ValidationMessages.Project.NoPermissionToModify);
        Ensure.BusinessRule(CanTransitionTo(ProjectStatus.OnHold),
            ValidationMessages.Formatting.Format(ValidationMessages.Common.InvalidStatusTransition, "project", Status.ToString(), ProjectStatus.OnHold.ToString()));

        var previousStatus = Status;
        Status = ProjectStatus.OnHold;

        // Raise domain event
        RaiseDomainEvent(new ProjectStatusChangedDomainEvent(Id, previousStatus, Status, pausedBy.Id));
    }

    /// <summary>
    /// Resumes the project from on-hold status.
    /// </summary>
    /// <param name="resumedBy">The user resuming the project.</param>
    /// <exception cref="ArgumentNullException">Thrown when resumedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Resume(User resumedBy)
    {
        Ensure.NotNull(resumedBy, nameof(resumedBy));
        Ensure.BusinessRule(CanModifyProject(resumedBy), ValidationMessages.Project.NoPermissionToModify);
        Ensure.BusinessRule(Status == ProjectStatus.OnHold, ValidationMessages.Project.CanOnlyResumeFromHold);

        var previousStatus = Status;
        Status = ProjectStatus.Active;

        // Raise domain event
        RaiseDomainEvent(new ProjectStatusChangedDomainEvent(Id, previousStatus, Status, resumedBy.Id));
    }

    /// <summary>
    /// Completes the project.
    /// </summary>
    /// <param name="completedBy">The user completing the project.</param>
    /// <param name="hasActiveTasks">Whether the project has any active tasks.</param>
    /// <exception cref="ArgumentNullException">Thrown when completedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Complete(User completedBy, bool hasActiveTasks = false)
    {
        Ensure.NotNull(completedBy, nameof(completedBy));
        Ensure.BusinessRule(CanModifyProject(completedBy), ValidationMessages.Project.NoPermissionToComplete);
        Ensure.BusinessRule(CanTransitionTo(ProjectStatus.Completed),
            ValidationMessages.Formatting.Format(ValidationMessages.Common.InvalidStatusTransition, "project", Status.ToString(), ProjectStatus.Completed.ToString()));
        Ensure.BusinessRule(!hasActiveTasks, ValidationMessages.Project.CannotCompleteWithActiveTasks);

        var previousStatus = Status;
        Status = ProjectStatus.Completed;
        EndDate = DateTime.UtcNow;

        // Raise domain event
        RaiseDomainEvent(new ProjectStatusChangedDomainEvent(Id, previousStatus, Status, completedBy.Id));
    }

    /// <summary>
    /// Cancels the project.
    /// </summary>
    /// <param name="cancelledBy">The user cancelling the project.</param>
    /// <exception cref="ArgumentNullException">Thrown when cancelledBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Cancel(User cancelledBy)
    {
        Ensure.NotNull(cancelledBy, nameof(cancelledBy));
        Ensure.BusinessRule(CanModifyProject(cancelledBy), ValidationMessages.Project.NoPermissionToCancel);
        Ensure.BusinessRule(Status != ProjectStatus.Completed, ValidationMessages.Project.CannotCancelCompleted);

        var previousStatus = Status;
        Status = ProjectStatus.Cancelled;
        EndDate = DateTime.UtcNow;

        // Raise domain event
        RaiseDomainEvent(new ProjectStatusChangedDomainEvent(Id, previousStatus, Status, cancelledBy.Id));
    }

    /// <summary>
    /// Checks if the project can be deleted (only if it has no tasks).
    /// </summary>
    /// <param name="hasAnyTasks">Whether the project has any tasks.</param>
    /// <returns>True if the project can be deleted, false otherwise.</returns>
    public bool CanBeDeleted(bool hasAnyTasks)
    {
        return !hasAnyTasks && (Status == ProjectStatus.Planning || Status == ProjectStatus.Cancelled);
    }

    /// <summary>
    /// Checks if the project is currently active.
    /// </summary>
    /// <returns>True if the project is active, false otherwise.</returns>
    public bool IsActive()
    {
        return Status == ProjectStatus.Active;
    }

    /// <summary>
    /// Checks if the project is completed.
    /// </summary>
    /// <returns>True if the project is completed, false otherwise.</returns>
    public bool IsCompleted()
    {
        return Status == ProjectStatus.Completed;
    }

    /// <summary>
    /// Checks if the project is overdue based on the planned end date.
    /// </summary>
    /// <returns>True if the project is overdue, false otherwise.</returns>
    public bool IsOverdue()
    {
        return PlannedEndDate.HasValue &&
               PlannedEndDate.Value < DateTime.UtcNow &&
               Status != ProjectStatus.Completed &&
               Status != ProjectStatus.Cancelled;
    }

    /// <summary>
    /// Gets the duration of the project (if started).
    /// </summary>
    /// <returns>The project duration, or null if not started.</returns>
    public TimeSpan? GetDuration()
    {
        if (!StartDate.HasValue)
            return null;

        var endDateTime = EndDate ?? DateTime.UtcNow;
        return endDateTime - StartDate.Value;
    }

    /// <summary>
    /// Checks if the specified user can modify this project.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <returns>True if the user can modify the project, false otherwise.</returns>
    private bool CanModifyProject(User user)
    {
        // Admin can always modify projects
        if (user.Role == UserRole.Admin)
            return true;

        // Project managers can modify projects in their teams
        if (user.Role == UserRole.ProjectManager && user.IsMemberOfTeam(TeamId))
            return true;

        // Team leads can modify projects in their teams
        if (user.GetRoleInTeam(TeamId) == TeamRole.TeamLead)
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the project can transition to the specified status.
    /// </summary>
    /// <param name="newStatus">The target status.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    private bool CanTransitionTo(ProjectStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (ProjectStatus.Planning, ProjectStatus.Active) => true,
            (ProjectStatus.Planning, ProjectStatus.Cancelled) => true,
            (ProjectStatus.Active, ProjectStatus.OnHold) => true,
            (ProjectStatus.Active, ProjectStatus.Completed) => true,
            (ProjectStatus.Active, ProjectStatus.Cancelled) => true,
            (ProjectStatus.OnHold, ProjectStatus.Active) => true,
            (ProjectStatus.OnHold, ProjectStatus.Cancelled) => true,
            _ => false
        };
    }
}
