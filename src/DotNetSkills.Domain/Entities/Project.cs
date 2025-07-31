using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.Events;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Entities;

public class Project : BaseEntity<ProjectId>
{
    private readonly List<Task> _tasks = new();

    private Project() : base() { }

    public Project(ProjectId id, string name, string? description, TeamId teamId, DateTime? startDate = null, DateTime? endDate = null)
        : base(id)
    {
        Name = ValidateAndSetName(name);
        Description = description?.Trim();
        TeamId = teamId;
        Status = ProjectStatus.Planning;
        StartDate = startDate;
        EndDate = ValidateEndDate(startDate, endDate);
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TeamId TeamId { get; private set; }
    public ProjectStatus Status { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public Team Team { get; private set; } = null!;
    public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

    public int TotalTaskCount => _tasks.Count;
    public int CompletedTaskCount => _tasks.Count(t => t.Status.IsComplete());
    public int ActiveTaskCount => _tasks.Count(t => !t.Status.IsComplete());
    
    public double CompletionPercentage => TotalTaskCount == 0 ? 0 : (double)CompletedTaskCount / TotalTaskCount * 100;

    public bool IsOverdue => EndDate.HasValue && GetCurrentTime() > EndDate.Value && !Status.IsFinalized();

    public void UpdateDetails(string name, string? description, UserId updatedBy, DateTime? startDate = null, DateTime? endDate = null)
    {
        var oldEndDate = EndDate;
        
        Name = ValidateAndSetName(name);
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = ValidateEndDate(startDate, endDate);
        var updatedAt = GetCurrentTime();
        UpdateTimestamp();

        if (oldEndDate != EndDate)
        {
            RaiseDomainEvent(new ProjectDeadlineUpdatedDomainEvent(
                Id,
                Name,
                TeamId,
                oldEndDate,
                EndDate,
                updatedBy,
                updatedAt
            ));
        }
    }

    public void UpdateStatus(ProjectStatus newStatus, UserRole updatedByRole, UserId updatedBy)
    {
        if (!updatedByRole.CanManageProjects())
            throw new DomainException("Only project managers and administrators can update project status.");

        if (Status == newStatus)
            return;

        if (!Status.CanTransitionTo(newStatus))
            throw new DomainException($"Cannot transition project status from {Status.GetDisplayName()} to {newStatus.GetDisplayName()}.");

        var oldStatus = Status;
        Status = newStatus;
        var updatedAt = GetCurrentTime();
        UpdateTimestamp();

        RaiseDomainEvent(new ProjectStatusUpdatedDomainEvent(
            Id,
            Name,
            TeamId,
            oldStatus,
            newStatus,
            updatedBy,
            updatedAt
        ));
    }

    public void AssignToTeam(TeamId newTeamId, UserRole assignedByRole, UserId reassignedBy)
    {
        if (!assignedByRole.CanManageProjects())
            throw new DomainException("Only project managers and administrators can reassign projects to teams.");

        if (TeamId == newTeamId)
            return;

        if (Status.IsFinalized())
            throw new DomainException("Cannot reassign finalized projects to different teams.");

        var previousTeamId = TeamId;
        TeamId = newTeamId;
        var reassignedAt = GetCurrentTime();
        UpdateTimestamp();

        RaiseDomainEvent(new ProjectReassignedDomainEvent(
            Id,
            Name,
            previousTeamId,
            newTeamId,
            reassignedBy,
            reassignedAt
        ));
    }

    public void AddTask(Task task)
    {
        if (Status.IsFinalized())
            throw new DomainException("Cannot add tasks to finalized projects.");

        if (_tasks.Any(t => t.Id == task.Id))
            return;

        _tasks.Add(task);
        UpdateTimestamp();
    }

    public void RemoveTask(TaskId taskId, UserRole removedByRole)
    {
        if (!removedByRole.CanManageTasks())
            throw new DomainException("Only developers and above can remove tasks.");

        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task == null)
            throw new DomainException("Task not found in this project.");

        if (Status.IsFinalized())
            throw new DomainException("Cannot remove tasks from finalized projects.");

        // Remove subtasks first
        var subtasks = _tasks.Where(t => t.ParentTaskId == taskId).ToList();
        foreach (var subtask in subtasks)
        {
            _tasks.Remove(subtask);
        }

        _tasks.Remove(task);
        UpdateTimestamp();
    }

    public bool CanUserAccessProject(User user)
    {
        if (!user.IsActive)
            return false;

        if (user.Role.CanManageProjects())
            return true;

        return user.IsInTeam(TeamId);
    }

    public bool CanUserManageProject(User user)
    {
        return user.IsActive && user.Role.CanManageProjects();
    }

    public Task? GetTask(TaskId taskId)
    {
        return _tasks.FirstOrDefault(t => t.Id == taskId);
    }

    public IEnumerable<Task> GetTasksAssignedTo(UserId userId)
    {
        return _tasks.Where(t => t.AssignedToId == userId);
    }

    public IEnumerable<Task> GetTasksByStatus(Enums.TaskStatus status)
    {
        return _tasks.Where(t => t.Status == status);
    }

    public IEnumerable<Task> GetParentTasks()
    {
        return _tasks.Where(t => t.ParentTaskId == null);
    }

    public IEnumerable<Task> GetSubtasks(TaskId parentTaskId)
    {
        return _tasks.Where(t => t.ParentTaskId == parentTaskId);
    }

    public bool HasActiveTasks()
    {
        return _tasks.Any(t => !t.Status.IsComplete());
    }

    internal void SetTeam(Team team)
    {
        Team = team;
        team.AddProject(this);
    }

    private static string ValidateAndSetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Project name is required.");

        if (name.Length < DomainConstants.Project.MinNameLength)
            throw new DomainException($"Project name must be at least {DomainConstants.Project.MinNameLength} characters long.");

        if (name.Length > DomainConstants.Project.MaxNameLength)
            throw new DomainException($"Project name cannot exceed {DomainConstants.Project.MaxNameLength} characters.");

        return name.Trim();
    }

    private static DateTime? ValidateEndDate(DateTime? startDate, DateTime? endDate)
    {
        if (endDate.HasValue && startDate.HasValue && endDate.Value <= startDate.Value)
            throw new DomainException("End date must be after start date.");

        return endDate;
    }
}