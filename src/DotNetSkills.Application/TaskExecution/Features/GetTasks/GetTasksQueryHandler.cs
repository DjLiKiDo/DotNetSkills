namespace DotNetSkills.Application.TaskExecution.Features.GetTasks;

/// <summary>
/// Handler for GetTasksQuery that retrieves tasks with comprehensive filtering and pagination.
/// </summary>
public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, PagedTaskResponse>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTasksQueryHandler> _logger;

    public GetTasksQueryHandler(
        ITaskRepository taskRepository,
        IMapper mapper,
        ILogger<GetTasksQueryHandler> logger)
    {
        _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(taskRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PagedTaskResponse> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving tasks with filters - Page: {PageNumber}, Size: {PageSize}, Project: {ProjectId}, AssignedUser: {AssignedUserId}", 
            request.PageNumber, request.PageSize, request.ProjectId, request.AssignedUserId);

        // Apply pagination with comprehensive filtering
        var (tasks, totalCount) = await _taskRepository.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.ProjectId,
            request.AssignedUserId,
            request.Status,
            request.Priority,
            request.DueDateFrom,
            request.DueDateTo,
            request.OnlyOverdue ? true : null,
            request.IncludeSubtasks ? null : false, // null means include both, false means exclude subtasks
            cancellationToken)
            .ConfigureAwait(false);

        // Create PagedResponse for mapping
        var pagedTasks = new PagedResponse<DomainTask>(tasks, request.PageNumber, request.PageSize, totalCount);
        
        // Map to response DTO with comprehensive filter metadata
        var response = _mapper.Map<PagedTaskResponse>(pagedTasks);

        _logger.LogDebug("Successfully retrieved {Count} tasks out of {TotalCount} total", tasks.Count(), totalCount);
        return response;
    }
}
