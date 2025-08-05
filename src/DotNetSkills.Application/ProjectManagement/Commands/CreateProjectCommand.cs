namespace DotNetSkills.Application.ProjectManagement.Commands;

/// <summary>
/// Command for creating a new project.
/// </summary>
public record CreateProjectCommand(
    string Name,
    string? Description,
    TeamId TeamId,
    DateTime? PlannedEndDate,
    UserId CreatedBy) : IRequest<ProjectResponse>
{
    /// <summary>
    /// Validates the command parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Project name is required.", nameof(Name));

        if (Name.Length > 200)
            throw new ArgumentException("Project name cannot exceed 200 characters.", nameof(Name));

        if (Description?.Length > 1000)
            throw new ArgumentException("Project description cannot exceed 1000 characters.", nameof(Description));

        if (TeamId.Value == Guid.Empty)
            throw new ArgumentException("Team ID cannot be empty.", nameof(TeamId));

        if (CreatedBy.Value == Guid.Empty)
            throw new ArgumentException("Created by user ID cannot be empty.", nameof(CreatedBy));

        if (PlannedEndDate.HasValue && PlannedEndDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Planned end date must be in the future.", nameof(PlannedEndDate));
    }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement project creation logic
        // 1. Load user from repository to verify permissions
        // 2. Load team from repository to verify it exists
        // 3. Create project using domain factory method
        // 4. Save project to repository
        // 5. Map to DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("CreateProjectCommandHandler requires Infrastructure layer implementation");
    }
}
