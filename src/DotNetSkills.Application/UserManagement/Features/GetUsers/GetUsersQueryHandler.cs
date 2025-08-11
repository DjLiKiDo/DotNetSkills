namespace DotNetSkills.Application.UserManagement.Features.GetUsers;

/// <summary>
/// Handler for GetUsersQuery that retrieves paginated users with filtering and search capabilities.
/// Supports role filtering, status filtering, and case-insensitive search by name and email.
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the GetUsersQueryHandler class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="mapper">AutoMapper for entity to DTO mapping.</param>
    /// <param name="logger">Logger for structured logging.</param>
    public GetUsersQueryHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUsersQuery by retrieving paginated and filtered users from repository.
    /// Applies search filtering, role filtering, and status filtering as specified.
    /// </summary>
    /// <param name="request">The get users query with pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>PagedUserResponse with users and pagination metadata.</returns>
    public async Task<PagedUserResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving users with pagination - Page: {Page}, PageSize: {PageSize}, SearchTerm: {SearchTerm}, Role: {Role}, Status: {Status}",
                request.Page, request.PageSize, request.SearchTerm, request.Role, request.Status);

            // Validate pagination parameters
            if (request.Page <= 0)
            {
                _logger.LogWarning("Invalid page number: {Page}", request.Page);
                throw new DomainException("Page number must be greater than 0");
            }

            if (request.PageSize <= 0 || request.PageSize > 100)
            {
                _logger.LogWarning("Invalid page size: {PageSize}", request.PageSize);
                throw new DomainException("Page size must be between 1 and 100");
            }

            // Get paginated users from repository with filtering
            var (users, totalCount) = await _userRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.Role,
                request.Status,
                cancellationToken)
                .ConfigureAwait(false);

            // Map entities to response DTOs using AutoMapper
            var userResponses = _mapper.Map<IReadOnlyList<UserResponse>>(users);

            // Create paginated response with metadata
            var pagedResponse = new PagedUserResponse(
                userResponses,
                totalCount,
                request.Page,
                request.PageSize);

            _logger.LogInformation("Successfully retrieved {Count} users out of {TotalCount} total users for page {Page}",
                userResponses.Count, totalCount, request.Page);

            return pagedResponse;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving users with pagination");
            throw new ApplicationException("An unexpected error occurred while retrieving users", ex);
        }
    }
}