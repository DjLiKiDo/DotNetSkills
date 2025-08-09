namespace DotNetSkills.Application.ProjectManagement.Features.UpdateTaskInProject;

/// <summary>
/// Command for updating a task within a specific project context.
/// Ensures the task belongs to the project and validates business rules.
/// </summary>
public record UpdateTaskInProjectCommand(
    ProjectId ProjectId,
    TaskId TaskId,
    string Title,
    string? Description,
    TaskPriority Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId UpdatedBy
) : IRequest<ProjectTaskResponse>;
