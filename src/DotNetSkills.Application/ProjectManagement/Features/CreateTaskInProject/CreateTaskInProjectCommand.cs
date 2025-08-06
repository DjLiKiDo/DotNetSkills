namespace DotNetSkills.Application.ProjectManagement.Features.CreateTaskInProject;

/// <summary>
/// Command for creating a new task within a specific project context.
/// Ensures the task belongs to the project and validates business rules.
/// </summary>
public record CreateTaskInProjectCommand(
    ProjectId ProjectId,
    string Title,
    string? Description,
    string Priority,
    TaskId? ParentTaskId,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId? AssignedUserId,
    UserId CreatedBy
) : IRequest<ProjectTaskResponse>;
