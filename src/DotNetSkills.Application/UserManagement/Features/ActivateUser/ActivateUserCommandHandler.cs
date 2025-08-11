namespace DotNetSkills.Application.UserManagement.Features.ActivateUser;

/// <summary>
/// Handler for ActivateUserCommand that orchestrates user activation using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<ActivateUserCommandHandler> _logger;

    public ActivateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the ActivateUserCommand by activating user using domain methods.
    /// This operation is idempotent - activating an already active user succeeds.
    /// </summary>
    public async Task<UserResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Activating user {UserId}", request.UserId.Value);

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", request.UserId.Value);
                throw new DomainException("User not found");
            }

            if (user.Status == UserStatus.Active)
            {
                _logger.LogInformation("User {UserId} already active - idempotent success", request.UserId.Value);
                return _mapper.Map<UserResponse>(user);
            }

            user.Activate();

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var response = _mapper.Map<UserResponse>(user);
            _logger.LogInformation("Successfully activated user {UserId}", request.UserId.Value);
            return response;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while activating user {UserId}", request.UserId.Value);
            throw new ApplicationException("An unexpected error occurred while activating the user", ex);
        }
    }
}
