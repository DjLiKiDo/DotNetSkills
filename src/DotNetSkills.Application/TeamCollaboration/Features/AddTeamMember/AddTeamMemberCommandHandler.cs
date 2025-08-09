namespace DotNetSkills.Application.TeamCollaboration.Features.AddTeamMember;

/// <summary>
/// Handler for AddTeamMemberCommand that orchestrates team member addition using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class AddTeamMemberCommandHandler : IRequestHandler<AddTeamMemberCommand, TeamMemberResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddTeamMemberCommandHandler(
        ITeamRepository teamRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TeamMemberResponse> Handle(AddTeamMemberCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to add team members");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var team = await _teamRepository.GetWithMembersAsync(request.TeamId, cancellationToken).ConfigureAwait(false);
        if (team == null)
            throw new InvalidOperationException($"Team with ID '{request.TeamId}' not found");

        var userToAdd = await _userRepository.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
        if (userToAdd == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found");

        team.AddMember(userToAdd, request.Role, currentUser);

        _teamRepository.Update(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var addedMember = team.GetMember(request.UserId);
        return _mapper.Map<TeamMemberResponse>(addedMember!);
    }
}
