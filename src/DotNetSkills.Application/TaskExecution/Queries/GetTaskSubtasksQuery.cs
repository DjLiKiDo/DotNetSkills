namespace DotNetSkills.Application.TaskExecution.Queries;

/// <summary>
/// Query to get subtasks for a specific task.
/// </summary>
public record GetTaskSubtasksQuery(
    TaskId TaskId
) : IRequest<TaskSubtasksResponse>
{
    /// <summary>
    /// Validates the get task subtasks query.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when TaskId is null.</exception>
    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(TaskId);
    }
};
