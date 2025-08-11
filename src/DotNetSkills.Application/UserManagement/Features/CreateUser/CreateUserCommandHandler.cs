namespace DotNetSkills.Application.UserManagement.Features.CreateUser;

/// <summary>
/// Handler for CreateUserCommand that orchestrates user creation using domain factory methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the CreateUserCommandHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="unitOfWork">Unit of work for transaction management.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateUserCommand by creating a new user using domain factory methods.
    /// </summary>
    /// <param name="request">The create user command.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>UserResponse on success.</returns>
    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating user with email {Email} and role {Role}", 
                request.Email, request.Role);

            // Create EmailAddress value object from string input
            EmailAddress emailAddress = new EmailAddress(request.Email);

            // Get creator user if specified for authorization validation
            User? createdByUser = null;
            if (request.CreatedById != null)
            {
                createdByUser = await _userRepository.GetByIdAsync(request.CreatedById, cancellationToken)
                    .ConfigureAwait(false);
                
                if (createdByUser == null)
                {
                    _logger.LogWarning("Creator user not found: {CreatedById}", request.CreatedById.Value);
                    throw new DomainException("Creator user not found");
                }
            }

            // Use domain factory method User.Create() with proper creator validation
            User user = User.Create(request.Name, emailAddress, request.Role, createdByUser);

            // Check email uniqueness using repository
            var existingUser = await _userRepository.GetByEmailAsync(emailAddress, cancellationToken)
                .ConfigureAwait(false);
            
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists", request.Email);
                throw new DomainException("A user with this email address already exists");
            }

            // Add user to repository and save changes with transaction management
            _userRepository.Add(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Map entity to response DTO using AutoMapper
            var userResponse = _mapper.Map<UserResponse>(user);

            _logger.LogInformation("Successfully created user {UserId} with email {Email}", 
                user.Id.Value, request.Email);

            // Domain events are dispatched automatically through DomainEventDispatchBehavior
            return userResponse;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while creating user {Email}", request.Email);
            throw new DomainException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while creating user with email {Email}", request.Email);
            throw new ApplicationException("An unexpected error occurred while creating the user", ex);
        }
    }
}