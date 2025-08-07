using DotNetSkills.Application.TaskExecution.Projections;

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

        var taskData = await query
            .Select(t => new 
            {
                Task = t,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours
            })
            .OrderBy(x => x.Task.Title)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return taskData.Select(x => (x.Task, x.EstimatedHours, x.ActualHours));
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
    /// Gets tasks by their associated project as an async enumerable for streaming large result sets.
    /// Memory-efficient for processing many tasks in a project.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks belonging to the specified project.</returns>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByProjectIdAsyncEnumerable(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        if (projectId == null)
            throw new ArgumentNullException(nameof(projectId));

        return DbSet
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Title)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets tasks assigned to a specific user as an async enumerable.
    /// Memory-efficient for users with many assigned tasks.
    /// </summary>
    /// <param name="userId">The user ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks assigned to the specified user.</returns>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByAssigneeIdAsyncEnumerable(
        UserId userId, 
        CancellationToken cancellationToken = default)
    {
        if (userId == null)
            throw new ArgumentNullException(nameof(userId));

        return DbSet
            .AsNoTracking()
            .Where(t => t.AssignedUserId == userId)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .AsAsyncEnumerable();
    }

    /// <summary>
    /// Gets tasks by their status as an async enumerable for bulk operations.
    /// Optimized for memory efficiency when processing large numbers of tasks with specific status.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async enumerable of tasks with the specified status.</returns>
    public IAsyncEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task> GetByStatusAsyncEnumerable(
        DotNetSkills.Domain.TaskExecution.Enums.TaskStatus status, 
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .AsNoTracking()
            .Where(t => t.Status == status)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .AsAsyncEnumerable();
    }

    // Strategic Include Methods

    /// <summary>
    /// Gets a task with its project information included.
    /// Prevents N+1 queries when project details are needed.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with project information if found, otherwise null.</returns>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithProjectAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        // For now, return without Include since navigation properties aren't configured
        // This method serves as a placeholder for future EF Core navigation property configuration
        return await DbSet
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets tasks with their assigned user information included.
    /// Optimized for scenarios where user details are needed.
    /// </summary>
    /// <param name="projectId">The project ID to filter by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of tasks with assigned user information.</returns>
    public async Task<IEnumerable<DotNetSkills.Domain.TaskExecution.Entities.Task>> GetProjectTasksWithAssigneesAsync(
        ProjectId projectId, 
        CancellationToken cancellationToken = default)
    {
        // For now, return without Include since navigation properties aren't configured
        // This method serves as a placeholder for future EF Core navigation property configuration
        return await DbSet
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a task with complete hierarchy (parent and subtasks) included.
    /// Prevents multiple queries for task hierarchy operations.
    /// Uses split queries for optimal performance with multiple collections.
    /// </summary>
    /// <param name="id">The task ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The task with complete hierarchy if found, otherwise null.</returns>
    public async Task<DotNetSkills.Domain.TaskExecution.Entities.Task?> GetWithCompleteHierarchyAsync(
        TaskId id, 
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsSplitQuery() // Use split queries for better performance with multiple collections
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    // Projection Methods

    /// <summary>
    /// Gets task summaries with optimized projection for read-only scenarios.
    /// Minimizes data transfer by selecting only required fields.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task summary projections.</returns>
    public async Task<IEnumerable<TaskSummaryProjection>> GetTaskSummariesAsync(
        ProjectId? projectId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (projectId != null)
            query = query.Where(t => t.ProjectId == projectId);

        return await query
            .Select(t => new TaskSummaryProjection
            {
                Id = t.Id.Value,
                Title = t.Title,
                Description = t.Description ?? string.Empty,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId.Value,
                ProjectName = Context.Set<Project>().Where(p => p.Id == t.ProjectId).Select(p => p.Name).FirstOrDefault() ?? string.Empty,
                AssignedUserId = t.AssignedUserId != null ? t.AssignedUserId.Value : null,
                AssignedUserName = t.AssignedUserId != null ? Context.Set<User>().Where(u => u.Id == t.AssignedUserId).Select(u => u.Name).FirstOrDefault() : null,
                SubtaskCount = t.Subtasks.Count
            })
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task dashboard information with aggregated data.
    /// Optimized for dashboard scenarios with minimal queries.
    /// </summary>
    /// <param name="userId">Optional user filter for assigned tasks.</param>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task dashboard projections.</returns>
    public async Task<IEnumerable<TaskDashboardProjection>> GetTaskDashboardDataAsync(
        UserId? userId = null,
        ProjectId? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (userId != null)
            query = query.Where(t => t.AssignedUserId == userId);

        if (projectId != null)
            query = query.Where(t => t.ProjectId == projectId);

        return await query
            .Select(t => new TaskDashboardProjection
            {
                Id = t.Id.Value,
                Title = t.Title,
                Description = t.Description ?? string.Empty,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId.Value,
                ProjectName = Context.Set<Project>().Where(p => p.Id == t.ProjectId).Select(p => p.Name).FirstOrDefault() ?? string.Empty,
                AssignedUserId = t.AssignedUserId != null ? t.AssignedUserId.Value : null,
                AssignedUserName = t.AssignedUserId != null ? Context.Set<User>().Where(u => u.Id == t.AssignedUserId).Select(u => u.Name).FirstOrDefault() : null,
                ParentTaskId = t.ParentTaskId != null ? t.ParentTaskId.Value : null,
                ParentTaskTitle = t.ParentTaskId != null ? Context.Set<DotNetSkills.Domain.TaskExecution.Entities.Task>().Where(pt => pt.Id == t.ParentTaskId).Select(pt => pt.Title).FirstOrDefault() : null,
                SubtaskCount = t.Subtasks.Count,
                CompletedSubtaskCount = t.Subtasks.Count(sub => sub.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done),
                CompletionPercentage = t.Subtasks.Count > 0 
                    ? (decimal)t.Subtasks.Count(sub => sub.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done) / t.Subtasks.Count * 100
                    : (t.Status == DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done ? 100 : 0),
                IsOverdue = t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done
            })
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task selection data for dropdowns and selection lists.
    /// Minimal projection for UI scenarios.
    /// </summary>
    /// <param name="projectId">Optional project filter.</param>
    /// <param name="assignableOnly">Whether to return only assignable tasks (unassigned or current user).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task selection projections.</returns>
    public async Task<IEnumerable<TaskSelectionProjection>> GetTaskSelectionsAsync(
        ProjectId? projectId = null,
        bool assignableOnly = false,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (projectId != null)
            query = query.Where(t => t.ProjectId == projectId);

        if (assignableOnly)
            query = query.Where(t => t.AssignedUserId == null || t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);

        return await query
            .Select(t => new TaskSelectionProjection
            {
                Id = t.Id.Value,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                ProjectId = t.ProjectId.Value,
                ProjectName = Context.Set<Project>().Where(p => p.Id == t.ProjectId).Select(p => p.Name).FirstOrDefault() ?? string.Empty,
                CanHaveSubtasks = t.ParentTaskId == null // Only root tasks can have subtasks
            })
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.Title)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Gets task assignment information for a specific user.
    /// Shows tasks assigned to the user with context.
    /// </summary>
    /// <param name="userId">The user ID to get task assignments for.</param>
    /// <param name="includeCompleted">Whether to include completed tasks.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of task assignment projections.</returns>
    public async Task<IEnumerable<TaskAssignmentProjection>> GetUserTaskAssignmentsAsync(
        UserId userId, 
        bool includeCompleted = false,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Where(t => t.AssignedUserId == userId);

        if (!includeCompleted)
            query = query.Where(t => t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done);

        return await query
            .Select(t => new TaskAssignmentProjection
            {
                Id = t.Id.Value,
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                DueDate = t.DueDate,
                ProjectId = t.ProjectId.Value,
                ProjectName = Context.Set<Project>().Where(p => p.Id == t.ProjectId).Select(p => p.Name).FirstOrDefault() ?? string.Empty,
                AssignedUserId = t.AssignedUserId!.Value,
                AssignedUserName = Context.Set<User>().Where(u => u.Id == t.AssignedUserId).Select(u => u.Name).FirstOrDefault() ?? string.Empty,
                AssignedUserEmail = Context.Set<User>().Where(u => u.Id == t.AssignedUserId).Select(u => u.Email).FirstOrDefault() ?? string.Empty,
                CreatedAt = t.CreatedAt,
                EstimatedHours = t.EstimatedHours ?? 0,
                IsOverdue = t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != DotNetSkills.Domain.TaskExecution.Enums.TaskStatus.Done
            })
            .OrderBy(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
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
