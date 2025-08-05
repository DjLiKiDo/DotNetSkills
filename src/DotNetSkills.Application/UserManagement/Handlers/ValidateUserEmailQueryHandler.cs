namespace DotNetSkills.Application.UserManagement.Handlers;

/// <summary>
/// Handler for ValidateUserEmailQuery that checks email uniqueness for validation purposes.
/// Returns true if email is available (unique), false if already taken.
/// Used by FluentValidation validators for cross-entity validation.
/// </summary>
public class ValidateUserEmailQueryHandler : IRequestHandler<ValidateUserEmailQuery, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ValidateUserEmailQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ValidateUserEmailQueryHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public ValidateUserEmailQueryHandler(
        IUserRepository userRepository,
        ILogger<ValidateUserEmailQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the ValidateUserEmailQuery by checking email uniqueness in the repository.
    /// Returns true if email is available, false if already taken by another user.
    /// </summary>
    /// <param name="request">The validate user email query.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing boolean indicating if email is available (true) or taken (false).</returns>
    public async Task<Result<bool>> Handle(ValidateUserEmailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Validating email uniqueness: {Email}, ExcludeUserId: {ExcludeUserId}",
                request.Email, request.ExcludeUserId?.Value);

            // Validate email format first
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                _logger.LogWarning("Empty email provided for validation");
                return Result<bool>.Failure("Email cannot be empty");
            }

            // Create EmailAddress value object from string
            EmailAddress emailAddress;
            try
            {
                emailAddress = new EmailAddress(request.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Invalid email format provided: {Email}, Error: {Error}",
                    request.Email, ex.Message);
                return Result<bool>.Failure($"Invalid email format: {ex.Message}");
            }

            // Check if email is already taken by another user
            var existingUser = await _userRepository.GetByEmailAsync(emailAddress, cancellationToken)
                .ConfigureAwait(false);

            bool isEmailAvailable;
            if (existingUser == null)
            {
                // Email is not taken by anyone
                isEmailAvailable = true;
            }
            else if (request.ExcludeUserId != null && existingUser.Id == request.ExcludeUserId.Value)
            {
                // Email is taken by the user we're excluding (update scenario)
                isEmailAvailable = true;
            }
            else
            {
                // Email is taken by another user
                isEmailAvailable = false;
            }

            _logger.LogInformation("Email validation result for {Email}: Available={Available}",
                request.Email, isEmailAvailable);

            return Result<bool>.Success(isEmailAvailable);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while validating email uniqueness: {Email}",
                request.Email);
            return Result<bool>.Failure("An unexpected error occurred while validating email uniqueness");
        }
    }
}