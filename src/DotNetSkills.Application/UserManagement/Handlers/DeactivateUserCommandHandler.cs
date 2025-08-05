namespace DotNetSkills.Application.UserManagement.Handlers;

/// <summary>
/// Handler for DeactivateUserCommand that orchestrates user deactivation using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeactivateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the DeactivateUserCommandHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="unitOfWork">Unit of work for transaction management.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public DeactivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DeactivateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeactivateUserCommand by deactivating user using domain methods.
    /// This operation is idempotent - deactivating an already deactivated user succeeds.
    /// </summary>
    /// <param name="request">The deactivate user command.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing UserResponse on success or error details on failure.</returns>
    public async Task<Result<UserResponse>> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deactivating user {UserId} by user {DeactivatedById}", 
                request.UserId.Value, request.DeactivatedById.Value);

            // Load existing user from repository
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId.Value);
                return Result<UserResponse>.Failure("User not found");
            }

            // Load the user making the deactivation for authorization validation
            var deactivatedByUser = await _userRepository.GetByIdAsync(request.DeactivatedById, cancellationToken)
                .ConfigureAwait(false);

            if (deactivatedByUser == null)
            {
                _logger.LogWarning("User performing the deactivation not found: {DeactivatedById}", 
                    request.DeactivatedById.Value);
                return Result<UserResponse>.Failure("User performing the deactivation not found");
            }

            // Admin-only operation validation through domain BusinessRules
            if (!BusinessRules.Authorization.HasSufficientPrivileges(deactivatedByUser.Role, user.Role))
            {
                _logger.LogWarning("User {DeactivatedById} does not have sufficient privileges to deactivate user {UserId}", 
                    request.DeactivatedById.Value, request.UserId.Value);
                return Result<UserResponse>.Failure("Insufficient privileges to deactivate this user");
            }

            // Prevent users from deactivating themselves
            if (request.UserId == request.DeactivatedById)
            {
                _logger.LogWarning("User {UserId} attempted to deactivate themselves", request.UserId.Value);
                return Result<UserResponse>.Failure("Cannot deactivate your own account");
            }

            // Additional admin-only check for deactivation (only admins can deactivate users)
            if (deactivatedByUser.Role != UserRole.Admin)
            {
                _logger.LogWarning("Non-admin user {DeactivatedById} attempted to deactivate user {UserId}", 
                    request.DeactivatedById.Value, request.UserId.Value);
                return Result<UserResponse>.Failure("Only administrators can deactivate user accounts");
            }

            // Check if user is already deactivated (idempotent operation)
            if (user.Status == UserStatus.Inactive)
            {
                _logger.LogInformation("User {UserId} is already deactivated - operation is idempotent", 
                    request.UserId.Value);
                
                // Map to response and return success (idempotent behavior)
                var existingUserResponse = _mapper.Map<UserResponse>(user);
                return Result<UserResponse>.Success(existingUserResponse);
            }

            // Use domain method to deactivate user - domain handles business rules and events
            // The Deactivate() method is simple and doesn't require additional validation
            // Team membership cascade handling is managed by the domain/infrastructure layers
            user.Deactivate();

            // Update user in repository and save changes with transaction management
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully deactivated user {UserId}", request.UserId.Value);

            // Domain events for user deactivation are raised by entity through domain event system
            // The deactivation itself doesn't raise domain events, but this could be extended
            // if business requirements change to include audit trails or notifications
            return Result<UserResponse>.Success(userResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while deactivating user {UserId}", 
                request.UserId.Value);
            return Result<UserResponse>.Failure("An unexpected error occurred while deactivating the user");
        }
    }
}