namespace DotNetSkills.Application.TaskExecution.Contracts.Responses;

/// <summary>
/// Paginated response DTO for task listings with metadata and filtering information.
/// Supports comprehensive filtering and sorting capabilities for task management.
/// </summary>
public record PagedTaskResponse(
    IEnumerable<TaskResponse> Tasks,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage,
    TaskFilterMetadata FilterMetadata
);

/// <summary>
/// Metadata about applied filters and aggregate statistics for the task query.
/// Provides insights into the filtered task set for dashboard and reporting purposes.
/// </summary>
public record TaskFilterMetadata(
    Guid? ProjectId,
    string? ProjectName,
    Guid? AssignedUserId,
    string? AssignedUserName,
    DomainTaskStatus? Status,
    TaskPriority? Priority,
    DateTime? DueDateFrom,
    DateTime? DueDateTo,
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    string? SearchTerm,
    int TotalActiveTasks,
    int TotalCompletedTasks,
    int TotalOverdueTasks,
    int TotalUnassignedTasks,
    int TotalCriticalPriorityTasks
);

/// <summary>
/// Request DTO for creating a new task with validation attributes.
/// Contains all necessary information for task creation within the domain constraints.
/// </summary>
public record CreateTaskRequest(
    string Title,
    string? Description,
    Guid ProjectId,
    TaskPriority Priority,
    Guid? ParentTaskId,
    int? EstimatedHours,
    DateTime? DueDate,
    Guid? AssignedUserId
)
{
    /// <summary>
    /// Validates the create task request parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Task title is required and cannot be empty.", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Task title cannot exceed 200 characters.", nameof(Title));

        if (Description?.Length > 2000)
            throw new ArgumentException("Task description cannot exceed 2000 characters.", nameof(Description));

        if (ProjectId == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));

        if (!Enum.IsDefined(typeof(TaskPriority), Priority))
            throw new ArgumentException("Invalid task priority.", nameof(Priority));

        if (EstimatedHours.HasValue && EstimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive.", nameof(EstimatedHours));

        if (EstimatedHours.HasValue && EstimatedHours > 1000)
            throw new ArgumentException("Estimated hours cannot exceed 1000 hours.", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future.", nameof(DueDate));

        if (ParentTaskId.HasValue && ParentTaskId == Guid.Empty)
            throw new ArgumentException("Parent task ID cannot be empty GUID.", nameof(ParentTaskId));

        if (AssignedUserId.HasValue && AssignedUserId == Guid.Empty)
            throw new ArgumentException("Assigned user ID cannot be empty GUID.", nameof(AssignedUserId));
    }

    /// <summary>
    /// Validates task priority values.
    /// </summary>
    private static bool IsValidPriority(TaskPriority priority) => Enum.IsDefined(typeof(TaskPriority), priority);
}

/// <summary>
/// Request DTO for updating an existing task with validation attributes.
/// Contains all updatable task properties with proper validation rules.
/// </summary>
public record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    int? EstimatedHours,
    DateTime? DueDate
)
{
    /// <summary>
    /// Validates the update task request parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Task title is required and cannot be empty.", nameof(Title));

        if (Title.Length > 200)
            throw new ArgumentException("Task title cannot exceed 200 characters.", nameof(Title));

        if (Description?.Length > 2000)
            throw new ArgumentException("Task description cannot exceed 2000 characters.", nameof(Description));

        if (!Enum.IsDefined(typeof(TaskPriority), Priority))
            throw new ArgumentException("Invalid task priority.", nameof(Priority));

        if (EstimatedHours.HasValue && EstimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive.", nameof(EstimatedHours));

        if (EstimatedHours.HasValue && EstimatedHours > 1000)
            throw new ArgumentException("Estimated hours cannot exceed 1000 hours.", nameof(EstimatedHours));

        if (DueDate.HasValue && DueDate <= DateTime.UtcNow)
            throw new ArgumentException("Due date must be in the future.", nameof(DueDate));
    }

    /// <summary>
    /// Validates task priority values.
    /// </summary>
    private static bool IsValidPriority(TaskPriority priority) => Enum.IsDefined(typeof(TaskPriority), priority);
}

/// <summary>
/// Request DTO for updating task status with validation.
/// Supports task status transitions according to domain business rules.
/// </summary>
public record UpdateTaskStatusRequest(
    DomainTaskStatus Status,
    int? ActualHours
)
{
    /// <summary>
    /// Validates the update task status request parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    public void Validate()
    {
        if (!Enum.IsDefined(typeof(DomainTaskStatus), Status))
            throw new ArgumentException("Invalid task status.", nameof(Status));

        if (ActualHours.HasValue && ActualHours <= 0)
            throw new ArgumentException("Actual hours must be positive.", nameof(ActualHours));

        if (ActualHours.HasValue && ActualHours > 2000)
            throw new ArgumentException("Actual hours cannot exceed 2000 hours.", nameof(ActualHours));
    }

    /// <summary>
    /// Validates task status values.
    /// </summary>
    private static bool IsValidStatus(DomainTaskStatus status) => Enum.IsDefined(typeof(DomainTaskStatus), status);
}
