namespace DotNetSkills.Application.UserManagement.Handlers;

/// <summary>
/// Handler for GetUserTeamMembershipsQuery that retrieves all team memberships for a specific user.
/// Returns team membership details including team information and user roles within teams.
/// </summary>
public class GetUserTeamMembershipsQueryHandler : IRequestHandler<GetUserTeamMembershipsQuery, Result<TeamMembershipListDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserTeamMembershipsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetUserTeamMembershipsQueryHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public GetUserTeamMembershipsQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserTeamMembershipsQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserTeamMembershipsQuery by retrieving user with team memberships.
    /// Maps team membership entities to DTOs with team details and membership roles.
    /// </summary>
    /// <param name="request">The get user team memberships query.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing TeamMembershipListDto with user's team memberships.</returns>
    public async Task<Result<TeamMembershipListDto>> Handle(GetUserTeamMembershipsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving team memberships for user: {UserId}", request.UserId.Value);

            // Load user from repository (this should include team memberships through navigation properties)
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found for team memberships query: {UserId}", request.UserId.Value);
                return Result<TeamMembershipListDto>.Failure("User not found");
            }

            // Map user team memberships to DTOs
            // Note: The mapping profile should handle the complex navigation from User -> TeamMemberships -> Team
            var teamMembershipListDto = _mapper.Map<TeamMembershipListDto>(user);

            _logger.LogInformation("Successfully retrieved {Count} team memberships for user: {UserId}",
                teamMembershipListDto.TeamMemberships.Count, request.UserId.Value);

            return Result<TeamMembershipListDto>.Success(teamMembershipListDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving team memberships for user {UserId}",
                request.UserId.Value);
            return Result<TeamMembershipListDto>.Failure("An unexpected error occurred while retrieving team memberships");
        }
    }
}