using DotNetSkills.Application.Common.Abstractions;
using DotNetSkills.Application.UserManagement.Features.ActivateUser;
using DotNetSkills.Application.UserManagement.Features.DeactivateUser;
using DotNetSkills.Application.UserManagement.Features.UpdateUserRole;
using DotNetSkills.Application.UserManagement.Features.GetUserTeamMemberships;
using MediatR;

namespace DotNetSkills.API.Endpoints.UserManagement;

public static class UserAccountEndpoints
{
    public static void MapUserAccountEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/users")
            .WithTags("User Account Management")
            .RequireAuthorization();

        // POST /api/v1/users/{id}/activate
        group.MapPost("/{id}/activate", ActivateUserAsync)
            .WithName("ActivateUser")
            .WithSummary("Activate a user account")
            .WithDescription("Activates a deactivated user account. Requires Admin privileges or self-modification rights.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // POST /api/v1/users/{id}/deactivate
        group.MapPost("/{id}/deactivate", DeactivateUserAsync)
            .WithName("DeactivateUser")
            .WithSummary("Deactivate a user account")
            .WithDescription("Deactivates an active user account. Requires Admin privileges or self-modification rights.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // PUT /api/v1/users/{id}/role
        group.MapPut("/{id}/role", UpdateUserRoleAsync)
            .WithName("UpdateUserRole")
            .WithSummary("Update user role")
            .WithDescription("Updates a user's role. Requires Admin privileges only.")
            .Produces<UserResponse>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // GET /api/v1/users/{id}/teams
        group.MapGet("/{id}/teams", GetUserTeamMembershipsAsync)
            .WithName("GetUserTeamMemberships")
            .WithSummary("Get user team memberships")
            .WithDescription("Retrieves all team memberships for a specific user. Requires Admin privileges or self-access rights.")
            .Produces<TeamMembershipListDto>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> ActivateUserAsync(
        IMediator mediator,
        ICurrentUserService currentUserService,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return Results.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["id"] = ["Invalid user ID format."]
                }));
            }

            var command = new ActivateUserCommand(new UserId(userId));
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
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            return Results.Problem(
                title: "An error occurred while activating the user",
                detail: $"{ex.Message} (UserId: {currentUserId})",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> DeactivateUserAsync(
        IMediator mediator,
        ICurrentUserService currentUserService,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return Results.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["id"] = ["Invalid user ID format."]
                }));
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId is null)
            {
                return Results.Unauthorized();
            }

            var command = new DeactivateUserCommand(new UserId(userId), currentUserId);
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
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            return Results.Problem(
                title: "An error occurred while deactivating the user",
                detail: $"{ex.Message} (UserId: {currentUserId})",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateUserRoleAsync(
        IMediator mediator,
        ICurrentUserService currentUserService,
        string id,
        UpdateUserRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return Results.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["id"] = ["Invalid user ID format."]
                }));
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId is null)
            {
                return Results.Unauthorized();
            }

            var command = new UpdateUserRoleCommand(new UserId(userId), request.Role, currentUserId);
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
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            return Results.Problem(
                title: "An error occurred while updating the user role",
                detail: $"{ex.Message} (UserId: {currentUserId})",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetUserTeamMembershipsAsync(
        IMediator mediator,
        ICurrentUserService currentUserService,
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var userId))
            {
                return Results.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    ["id"] = ["Invalid user ID format."]
                }));
            }

            var query = new GetUserTeamMembershipsQuery(new UserId(userId));
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
        catch (Exception ex)
        {
            var currentUserId = currentUserService.GetCurrentUserId()?.Value.ToString() ?? "anonymous";
            return Results.Problem(
                title: "An error occurred while retrieving user team memberships",
                detail: $"{ex.Message} (UserId: {currentUserId})",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
