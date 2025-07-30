using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.Events;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Entities;

public class Task : BaseEntity<TaskId>
{
    private readonly List<Task> _subtasks = new();

    private Task() : base() { }

    public Task(TaskId id, string title, string? description, ProjectId projectId, TaskType type = TaskType.Task, TaskPriority priority = TaskPriority.Medium, TaskId? parentTaskId = null)
        : base(id)
    {
        Title = ValidateAndSetTitle(title);
        Description = description?.Trim();
        ProjectId = projectId;
        Type = ValidateTaskType(type, parentTaskId);
        Priority = priority;
        Status = Enums.TaskStatus.ToDo;
        ParentTaskId = parentTaskId;
        EstimatedHours = null;
        ActualHours = null;
        DueDate = null;
    }

    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectId ProjectId { get; private set; }
    public TaskType Type { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Enums.TaskStatus Status { get; private set; }
    public TaskId? ParentTaskId { get; private set; }
    public UserId? AssignedToId { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public decimal? ActualHours { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public Task? ParentTask { get; private set; }
    public User? AssignedTo { get; private set; }
    public IReadOnlyList<Task> Subtasks => _subtasks.AsReadOnly();

    public bool IsSubtask => ParentTaskId.HasValue;
    public bool HasSubtasks => _subtasks.Count > 0;
    public int SubtaskCount => _subtasks.Count;
    public int CompletedSubtaskCount => _subtasks.Count(st => st.Status.IsComplete());
    public bool IsOverdue => DueDate.HasValue && DateTime.UtcNow > DueDate.Value && !Status.IsComplete();

    public double SubtaskCompletionPercentage => SubtaskCount == 0 ? 100 : (double)CompletedSubtaskCount / SubtaskCount * 100;

    public void UpdateDetails(string title, string? description, TaskPriority priority, decimal? estimatedHours = null, DateTime? dueDate = null)
    {
        Title = ValidateAndSetTitle(title);
        Description = description?.Trim();
        Priority = priority;
        EstimatedHours = ValidateEstimatedHours(estimatedHours);
        DueDate = dueDate;
        UpdateTimestamp();
    }

    public void UpdateStatus(Enums.TaskStatus newStatus, UserRole updatedByRole, UserId updatedBy)
    {
        if (!updatedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can update task status.");

        if (Status == newStatus)
            return;

        if (!Status.CanTransitionTo(newStatus))
            throw new DomainException($"Cannot transition task status from {Status.GetDisplayName()} to {newStatus.GetDisplayName()}.");

        var oldStatus = Status;
        Status = newStatus;
        var updatedAt = DateTime.UtcNow;

        if (newStatus == Enums.TaskStatus.Done)
        {
            CompletedAt = updatedAt;
            
            RaiseDomainEvent(new TaskCompletedDomainEvent(
                Id,
                Title,
                ProjectId,
                AssignedToId,
                AssignedTo?.FullName,
                Priority,
                EstimatedHours,
                ActualHours,
                updatedAt,
                updatedBy
            ));
        }
        else if (oldStatus == Enums.TaskStatus.Done)
        {
            CompletedAt = null;
        }

        UpdateTimestamp();

        RaiseDomainEvent(new TaskStatusUpdatedDomainEvent(
            Id,
            Title,
            ProjectId,
            oldStatus,
            newStatus,
            AssignedToId,
            updatedBy,
            updatedAt
        ));
    }

    public void AssignTo(User user, UserRole assignedByRole, UserId assignedBy)
    {
        if (!assignedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can assign tasks.");

        if (!user.IsActive)
            throw new DomainException("Cannot assign tasks to inactive users.");

        if (!user.CanAccessTeam(Project.TeamId))
            throw new DomainException("Cannot assign task to user who is not a member of the project's team.");

        var previousAssignee = AssignedTo;
        AssignedToId = user.Id;
        AssignedTo = user;

        if (previousAssignee != null)
            previousAssignee.UnassignTask(Id);

        user.AssignTask(this);
        var assignedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new TaskAssignedDomainEvent(
            Id,
            Title,
            ProjectId,
            user.Id,
            user.FullName,
            previousAssignee?.Id,
            previousAssignee?.FullName,
            assignedBy,
            assignedAt
        ));
    }

    public void Unassign(UserRole unassignedByRole, UserId unassignedBy)
    {
        if (!unassignedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can unassign tasks.");

        if (AssignedTo == null)
            return;

        var previousAssignee = AssignedTo;
        AssignedToId = null;
        AssignedTo = null;

        previousAssignee.UnassignTask(Id);
        var unassignedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new TaskUnassignedDomainEvent(
            Id,
            Title,
            ProjectId,
            previousAssignee.Id,
            previousAssignee.FullName,
            unassignedBy,
            unassignedAt
        ));
    }

    public void RecordActualHours(decimal hours, UserRole recordedByRole)
    {
        if (!recordedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can record actual hours.");

        if (hours < 0)
            throw new DomainException("Actual hours cannot be negative.");

        ActualHours = hours;
        UpdateTimestamp();
    }

    public Task CreateSubtask(TaskId subtaskId, string title, string? description, TaskPriority priority = TaskPriority.Medium, UserRole createdByRole = UserRole.Developer)
    {
        if (!createdByRole.CanManageTasks())
            throw new DomainException("Only developers and above can create subtasks.");

        if (!Type.CanHaveSubtasks())
            throw new DomainException($"Tasks of type {Type.GetDisplayName()} cannot have subtasks.");

        if (IsSubtask)
            throw new DomainException("Subtasks cannot have their own subtasks (single-level nesting only).");

        var subtask = new Task(subtaskId, title, description, ProjectId, TaskType.Subtask, priority, Id);
        _subtasks.Add(subtask);
        subtask.ParentTask = this;

        UpdateTimestamp();
        return subtask;
    }

    public void RemoveSubtask(TaskId subtaskId, UserRole removedByRole)
    {
        if (!removedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can remove subtasks.");

        var subtask = _subtasks.FirstOrDefault(st => st.Id == subtaskId);
        if (subtask == null)
            throw new DomainException("Subtask not found.");

        _subtasks.Remove(subtask);
        UpdateTimestamp();
    }

    public bool CanUserModifyTask(User user)
    {
        if (!user.IsActive)
            return false;

        if (user.Role.CanManageTasks() && user.CanAccessTeam(Project.TeamId))
            return true;

        return AssignedToId == user.Id;
    }

    public Task? GetSubtask(TaskId subtaskId)
    {
        return _subtasks.FirstOrDefault(st => st.Id == subtaskId);
    }

    public IEnumerable<Task> GetSubtasksByStatus(Enums.TaskStatus status)
    {
        return _subtasks.Where(st => st.Status == status);
    }

    internal void SetProject(Project project)
    {
        Project = project;
        project.AddTask(this);
    }

    internal void SetParentTask(Task parentTask)
    {
        ParentTask = parentTask;
    }

    internal void SetAssignedUser(User user)
    {
        AssignedTo = user;
    }

    private static string ValidateAndSetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title is required.");

        if (title.Length < 3)
            throw new DomainException("Task title must be at least 3 characters long.");

        if (title.Length > 200)
            throw new DomainException("Task title cannot exceed 200 characters.");

        return title.Trim();
    }

    private static TaskType ValidateTaskType(TaskType type, TaskId? parentTaskId)
    {
        if (parentTaskId.HasValue && type != TaskType.Subtask)
            throw new DomainException("Tasks with a parent must be of type Subtask.");

        if (!parentTaskId.HasValue && type == TaskType.Subtask)
            throw new DomainException("Subtasks must have a parent task.");

        return type;
    }

    private static decimal? ValidateEstimatedHours(decimal? estimatedHours)
    {
        if (estimatedHours.HasValue && estimatedHours.Value < 0)
            throw new DomainException("Estimated hours cannot be negative.");

        return estimatedHours;
    }
}