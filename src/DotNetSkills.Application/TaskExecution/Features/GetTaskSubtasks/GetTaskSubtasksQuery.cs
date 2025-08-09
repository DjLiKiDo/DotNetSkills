namespace DotNetSkills.Application.TaskExecution.Features.GetTaskSubtasks;

/// <summary>
/// Query to get subtasks for a specific task.
/// </summary>
public record GetTaskSubtasksQuery(
    TaskId TaskId
) : IRequest<TaskSubtasksResponse>;
