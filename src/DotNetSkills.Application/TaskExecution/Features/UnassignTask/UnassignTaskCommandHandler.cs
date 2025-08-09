namespace DotNetSkills.Application.TaskExecution.Features.UnassignTask;

/// <summary>
/// Handler for unassigning a task from its current assignee.
/// </summary>
public class UnassignTaskCommandHandler : IRequestHandler<UnassignTaskCommand, TaskAssignmentResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UnassignTaskCommandHandler> _logger;

    public UnassignTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UnassignTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskAssignmentResponse> Handle(UnassignTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unassigning task {TaskId}", request.TaskId);

        // Validate that the task exists
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new NotFoundException($"Task with ID {request.TaskId.Value} not found.");

        // Validate that the unassigner user exists
        var unassigner = await _userRepository.GetByIdAsync(request.UnassignedByUserId, cancellationToken)
            .ConfigureAwait(false);
        if (unassigner == null)
            throw new NotFoundException($"User with ID {request.UnassignedByUserId.Value} not found.");

        // Store previous assignee information for response
        string? previousAssigneeName = null;
        if (task.AssignedUserId.HasValue)
        {
            var previousAssignee = await _userRepository.GetByIdAsync(task.AssignedUserId.Value, cancellationToken)
                .ConfigureAwait(false);
            previousAssigneeName = previousAssignee?.Name;
        }

        // Unassign task using domain method
        task.Unassign(unassigner);

        // Save changes through repository
        await _taskRepository.UpdateAsync(task, cancellationToken)
            .ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully unassigned task {TaskId}", task.Id);

        // Map to assignment response
        var response = _mapper.Map<TaskAssignmentResponse>(task);
        
        // Set additional context data
        response = response with 
        {
            AssignedUserName = null, // Task is now unassigned
            AssignedByUserId = unassigner.Id.Value,
            AssignedByUserName = unassigner.Name
        };

        return response;
    }
}
