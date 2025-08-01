namespace DotNetSkills.Domain.TaskExecution.Entities;

/// <summary>
/// Represents a task in the DotNetSkills system.
/// This is an aggregate root that manages task lifecycle, assignment, and subtasks.
/// </summary>
public class Task : AggregateRoot<TaskId>
{
    private readonly List<Task> _subtasks = [];

    /// <summary>
    /// Gets the task title.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the task description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the current status of the task.
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// Gets the priority of the task.
    /// </summary>
    public TaskPriority Priority { get; private set; }

    /// <summary>
    /// Gets the ID of the project this task belongs to.
    /// </summary>
    public ProjectId ProjectId { get; private set; }

    /// <summary>
    /// Gets the ID of the user assigned to this task (if any).
    /// </summary>
    public UserId? AssignedUserId { get; private set; }

    /// <summary>
    /// Gets the ID of the parent task (if this is a subtask).
    /// </summary>
    public TaskId? ParentTaskId { get; private set; }

    /// <summary>
    /// Gets the estimated effort in hours.
    /// </summary>
    public int? EstimatedHours { get; private set; }

    /// <summary>
    /// Gets the actual effort in hours (when completed).
    /// </summary>
    public int? ActualHours { get; private set; }

    /// <summary>
    /// Gets the due date for the task.
    /// </summary>
    public DateTime? DueDate { get; private set; }

    /// <summary>
    /// Gets the date when the task was started.
    /// </summary>
    public DateTime? StartedAt { get; private set; }

    /// <summary>
    /// Gets the date when the task was completed.
    /// </summary>
    public DateTime? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the read-only collection of subtasks.
    /// </summary>
    public IReadOnlyList<Task> Subtasks => _subtasks.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Task() : base(TaskId.New())
    {
        Title = string.Empty;
        ProjectId = null!;
    }

