namespace DotNetSkills.Application.TaskExecution.Features.CreateTask;

/// <summary>
/// Command for creating a new task with comprehensive validation and business rule enforcement.
/// Supports task creation with optional assignment and parent task relationships.
/// </summary>
public record CreateTaskCommand(
    string Title,
    string? Description,
    ProjectId ProjectId,
    string Priority,
    TaskId? ParentTaskId,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId? AssignedUserId,
    UserId CreatedBy
) : IRequest<TaskResponse>;
