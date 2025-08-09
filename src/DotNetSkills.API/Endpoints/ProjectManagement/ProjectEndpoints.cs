using DotNetSkills.API.Authorization;
using DotNetSkills.Application.Common.Abstractions;
using MediatR;

namespace DotNetSkills.API.Endpoints.ProjectManagement;

/// <summary>
/// Project management endpoints following Clean Architecture and DDD principles.
/// This class implements project CRUD operations with proper authorization and validation.
/// Projects belong to exactly one team and support filtering by various criteria.
/// </summary>
public static class ProjectEndpoints
{
    /// <summary>
    /// Maps all project management endpoints to the provided route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    public static void MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/projects")
            .WithTags("Project Management")
            .RequireAuthorization();

        // Map individual endpoints
        MapGetProjectsEndpoint(group);
        MapGetProjectByIdEndpoint(group);
        MapCreateProjectEndpoint(group);
        MapUpdateProjectEndpoint(group);
        MapArchiveProjectEndpoint(group);
    }

    /// <summary>
    /// Maps the GET /api/v1/projects endpoint for listing projects with filtering.
    /// Supports filtering by team, status, and date ranges.
    /// </summary>
    private static void MapGetProjectsEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("", GetProjects)
            .WithName("GetProjects")
            .WithSummary("Get projects with filtering")
            .WithDescription("Retrieves a paginated list of projects with optional filtering by team, status, and date ranges. Query parameters: page (int), pageSize (int), teamId (Guid), status (ProjectStatus), startDateFrom (date), startDateTo (date), endDateFrom (date), endDateTo (date), search (string). Enums are provided as strings, e.g., status=Active.")
            .Produces<PagedProjectResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    /// <summary>
    /// Maps the GET /api/v1/projects/{id} endpoint for retrieving a specific project.
    /// </summary>
    private static void MapGetProjectByIdEndpoint(RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}", GetProjectById)
            .WithName("GetProjectById")
            .WithSummary("Get project by ID")
            .WithDescription("Retrieves a specific project by their unique identifier with detailed information")
            .Produces<ProjectResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Maps the POST /api/v1/projects endpoint for creating new projects.
    /// Projects belong to exactly one team and require proper authorization.
    /// </summary>
    private static void MapCreateProjectEndpoint(RouteGroupBuilder group)
    {
        group.MapPost("", CreateProject)
            .WithName("CreateProject")
            .WithSummary("Create a new project")
            .WithDescription("Creates a new project associated with a team - ProjectManager or Admin only operation")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Accepts<CreateProjectRequest>("application/json")
            .Produces<ProjectResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the PUT /api/v1/projects/{id} endpoint for updating projects.
    /// </summary>
    private static void MapUpdateProjectEndpoint(RouteGroupBuilder group)
    {
        group.MapPut("{id:guid}", UpdateProject)
            .WithName("UpdateProject")
            .WithSummary("Update project")
            .WithDescription("Updates an existing project's information")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Accepts<UpdateProjectRequest>("application/json")
            .Produces<ProjectResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .ProducesValidationProblem();
    }

    /// <summary>
    /// Maps the DELETE /api/v1/projects/{id} endpoint for archiving projects.
    /// Uses soft delete pattern - projects are marked as archived rather than deleted.
    /// </summary>
    private static void MapArchiveProjectEndpoint(RouteGroupBuilder group)
    {
        group.MapDelete("{id:guid}", ArchiveProject)
            .WithName("ArchiveProject")
            .WithSummary("Archive project")
            .WithDescription("Archives an existing project (soft delete) - ProjectManager or Admin only operation")
            .RequireAuthorization(Policies.ProjectManagerOrAdmin)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized)
            .Produces<ProblemDetails>(StatusCodes.Status403Forbidden)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    /// <summary>
    /// Handles GET /api/v1/projects - retrieves projects with filtering and pagination.
    /// Supports filtering by team, status, and date ranges.
    /// </summary>
    private static async Task<IResult> GetProjects(
        IMediator mediator,
        int page = 1,
        int pageSize = 20,
        Guid? teamId = null,
        ProjectStatus? status = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        DateTime? endDateFrom = null,
        DateTime? endDateTo = null,
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create and validate query
            var query = new GetProjectsQuery(
                page,
                pageSize,
                teamId.HasValue ? new TeamId(teamId.Value) : null,
                status,
                startDateFrom,
                startDateTo,
                endDateFrom,
                endDateTo,
                search);

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
            return Results.Problem(
                title: "An error occurred while retrieving projects",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles GET /api/v1/projects/{id} - retrieves a specific project by ID.
    /// </summary>
    private static async Task<IResult> GetProjectById(
        string id,
        IMediator mediator,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var projectId))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            // Create and validate query
            var query = new GetProjectByIdQuery(new ProjectId(projectId));

            var result = await mediator.Send(query, cancellationToken);
            
            if (result == null)
            {
                return Results.NotFound(new ProblemDetails
                {
                    Title = "Project Not Found",
                    Detail = $"Project with ID {projectId} was not found.",
                    Status = StatusCodes.Status404NotFound
                });
            }

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
                title: "An error occurred while retrieving the project",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles POST /api/v1/projects - creates a new project.
    /// Projects belong to exactly one team as per business rules.
    /// </summary>
    private static async Task<IResult> CreateProject(
        CreateProjectRequest request,
        IMediator mediator,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Results.Unauthorized();
            }

            // Create and validate command
            var command = new CreateProjectCommand(
                request.Name,
                request.Description,
                new TeamId(request.TeamId),
                request.PlannedEndDate,
                currentUserId);

            var result = await mediator.Send(command, cancellationToken);

            return Results.Created($"/api/v1/projects/{result.Id}", result);
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
                title: "An error occurred while creating the project",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles PUT /api/v1/projects/{id} - updates an existing project.
    /// </summary>
    private static async Task<IResult> UpdateProject(
        string id,
        UpdateProjectRequest request,
        IMediator mediator,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var projectId))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Results.Unauthorized();
            }

            // Create and validate command
            var command = new UpdateProjectCommand(
                new ProjectId(projectId),
                request.Name,
                request.Description,
                request.PlannedEndDate,
                currentUserId);

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
                title: "An error occurred while updating the project",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Handles DELETE /api/v1/projects/{id} - archives an existing project.
    /// Uses soft delete pattern for data preservation.
    /// </summary>
    private static async Task<IResult> ArchiveProject(
        string id,
        IMediator mediator,
        ICurrentUserService currentUserService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Guid.TryParse(id, out var projectId))
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Invalid Request",
                    Detail = "Invalid project ID format.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var currentUserId = currentUserService.GetCurrentUserId();
            if (currentUserId == null)
            {
                return Results.Unauthorized();
            }

            // Create and validate command
            var command = new ArchiveProjectCommand(new ProjectId(projectId), currentUserId);

            await mediator.Send(command, cancellationToken);

            return Results.NoContent();
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
                title: "An error occurred while archiving the project",
                detail: ex.Message,
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
