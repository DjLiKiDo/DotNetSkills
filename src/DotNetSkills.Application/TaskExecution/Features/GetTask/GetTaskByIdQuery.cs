namespace DotNetSkills.Application.TaskExecution.Features.GetTask;

/// <summary>
/// Query for retrieving a specific task by its ID with full details.
/// Returns complete task information including relationships and calculated properties.
/// </summary>
public record GetTaskByIdQuery(TaskId TaskId) : IRequest<TaskResponse?>;
