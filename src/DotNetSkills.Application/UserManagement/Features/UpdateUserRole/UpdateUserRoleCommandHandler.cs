namespace DotNetSkills.Application.UserManagement.Features.UpdateUserRole;

/// <summary>
/// Handler for UpdateUserRoleCommand that orchestrates user role changes using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand, Result<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserRoleCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the UpdateUserRoleCommandHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="unitOfWork">Unit of work for transaction management.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public UpdateUserRoleCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateUserRoleCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateUserRoleCommand by updating user role using domain methods.
    /// </summary>
    /// <param name="request">The update user role command.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>Result containing UserResponse on success or error details on failure.</returns>
    public async Task<Result<UserResponse>> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user {UserId} role to {NewRole} by user {ChangedById}", 
                request.UserId.Value, request.Role, request.ChangedById.Value);

            // Load existing user from repository
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId.Value);
                return Result<UserResponse>.Failure("User not found");
            }

            // Load the user making the change for authorization validation
            var changedByUser = await _userRepository.GetByIdAsync(request.ChangedById, cancellationToken)
                .ConfigureAwait(false);

            if (changedByUser == null)
            {
                _logger.LogWarning("User performing the change not found: {ChangedById}", 
                    request.ChangedById.Value);
                return Result<UserResponse>.Failure("User performing the change not found");
            }

            // Use domain method to change role - domain enforces business rules and authorization
            try
            {
                user.ChangeRole(request.Role, changedByUser);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning("Domain validation failed during role change for user {UserId}: {Error}", 
                    request.UserId.Value, ex.Message);
                return Result<UserResponse>.Failure(ex.Message);
            }

            // Update user in repository and save changes with transaction management
            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully updated user {UserId} role to {NewRole}", 
                request.UserId.Value, request.Role);

            // Domain events are dispatched automatically through DomainEventDispatchBehavior
            return Result<UserResponse>.Success(userResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while updating user {UserId} role", 
                request.UserId.Value);
            return Result<UserResponse>.Failure("An unexpected error occurred while updating the user role");
        }
    }
}