namespace DotNetSkills.API.Endpoints.ProjectManagement;

/// <summary>
/// Project-task relationship endpoints following Clean Architecture and DDD principles.
/// This class implements task operations specifically within project context with cross-aggregate coordination.
/// Tasks belong to projects and must respect project team member boundaries.
/// </summary>
public static class ProjectTaskEndpoints
{
    /// <summary>
    /// Maps all project-task relationship endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapProjectTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/projects/{projectId:guid}/tasks")
            .WithTags("Project Task Management")
            .RequireAuthorization();

        // Map individual endpoints
        MapGetProjectTasksEndpoint(group);
        MapCreateTaskInProjectEndpoint(group);
        MapUpdateTaskInProjectEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/projects/{projectId}/tasks endpoint for listing project tasks.
    /// Supports comprehensive filtering by status, assignee, priority, and date ranges.
    /// </summary>
    private static void MapGetProjectTasksEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("", GetProjectTasks)
            .WithName("GetProjectTasks")
            .WithSummary("Get tasks for a specific project")
            .WithDescription("Retrieves a paginated list of tasks belonging to a specific project with optional filtering")
            .Produces<PagedProjectTaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/projects/{projectId}/tasks endpoint for creating tasks in project.
    /// Tasks created this way automatically belong to the specified project.
    /// </summary>
    private static void MapCreateTaskInProjectEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("", CreateTaskInProject)
            .WithName("CreateTaskInProject")
            .WithSummary("Create a new task in project")
            .WithDescription("Creates a new task that belongs to the specified project - requires project team membership")
            .RequireAuthorization("ProjectMemberOrAdmin") // TODO: Implement ProjectMemberOrAdmin policy
            .Accepts<CreateTaskInProjectRequest>("application/json")
            .Produces<ProjectTaskResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the PUT /api/v1/projects/{projectId}/tasks/{taskId} endpoint for updating project tasks.
    /// Ensures task belongs to the specified project before allowing updates.
    /// </summary>
    private static void MapUpdateTaskInProjectEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{taskId:guid}", UpdateTaskInProject)
            .WithName("UpdateTaskInProject")
            .WithSummary("Update task in project context")
            .WithDescription("Updates a task within the context of its project with validation of project membership")
            .RequireAuthorization("ProjectMemberOrAdmin") // TODO: Implement ProjectMemberOrAdmin policy
            .Accepts<UpdateTaskInProjectRequest>("application/json")
            .Produces<ProjectTaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Handles GET /api/v1/projects/{projectId}/tasks - retrieves tasks for a specific project.
    /// Supports comprehensive filtering and enforces project boundary validation.
    /// </summary>
    private static async Task<IResult> GetProjectTasks(
        string projectId,
        int page = 1,
        int pageSize = 20,
        string? status = null,
        Guid? assignedUserId = null,
        string? priority = null,
        DateTime? dueDateFrom = null,
        DateTime? dueDateTo = null,
        bool? isOverdue = null,
        bool? isSubtask = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(projectId, out var projectGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate query
            var query = new GetProjectTasksQuery(
                ProjectId: new ProjectId(projectGuid),
                Page: page,
                PageSize: pageSize,
                Status: status,
                AssignedUserId: assignedUserId,
                Priority: priority,
                DueDateFrom: dueDateFrom,
                DueDateTo: dueDateTo,
                IsOverdue: isOverdue,
                IsSubtask: isSubtask,
                Search: search
            );

            query.Validate();

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            var placeholderResponse = new PagedProjectTaskResponse(
                Tasks: new List<ProjectTaskResponse>(),
                TotalCount: 0,
                Page: page,
                PageSize: pageSize,
                ProjectId: projectGuid,
                ProjectName: "Sample Project", // TODO: Get from repository
                ActiveTaskCount: 0,
                CompletedTaskCount: 0,
                OverdueTaskCount: 0
            );

            return Results.Ok(placeholderResponse);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred while retrieving project tasks",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles POST /api/v1/projects/{projectId}/tasks - creates a new task in the specified project.
    /// Validates project membership and enforces business rules for project-task relationships.
    /// </summary>
    private static async Task<IResult> CreateTaskInProject(
        string projectId,
        CreateTaskInProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(projectId, out var projectGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Validate request
            try
            {
                request.Validate();
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ValidationError"] = new[] { ex.Message }
                });
            }

            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            // Create and validate command
            var command = new CreateTaskInProjectCommand(
                ProjectId: new ProjectId(projectGuid),
                Title: request.Title,
                Description: request.Description,
                Priority: request.Priority,
                ParentTaskId: request.ParentTaskId.HasValue ? new TaskId(request.ParentTaskId.Value) : null,
                EstimatedHours: request.EstimatedHours,
                DueDate: request.DueDate,
                AssignedUserId: request.AssignedUserId.HasValue ? new UserId(request.AssignedUserId.Value) : null,
                CreatedBy: currentUserId
            );

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);
            // return Results.Created($"/api/v1/projects/{projectId}/tasks/{result.TaskId}", result);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("CreateTaskInProject requires Application layer implementation");
        }
        catch (DomainException ex)
        {
            return Results.Conflict(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred while creating the task",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/projects/{projectId}/tasks/{taskId} - updates a task within project context.
    /// Validates that task belongs to project and enforces cross-aggregate business rules.
    /// </summary>
    private static async Task<IResult> UpdateTaskInProject(
        string projectId,
        string taskId,
        UpdateTaskInProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(projectId, out var projectGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (!Guid.TryParse(taskId, out var taskGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid task ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Validate request
            try
            {
                request.Validate();
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ValidationError"] = new[] { ex.Message }
                });
            }

            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            // Create and validate command
            var command = new UpdateTaskInProjectCommand(
                ProjectId: new ProjectId(projectGuid),
                TaskId: new TaskId(taskGuid),
                Title: request.Title,
                Description: request.Description,
                Priority: request.Priority,
                EstimatedHours: request.EstimatedHours,
                DueDate: request.DueDate,
                UpdatedBy: currentUserId
            );

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);
            // return Results.Ok(result);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UpdateTaskInProject requires Application layer implementation");
        }
        catch (DomainException ex)
        {
            return Results.Conflict(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Invalid Request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(
                title: "An error occurred while updating the task",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
