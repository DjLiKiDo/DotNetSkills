using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.GetTaskSubtasks;

/// <summary>
/// Handler for retrieving subtasks of a specific task.
/// </summary>
public class GetTaskSubtasksQueryHandler : IRequestHandler<GetTaskSubtasksQuery, TaskSubtasksResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTaskSubtasksQueryHandler> _logger;

    public GetTaskSubtasksQueryHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetTaskSubtasksQueryHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskSubtasksResponse> Handle(GetTaskSubtasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving subtasks for task {TaskId}", request.TaskId);

        // Retrieve the parent task by ID with subtasks
        var parentTask = await _taskRepository.GetWithSubtasksAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        
        if (parentTask == null)
            throw new NotFoundException("Task", request.TaskId);

        // Get all subtasks
        var subtasks = await _taskRepository.GetSubtasksAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);

        // Map subtasks to response DTOs
        var subtaskResponses = new List<SubtaskResponse>();
        
        foreach (var subtask in subtasks)
        {
            string? assignedUserName = null;
            if (subtask.AssignedUserId is not null)
            {
                var assignedUser = await _userRepository.GetByIdAsync(subtask.AssignedUserId, cancellationToken)
                    .ConfigureAwait(false);
                assignedUserName = assignedUser?.Name;
            }

            var subtaskResponse = new SubtaskResponse(
                subtask.Id.Value,
                subtask.Title,
                subtask.Description,
                subtask.Status,
                subtask.Priority,
                subtask.AssignedUserId?.Value,
                assignedUserName,
                subtask.EstimatedHours,
                subtask.ActualHours,
                subtask.DueDate,
                subtask.StartedAt,
                subtask.CompletedAt,
                subtask.CreatedAt,
                subtask.UpdatedAt,
                subtask.IsOverdue(),
                subtask.IsAssigned());
            
            subtaskResponses.Add(subtaskResponse);
        }

        // Calculate completion statistics
        var totalSubtasks = subtaskResponses.Count;
        var completedSubtasks = subtaskResponses.Count(s => s.Status == DomainTaskStatus.Done);
        var completionPercentage = totalSubtasks > 0 ? (decimal)completedSubtasks / totalSubtasks * 100 : 0m;

        var response = new TaskSubtasksResponse(
            parentTask.Id.Value,
            parentTask.Title,
            subtaskResponses.AsReadOnly(),
            totalSubtasks,
            completedSubtasks,
            completionPercentage);

        _logger.LogDebug("Successfully retrieved {SubtaskCount} subtasks for task {TaskId}", totalSubtasks, request.TaskId);
        return response;
    }
}
