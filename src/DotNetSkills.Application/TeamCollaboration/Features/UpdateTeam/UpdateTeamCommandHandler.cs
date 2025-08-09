namespace DotNetSkills.Application.TeamCollaboration.Features.UpdateTeam;

/// <summary>
/// Handler for UpdateTeamCommand that orchestrates team updates using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTeamCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamResponse> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to update teams");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        if (!currentUser.CanManageTeams() && team.GetMember(currentUserId)?.HasLeadershipPrivileges() != true)
            throw new UnauthorizedAccessException("User does not have permission to update this team");

        var existingTeam = await _teamRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);
        if (existingTeam != null && existingTeam.Id != request.TeamId)
            throw new InvalidOperationException($"Team with name '{request.Name}' already exists");

        team.UpdateInfo(request.Name, request.Description);

        _teamRepository.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return TeamResponse.FromDomain(team);
    }
}
