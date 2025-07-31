using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;
using DotNetSkills.Domain.Enums;

namespace DotNetSkills.Domain.Repositories;

/// <summary>
/// Repository contract for Task aggregate operations.
/// Handles task management, assignments, and subtask relationships.
/// Note: Using TaskEntity instead of Task to avoid naming conflicts with System.Threading.Tasks.Task.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Gets a task by its unique identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Entities.Task?> GetByIdAsync(TaskId id);
    
    /// <summary>
    /// Gets all tasks belonging to a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>Tasks belonging to the specified project.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetByProjectIdAsync(ProjectId projectId);
    
    /// <summary>
    /// Gets all tasks assigned to a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Tasks assigned to the specified user.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetByAssigneeIdAsync(UserId userId);
    
    /// <summary>
    /// Gets a task with all its subtasks loaded.
    /// Used when subtask operations are needed.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <returns>The task with subtasks if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Entities.Task?> GetTaskWithSubtasksAsync(TaskId id);
    
    /// <summary>
    /// Gets all subtasks of a parent task.
    /// </summary>
    /// <param name="parentTaskId">The parent task identifier.</param>
    /// <returns>Subtasks of the specified parent task.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetSubtasksAsync(TaskId parentTaskId);
    
    /// <summary>
    /// Gets tasks by their status.
    /// </summary>
    /// <param name="status">The task status to filter by.</param>
    /// <returns>Tasks with the specified status.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetByStatusAsync(Enums.TaskStatus status);
    
    /// <summary>
    /// Gets tasks by their priority level.
    /// </summary>
    /// <param name="priority">The task priority to filter by.</param>
    /// <returns>Tasks with the specified priority.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetByPriorityAsync(TaskPriority priority);
    
    /// <summary>
    /// Gets overdue tasks (tasks with due dates in the past that are not completed).
    /// </summary>
    /// <returns>Tasks that are overdue.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetOverdueTasksAsync();
    
    /// <summary>
    /// Gets tasks due within a specified number of days.
    /// </summary>
    /// <param name="daysFromNow">Number of days from now to consider as approaching.</param>
    /// <returns>Tasks due within the specified timeframe.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetTasksDueWithinAsync(int daysFromNow);
    
    /// <summary>
    /// Gets unassigned tasks in a project.
    /// Used for task assignment workflows.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>Unassigned tasks in the specified project.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetUnassignedTasksInProjectAsync(ProjectId projectId);
    
    /// <summary>
    /// Gets tasks by type (Story, Bug, Epic, etc.).
    /// </summary>
    /// <param name="taskType">The task type to filter by.</param>
    /// <returns>Tasks with the specified type.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Entities.Task>> GetByTypeAsync(TaskType taskType);
    
    /// <summary>
    /// Gets the count of tasks by status for a project.
    /// Used for project dashboard and metrics.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>Dictionary with status as key and count as value.</returns>
    System.Threading.Tasks.Task<Dictionary<Enums.TaskStatus, int>> GetTaskCountByStatusAsync(ProjectId projectId);
    
    /// <summary>
    /// Adds a new task to the repository.
    /// </summary>
    /// <param name="task">The task to add.</param>
    /// <returns>The added task with any generated values.</returns>
    System.Threading.Tasks.Task<Entities.Task> AddAsync(Entities.Task task);
    
    /// <summary>
    /// Updates an existing task in the repository.
    /// </summary>
    /// <param name="task">The task to update.</param>
    System.Threading.Tasks.Task UpdateAsync(Entities.Task task);
    
    /// <summary>
    /// Removes a task from the repository.
    /// Should handle cascade deletion of subtasks according to business rules.
    /// </summary>
    /// <param name="id">The identifier of the task to remove.</param>
    System.Threading.Tasks.Task DeleteAsync(TaskId id);
    
    /// <summary>
    /// Checks if a task can be safely deleted (no dependent subtasks or constraints).
    /// </summary>
    /// <param name="id">The task identifier to check.</param>
    /// <returns>True if the task can be deleted, false otherwise.</returns>
    System.Threading.Tasks.Task<bool> CanDeleteAsync(TaskId id);
}
