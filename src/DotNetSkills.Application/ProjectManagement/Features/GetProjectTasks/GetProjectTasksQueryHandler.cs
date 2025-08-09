namespace DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;

/// <summary>
/// Handler for GetProjectTasksQuery that retrieves project tasks with filtering and pagination.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetProjectTasksQueryHandler : IRequestHandler<GetProjectTasksQuery, PagedProjectTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetProjectTasksQueryHandler(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PagedProjectTaskResponse> Handle(GetProjectTasksQuery request, CancellationToken cancellationToken)
    {
        // First, verify the project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken).ConfigureAwait(false);
        if (project == null)
        {
            throw new DomainException($"Project with ID {request.ProjectId.Value} not found.");
        }

        // Get paginated tasks with filtering
        var (tasks, totalCount) = await _taskRepository.GetPagedAsync(
            pageNumber: request.Page,
            pageSize: request.PageSize,
            searchTerm: request.Search,
            projectId: request.ProjectId,
            assignedUserId: request.AssignedUserId,
            status: request.Status,
            priority: request.Priority,
            dueDateFrom: request.DueDateFrom,
            dueDateTo: request.DueDateTo,
            isOverdue: request.IsOverdue,
            isSubtask: request.IsSubtask,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Get unique user IDs for assigned users (excluding null values)
        var assignedUserIds = tasks
            .Where(t => t.AssignedUserId != null)
            .Select(t => t.AssignedUserId!)
            .Distinct()
            .ToList();

        // Get assigned users information
        var assignedUsers = new Dictionary<UserId, User>();
        foreach (var userId in assignedUserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user != null)
            {
                assignedUsers[userId] = user;
            }
        }

        // Get task counts for the project
        var allProjectTasks = await _taskRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken).ConfigureAwait(false);
        var activeTaskCount = allProjectTasks.Count(t => t.Status != DomainTaskStatus.Done && t.Status != DomainTaskStatus.Cancelled);
        var completedTaskCount = allProjectTasks.Count(t => t.Status == DomainTaskStatus.Done);
        var overdueTaskCount = allProjectTasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != DomainTaskStatus.Done && t.Status != DomainTaskStatus.Cancelled);

        // Map tasks to response DTOs
        var taskResponses = new List<ProjectTaskResponse>();
        foreach (var task in tasks)
        {
            var assignedUser = task.AssignedUserId != null && assignedUsers.ContainsKey(task.AssignedUserId) 
                ? assignedUsers[task.AssignedUserId] 
                : null;

            // Check if task has subtasks
            var hasSubtasks = await _taskRepository.HasSubtasksAsync(task.Id, cancellationToken).ConfigureAwait(false);

            // Get subtask count
            var subtasks = await _taskRepository.GetSubtasksAsync(task.Id, cancellationToken).ConfigureAwait(false);
            var subtaskCount = subtasks.Count();

            // Calculate completion percentage for subtasks
            var completionPercentage = subtaskCount > 0
                ? (decimal)subtasks.Count(st => st.Status == DomainTaskStatus.Done) / subtaskCount * 100m
                : task.Status == DomainTaskStatus.Done ? 100m : 0m;

            var response = new ProjectTaskResponse(
                TaskId: task.Id.Value,
                Title: task.Title,
                Description: task.Description,
                Status: task.Status,
                Priority: task.Priority,
                ProjectId: task.ProjectId.Value,
                AssignedUserId: task.AssignedUserId?.Value,
                AssignedUserName: assignedUser?.Name,
                ParentTaskId: task.ParentTaskId?.Value,
                EstimatedHours: task.EstimatedHours,
                ActualHours: null, // TODO: Implement time tracking
                DueDate: task.DueDate,
                StartedAt: task.StartedAt,
                CompletedAt: task.CompletedAt,
                CreatedAt: task.CreatedAt,
                UpdatedAt: task.UpdatedAt,
                IsOverdue: task.DueDate.HasValue && task.DueDate < DateTime.UtcNow && task.Status != DomainTaskStatus.Done && task.Status != DomainTaskStatus.Cancelled,
                IsAssigned: task.AssignedUserId != null,
                IsSubtask: task.ParentTaskId != null,
                HasSubtasks: hasSubtasks,
                CompletionPercentage: completionPercentage,
                Duration: task.CompletedAt.HasValue && task.StartedAt.HasValue 
                    ? task.CompletedAt.Value - task.StartedAt.Value 
                    : null,
                SubtaskCount: subtaskCount
            );

            taskResponses.Add(response);
        }

        return new PagedProjectTaskResponse(
            Tasks: taskResponses.AsReadOnly(),
            TotalCount: totalCount,
            Page: request.Page,
            PageSize: request.PageSize,
            ProjectId: request.ProjectId.Value,
            ProjectName: project.Name,
            ActiveTaskCount: activeTaskCount,
            CompletedTaskCount: completedTaskCount,
            OverdueTaskCount: overdueTaskCount
        );
    }
}
