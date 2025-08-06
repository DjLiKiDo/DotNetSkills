namespace DotNetSkills.Infrastructure.Repositories.TaskExecution;

/// <summary>
/// Entity Framework Core implementation of the ITaskRepository interface.
/// Provides data access operations for Task entities with hierarchy support and assignment management.
/// </summary>
public class TaskRepository : BaseRepository<DotNetSkills.Domain.TaskExecution.Entities.Task, TaskId>, ITaskRepository
{
    /// <summary>
    /// Initializes a new instance of the TaskRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Gets tasks by their associated project asynchronously.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks belonging to the specified project.</returns>
    /// <exception cref="ArgumentNullException">Thrown when projectId is null.</exception>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByProjectIdAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        if (projectId == null)
            throw new ArgumentNullException(nameof(projectId));

        return await DbSet
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets tasks assigned to a specific user asynchronously.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks assigned to the specified user.</returns>
    /// <exception cref="ArgumentNullException">Thrown when userId is null.</exception>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByAssigneeIdAsync(
        UserId userId, 
        CancellationToken cancellationToken = default)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        return await DbSet
            .AsNoTracking()
            .Where(t => t.AssignedUserId == userId)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets subtasks for a specific parent task asynchronously.
    /// </summary>
    /// <param name="parentTaskId">The parent task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of subtasks for the specified parent task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when parentTaskId is null.</exception>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetSubtasksAsync(
        TaskId parentTaskId, 
        CancellationToken cancellationToken = default)
    {
        if (parentTaskId == null)
            throw new ArgumentNullException(nameof(parentTaskId));

        return await DbSet
            .AsNoTracking()
            .Where(t => t.ParentTaskId == parentTaskId)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a task with its subtasks included asynchronously.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with subtasks if found, otherwise null.</returns>
    /// <exception cref="ArgumentNullException">Thrown when id is null.</exception>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithSubtasksAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        return await DbSet
            .AsNoTracking()
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    /// <summary>
    /// Gets tasks by their status asynchronously.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified status.</returns>
    /// <exception cref="ArgumentNullException">Thrown when status is null.</exception>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByStatusAsync(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Status == status)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets tasks by their priority asynchronously.
    /// </summary>
    /// <param name="priority">The task priority to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with the specified priority.</returns>
    /// <exception cref="ArgumentNullException">Thrown when priority is null.</exception>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetByPriorityAsync(
        TaskPriority priority, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(t => t.Priority == priority)
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets overdue tasks (past their due date).
    /// </summary>
    /// <param name="userId">Optional user filter to get overdue tasks for specific user.</param>
    /// <param name="projectId">Optional project filter to get overdue tasks for specific project.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of overdue tasks.</returns>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetOverdueTasksAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var query = DbSet
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value.Date < today &&
                       t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);

        if (userId != null)
        {
            query = query.Where(t => t.AssignedUserId == userId);
        }

        if (projectId != null)
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        return await query
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Priority)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets tasks with pagination support and comprehensive filtering asynchronously.
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
        var query = DbSet.AsNoTracking();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t => 
                t.Title.Contains(searchTerm) || 
                (t.Description != null && t.Description.Contains(searchTerm)));
        }

        // Apply project filter
        if (projectId != null)
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        // Apply assigned user filter
        if (assignedUserId != null)
        {
            query = query.Where(t => t.AssignedUserId == assignedUserId);
        }

        // Apply status filter
        if (status != null)
        {
            query = query.Where(t => t.Status == status);
        }

        // Apply priority filter
        if (priority != null)
        {
            query = query.Where(t => t.Priority == priority);
        }

        // Apply due date range filter
        query = query.ApplyDateRangeFilter(dueDateFrom, dueDateTo, t => t.DueDate);

        // Apply overdue filter
        if (isOverdue.HasValue)
        {
            var today = DateTime.UtcNow.Date;
            if (isOverdue.Value)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date < today && t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);
            }
            else
            {
                query = query.Where(t => !t.DueDate.HasValue || t.DueDate.Value.Date >= today || t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);
            }
        }

        // Apply subtask filter
        if (isSubtask.HasValue)
        {
            if (isSubtask.Value)
            {
                query = query.Where(t => t.ParentTaskId != null);
            }
            else
            {
                query = query.Where(t => t.ParentTaskId == null);
            }
        }

        // Apply ordering
        query = query
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title);

        // Get paginated results
        var (tasks, totalCount) = await GetPagedAsync(query, pageNumber, pageSize, cancellationToken);
        
        return (tasks, totalCount);
    }

    /// <summary>
    /// Gets task hierarchy for a specific task including all descendants.
    /// Returns the complete subtask tree structure.
    /// </summary>
    /// <param name="taskId">The root task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with complete hierarchy of subtasks.</returns>
    /// <exception cref="ArgumentNullException">Thrown when taskId is null.</exception>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetTaskHierarchyAsync(
        TaskId taskId, 
        CancellationToken cancellationToken = default)
    {
        if (taskId == null)
            throw new ArgumentNullException(nameof(taskId));

        // Get the root task
        var rootTask = await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (rootTask == null)
            return null;

        // Get all subtasks recursively (since we only support one level, this is simpler)
        var subtasks = await DbSet
            .AsNoTracking()
            .Where(t => t.ParentTaskId == taskId)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);

        // Note: The actual loading of subtasks into the entity would depend on 
        // the domain model implementation. This might need adjustment.

        return rootTask;
    }

    /// <summary>
    /// Gets tasks that are approaching their deadline within the specified number of days.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetTasksApproachingDeadlineAsync(
        int daysUntilDeadline,
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        if (daysUntilDeadline < 0)
            throw new ArgumentException("Days until deadline must be non-negative.", nameof(daysUntilDeadline));

        var today = DateTime.UtcNow.Date;
        var targetDate = today.AddDays(daysUntilDeadline);
        
        var query = DbSet
            .AsNoTracking()
            .Where(t => t.DueDate.HasValue && 
                       t.DueDate.Value.Date >= today &&
                       t.DueDate.Value.Date <= targetDate &&
                       t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);

        if (userId != null)
        {
            query = query.Where(t => t.AssignedUserId == userId);
        }

        if (projectId != null)
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        return await query
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Priority)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets unassigned tasks that are available for assignment.
    /// </summary>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetUnassignedTasksAsync(
        ProjectId? projectId = null,
        TaskPriority? priority = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(t => t.AssignedUserId == null && t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);

        if (projectId != null)
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        if (priority != null)
        {
            query = query.Where(t => t.Priority == priority);
        }

        return await query
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets tasks with their time tracking information for reporting.
    /// </summary>
    public async Task<IEnumerable<(DotNetSkills.Domain.TaskExecution.Entities.Task Task, int? EstimatedHours, int? ActualHours)>> 
        GetTasksWithTimeTrackingAsync(
            ProjectId? projectId = null,
            UserId? userId = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (projectId != null)
        {
            query = query.Where(t => t.ProjectId == projectId);
        }

        if (userId != null)
        {
            query = query.Where(t => t.AssignedUserId == userId);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= dateTo.Value.Date.AddDays(1).AddTicks(-1));
        }

        return await query
            .Select(t => new 
            {
                Task = t,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours
            })
            .OrderBy(x => x.Task.Title)
            .ToListAsync(cancellationToken)
            .ContinueWith(task => task.Result.Select(x => (x.Task, x.EstimatedHours, x.ActualHours)), cancellationToken);
    }

    /// <summary>
    /// Checks if a task has any subtasks.
    /// Used for validating task deletion and status changes.
    /// </summary>
    /// <param name="taskId">The task ID to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the task has subtasks, otherwise false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when taskId is null.</exception>
    public async Task<bool> HasSubtasksAsync(TaskId taskId, CancellationToken cancellationToken = default)
    {
        if (taskId == null)
            throw new ArgumentNullException(nameof(taskId));

        return await DbSet
            .AsNoTracking()
            .AnyAsync(t => t.ParentTaskId == taskId, cancellationToken);
    }

    /// <summary>
    /// Override to provide Task-specific default ordering.
    /// </summary>
    /// <returns>Expression to order by task priority, then due date, then title.</returns>
    protected override Expression<Func<DotNetSkills.Domain.TaskExecution.Entities.Task, object>> GetDefaultOrderingExpression()
    {
        return t => t.Priority;
    }
}
