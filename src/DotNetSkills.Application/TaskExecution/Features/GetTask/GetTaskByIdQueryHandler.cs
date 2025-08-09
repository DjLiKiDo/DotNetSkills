namespace DotNetSkills.Application.TaskExecution.Features.GetTask;

/// <summary>
/// Handler for GetTaskByIdQuery that retrieves task details with comprehensive information.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskResponse?>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTaskByIdQueryHandler> _logger;

    public GetTaskByIdQueryHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetTaskByIdQueryHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving task {TaskId}", request.TaskId);

        // Get task with subtasks included for comprehensive data
        var task = await _taskRepository.GetWithSubtasksAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        
        if (task == null)
        {
            _logger.LogDebug("Task {TaskId} not found", request.TaskId);
            return null;
        }

        // Build context for mapping
        var context = new Dictionary<string, object>();
        
        // Get assigned user name if task is assigned
        if (task.AssignedUserId.HasValue)
        {
            var assignedUser = await _userRepository.GetByIdAsync(task.AssignedUserId.Value, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser != null)
                context["AssignedUserName"] = assignedUser.Name;
        }
        
        // Get parent task title if this is a subtask
        if (task.ParentTaskId.HasValue)
        {
            var parentTask = await _taskRepository.GetByIdAsync(task.ParentTaskId.Value, cancellationToken)
                .ConfigureAwait(false);
            if (parentTask != null)
                context["ParentTaskTitle"] = parentTask.Title;
        }

        // Map to response DTO with context
        var response = _mapper.Map<TaskResponse>(task, opts => 
        {
            foreach (var item in context)
            {
                opts.Items[item.Key] = item.Value;
            }
        });

        _logger.LogDebug("Successfully retrieved task {TaskId} '{Title}'", task.Id, task.Title);
        return response;
    }
}
