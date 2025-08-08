namespace DotNetSkills.API.Endpoints.TaskExecution;

/// <summary>
/// Task execution endpoints following Clean Architecture and DDD principles.
/// This class implements comprehensive task management operations with proper authorization and validation.
/// Tasks support single assignment, one-level subtask nesting, and status transitions.
/// </summary>
public static class TaskEndpoints
{
    /// <summary>
    /// Maps all task management endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tasks")
            .WithTags("Task Management")
            .RequireAuthorization();

        // Map individual endpoints
        MapGetTasksEndpoint(group);
        MapGetTaskByIdEndpoint(group);
        MapCreateTaskEndpoint(group);
        MapUpdateTaskEndpoint(group);
        MapDeleteTaskEndpoint(group);
        MapUpdateTaskStatusEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/tasks endpoint for listing tasks with comprehensive filtering.
    /// Supports filtering by project, assignee, status, priority, due dates, and search terms.
    /// </summary>
    private static void MapGetTasksEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("", GetTasks)
            .WithName("GetTasks")
            .WithSummary("Get tasks with filtering")
            .WithDescription("Retrieves a paginated list of tasks with comprehensive filtering. Query parameters: pageNumber (int), pageSize (int), projectId (Guid), assignedUserId (Guid), status (TaskStatus), priority (TaskPriority), dueDateFrom (date), dueDateTo (date), createdFrom (date), createdTo (date), searchTerm (string), includeSubtasks (bool), onlyOverdue (bool), onlyUnassigned (bool), sortBy (string), sortDirection (asc|desc). Enums are provided as strings, e.g., status=InProgress, priority=High.")
            .Produces<PagedTaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    /// <summary>
    /// Maps the GET /api/v1/tasks/{id} endpoint for retrieving a specific task.
    /// </summary>
    private static void MapGetTaskByIdEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}", GetTaskById)
            .WithName("GetTaskById")
            .WithSummary("Get task by ID")
            .WithDescription("Retrieves a specific task by its ID with full details including relationships and calculated properties")
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/tasks endpoint for creating a new task.
    /// </summary>
    private static void MapCreateTaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("", CreateTask)
            .WithName("CreateTask")
            .WithSummary("Create a new task")
            .WithDescription("Creates a new task with optional assignment and parent task relationship. Supports business rules for single assignment and one-level subtask nesting")
            .RequireAuthorization("ProjectMemberOrAdmin")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the PUT /api/v1/tasks/{id} endpoint for updating a task.
    /// </summary>
    private static void MapUpdateTaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{id:guid}", UpdateTask)
            .WithName("UpdateTask")
            .WithSummary("Update task information")
            .WithDescription("Updates task information while respecting domain constraints and status restrictions. Completed tasks cannot be modified")
            .RequireAuthorization("ProjectMemberOrAdmin")
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the DELETE /api/v1/tasks/{id} endpoint for deleting a task.
    /// </summary>
    private static void MapDeleteTaskEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("{id:guid}", DeleteTask)
            .WithName("DeleteTask")
            .WithSummary("Delete task (soft delete)")
            .WithDescription("Deletes a task by cancelling it (soft delete). This operation also cancels all subtasks automatically")
            .RequireAuthorization("ProjectMemberOrAdmin")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the PUT /api/v1/tasks/{id}/status endpoint for updating task status.
    /// </summary>
    private static void MapUpdateTaskStatusEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{id:guid}/status", UpdateTaskStatus)
            .WithName("UpdateTaskStatus")
            .WithSummary("Update task status")
            .WithDescription("Updates task status with proper state transition validation. Supports all valid status transitions according to domain business rules")
            .RequireAuthorization("ProjectMemberOrAdmin")
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Handles GET /api/v1/tasks requests with comprehensive filtering support.
    /// </summary>
    private static async Task<IResult> GetTasks(
        [AsParameters] GetTasksQueryParameters queryParams,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = new GetTasksQuery(
                queryParams.PageNumber,
                queryParams.PageSize,
                queryParams.ProjectId.HasValue ? new ProjectId(queryParams.ProjectId.Value) : null,
                queryParams.AssignedUserId.HasValue ? new UserId(queryParams.AssignedUserId.Value) : null,
                queryParams.Status,
                queryParams.Priority,
                queryParams.DueDateFrom,
                queryParams.DueDateTo,
                queryParams.CreatedFrom,
                queryParams.CreatedTo,
                queryParams.SearchTerm,
                queryParams.IncludeSubtasks,
                queryParams.OnlyOverdue,
                queryParams.OnlyUnassigned,
                queryParams.SortBy,
                queryParams.SortDirection
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            var placeholderResponse = new PagedTaskResponse(
                Tasks: new List<TaskResponse>(),
                PageNumber: queryParams.PageNumber,
                PageSize: queryParams.PageSize,
                TotalCount: 0,
                TotalPages: 0,
                HasNextPage: false,
                HasPreviousPage: false,
                FilterMetadata: new TaskFilterMetadata(
                    queryParams.ProjectId,
                    null,
                    queryParams.AssignedUserId,
                    null,
                    queryParams.Status,
                    queryParams.Priority,
                    queryParams.DueDateFrom,
                    queryParams.DueDateTo,
                    queryParams.CreatedFrom,
                    queryParams.CreatedTo,
                    queryParams.SearchTerm,
                    0, 0, 0, 0, 0
                ));

            return Results.Ok(placeholderResponse);
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Query Parameters");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while retrieving tasks",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles GET /api/v1/tasks/{id} requests.
    /// </summary>
    private static async Task<IResult> GetTaskById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var taskId = new TaskId(id);
            var query = new GetTaskByIdQuery(taskId);


            // TODO: Replace with MediatR.Send when implemented
            // var task = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("GetTaskById requires Infrastructure layer implementation");
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Task ID");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while retrieving the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles POST /api/v1/tasks requests.
    /// </summary>
    private static async Task<IResult> CreateTask(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {

            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            var command = new CreateTaskCommand(
                request.Title,
                request.Description,
                new ProjectId(request.ProjectId),
                request.Priority,
                request.ParentTaskId.HasValue ? new TaskId(request.ParentTaskId.Value) : null,
                request.EstimatedHours,
                request.DueDate,
                request.AssignedUserId.HasValue ? new UserId(request.AssignedUserId.Value) : null,
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("CreateTask requires Infrastructure layer implementation");
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Request");
        }
        catch (DomainException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Business Rule Violation");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while creating the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/tasks/{id} requests.
    /// </summary>
    private static async Task<IResult> UpdateTask(
        Guid id,
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {

            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            var command = new UpdateTaskCommand(
                new TaskId(id),
                request.Title,
                request.Description,
                request.Priority,
                request.EstimatedHours,
                request.DueDate,
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UpdateTask requires Infrastructure layer implementation");
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Request");
        }
        catch (DomainException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Business Rule Violation");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while updating the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles DELETE /api/v1/tasks/{id} requests.
    /// </summary>
    private static async Task<IResult> DeleteTask(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            var command = new DeleteTaskCommand(
                new TaskId(id),
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("DeleteTask requires Infrastructure layer implementation");
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Task ID");
        }
        catch (DomainException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Business Rule Violation");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while deleting the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/tasks/{id}/status requests.
    /// </summary>
    private static async Task<IResult> UpdateTaskStatus(
        Guid id,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {

            // TODO: Get current user ID from authentication context
            var currentUserId = new UserId(Guid.NewGuid()); // Placeholder - replace with actual user from JWT

            var command = new UpdateTaskStatusCommand(
                new TaskId(id),
                request.Status,
                request.ActualHours,
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UpdateTaskStatus requires Infrastructure layer implementation");
        }
        catch (ArgumentException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid Request");
        }
        catch (DomainException ex)
        {
            return Results.Problem(
                detail: ex.Message,
                statusCode: StatusCodes.Status409Conflict,
                title: "Business Rule Violation");
        }
    catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while updating task status",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }
}

/// <summary>
/// Query parameters for GET /api/v1/tasks endpoint with comprehensive filtering support.
/// Supports pagination, filtering by multiple criteria, and sorting options.
/// </summary>
public record GetTasksQueryParameters(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? ProjectId = null,
    Guid? AssignedUserId = null,
    DomainTaskStatus? Status = null,
    TaskPriority? Priority = null,
    DateTime? DueDateFrom = null,
    DateTime? DueDateTo = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    string? SearchTerm = null,
    bool IncludeSubtasks = true,
    bool OnlyOverdue = false,
    bool OnlyUnassigned = false,
    string SortBy = "CreatedAt",
    string SortDirection = "desc"
);
