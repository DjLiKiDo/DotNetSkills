using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.CreateSubtask;

/// <summary>
/// Handler for creating a subtask under a parent task.
/// </summary>
public class CreateSubtaskCommandHandler : IRequestHandler<CreateSubtaskCommand, TaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSubtaskCommandHandler> _logger;

    public CreateSubtaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateSubtaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating subtask '{Title}' under parent task {ParentTaskId}", request.Title, request.ParentTaskId);

        // Validate that parent task exists and can have subtasks
        var parentTask = await _taskRepository.GetByIdAsync(request.ParentTaskId, cancellationToken)
            .ConfigureAwait(false);
        if (parentTask == null)
            throw new NotFoundException("Task", request.ParentTaskId);

        // Get creator user
        var creator = await _userRepository.GetByIdAsync(request.CreatedByUserId, cancellationToken)
            .ConfigureAwait(false);
        if (creator == null)
            throw new NotFoundException("User", request.CreatedByUserId);

        // Validate assigned user if specified
        User? assignedUser = null;
        if (request.AssignedUserId is not null)
        {
            assignedUser = await _userRepository.GetByIdAsync(request.AssignedUserId, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser == null)
                throw new NotFoundException("User", request.AssignedUserId);
        }

        // Create subtask using domain factory method
        var subtask = DomainTask.Create(
            request.Title,
            request.Description,
            parentTask.ProjectId, // Subtask must be in same project as parent
            request.Priority,
            parentTask, // This will be the parent task
            request.EstimatedHours,
            request.DueDate,
            creator);

        // Assign subtask if assignee specified
        if (assignedUser != null)
        {
            subtask.AssignTo(assignedUser, creator);
        }

        // Add subtask to parent (this validates business rules)
        parentTask.AddSubtask(subtask);

        // Save subtask through repository
    _taskRepository.Add(subtask);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully created subtask {TaskId} '{Title}' under parent {ParentTaskId}", 
            subtask.Id, subtask.Title, parentTask.Id);

        // Map to response DTO
        var context = new Dictionary<string, object>();
        if (assignedUser != null)
            context["AssignedUserName"] = assignedUser.Name;
        context["ParentTaskTitle"] = parentTask.Title;

        return _mapper.Map<TaskResponse>(subtask, opts => 
        {
            foreach (var item in context)
            {
                opts.Items[item.Key] = item.Value;
            }
        });
    }
}
