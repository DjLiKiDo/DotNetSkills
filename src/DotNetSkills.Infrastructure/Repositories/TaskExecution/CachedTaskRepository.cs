using DotNetSkills.Application.TaskExecution.Projections;

namespace DotNetSkills.Infrastructure.Repositories.TaskExecution;

/// <summary>
/// Cached implementation of the ITaskRepository interface using the Decorator pattern.
/// Provides in-memory caching for frequently accessed task data to improve performance.
/// Falls back to the underlying repository for cache misses and write operations.
/// </summary>
public class CachedTaskRepository : CachedRepositoryBase<DotNetSkills.Domain.TaskExecution.Entities.Task, TaskId, ITaskRepository>, ITaskRepository
{
    /// <summary>
    /// The entity name used for cache key generation.
    /// </summary>
    protected override string EntityName => "task";

    /// <summary>
    /// Initializes a new instance of the CachedTaskRepository class.
    /// </summary>
    /// <param name="innerRepository">The underlying repository implementation.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when innerRepository or cache is null.</exception>
    public CachedTaskRepository(ITaskRepository innerRepository, IMemoryCache cache) 
        : base(innerRepository, cache)
    {
    }

    #region ITaskRepository Specific Methods

    /// <summary>
    /// Gets tasks by project ID. This method bypasses cache due to project-specific nature.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByProjectIdAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetByProjectIdAsync(projectId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks by assignee ID. This method bypasses cache due to user-specific nature.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByAssigneeIdAsync(
        UserId userId, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetByAssigneeIdAsync(userId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets subtasks. This method bypasses cache due to hierarchy complexity.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetSubtasksAsync(
        TaskId parentTaskId, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetSubtasksAsync(parentTaskId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task with subtasks. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithSubtasksAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithSubtasksAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks by status with caching support.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified status.</returns>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByStatusAsync(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("status", status);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByStatusAsync(status, cancellationToken));
    }

    /// <summary>
    /// Gets tasks by priority with caching support.
    /// </summary>
    /// <param name="priority">The task priority to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified priority.</returns>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByPriorityAsync(
        TaskPriority priority, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetFilterCacheKey("priority", priority);
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetByPriorityAsync(priority, cancellationToken));
    }

