using DotNetSkills.API.Authorization;
using DotNetSkills.API.Services;
using MediatR;

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
            .WithDescription("Retrieves a paginated list of tasks for a specific project with filtering. Query parameters: page (int), pageSize (int), status (TaskStatus), assignedUserId (Guid), priority (TaskPriority), dueDateFrom (date), dueDateTo (date), isOverdue (bool), isSubtask (bool), search (string). Enums are provided as strings, e.g., status=InProgress, priority=High.")
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
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
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
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
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
        IMediator mediator,
        int page = 1,
        int pageSize = 20,
        DomainTaskStatus? status = null,
        Guid? assignedUserId = null,
        TaskPriority? priority = null,
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
                AssignedUserId: assignedUserId.HasValue ? new UserId(assignedUserId.Value) : null,
                Priority: priority,
                DueDateFrom: dueDateFrom,
                DueDateTo: dueDateTo,
                IsOverdue: isOverdue,
                IsSubtask: isSubtask,
                Search: search
            );


            var result = await mediator.Send(query, cancellationToken);

            return Results.Ok(result);
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
    catch (Exception)
        {
            return Results.Problem(
                title: "An error occurred while retrieving project tasks",
        detail: "An unexpected error occurred.",
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
        IMediator mediator,
        ICurrentUserService currentUserService,
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
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ValidationError"] = new[] { ex.Message }
                });
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Results.Unauthorized();
            }

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

            var result = await mediator.Send(command, cancellationToken);
            return Results.Created($"/api/v1/projects/{projectId}/tasks/{result.TaskId}", result);
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
        IMediator mediator,
        ICurrentUserService currentUserService,
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
            }
            catch (ValidationException ex)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["ValidationError"] = new[] { ex.Message }
                });
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Results.Unauthorized();
            }

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

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(result);
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
