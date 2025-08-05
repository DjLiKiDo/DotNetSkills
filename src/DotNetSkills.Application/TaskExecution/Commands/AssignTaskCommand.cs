namespace DotNetSkills.Application.TaskExecution.Commands;

/// <summary>
/// Command to assign a task to a user.
/// </summary>
public record AssignTaskCommand(
    TaskId TaskId,
    UserId AssignedUserId,
    UserId AssignedByUserId
) : IRequest<TaskAssignmentResponse>
{
    /// <summary>
    /// Validates the assign task command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when required parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("TaskId cannot be empty", nameof(TaskId));
        if (AssignedUserId.Value == Guid.Empty)
            throw new ArgumentException("AssignedUserId cannot be empty", nameof(AssignedUserId));
        if (AssignedByUserId.Value == Guid.Empty)
            throw new ArgumentException("AssignedByUserId cannot be empty", nameof(AssignedByUserId));
    }
};

/// <summary>
/// Command to unassign a task from its current assignee.
/// </summary>
public record UnassignTaskCommand(
    TaskId TaskId,
    UserId UnassignedByUserId
) : IRequest<TaskAssignmentResponse>
{
    /// <summary>
    /// Validates the unassign task command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when required parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("TaskId cannot be empty", nameof(TaskId));
        if (UnassignedByUserId.Value == Guid.Empty)
            throw new ArgumentException("UnassignedByUserId cannot be empty", nameof(UnassignedByUserId));
    }
};

/// <summary>
/// Command to create a subtask.
/// </summary>
public record CreateSubtaskCommand(
    TaskId ParentTaskId,
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId? AssignedUserId,
    UserId CreatedByUserId
) : IRequest<TaskResponse>
{
    /// <summary>
    /// Validates the create subtask command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (ParentTaskId.Value == Guid.Empty)
            throw new ArgumentException("ParentTaskId cannot be empty", nameof(ParentTaskId));
        if (CreatedByUserId.Value == Guid.Empty)
            throw new ArgumentException("CreatedByUserId cannot be empty", nameof(CreatedByUserId));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(Title));

        if (Description is not null && Description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(Description));

        if (EstimatedHours.HasValue && EstimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future", nameof(DueDate));

        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(Priority))
            throw new ArgumentException($"Priority must be one of: {string.Join(", ", validPriorities)}", nameof(Priority));
    }
};

/// <summary>
/// Command to update a subtask.
/// </summary>
public record UpdateSubtaskCommand(
    TaskId TaskId,
    string Title,
    string? Description,
    string Priority,
    int? EstimatedHours,
    DateTime? DueDate,
    UserId UpdatedByUserId
) : IRequest<TaskResponse>
{
    /// <summary>
    /// Validates the update subtask command.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("TaskId cannot be empty", nameof(TaskId));
        if (UpdatedByUserId.Value == Guid.Empty)
            throw new ArgumentException("UpdatedByUserId cannot be empty", nameof(UpdatedByUserId));

        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Title is required", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Title cannot exceed 200 characters", nameof(Title));

        if (Description is not null && Description.Length > 2000)
            throw new ArgumentException("Description cannot exceed 2000 characters", nameof(Description));

        if (EstimatedHours.HasValue && EstimatedHours.Value <= 0)
            throw new ArgumentException("Estimated hours must be positive", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future", nameof(DueDate));

        var validPriorities = new[] { "Low", "Medium", "High", "Critical" };
        if (!validPriorities.Contains(Priority))
            throw new ArgumentException($"Priority must be one of: {string.Join(", ", validPriorities)}", nameof(Priority));
    }
};
