namespace DotNetSkills.Application.UserManagement.Features.CheckUserExists;

/// <summary>
/// Handler for CheckUserExistsQuery that verifies user existence for validation purposes.
/// Returns true if user exists, false otherwise.
/// Used by FluentValidation validators for cross-entity validation and authorization checks.
/// </summary>
public class CheckUserExistsQueryHandler : IRequestHandler<CheckUserExistsQuery, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CheckUserExistsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the CheckUserExistsQueryHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public CheckUserExistsQueryHandler(
        IUserRepository userRepository,
        ILogger<CheckUserExistsQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CheckUserExistsQuery by verifying user existence in the repository.
    /// Returns true if user exists, false otherwise.
    /// </summary>
    /// <param name="request">The check user exists query.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>True if user exists; false otherwise.</returns>
    public async Task<bool> Handle(CheckUserExistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking user existence: {UserId}", request.UserId.Value);

            // Check if user exists in repository
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            bool userExists = user != null;

            _logger.LogInformation("User existence check result for {UserId}: Exists={Exists}",
                request.UserId.Value, userExists);

            return userExists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while checking user existence: {UserId}", request.UserId.Value);
            throw new ApplicationException("An unexpected error occurred while checking user existence", ex);
        }
    }
}