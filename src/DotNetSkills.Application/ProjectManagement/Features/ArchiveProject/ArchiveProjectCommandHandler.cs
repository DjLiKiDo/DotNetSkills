namespace DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;

/// <summary>
/// Handler for ArchiveProjectCommand that orchestrates project archiving using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        // Get current user from authentication context
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to archive projects");

        // Load the user entity for domain validation
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        // Load the project to be archived
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken).ConfigureAwait(false);
        if (project == null)
            throw new InvalidOperationException($"Project with ID '{request.ProjectId}' not found");

        // Archive the project using domain method (Cancel acts as soft delete/archive)
        project.Cancel(currentUser);

        // Update the project in repository
        _projectRepository.Update(project);

        // Persist changes
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
