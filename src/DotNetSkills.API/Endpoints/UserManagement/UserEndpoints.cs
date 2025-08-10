using DotNetSkills.API.Authorization;
using DotNetSkills.Application.Common.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DotNetSkills.API.Endpoints.UserManagement;

/// <summary>
/// User management endpoints following Clean Architecture and DDD principles.
/// This class implements user CRUD operations with proper authorization and validation.
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// Maps all user management endpoints to the provided route builder.
    /// This method follows the bounded context organization pattern.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        // Create a route group for user endpoints with shared configuration
        var group = app.MapGroup("/api/v1/users")
            .WithTags("User Management")
            .WithOpenApi()
            .RequireAuthorization(); // All user endpoints require authentication

        // Map individual endpoint operations
        MapGetUsersEndpoint(group);
        MapGetUserByIdEndpoint(group);
        MapCreateUserEndpoint(group);
        MapUpdateUserEndpoint(group);
        MapDeleteUserEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/users endpoint for retrieving paginated user lists.
    /// Supports pagination and optional search functionality.
    /// </summary>
    private static void MapGetUsersEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("", GetUsers)
            .WithName("GetUsers")
            .WithSummary("Get paginated list of users")
            .WithDescription("Retrieves a paginated list of users with optional search filtering and enum-based filters. Query parameters: page (int), pageSize (int), search (string), role (UserRole), status (UserStatus). Enums are provided as strings, e.g., role=Admin, status=Active.")
            .Produces<PagedUserResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the GET /api/v1/users/{id} endpoint for retrieving a specific user.
    /// Returns user details or 404 if not found.
    /// </summary>
    private static void MapGetUserByIdEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a specific user by their unique identifier")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/users endpoint for creating new users.
    /// Requires Admin role as per domain business rules.
    /// </summary>
    private static void MapCreateUserEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .WithDescription("Creates a new user account - Admin only operation. Request body uses enum fields as strings for role/status when applicable.")
            .RequireAuthorization(Policies.AdminOnly)
            .Accepts<CreateUserRequest>("application/json")
            .Produces<UserResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the PUT /api/v1/users/{id} endpoint for updating existing users.
    /// Supports partial updates of user profile information.
    /// </summary>
    private static void MapUpdateUserEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithSummary("Update an existing user")
            .WithDescription("Updates user profile information. Request body uses enum fields as strings where applicable.")
            .Accepts<UpdateUserRequest>("application/json")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the DELETE /api/v1/users/{id} endpoint for soft-deleting users.
    /// Deactivates user account rather than hard deletion - Admin only operation.
    /// </summary>
    private static void MapDeleteUserEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithSummary("Delete (deactivate) a user")
            .WithDescription("Soft deletes a user by deactivating their account - Admin only operation")
            .RequireAuthorization(Policies.AdminOnly)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    #region Endpoint Handler Methods

    /// <summary>
    /// Handler for GET /api/v1/users endpoint.
    /// Returns paginated list of users with optional search filtering.
    /// </summary>
    /// <param name="mediator">MediatR instance for sending queries</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="currentUserService">Service for getting current user context</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
    /// <param name="search">Optional search term to filter users</param>
    /// <returns>Paginated user response with metadata</returns>
    private static async Task<IResult> GetUsers(
        IMediator mediator,
        [FromServices] ILogger<UserManagementLogCategory> logger,
        ICurrentUserService currentUserService,
        int page = 1,
        int pageSize = 20,
        string? search = null,
        UserRole? role = null,
        UserStatus? status = null)
    {
        try
        {
            // Create query
            var query = new GetUsersQuery(page, pageSize, search, role, status);

            // Send query through MediatR
            var result = await mediator.Send(query);

            return result.IsSuccess 
                ? Results.Ok(result.Value) 
                : Results.BadRequest(new ProblemDetails
                {
                    Title = "Query Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
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
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            logger.LogError(ex,
                "Unexpected error occurred while retrieving users. UserId: {UserId}, Route: {Route}, Page: {Page}, PageSize: {PageSize}, Search: {Search}, Role: {Role}, Status: {Status}",
                currentUserId, "/api/v1/users", page, pageSize, search, role, status);
                
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing the request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handler for GET /api/v1/users/{id} endpoint.
    /// Returns specific user details or 404 if not found.
    /// </summary>
    /// <param name="mediator">MediatR instance for sending queries</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="currentUserService">Service for getting current user context</param>
    /// <param name="id">User unique identifier</param>
    /// <returns>User details or 404 if not found</returns>
    private static async Task<IResult> GetUserById(IMediator mediator, [FromServices] ILogger<UserManagementLogCategory> logger, ICurrentUserService currentUserService, Guid id)
    {
        try
        {
            // Create query
            var query = new GetUserByIdQuery(new UserId(id));

            // Send query through MediatR
            var result = await mediator.Send(query);

            if (!result.IsSuccess)
            {
                return Results.NotFound(new ProblemDetails
                {
                    Title = "User Not Found",
                    Detail = result.Error,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Results.Ok(result.Value);
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
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            logger.LogError(ex,
                "Unexpected error occurred while retrieving user by ID. UserId: {UserId}, Route: {Route}, RequestedUserId: {RequestedUserId}",
                currentUserId, $"/api/v1/users/{id}", id);
                
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing the request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handler for POST /api/v1/users endpoint.
    /// Creates a new user account - Admin only operation.
    /// </summary>
    /// <param name="mediator">MediatR instance for sending commands</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="currentUserService">Service for getting current user context</param>
    /// <param name="request">User creation request data</param>
    /// <returns>Created user response with 201 status</returns>
    private static async Task<IResult> CreateUser(IMediator mediator, [FromServices] ILogger<UserManagementLogCategory> logger, ICurrentUserService currentUserService, CreateUserRequest request)
    {
        try
        {
            // Create command
            var command = request.ToCommand();

            // Send command through MediatR
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "User Creation Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Results.Created($"/api/v1/users/{result.Value!.Id}", result.Value);
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
        catch (DomainException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            logger.LogError(ex,
                "Unexpected error occurred while creating user. UserId: {UserId}, Route: {Route}, RequestedUserName: {RequestedUserName}, RequestedUserEmail: {RequestedUserEmail}",
                currentUserId, "/api/v1/users", request.Name, request.Email);
                
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing the request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handler for PUT /api/v1/users/{id} endpoint.
    /// Updates existing user profile information.
    /// </summary>
    /// <param name="mediator">MediatR instance for sending commands</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="currentUserService">Service for getting current user context</param>
    /// <param name="id">User unique identifier</param>
    /// <param name="request">User update request data</param>
    /// <returns>Updated user response</returns>
    private static async Task<IResult> UpdateUser(IMediator mediator, [FromServices] ILogger<UserManagementLogCategory> logger, ICurrentUserService currentUserService, Guid id, UpdateUserRequest request)
    {
        try
        {
            // Create command
            var command = request.ToCommand(new UserId(id));

            // Send command through MediatR
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "User Update Failed",
                    Detail = result.Error,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            return Results.Ok(result.Value);
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
        catch (DomainException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            logger.LogError(ex,
                "Unexpected error occurred while updating user. UserId: {UserId}, Route: {Route}, TargetUserId: {TargetUserId}, RequestedUserName: {RequestedUserName}",
                currentUserId, $"/api/v1/users/{id}", id, request.Name);
                
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing the request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handler for DELETE /api/v1/users/{id} endpoint.
    /// Soft deletes a user by deactivating their account - Admin only operation.
    /// </summary>
    /// <param name="mediator">MediatR instance for sending commands</param>
    /// <param name="logger">Logger for structured logging</param>
    /// <param name="currentUserService">Service for getting current user context</param>
    /// <param name="id">User unique identifier</param>
    /// <returns>204 No Content on successful deletion</returns>
    private static async Task<IResult> DeleteUser(IMediator mediator, [FromServices] ILogger<UserManagementLogCategory> logger, ICurrentUserService currentUserService, Guid id)
    {
        try
        {
            // Create and validate command
            var command = new DeleteUserCommand(new UserId(id));

            // Send command through MediatR
            var result = await mediator.Send(command);

            // Since DeleteUserCommand returns UserResponse directly, not Result<UserResponse>
            // we treat successful execution as success
            return Results.NoContent();
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
        catch (DomainException ex)
        {
            return Results.BadRequest(new ProblemDetails
            {
                Title = "Business Rule Violation",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (Exception ex)
        {
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            logger.LogError(ex,
                "Unexpected error occurred while deleting user. UserId: {UserId}, Route: {Route}, TargetUserId: {TargetUserId}",
                currentUserId, $"/api/v1/users/{id}", id);
                
            return Results.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while processing the request",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    #endregion
}

/// <summary>
/// Marker type for user management endpoint logging category.
/// </summary>
internal sealed class UserManagementLogCategory { }
