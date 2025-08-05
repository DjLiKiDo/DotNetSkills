namespace DotNetSkills.Domain.TaskExecution.Events;

/// <summary>
/// Domain event raised when a task is created.
/// </summary>
/// <param name="TaskId">The ID of the created task.</param>
/// <param name="Title">The title of the created task.</param>
/// <param name="ProjectId">The ID of the project the task belongs to.</param>
/// <param name="ParentTaskId">The ID of the parent task, if this is a subtask.</param>
/// <param name="CreatedBy">The ID of the user who created the task.</param>
public record TaskCreatedDomainEvent(
    TaskId TaskId,
    string Title,
    ProjectId ProjectId,
    TaskId? ParentTaskId,
    UserId CreatedBy) : BaseDomainEvent;
