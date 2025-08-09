namespace DotNetSkills.Application.ProjectManagement.Features.CreateProject;

/// <summary>
/// Handler for CreateProjectCommand that orchestrates project creation using domain factory methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(
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

    public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to create projects");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        var existingProject = await _projectRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);
        if (existingProject != null)
            throw new InvalidOperationException($"Project with name '{request.Name}' already exists");

        var project = Project.Create(request.Name, request.Description, request.TeamId, request.PlannedEndDate, currentUser);

        _projectRepository.Add(project);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return ProjectResponse.FromDomain(project, team.Name);
    }
}
