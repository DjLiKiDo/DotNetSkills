namespace DotNetSkills.Application.UserManagement.Handlers;

/// <summary>
/// Handler for UpdateUserCommand that orchestrates user profile updates using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateUserCommandHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="unitOfWork">Unit of work for transaction management.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateUserCommand by updating user profile using domain methods.
    /// </summary>
    /// <param name="request">The update user command.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing UserResponse on success or error details on failure.</returns>
    public async Task<Result<UserResponse>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user {UserId} with new profile information", 
                request.UserId.Value);

            // Load existing user from repository
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId.Value);
                return Result<UserResponse>.Failure("User not found");
            }

            // Create EmailAddress value object from string input
            EmailAddress emailAddress;
            try
            {
                emailAddress = new EmailAddress(request.Email);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid email address provided: {Email}. Error: {Error}", 
                    request.Email, ex.Message);
                return Result<UserResponse>.Failure($"Invalid email address: {ex.Message}");
            }

            // Check email uniqueness if email is being changed
            if (user.Email.Value != request.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(emailAddress, cancellationToken)
                    .ConfigureAwait(false);

                if (existingUser != null && existingUser.Id != user.Id)
                {
                    _logger.LogWarning("Email address {Email} is already in use by another user", 
                        request.Email);
                    return Result<UserResponse>.Failure("Email address is already in use");
                }
            }

            // Use domain method to update profile - domain enforces business rules
            try
            {
                user.UpdateProfile(request.Name, emailAddress);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid profile data for user {UserId}: {Error}", 
                    request.UserId.Value, ex.Message);
                return Result<UserResponse>.Failure($"Invalid profile data: {ex.Message}");
            }

            // Update user in repository and save changes with transaction management
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully updated user {UserId} profile", 
                request.UserId.Value);

            // Domain events are dispatched automatically through DomainEventDispatchBehavior
            return Result<UserResponse>.Success(userResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating user {UserId}", 
                request.UserId.Value);
            return Result<UserResponse>.Failure("An unexpected error occurred while updating the user");
        }
    }
}