    /// <summary>
    /// Gets overdue tasks. This method bypasses cache due to time-sensitive nature.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetOverdueTasksAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetOverdueTasksAsync(userId, projectId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets paginated tasks. This method bypasses cache due to dynamic nature of pagination.
    /// </summary>
    public async Task<(IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> Tasks, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchTerm = null,
        ProjectId? projectId = null,
        UserId? assignedUserId = null,
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus? status = null,
        TaskPriority? priority = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        bool? isOverdue = null,
        bool? isSubtask = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetPagedAsync(
            pageNumber, pageSize, searchTerm, projectId, assignedUserId, 
            status, priority, dueDateFrom, dueDateTo, isOverdue, isSubtask, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task hierarchy. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetTaskHierarchyAsync(
        TaskId taskId, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetTaskHierarchyAsync(taskId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks approaching deadline. This method bypasses cache due to time-sensitive nature.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetTasksApproachingDeadlineAsync(
        int daysUntilDeadline,
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetTasksApproachingDeadlineAsync(daysUntilDeadline, userId, projectId, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets unassigned tasks. This method bypasses cache due to assignment-specific nature.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetUnassignedTasksAsync(
        ProjectId? projectId = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetUnassignedTasksAsync(projectId, priority, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks with time tracking. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<(DotNetSkills.Domain.TaskExecution.Entities.Task Task, int? EstimatedHours, int? ActualHours)>> 
        GetTasksWithTimeTrackingAsync(
            ProjectId? projectId = null,
            UserId? userId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetTasksWithTimeTrackingAsync(projectId, userId, dateFrom, dateTo, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Checks if task has subtasks with caching support.
    /// </summary>
    /// <param name="taskId">The task ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the task has subtasks, otherwise false.</returns>
    public async Task<bool> HasSubtasksAsync(TaskId taskId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{EntityName}:hassubtasks:{taskId.Value}";
        
        return await GetCachedBoolAsync(cacheKey, () => InnerRepository.HasSubtasksAsync(taskId, cancellationToken));
    }

    /// <summary>
    /// Gets tasks by project ID as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByProjectIdAsyncEnumerable(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByProjectIdAsyncEnumerable(projectId, cancellationToken);
    }

    /// <summary>
    /// Gets tasks by assignee ID as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByAssigneeIdAsyncEnumerable(
        UserId userId, 
        CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByAssigneeIdAsyncEnumerable(userId, cancellationToken);
    }

    /// <summary>
    /// Gets tasks by status as async enumerable. Bypasses cache for streaming scenarios.
    /// </summary>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByStatusAsyncEnumerable(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default)
    {
        return InnerRepository.GetByStatusAsyncEnumerable(status, cancellationToken);
    }

    /// <summary>
    /// Gets task with project. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithProjectAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithProjectAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets project tasks with assignees. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetProjectTasksWithAssigneesAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetProjectTasksWithAssigneesAsync(projectId, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task with complete hierarchy. This method bypasses cache due to complexity.
    /// </summary>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithCompleteHierarchyAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetWithCompleteHierarchyAsync(id, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task summaries with caching support.
    /// </summary>
    public async Task<IEnumerable<TaskSummaryProjection>> GetTaskSummariesAsync(
        ProjectId? projectId = null, 
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("summaries", projectId?.Value.ToString());
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTaskSummariesAsync(projectId, cancellationToken));
    }

    /// <summary>
    /// Gets task dashboard data with caching support.
    /// </summary>
    public async Task<IEnumerable<TaskDashboardProjection>> GetTaskDashboardDataAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("dashboard", $"{userId?.Value}:{projectId?.Value}");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTaskDashboardDataAsync(userId, projectId, cancellationToken));
    }

    /// <summary>
    /// Gets task selections with caching support.
    /// </summary>
    public async Task<IEnumerable<TaskSelectionProjection>> GetTaskSelectionsAsync(
        ProjectId? projectId = null,
        bool assignableOnly = false,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = GetProjectionCacheKey("selections", $"{projectId?.Value}:{assignableOnly}");
        
        return await GetCachedCollectionAsync(cacheKey, () => InnerRepository.GetTaskSelectionsAsync(projectId, assignableOnly, cancellationToken));
    }

    /// <summary>
    /// Gets user task assignments. This method bypasses cache due to user-specific nature.
    /// </summary>
    public async Task<IEnumerable<TaskAssignmentProjection>> GetUserTaskAssignmentsAsync(
        UserId userId, 
        bool includeCompleted = false,
        CancellationToken cancellationToken = default)
    {
        return await InnerRepository.GetUserTaskAssignmentsAsync(userId, includeCompleted, cancellationToken)
            .ConfigureAwait(false);
    }

    #endregion

    #region Cache Invalidation

    /// <summary>
    /// Invalidates entity-specific cache entries.
    /// Called automatically by the base class after add, update, or remove operations.
    /// </summary>
    /// <param name="task">The task entity that was modified.</param>
    protected override void InvalidateEntitySpecificCaches(DotNetSkills.Domain.TaskExecution.Entities.Task task)
    {
        // Task-specific caches
        InvalidateCache($"{EntityName}:hassubtasks:{task.Id.Value}");
        
        // Status and priority based caches
        InvalidateCaches(
            GetFilterCacheKey("status", task.Status),
            GetFilterCacheKey("priority", task.Priority)
        );
        
        // Project-based caches
        if (task.ProjectId != null)
        {
            InvalidateCaches(
                GetProjectionCacheKey("summaries", task.ProjectId.Value.ToString()),
                GetProjectionCacheKey("selections", $"{task.ProjectId.Value}:True"),
                GetProjectionCacheKey("selections", $"{task.ProjectId.Value}:False")
            );
        }
        
        // User-based caches
        if (task.AssignedUserId != null)
        {
            InvalidateCache(GetProjectionCacheKey("dashboard", $"{task.AssignedUserId.Value}:{task.ProjectId?.Value}"));
        }
        
        // General aggregated caches
        InvalidateCaches(
            GetProjectionCacheKey("summaries"),
            GetProjectionCacheKey("dashboard"),
            GetProjectionCacheKey("selections", "True"),
            GetProjectionCacheKey("selections", "False")
        );
    }

    #endregion
}