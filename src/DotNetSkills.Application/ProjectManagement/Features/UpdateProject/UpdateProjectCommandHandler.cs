namespace DotNetSkills.Application.ProjectManagement.Features.UpdateProject;

/// <summary>
/// Handler for UpdateProjectCommand that orchestrates project updates using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to update projects");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken).ConfigureAwait(false);
        if (project == null)
            throw new InvalidOperationException($"Project with ID '{request.ProjectId}' not found");

        var existingProject = await _projectRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);
        if (existingProject != null && existingProject.Id != request.ProjectId)
            throw new InvalidOperationException($"Project with name '{request.Name}' already exists");

        project.UpdateInfo(request.Name, request.Description, request.PlannedEndDate, currentUser);

        _projectRepository.Update(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var team = await _teamRepository.GetByIdAsync(project.TeamId, cancellationToken).ConfigureAwait(false);
        return ProjectResponse.FromDomain(project, team?.Name ?? "Unknown");
    }
}