    /// <summary>
    /// Initializes a new instance of the Task class.
    /// </summary>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description (optional).</param>
    /// <param name="projectId">The ID of the project this task belongs to.</param>
    /// <param name="priority">The priority of the task.</param>
    /// <param name="parentTaskId">The ID of the parent task (if this is a subtask).</param>
    /// <param name="estimatedHours">The estimated effort in hours (optional).</param>
    /// <param name="dueDate">The due date for the task (optional).</param>
    /// <param name="createdBy">The user creating the task.</param>
    /// <exception cref="ArgumentException">Thrown when title is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when projectId or createdBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public Task(string title, string? description, ProjectId projectId, TaskPriority priority, 
                TaskId? parentTaskId, int? estimatedHours, DateTime? dueDate, User createdBy) 
        : base(TaskId.New())
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title cannot be empty", nameof(title));

        ArgumentNullException.ThrowIfNull(projectId);
        ArgumentNullException.ThrowIfNull(createdBy);

        if (estimatedHours.HasValue && estimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(estimatedHours));

        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
            throw new DomainException("Due date must be in the future");

        Title = title.Trim();
        Description = description?.Trim();
        ProjectId = projectId;
        Priority = priority;
        ParentTaskId = parentTaskId;
        EstimatedHours = estimatedHours;
        DueDate = dueDate;
        Status = TaskStatus.ToDo;

        // Raise domain event
        RaiseDomainEvent(new TaskCreatedDomainEvent(Id, Title, ProjectId, ParentTaskId, createdBy.Id));
    }

    /// <summary>
    /// Creates a new task with the specified details.
    /// </summary>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description (optional).</param>
    /// <param name="projectId">The ID of the project this task belongs to.</param>
    /// <param name="priority">The priority of the task.</param>
    /// <param name="parentTask">The parent task (if this is a subtask).</param>
    /// <param name="estimatedHours">The estimated effort in hours (optional).</param>
    /// <param name="dueDate">The due date for the task (optional).</param>
    /// <param name="createdBy">The user creating the task.</param>
    /// <returns>A new Task instance.</returns>
    /// <exception cref="DomainException">Thrown when trying to create a subtask of a subtask.</exception>
    public static Task Create(string title, string? description, ProjectId projectId, TaskPriority priority,
                             Task? parentTask, int? estimatedHours, DateTime? dueDate, User createdBy)
    {
        // Enforce single-level subtask nesting
        if (parentTask?.ParentTaskId != null)
            throw new DomainException("Cannot create subtasks of subtasks (only single-level nesting allowed)");

        return new Task(title, description, projectId, priority, parentTask?.Id, estimatedHours, dueDate, createdBy);
    }

    /// <summary>
    /// Updates the task information.
    /// </summary>
    /// <param name="title">The new task title.</param>
    /// <param name="description">The new task description (optional).</param>
    /// <param name="priority">The new priority.</param>
    /// <param name="estimatedHours">The new estimated effort (optional).</param>
    /// <param name="dueDate">The new due date (optional).</param>
    /// <param name="updatedBy">The user updating the task.</param>
    /// <exception cref="ArgumentException">Thrown when title is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when updatedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void UpdateInfo(string title, string? description, TaskPriority priority, 
                          int? estimatedHours, DateTime? dueDate, User updatedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Task title cannot be empty", nameof(title));

        ArgumentNullException.ThrowIfNull(updatedBy);

        if (Status == TaskStatus.Done)
            throw new DomainException("Cannot modify completed tasks");

        if (estimatedHours.HasValue && estimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(estimatedHours));

        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow && Status != TaskStatus.Done)
            throw new DomainException("Due date must be in the future for active tasks");

        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority;
        EstimatedHours = estimatedHours;
        DueDate = dueDate;
    }

    /// <summary>
    /// Assigns the task to a user.
    /// </summary>
    /// <param name="assignee">The user to assign the task to.</param>
    /// <param name="assignedBy">The user making the assignment.</param>
    /// <exception cref="ArgumentNullException">Thrown when assignee or assignedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void AssignTo(User assignee, User assignedBy)
    {
        ArgumentNullException.ThrowIfNull(assignee);
        ArgumentNullException.ThrowIfNull(assignedBy);

        if (Status == TaskStatus.Done)
            throw new DomainException("Cannot assign completed tasks");

        if (Status == TaskStatus.Cancelled)
            throw new DomainException("Cannot assign cancelled tasks");

        if (!assignee.CanBeAssignedTasks())
            throw new DomainException("User cannot be assigned tasks");

        if (AssignedUserId == assignee.Id)
            throw new DomainException("Task is already assigned to this user");

        AssignedUserId = assignee.Id;

        // Raise domain event
        RaiseDomainEvent(new TaskAssignedDomainEvent(Id, assignee.Id, assignedBy.Id));
    }

    /// <summary>
    /// Unassigns the task from its current assignee.
    /// </summary>
    /// <param name="unassignedBy">The user removing the assignment.</param>
    /// <exception cref="ArgumentNullException">Thrown when unassignedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Unassign(User unassignedBy)
    {
        ArgumentNullException.ThrowIfNull(unassignedBy);

        if (AssignedUserId == null)
            throw new DomainException("Task is not assigned to anyone");

        if (Status == TaskStatus.Done)
            throw new DomainException("Cannot unassign completed tasks");

        AssignedUserId = null;
    }

    /// <summary>
    /// Starts work on the task.
    /// </summary>
    /// <param name="startedBy">The user starting the task.</param>
    /// <exception cref="ArgumentNullException">Thrown when startedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Start(User startedBy)
    {
        ArgumentNullException.ThrowIfNull(startedBy);

        if (!CanTransitionTo(TaskStatus.InProgress))
            throw new DomainException($"Cannot start task from {Status} status");

        if (AssignedUserId != null && AssignedUserId != startedBy.Id)
            throw new DomainException("Only the assigned user can start this task");

        if (AssignedUserId == null)
        {
            // Auto-assign to the user starting the task
            AssignedUserId = startedBy.Id;
            RaiseDomainEvent(new TaskAssignedDomainEvent(Id, startedBy.Id, startedBy.Id));
        }

        var previousStatus = Status;
        Status = TaskStatus.InProgress;
        StartedAt = DateTime.UtcNow;

        // Raise domain event
        RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, previousStatus, Status, startedBy.Id));
    }

    /// <summary>
    /// Moves the task to review status.
    /// </summary>
    /// <param name="submittedBy">The user submitting the task for review.</param>
    /// <exception cref="ArgumentNullException">Thrown when submittedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void SubmitForReview(User submittedBy)
    {
        ArgumentNullException.ThrowIfNull(submittedBy);

        if (!CanTransitionTo(TaskStatus.InReview))
            throw new DomainException($"Cannot submit task for review from {Status} status");

        if (AssignedUserId != null && AssignedUserId != submittedBy.Id)
            throw new DomainException("Only the assigned user can submit this task for review");

        var previousStatus = Status;
        Status = TaskStatus.InReview;

        // Raise domain event
        RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, previousStatus, Status, submittedBy.Id));
    }

    /// <summary>
    /// Marks the task as completed.
    /// </summary>
    /// <param name="completedBy">The user completing the task.</param>
    /// <param name="actualHours">The actual hours spent on the task (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when completedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Complete(User completedBy, int? actualHours = null)
    {
        ArgumentNullException.ThrowIfNull(completedBy);

        if (!CanTransitionTo(TaskStatus.Done))
            throw new DomainException($"Cannot complete task from {Status} status");

        if (actualHours.HasValue && actualHours.Value <= 0)
            throw new ArgumentException("Actual hours must be positive", nameof(actualHours));

        // Check if all subtasks are completed
        if (_subtasks.Any(st => st.Status != TaskStatus.Done && st.Status != TaskStatus.Cancelled))
            throw new DomainException("Cannot complete task with incomplete subtasks");

        var previousStatus = Status;
        Status = TaskStatus.Done;
        CompletedAt = DateTime.UtcNow;
        ActualHours = actualHours;

        // Raise domain event
        RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, previousStatus, Status, completedBy.Id));
    }

    /// <summary>
    /// Cancels the task.
    /// </summary>
    /// <param name="cancelledBy">The user cancelling the task.</param>
    /// <exception cref="ArgumentNullException">Thrown when cancelledBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Cancel(User cancelledBy)
    {
        ArgumentNullException.ThrowIfNull(cancelledBy);

        if (Status == TaskStatus.Done)
            throw new DomainException("Cannot cancel completed tasks");

        var previousStatus = Status;
        Status = TaskStatus.Cancelled;

        // Cancel all subtasks
        foreach (var subtask in _subtasks.Where(st => st.Status != TaskStatus.Done && st.Status != TaskStatus.Cancelled))
        {
            subtask.Cancel(cancelledBy);
        }

        // Raise domain event
        RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, previousStatus, Status, cancelledBy.Id));
    }

    /// <summary>
    /// Reopens a cancelled or completed task.
    /// </summary>
    /// <param name="reopenedBy">The user reopening the task.</param>
    /// <exception cref="ArgumentNullException">Thrown when reopenedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void Reopen(User reopenedBy)
    {
        ArgumentNullException.ThrowIfNull(reopenedBy);

        if (Status != TaskStatus.Done && Status != TaskStatus.Cancelled)
            throw new DomainException("Can only reopen completed or cancelled tasks");

        var previousStatus = Status;
        Status = StartedAt.HasValue ? TaskStatus.InProgress : TaskStatus.ToDo;
        CompletedAt = null;
        ActualHours = null;

        // Raise domain event
        RaiseDomainEvent(new TaskStatusChangedDomainEvent(Id, previousStatus, Status, reopenedBy.Id));
    }

    /// <summary>
    /// Adds a subtask to this task.
    /// </summary>
    /// <param name="subtask">The subtask to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when subtask is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void AddSubtask(Task subtask)
    {
        ArgumentNullException.ThrowIfNull(subtask);

        if (ParentTaskId != null)
            throw new DomainException("Subtasks cannot have their own subtasks (only single-level nesting allowed)");

        if (subtask.ParentTaskId != Id)
            throw new DomainException("Subtask parent ID does not match this task");

        if (_subtasks.Any(st => st.Id == subtask.Id))
            throw new DomainException("Subtask is already added to this task");

        _subtasks.Add(subtask);
    }

    /// <summary>
    /// Checks if the task is overdue.
    /// </summary>
    /// <returns>True if the task is overdue, false otherwise.</returns>
    public bool IsOverdue()
    {
        return DueDate.HasValue && 
               DueDate.Value < DateTime.UtcNow && 
               Status != TaskStatus.Done &&
               Status != TaskStatus.Cancelled;
    }

    /// <summary>
    /// Checks if the task is assigned.
    /// </summary>
    /// <returns>True if the task is assigned, false otherwise.</returns>
    public bool IsAssigned()
    {
        return AssignedUserId != null;
    }

    /// <summary>
    /// Checks if the task is a subtask.
    /// </summary>
    /// <returns>True if the task is a subtask, false otherwise.</returns>
    public bool IsSubtask()
    {
        return ParentTaskId != null;
    }

    /// <summary>
    /// Checks if the task has subtasks.
    /// </summary>
    /// <returns>True if the task has subtasks, false otherwise.</returns>
    public bool HasSubtasks()
    {
        return _subtasks.Count > 0;
    }

    /// <summary>
    /// Gets the completion percentage based on subtasks (if any).
    /// </summary>
    /// <returns>The completion percentage (0-100).</returns>
    public decimal GetCompletionPercentage()
    {
        if (!HasSubtasks())
        {
            return Status == TaskStatus.Done ? 100m : 0m;
        }

        var completedSubtasks = _subtasks.Count(st => st.Status == TaskStatus.Done);
        return _subtasks.Count == 0 ? 0m : (decimal)completedSubtasks / _subtasks.Count * 100m;
    }

    /// <summary>
    /// Gets the duration of the task (if started).
    /// </summary>
    /// <returns>The task duration, or null if not started.</returns>
    public TimeSpan? GetDuration()
    {
        if (!StartedAt.HasValue)
            return null;

        var endDateTime = CompletedAt ?? DateTime.UtcNow;
        return endDateTime - StartedAt.Value;
    }

    /// <summary>
    /// Checks if the task can transition to the specified status.
    /// </summary>
    /// <param name="newStatus">The target status.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    private bool CanTransitionTo(TaskStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (TaskStatus.ToDo, TaskStatus.InProgress) => true,
            (TaskStatus.ToDo, TaskStatus.Cancelled) => true,
            (TaskStatus.InProgress, TaskStatus.InReview) => true,
            (TaskStatus.InProgress, TaskStatus.Done) => true,
            (TaskStatus.InProgress, TaskStatus.Cancelled) => true,
            (TaskStatus.InReview, TaskStatus.InProgress) => true,
            (TaskStatus.InReview, TaskStatus.Done) => true,
            (TaskStatus.InReview, TaskStatus.Cancelled) => true,
            _ => false
        };
    }
}
