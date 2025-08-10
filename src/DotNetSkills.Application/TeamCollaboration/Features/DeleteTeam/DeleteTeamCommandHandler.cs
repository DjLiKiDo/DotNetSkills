namespace DotNetSkills.Application.TeamCollaboration.Features.DeleteTeam;

/// <summary>
/// Handler for DeleteTeamCommand that orchestrates team deletion using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTeamCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        IProjectRepository projectRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to delete teams");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        if (!currentUser.CanManageTeams())
            throw new UnauthorizedAccessException("User does not have permission to delete teams");

        var activeProjects = await _projectRepository.GetByTeamIdAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        var hasActiveProjects = activeProjects.Any(p => p.Status != ProjectStatus.Completed && p.Status != ProjectStatus.Cancelled);
        
        if (hasActiveProjects)
            throw new InvalidOperationException("Cannot delete team with active projects. Complete or cancel all projects first.");

        _teamRepository.Remove(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
