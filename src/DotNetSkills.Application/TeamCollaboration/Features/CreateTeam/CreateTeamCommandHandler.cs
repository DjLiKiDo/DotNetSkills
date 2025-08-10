namespace DotNetSkills.Application.TeamCollaboration.Features.CreateTeam;

/// <summary>
/// Handler for CreateTeamCommand that orchestrates team creation using domain factory methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamResponse>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateTeamCommandHandler(
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

    public async Task<TeamResponse> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to create teams");

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        var existingTeam = await _teamRepository.GetByNameAsync(request.Name, cancellationToken).ConfigureAwait(false);
        if (existingTeam != null)
            throw new InvalidOperationException($"Team with name '{request.Name}' already exists");

        var team = Team.Create(request.Name, request.Description, currentUser);

        _teamRepository.Add(team);
        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return _mapper.Map<TeamResponse>(team);
    }
}
