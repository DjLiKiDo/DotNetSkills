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
    /// <exception cref="ArgumentException">Thrown when TaskId is invalid (Guid.Empty).</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("TaskId cannot be empty", nameof(TaskId));
    }
};
