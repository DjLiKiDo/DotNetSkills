using DotNetSkills.Application.Common.Exceptions;
namespace DotNetSkills.Application.TaskExecution.Features.DeleteTask;

/// <summary>
/// Handler for DeleteTaskCommand that orchestrates task deletion with business rule validation.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTaskCommandHandler> _logger;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteTaskCommandHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting task {TaskId}", request.TaskId);

        // Load existing task by ID
        var task = await _taskRepository.GetWithSubtasksAsync(request.TaskId, cancellationToken)
            .ConfigureAwait(false);
        if (task == null)
            throw new NotFoundException("Task", request.TaskId);

        // Validate that the user exists
        var user = await _userRepository.GetByIdAsync(request.DeletedBy, cancellationToken)
            .ConfigureAwait(false);
        if (user == null)
            throw new NotFoundException("User", request.DeletedBy);

        // Cancel task using domain method (soft delete)
        // This will also cancel all subtasks automatically
        task.Cancel(user);

        // Save task through repository
    _taskRepository.Update(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully deleted (cancelled) task {TaskId} '{Title}'", task.Id, task.Title);
    }
}
