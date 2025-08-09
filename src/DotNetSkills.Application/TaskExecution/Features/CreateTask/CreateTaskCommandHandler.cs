namespace DotNetSkills.Application.TaskExecution.Features.CreateTask;

/// <summary>
/// Handler for CreateTaskCommand that orchestrates task creation with comprehensive business logic.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateTaskCommandHandler> _logger;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating task '{Title}' for project {ProjectId}", request.Title, request.ProjectId);

        // Validate project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            .ConfigureAwait(false);
        if (project == null)
            throw new InvalidOperationException($"Project with ID {request.ProjectId.Value} not found.");

        // Get creator user
        var creator = await _userRepository.GetByIdAsync(request.CreatedBy, cancellationToken)
            .ConfigureAwait(false);
        if (creator == null)
            throw new InvalidOperationException($"User with ID {request.CreatedBy.Value} not found.");

        // Validate parent task if specified
        DomainTask? parentTask = null;
        if (request.ParentTaskId != null)
        {
            parentTask = await _taskRepository.GetByIdAsync(request.ParentTaskId, cancellationToken)
                .ConfigureAwait(false);
            if (parentTask == null)
                throw new InvalidOperationException($"Parent task with ID {request.ParentTaskId.Value} not found.");
            
            if (parentTask.ProjectId != request.ProjectId)
                throw new DomainException("Parent task must belong to the same project.");
        }

        // Validate assigned user if specified
        User? assignedUser = null;
        if (request.AssignedUserId != null)
        {
            assignedUser = await _userRepository.GetByIdAsync(request.AssignedUserId, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser == null)
                throw new InvalidOperationException($"Assigned user with ID {request.AssignedUserId.Value} not found.");
        }

        // Create task using domain factory method
        var task = DomainTask.Create(
            request.Title,
            request.Description,
            request.ProjectId,
            request.Priority,
            parentTask,
            request.EstimatedHours,
            request.DueDate,
            creator);

        // Assign task if assignee specified
        if (assignedUser != null)
        {
            task.AssignTo(assignedUser, creator);
        }

        // Save task through repository
        _taskRepository.Add(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully created task {TaskId} '{Title}'", task.Id, task.Title);

        // Map to response DTO
        var context = new Dictionary<string, object>();
        if (assignedUser != null)
            context["AssignedUserName"] = assignedUser.Name;
        if (parentTask != null)
            context["ParentTaskTitle"] = parentTask.Title;

        return _mapper.Map<TaskResponse>(task, opts => 
        {
            foreach (var item in context)
            {
                opts.Items[item.Key] = item.Value;
            }
        });
    }
}
