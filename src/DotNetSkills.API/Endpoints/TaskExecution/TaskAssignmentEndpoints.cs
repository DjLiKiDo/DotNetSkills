using DotNetSkills.API.Authorization;
using DotNetSkills.Application.Common.Abstractions;

namespace DotNetSkills.API.Endpoints.TaskExecution;

/// <summary>
/// Task assignment and subtask management endpoints following Clean Architecture and DDD principles.
/// This class implements task assignment operations and subtask management with proper authorization and validation.
/// Tasks support single assignment and one-level subtask nesting as per domain business rules.
/// </summary>
public static class TaskAssignmentEndpoints
{
    /// <summary>
    /// Maps all task assignment and subtask management endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapTaskAssignmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/tasks")
            .WithTags("Task Assignment & Subtasks")
            .RequireAuthorization();

        // Map individual endpoints
        MapAssignTaskEndpoint(group);
        MapUnassignTaskEndpoint(group);
        MapCreateSubtaskEndpoint(group);
        MapGetSubtasksEndpoint(group);
        MapUpdateSubtaskEndpoint(group);
    }

    /// <summary>
    /// Maps the POST /api/v1/tasks/{id}/assign endpoint for assigning a task to a user.
    /// </summary>
    private static void MapAssignTaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/assign", AssignTask)
            .WithName("AssignTask")
            .WithSummary("Assign task to user")
            .WithDescription("Assigns a task to a specific user. Tasks support single assignment only as per domain business rules")
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
            .Accepts<AssignTaskRequest>("application/json")
            .Produces<TaskAssignmentResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the POST /api/v1/tasks/{id}/unassign endpoint for unassigning a task from its current assignee.
    /// </summary>
    private static void MapUnassignTaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/unassign", UnassignTask)
            .WithName("UnassignTask")
            .WithSummary("Unassign task from user")
            .WithDescription("Removes the current assignment from a task, making it available for reassignment")
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
            .Produces<TaskAssignmentResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the POST /api/v1/tasks/{id}/subtasks endpoint for creating a subtask.
    /// </summary>
    private static void MapCreateSubtaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/subtasks", CreateSubtask)
            .WithName("CreateSubtask")
            .WithSummary("Create subtask")
            .WithDescription("Creates a subtask under the specified parent task. Only one-level subtask nesting is allowed as per domain business rules")
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
            .Accepts<CreateSubtaskRequest>("application/json")
            .Produces<TaskResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the GET /api/v1/tasks/{id}/subtasks endpoint for listing subtasks.
    /// </summary>
    private static void MapGetSubtasksEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}/subtasks", GetSubtasks)
            .WithName("GetTaskSubtasks")
            .WithSummary("Get task subtasks")
            .WithDescription("Retrieves all subtasks for the specified parent task with completion tracking")
            .Produces<TaskSubtasksResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the PUT /api/v1/tasks/{taskId}/subtasks/{subtaskId} endpoint for updating a subtask.
    /// </summary>
    private static void MapUpdateSubtaskEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{taskId:guid}/subtasks/{subtaskId:guid}", UpdateSubtask)
            .WithName("UpdateSubtask")
            .WithSummary("Update subtask")
            .WithDescription("Updates a subtask's information while respecting domain constraints and parent-child relationships")
            .RequireAuthorization(Policies.ProjectMemberOrAdmin)
            .Accepts<UpdateSubtaskRequest>("application/json")
            .Produces<TaskResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Handles POST /api/v1/tasks/{id}/assign requests.
    /// </summary>
    private static async Task<IResult> AssignTask(
        Guid id,
        AssignTaskRequest request,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
                return Results.Unauthorized();

            var command = new AssignTaskCommand(
                new TaskId(id),
                new UserId(request.UserId),
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("AssignTask requires Infrastructure layer implementation");
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
                detail: "An error occurred while assigning the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles POST /api/v1/tasks/{id}/unassign requests.
    /// </summary>
    private static async Task<IResult> UnassignTask(
        Guid id,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
                return Results.Unauthorized();

            var command = new UnassignTaskCommand(
                new TaskId(id),
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UnassignTask requires Infrastructure layer implementation");
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
                detail: "An error occurred while unassigning the task",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles POST /api/v1/tasks/{id}/subtasks requests.
    /// </summary>
    private static async Task<IResult> CreateSubtask(
        Guid id,
        CreateSubtaskRequest request,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
                return Results.Unauthorized();

            var command = new CreateSubtaskCommand(
                new TaskId(id),
                request.Title,
                request.Description,
                request.Priority,
                request.EstimatedHours,
                request.DueDate,
                request.AssignedUserId.HasValue ? new UserId(request.AssignedUserId.Value) : null,
                currentUserId
            );


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("CreateSubtask requires Infrastructure layer implementation");
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
                detail: "An error occurred while creating the subtask",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles GET /api/v1/tasks/{id}/subtasks requests.
    /// </summary>
    private static async Task<IResult> GetSubtasks(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var taskId = new TaskId(id);
            var query = new GetTaskSubtasksQuery(taskId);


            // TODO: Replace with MediatR.Send when implemented
            // var response = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            var placeholderResponse = new TaskSubtasksResponse(
                ParentTaskId: id,
                ParentTaskTitle: "Sample Task",
                Subtasks: new List<SubtaskResponse>(),
                TotalSubtasks: 0,
                CompletedSubtasks: 0,
                CompletionPercentage: 0m
            );

            return Results.Ok(placeholderResponse);
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
                detail: "An error occurred while retrieving subtasks",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/tasks/{taskId}/subtasks/{subtaskId} requests.
    /// </summary>
    private static async Task<IResult> UpdateSubtask(
        Guid taskId,
        Guid subtaskId,
        UpdateSubtaskRequest request,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
                return Results.Unauthorized();

            var command = new UpdateSubtaskCommand(
                new TaskId(subtaskId),
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
            throw new NotImplementedException("UpdateSubtask requires Infrastructure layer implementation");
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
                detail: "An error occurred while updating the subtask",
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Internal Server Error");
        }
    }
}
