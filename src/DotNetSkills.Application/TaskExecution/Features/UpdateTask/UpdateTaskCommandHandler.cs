using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.UpdateTask;

/// <summary>
/// Handler for UpdateTaskCommand that orchestrates task updates with business rule validation.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTaskCommandHandler> _logger;

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task {TaskId}", request.TaskId);

        // Get existing task by ID
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new NotFoundException("Task", request.TaskId);

        // Get updater user
        var updater = await _userRepository.GetByIdAsync(request.UpdatedBy, cancellationToken)
            .ConfigureAwait(false);
        if (updater == null)
            throw new NotFoundException("User", request.UpdatedBy);

        // Update task info through domain method
        task.UpdateInfo(
            request.Title,
            request.Description,
            request.Priority,
            request.EstimatedHours,
            request.DueDate,
            updater);

        // Save changes through repository
        _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully updated task {TaskId} '{Title}'", task.Id, task.Title);

        // Map to response DTO
        var context = new Dictionary<string, object>();
        if (task.AssignedUserId != null)
        {
            var assignedUser = await _userRepository.GetByIdAsync(task.AssignedUserId, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser != null)
                context["AssignedUserName"] = assignedUser.Name;
        }
        
        if (task.ParentTaskId != null)
        {
            var parentTask = await _taskRepository.GetByIdAsync(task.ParentTaskId, cancellationToken)
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
