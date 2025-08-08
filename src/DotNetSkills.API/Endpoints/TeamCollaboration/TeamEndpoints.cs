using DotNetSkills.API.Authorization;

namespace DotNetSkills.API.Endpoints.TeamCollaboration;

/// <summary>
/// Team management endpoints following Clean Architecture and DDD principles.
/// This class implements team CRUD operations with proper authorization and validation.
/// </summary>
public static class TeamEndpoints
{
    /// <summary>
    /// Maps all team management endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/teams")
            .WithTags("Team Management")
            .RequireAuthorization();

        // Map individual endpoints
        MapGetTeamsEndpoint(group);
        MapGetTeamByIdEndpoint(group);
        MapCreateTeamEndpoint(group);
        MapUpdateTeamEndpoint(group);
        MapDeleteTeamEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/teams endpoint for listing teams with pagination.
    /// </summary>
    private static void MapGetTeamsEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("", GetTeams)
            .WithName("GetTeams")
            .WithSummary("Get teams with pagination")
            .WithDescription("Retrieves a paginated list of teams with optional search functionality. Query parameters: page (int), pageSize (int), search (string). Enum filters (e.g., TeamStatus, TeamRole) are provided as strings when available in future endpoints.")
            .Produces<PagedTeamResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    /// <summary>
    /// Maps the GET /api/v1/teams/{id} endpoint for retrieving a specific team.
    /// </summary>
    private static void MapGetTeamByIdEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}", GetTeamById)
            .WithName("GetTeamById")
            .WithSummary("Get team by ID")
            .WithDescription("Retrieves a specific team by their unique identifier with member details")
            .Produces<TeamResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/teams endpoint for creating new teams.
    /// Requires ProjectManager or Admin role as per domain business rules.
    /// </summary>
    private static void MapCreateTeamEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("", CreateTeam)
            .WithName("CreateTeam")
            .WithSummary("Create a new team")
            .WithDescription("Creates a new team - ProjectManager or Admin only operation")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Accepts<CreateTeamRequest>("application/json")
            .Produces<TeamResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the PUT /api/v1/teams/{id} endpoint for updating teams.
    /// </summary>
    private static void MapUpdateTeamEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{id:guid}", UpdateTeam)
            .WithName("UpdateTeam")
            .WithSummary("Update team")
            .WithDescription("Updates an existing team's information")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Accepts<UpdateTeamRequest>("application/json")
            .Produces<TeamResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the DELETE /api/v1/teams/{id} endpoint for deleting teams.
    /// </summary>
    private static void MapDeleteTeamEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("{id:guid}", DeleteTeam)
            .WithName("DeleteTeam")
            .WithSummary("Delete team")
            .WithDescription("Deletes an existing team - ProjectManager or Admin only operation")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Handles GET /api/v1/teams - retrieves teams with pagination and optional search.
    /// </summary>
    private static async Task<IResult> GetTeams(
        int page = 1,
        int pageSize = 20,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create query - validation will be handled by FluentValidation in the pipeline
            var query = new GetTeamsQuery(page, pageSize, search);

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            var placeholderResponse = new PagedTeamResponse(
                Teams: new List<TeamResponse>(),
                TotalCount: 0,
                Page: page,
                PageSize: pageSize);

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
                title: "An error occurred while retrieving teams",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles GET /api/v1/teams/{id} - retrieves a specific team by ID with member details.
    /// </summary>
    private static async Task<IResult> GetTeamById(
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
            var query = new GetTeamByIdQuery(new TeamId(teamId));

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(query, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("GetTeamById requires Application layer implementation");
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
                title: "An error occurred while retrieving the team",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles POST /api/v1/teams - creates a new team.
    /// </summary>
    private static async Task<IResult> CreateTeam(
        CreateTeamRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create and validate command
            var command = new CreateTeamCommand(request.Name, request.Description);

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("CreateTeam requires Application layer implementation");
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
                title: "An error occurred while creating the team",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/teams/{id} - updates an existing team.
    /// </summary>
    private static async Task<IResult> UpdateTeam(
        string id,
        UpdateTeamRequest request,
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
            var command = new UpdateTeamCommand(new TeamId(teamId), request.Name, request.Description);

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("UpdateTeam requires Application layer implementation");
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
                title: "An error occurred while updating the team",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles DELETE /api/v1/teams/{id} - deletes an existing team.
    /// </summary>
    private static async Task<IResult> DeleteTeam(
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

            // Create and validate command
            var command = new DeleteTeamCommand(new TeamId(teamId));

            // TODO: Replace with MediatR.Send when implemented
            // var result = await mediator.Send(command, cancellationToken);

            // Placeholder response - TODO: Replace with actual implementation
            await Task.CompletedTask;
            throw new NotImplementedException("DeleteTeam requires Application layer implementation");
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
                title: "An error occurred while deleting the team",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
