namespace DotNetSkills.Application.TaskExecution.Commands;

/// <summary>
/// Command for updating task status with proper state transition validation and business rule enforcement.
/// Supports all valid status transitions according to domain business rules.
/// </summary>
public record UpdateTaskStatusCommand(
    TaskId TaskId,
    string Status,
    int? ActualHours,
    UserId UpdatedBy
) : IRequest<TaskResponse>
{
    /// <summary>
    /// Validates the command parameters and throws ArgumentException if invalid.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when command parameters are invalid.</exception>
    public void Validate()
    {
        if (TaskId.Value == Guid.Empty)
            throw new ArgumentException("Task ID cannot be empty.", nameof(TaskId));

        if (string.IsNullOrWhiteSpace(Status))
            throw new ArgumentException("Status is required and cannot be empty.", nameof(Status));

        if (!IsValidStatus(Status))
            throw new ArgumentException("Status must be one of: ToDo, InProgress, InReview, Done, Cancelled.", nameof(Status));

        if (ActualHours.HasValue && ActualHours <= 0)
            throw new ArgumentException("Actual hours must be positive.", nameof(ActualHours));

        if (ActualHours.HasValue && ActualHours > 2000)
            throw new ArgumentException("Actual hours cannot exceed 2000 hours.", nameof(ActualHours));

        if (UpdatedBy.Value == Guid.Empty)
            throw new ArgumentException("Updated by user ID cannot be empty.", nameof(UpdatedBy));
    }

    /// <summary>
    /// Validates task status values.
    /// </summary>
    private static bool IsValidStatus(string status)
    {
        return status is "ToDo" or "InProgress" or "InReview" or "Done" or "Cancelled";
    }
}

/// <summary>
/// Placeholder handler for UpdateTaskStatusCommand.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskResponse>
{
    public async Task<TaskResponse> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement actual command handling with repository
        // This would involve:
        // 1. Load existing task by ID
        // 2. Check if task exists and validate user has permission
        // 3. Parse status to TaskStatus enum
        // 4. Execute appropriate domain method based on target status:
        //    - ToDo: Reopen() if currently Done/Cancelled
        //    - InProgress: Start()
        //    - InReview: SubmitForReview()
        //    - Done: Complete() with actualHours
        //    - Cancelled: Cancel()
        // 5. Save task through repository
        // 6. Map to response DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("UpdateTaskStatusCommand requires Infrastructure layer implementation");
    }
}