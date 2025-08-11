namespace DotNetSkills.Application.UserManagement.Features.DeleteUser;

/// <summary>
/// Handler for deleting (soft delete / deactivating) a user account.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// Note: This performs soft deletion by deactivating the user account rather than permanent deletion.
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<UserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting (deactivating) user {UserId}", request.UserId);

        // Get current user from authentication context for authorization
        var currentUserId = _currentUserService.GetCurrentUserId();
        if (currentUserId == null)
            throw new UnauthorizedAccessException("User must be authenticated to delete users");

        // Load the user performing the deletion for authorization validation
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            .ConfigureAwait(false);
        if (currentUser == null)
            throw new UnauthorizedAccessException("Current user not found");

        // Load existing user to be deleted
        var userToDelete = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .ConfigureAwait(false);
        if (userToDelete == null)
            throw new InvalidOperationException($"User with ID '{request.UserId}' not found");

        // Authorization checks: Only admins can delete users, and users cannot delete themselves
        if (currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning("Non-admin user {CurrentUserId} attempted to delete user {UserId}", 
                currentUserId, request.UserId);
            throw new DomainException("Only administrators can delete user accounts");
        }

        if (request.UserId == currentUserId)
        {
            _logger.LogWarning("User {UserId} attempted to delete themselves", request.UserId);
            throw new DomainException("Cannot delete your own account");
        }

        // Check if user is already deactivated (idempotent operation)
        if (userToDelete.Status == UserStatus.Inactive)
        {
            _logger.LogInformation("User {UserId} is already deleted (deactivated) - operation is idempotent", 
                request.UserId);
            
            // Map to response and return success (idempotent behavior)
            var existingUserResponse = _mapper.Map<UserResponse>(userToDelete);
            return existingUserResponse;
        }

        // Use domain method to deactivate user (soft delete)
        // The Deactivate() method sets status to Inactive, preserving data integrity
        userToDelete.Deactivate();

        // Update user in repository and save changes
        _userRepository.Update(userToDelete);
        await _unitOfWork.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);

        _logger.LogInformation("Successfully deleted (deactivated) user {UserId} by user {CurrentUserId}", 
            request.UserId, currentUserId);

        // Map to response DTO
        var userResponse = _mapper.Map<UserResponse>(userToDelete);
        return userResponse;
    }
}
