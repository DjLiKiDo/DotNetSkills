using DotNetSkills.API.Authorization;

namespace DotNetSkills.API.Endpoints.TeamCollaboration;

/// <summary>
/// Team member management endpoints following Clean Architecture and DDD principles.
/// This class implements team member operations as part of the Team aggregate.
/// </summary>
public static class TeamMemberEndpoints
{
    /// <summary>
    /// Maps all team member management endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapTeamMemberEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/teams")
            .WithTags("Team Member Management")
            .RequireAuthorization();

        // Map individual endpoints
        MapGetTeamMembersEndpoint(group);
        MapAddTeamMemberEndpoint(group);
        MapRemoveTeamMemberEndpoint(group);
        MapUpdateMemberRoleEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/teams/{id}/members endpoint for listing team members.
    /// </summary>
    private static void MapGetTeamMembersEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}/members", GetTeamMembers)
            .WithName("GetTeamMembers")
            .WithSummary("Get team members")
            .WithDescription("Retrieves all members of a specific team with their roles and details")
            .Produces<TeamMembersResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/teams/{id}/members endpoint for adding team members.
    /// Enforces Team.MaxMembers = 50 constraint and prevents duplicate memberships.
    /// </summary>
    private static void MapAddTeamMemberEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/members", AddTeamMember)
            .WithName("AddTeamMember")
            .WithSummary("Add team member")
            .WithDescription("Adds a user to the team with the specified role. Enforces max members limit and prevents duplicates.")
            .RequireAuthorization(Policies.TeamManager)
            .Accepts<AddTeamMemberRequest>("application/json")
            .Produces<TeamMemberResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the DELETE /api/v1/teams/{teamId}/members/{userId} endpoint for removing team members.
    /// </summary>
    private static void MapRemoveTeamMemberEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("{teamId:guid}/members/{userId:guid}", RemoveTeamMember)
            .WithName("RemoveTeamMember")
            .WithSummary("Remove team member")
            .WithDescription("Removes a user from the team")
            .RequireAuthorization(Policies.TeamManager)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Maps the PUT /api/v1/teams/{teamId}/members/{userId}/role endpoint for updating member roles.
    /// </summary>
    private static void MapUpdateMemberRoleEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{teamId:guid}/members/{userId:guid}/role", UpdateMemberRole)
            .WithName("UpdateMemberRole")
            .WithSummary("Update member role")
            .WithDescription("Updates the role of a team member")
            .RequireAuthorization(Policies.TeamManager)
            .Accepts<UpdateMemberRoleRequest>("application/json")
            .Produces<TeamMemberResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Handles GET /api/v1/teams/{id}/members - retrieves all team members.
    /// </summary>
    private static async Task<IResult> GetTeamMembers(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var teamId))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid team ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate query
            var query = new GetTeamMembersQuery(new TeamId(teamId));

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("GetTeamMembers requires Application layer implementation");
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
                title: "An error occurred while retrieving team members",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles POST /api/v1/teams/{id}/members - adds a new team member.
    /// Enforces Team.MaxMembers constraint and prevents duplicate memberships.
    /// </summary>
    private static async Task<IResult> AddTeamMember(
        string id,
        AddTeamMemberRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var teamId))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid team ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate command
            var command = new AddTeamMemberCommand(
                new TeamId(teamId),
                new UserId(request.UserId),
                request.Role);

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("AddTeamMember requires Application layer implementation");
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
                title: "An error occurred while adding the team member",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles DELETE /api/v1/teams/{teamId}/members/{userId} - removes a team member.
    /// </summary>
    private static async Task<IResult> RemoveTeamMember(
        string teamId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(teamId, out var teamGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid team ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid user ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate command
            var command = new RemoveTeamMemberCommand(
                new TeamId(teamGuid),
                new UserId(userGuid));

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("RemoveTeamMember requires Application layer implementation");
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
                title: "An error occurred while removing the team member",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/teams/{teamId}/members/{userId}/role - updates a team member's role.
    /// </summary>
    private static async Task<IResult> UpdateMemberRole(
        string teamId,
        string userId,
        UpdateMemberRoleRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(teamId, out var teamGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid team ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid user ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate command
            var command = new UpdateMemberRoleCommand(
                new TeamId(teamGuid),
                new UserId(userGuid),
                request.Role);

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UpdateMemberRole requires Application layer implementation");
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
                title: "An error occurred while updating the member role",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
