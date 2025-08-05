namespace DotNetSkills.Application.ProjectManagement.Queries;

/// <summary>
/// Query for retrieving a single project by its ID.
/// </summary>
public record GetProjectByIdQuery(ProjectId ProjectId) : IRequest<ProjectResponse?>
{
    /// <summary>
    /// Validates the query parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (ProjectId.Value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));
    }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectResponse?>
{
    public async Task<ProjectResponse?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement project retrieval logic
        // 1. Get project from repository by ID
        // 2. Return null if not found
        // 3. Map to DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("GetProjectByIdQueryHandler requires Infrastructure layer implementation");
    }
}
