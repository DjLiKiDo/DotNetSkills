namespace DotNetSkills.Application.UserManagement.Handlers;

/// <summary>
/// Handler for GetUserByIdQuery that retrieves a single user by their unique identifier.
/// Returns null wrapped in Result pattern when user is not found.
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse?>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetUserByIdQueryHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserByIdQuery by retrieving user from repository.
    /// Returns null in success Result if user is not found.
    /// </summary>
    /// <param name="request">The get user by ID query.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing UserResponse or null if user not found.</returns>
    public async Task<Result<UserResponse?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving user by ID: {UserId}", request.UserId.Value);

            // Load user from repository
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogInformation("User not found: {UserId}", request.UserId.Value);
                return Result<UserResponse?>.Success(null);
            }

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully retrieved user: {UserId}", request.UserId.Value);

            return Result<UserResponse?>.Success(userResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving user {UserId}", 
                request.UserId.Value);
            return Result<UserResponse?>.Failure("An unexpected error occurred while retrieving the user");
        }
    }
}