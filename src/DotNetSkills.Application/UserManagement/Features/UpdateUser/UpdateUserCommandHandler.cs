namespace DotNetSkills.Application.UserManagement.Features.UpdateUser;

/// <summary>
/// Handler for UpdateUserCommand that orchestrates user profile updates using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
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
    /// <returns>UserResponse on success.</returns>
    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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
                throw new DomainException("User not found");
            }

            // Create EmailAddress value object from string input
            EmailAddress emailAddress = new EmailAddress(request.Email);

            // Check email uniqueness if email is being changed
            if (user.Email.Value != request.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(emailAddress, cancellationToken)
                    .ConfigureAwait(false);

                if (existingUser != null && existingUser.Id != user.Id)
                {
                    _logger.LogWarning("Email address {Email} is already in use by another user", request.Email);
                    throw new DomainException("Email address is already in use");
                }
            }

            // Use domain method to update profile - domain enforces business rules
            user.UpdateProfile(request.Name, emailAddress);

            // Update user in repository and save changes with transaction management
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully updated user {UserId} profile", 
                request.UserId.Value);

            // Domain events are dispatched automatically through DomainEventDispatchBehavior
            return userResponse;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while updating user {UserId}", request.UserId.Value);
            throw new DomainException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating user {UserId}", request.UserId.Value);
            throw new ApplicationException("An unexpected error occurred while updating the user", ex);
        }
    }
}