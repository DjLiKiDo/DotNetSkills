using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.AssignTask;

/// <summary>
/// Handler for assigning a task to a user.
/// </summary>
public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskAssignmentResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AssignTaskCommandHandler> _logger;

    public AssignTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<AssignTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskAssignmentResponse> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning task {TaskId} to user {AssignedUserId}", request.TaskId, request.AssignedUserId);

        // Validate that the task exists
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new NotFoundException("Task", request.TaskId);

        // Validate that the assignee user exists
        var assignee = await _userRepository.GetByIdAsync(request.AssignedUserId, cancellationToken)
            .ConfigureAwait(false);
        if (assignee == null)
            throw new NotFoundException("User", request.AssignedUserId);

        // Validate that the assigner user exists
        var assigner = await _userRepository.GetByIdAsync(request.AssignedByUserId, cancellationToken)
            .ConfigureAwait(false);
        if (assigner == null)
            throw new NotFoundException("User", request.AssignedByUserId);

        // Assign task using domain method
        task.AssignTo(assignee, assigner);

        // Save changes through repository
    _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully assigned task {TaskId} to user {AssignedUserId}", task.Id, assignee.Id);

        // Map to assignment response
        var response = _mapper.Map<TaskAssignmentResponse>(task);
        
        // Set additional context data
        response = response with 
        {
            AssignedUserName = assignee.Name,
            AssignedByUserId = assigner.Id.Value,
            AssignedByUserName = assigner.Name
        };

        return response;
    }
}
