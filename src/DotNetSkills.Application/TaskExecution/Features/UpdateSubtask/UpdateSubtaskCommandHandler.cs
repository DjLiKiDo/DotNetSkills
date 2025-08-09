using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.UpdateSubtask;

/// <summary>
/// Handler for updating an existing subtask.
/// </summary>
public class UpdateSubtaskCommandHandler : IRequestHandler<UpdateSubtaskCommand, TaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSubtaskCommandHandler> _logger;

    public UpdateSubtaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateSubtaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse> Handle(UpdateSubtaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating subtask {TaskId}", request.TaskId);

        // Validate that the subtask exists
        var subtask = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (subtask == null)
            throw new NotFoundException("Task", request.TaskId);

        // Validate that it is indeed a subtask
        if (!subtask.IsSubtask())
            throw new BusinessRuleViolationException("The specified task is not a subtask.");

        // Get updater user
        var updater = await _userRepository.GetByIdAsync(request.UpdatedByUserId, cancellationToken)
            .ConfigureAwait(false);
        if (updater == null)
            throw new NotFoundException("User", request.UpdatedByUserId);

        // Update subtask info through domain method
        subtask.UpdateInfo(
            request.Title,
            request.Description,
            request.Priority,
            request.EstimatedHours,
            request.DueDate,
            updater);

        // Save changes through repository
    _taskRepository.Update(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully updated subtask {TaskId} '{Title}'", subtask.Id, subtask.Title);

        // Map to response DTO
        var context = new Dictionary<string, object>();
        if (subtask.AssignedUserId is not null)
        {
            var assignedUser = await _userRepository.GetByIdAsync(subtask.AssignedUserId, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser != null)
                context["AssignedUserName"] = assignedUser.Name;
        }
        
        if (subtask.ParentTaskId is not null)
        {
            var parentTask = await _taskRepository.GetByIdAsync(subtask.ParentTaskId, cancellationToken)
                .ConfigureAwait(false);
            if (parentTask != null)
                context["ParentTaskTitle"] = parentTask.Title;
        }

        return _mapper.Map<TaskResponse>(subtask, opts => 
        {
            foreach (var item in context)
            {
                opts.Items[item.Key] = item.Value;
            }
        });
    }
}
