namespace DotNetSkills.Application.ProjectManagement.Commands;

/// <summary>
/// Command for updating an existing project.
/// </summary>
public record UpdateProjectCommand(
    ProjectId ProjectId,
    string Name,
    string? Description,
    DateTime? PlannedEndDate,
    UserId UpdatedBy) : IRequest<ProjectResponse>
{
    /// <summary>
    /// Validates the command parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (ProjectId.Value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));

        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Project name is required.", nameof(Name));

        if (Name.Length > 200)
            throw new ArgumentException("Project name cannot exceed 200 characters.", nameof(Name));

        if (Description?.Length > 1000)
            throw new ArgumentException("Project description cannot exceed 1000 characters.", nameof(Description));

        if (UpdatedBy.Value == Guid.Empty)
            throw new ArgumentException("Updated by user ID cannot be empty.", nameof(UpdatedBy));

        if (PlannedEndDate.HasValue && PlannedEndDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Planned end date must be in the future.", nameof(PlannedEndDate));
    }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement project update logic
        // 1. Load project from repository
        // 2. Load user from repository to verify permissions
        // 3. Update project using domain methods
        // 4. Save project to repository
        // 5. Map to DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("UpdateProjectCommandHandler requires Infrastructure layer implementation");
    }
}
