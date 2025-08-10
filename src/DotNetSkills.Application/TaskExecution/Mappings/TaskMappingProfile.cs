using DotNetSkills.Application.Common.Mappings;
using DotNetSkills.Application.TaskExecution.Contracts.Responses;

namespace DotNetSkills.Application.TaskExecution.Mappings;

/// <summary>
/// AutoMapper profile for Task Execution bounded context.
/// Provides mappings between domain entities and DTOs with proper value object handling.
/// </summary>
public class TaskMappingProfile : MappingProfile
{
    /// <summary>
    /// Initializes a new instance of the TaskMappingProfile class.
    /// Sets up all mappings for Task Execution entities and DTOs.
    /// </summary>
    public TaskMappingProfile()
    {
        CreateTaskMappings();
        CreateTaskAssignmentMappings();
        CreatePagedResponseMappings();
    }

    /// <summary>
    /// Creates mappings for Task entity and related DTOs.
    /// Handles task data conversion with all calculated properties.
    /// </summary>
    private void CreateTaskMappings()
    {
        // Task to TaskResponse mapping with calculated properties
        CreateMap<DomainTask, TaskResponse>()
            .ConstructUsing((task, context) =>
            {
                // Extract optional contextual data
                var assignedUserName = context.Items.ContainsKey("AssignedUserName") 
                    ? context.Items["AssignedUserName"] as string 
                    : null;
                var parentTaskTitle = context.Items.ContainsKey("ParentTaskTitle") 
                    ? context.Items["ParentTaskTitle"] as string 
                    : null;

                // Calculate duration
                TimeSpan? duration = null;
                if (task.StartedAt.HasValue && task.CompletedAt.HasValue)
                {
                    duration = task.CompletedAt.Value - task.StartedAt.Value;
                }

                // Calculate completion percentage
                var subtaskCount = task.Subtasks.Count;
                var completedSubtasks = task.Subtasks.Count(s => s.Status == DomainTaskStatus.Done);
                var completionPercentage = subtaskCount > 0 
                    ? (decimal)completedSubtasks / subtaskCount * 100
                    : (task.Status == DomainTaskStatus.Done ? 100m : 0m);

                // Check if overdue
                var isOverdue = task.DueDate.HasValue &&
                               task.DueDate.Value < DateTime.UtcNow &&
                               task.Status != DomainTaskStatus.Done &&
                               task.Status != DomainTaskStatus.Cancelled;

                return new TaskResponse(
                    task.Id.Value,
                    task.Title,
                    task.Description,
                    task.Status,
                    task.Priority,
                    task.ProjectId.Value,
                    task.AssignedUserId?.Value,
                    assignedUserName,
                    task.ParentTaskId?.Value,
                    parentTaskTitle,
                    task.EstimatedHours,
                    task.ActualHours,
                    task.DueDate,
                    task.StartedAt,
                    task.CompletedAt,
                    task.CreatedAt,
                    task.UpdatedAt,
                    subtaskCount,
                    completionPercentage,
                    duration,
                    isOverdue,
                    task.AssignedUserId != null,
                    task.ParentTaskId != null,
                    subtaskCount > 0);
            })
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only
    }

    /// <summary>
    /// Creates mappings for task assignment related DTOs.
    /// </summary>
    private void CreateTaskAssignmentMappings()
    {
        // Task to TaskAssignmentResponse mapping (for assignment operations)
        CreateMap<DomainTask, TaskAssignmentResponse>()
            .ConstructUsing((task, context) => {
                Guid assignedByUserId = Guid.Empty;
                string assignedByUserName = string.Empty;
                string? assignedUserName = null;
                try
                {
                    if (context.Items.TryGetValue("AssignedUserName", out var auName))
                        assignedUserName = auName as string;
                    if (context.Items.TryGetValue("AssignedByUserId", out var abId) && abId is Guid g2)
                        assignedByUserId = g2;
                    if (context.Items.TryGetValue("AssignedByUserName", out var abName))
                        assignedByUserName = abName as string ?? string.Empty;
                }
                catch
                {
                    // context.Items not available (mapped without options) -> defaults already set
                }

                return new TaskAssignmentResponse(
                    TaskId: task.Id.Value,
                    TaskTitle: task.Title,
                    AssignedUserId: task.AssignedUserId?.Value,
                    AssignedUserName: assignedUserName,
                    AssignedAt: task.UpdatedAt,
                    AssignedByUserId: assignedByUserId,
                    AssignedByUserName: assignedByUserName
                );
            })
            .ForAllMembers(opt => opt.Ignore()); // Use constructor values only
    }

    /// <summary>
    /// Creates paginated response mappings for task collections.
    /// </summary>
    private void CreatePagedResponseMappings()
    {
        CreatePagedMapping<DomainTask, TaskResponse>();

        // Specific mapping for PagedTaskResponse using correct property names
        CreateMap<PagedResponse<DomainTask>, PagedTaskResponse>()
            .ConvertUsing((src, dest, context) => new PagedTaskResponse(
                Tasks: context.Mapper.Map<List<TaskResponse>>(src.Data),
                PageNumber: src.PageNumber,
                PageSize: src.PageSize,
                TotalCount: src.TotalCount,
                TotalPages: (int)Math.Ceiling((double)src.TotalCount / src.PageSize),
                HasNextPage: src.PageNumber < (int)Math.Ceiling((double)src.TotalCount / src.PageSize),
                HasPreviousPage: src.PageNumber > 1,
                FilterMetadata: new TaskFilterMetadata(
                    null, null, null, null, null, null, null, null, null, null, null,
                    src.Data.Count(t => t.Status != DomainTaskStatus.Done && t.Status != DomainTaskStatus.Cancelled),
                    src.Data.Count(t => t.Status == DomainTaskStatus.Done),
                    src.Data.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && t.Status != DomainTaskStatus.Done && t.Status != DomainTaskStatus.Cancelled),
                    src.Data.Count(t => t.AssignedUserId == null),
                    src.Data.Count(t => t.Priority == TaskPriority.Critical))));
    }
}