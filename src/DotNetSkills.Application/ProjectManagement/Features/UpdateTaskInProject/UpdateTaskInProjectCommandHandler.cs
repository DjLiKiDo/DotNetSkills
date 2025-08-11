namespace DotNetSkills.Application.ProjectManagement.Features.UpdateTaskInProject;

/// <summary>
/// Handler for UpdateTaskInProjectCommand that orchestrates task updates within a project context.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateTaskInProjectCommandHandler : IRequestHandler<UpdateTaskInProjectCommand, ProjectTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateTaskInProjectCommandHandler> _logger;

    public UpdateTaskInProjectCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateTaskInProjectCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProjectTaskResponse> Handle(UpdateTaskInProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task {TaskId} in project {ProjectId}", request.TaskId, request.ProjectId);

        // Validate project exists
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            .ConfigureAwait(false);
        if (project == null)
            throw new InvalidOperationException($"Project with ID '{request.ProjectId}' not found");

        // Get existing task by ID
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new InvalidOperationException($"Task with ID '{request.TaskId}' not found");

        // Validate task belongs to the specified project
        if (task.ProjectId != request.ProjectId)
            throw new DomainException($"Task '{request.TaskId}' does not belong to project '{request.ProjectId}'");

        // Get updater user
        var updater = await _userRepository.GetByIdAsync(request.UpdatedBy, cancellationToken)
            .ConfigureAwait(false);
        if (updater == null)
            throw new InvalidOperationException($"User with ID '{request.UpdatedBy}' not found");

        // Update task info through domain method (domain rules will prevent updating completed tasks)
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

        _logger.LogInformation("Successfully updated task {TaskId} '{Title}' in project {ProjectId}", 
            task.Id, task.Title, request.ProjectId);

        // Map to response DTO with context
        var mappingContext = new Dictionary<string, object>();
        if (task.AssignedUserId != null)
        {
            var assignedUser = await _userRepository.GetByIdAsync(task.AssignedUserId, cancellationToken)
                .ConfigureAwait(false);
            if (assignedUser != null)
                mappingContext["AssignedUserName"] = assignedUser.Name;
        }

        return _mapper.Map<ProjectTaskResponse>(task, opts => 
        {
            foreach (var item in mappingContext)
            {
                opts.Items[item.Key] = item.Value;
            }
        });
    }
}
