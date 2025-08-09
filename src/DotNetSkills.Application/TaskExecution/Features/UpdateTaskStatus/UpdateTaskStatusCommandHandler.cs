namespace DotNetSkills.Application.TaskExecution.Features.UpdateTaskStatus;

/// <summary>
/// Handler for UpdateTaskStatusCommand that orchestrates task status transitions with domain validation.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateTaskStatusCommandHandler : IRequestHandler<UpdateTaskStatusCommand, TaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTaskStatusCommandHandler> _logger;

    public UpdateTaskStatusCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateTaskStatusCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse> Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task {TaskId} status to {Status}", request.TaskId, request.Status);

        // Load existing task by ID
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new NotFoundException($"Task with ID {request.TaskId.Value} not found.");

        // Get user performing the status update
        var user = await _userRepository.GetByIdAsync(request.UpdatedBy, cancellationToken)
            .ConfigureAwait(false);
        if (user == null)
            throw new NotFoundException($"User with ID {request.UpdatedBy.Value} not found.");

        // Execute appropriate domain method based on target status
        switch (request.Status)
        {
            case DomainTaskStatus.ToDo:
                // Reopen task if currently Done/Cancelled
                task.Reopen(user);
                break;
                
            case DomainTaskStatus.InProgress:
                // Start task
                task.Start(user);
                break;
                
            case DomainTaskStatus.InReview:
                // Submit for review
                task.SubmitForReview(user);
                break;
                
            case DomainTaskStatus.Done:
                // Complete task with actual hours
                task.Complete(user, request.ActualHours);
                break;
                
            case DomainTaskStatus.Cancelled:
                // Cancel task
                task.Cancel(user);
                break;
                
            default:
                throw new ArgumentException($"Invalid task status: {request.Status}");
        }

        // Save task through repository
        await _taskRepository.UpdateAsync(task, cancellationToken)
            .ConfigureAwait(false);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully updated task {TaskId} status to {Status}", task.Id, task.Status);

        // Map to response DTO
        var context = new Dictionary<string, object>();
        if (task.AssignedUserId.HasValue)
        {
            var assignedUser = await _userRepository.GetByIdAsync(task.AssignedUserId.Value, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser != null)
                context["AssignedUserName"] = assignedUser.Name;
        }
        
        if (task.ParentTaskId.HasValue)
        {
            var parentTask = await _taskRepository.GetByIdAsync(task.ParentTaskId.Value, cancellationToken)
                .ConfigureAwait(false);
            if (parentTask != null)
                context["ParentTaskTitle"] = parentTask.Title;
        }

        return _mapper.Map<TaskResponse>(task, opts => 
        {
            foreach (var item in context)
            {
                opts.Items[item.Key] = item.Value;
            }
        });
    }
}
