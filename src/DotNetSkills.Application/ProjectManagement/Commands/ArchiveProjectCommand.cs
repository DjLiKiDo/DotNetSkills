namespace DotNetSkills.Application.ProjectManagement.Commands;

/// <summary>
/// Command for archiving (soft deleting) a project.
/// </summary>
public record ArchiveProjectCommand(
    ProjectId ProjectId,
    UserId ArchivedBy) : IRequest
{
    /// <summary>
    /// Validates the command parameters.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (ProjectId.Value == Guid.Empty)
            throw new ArgumentException("Project ID cannot be empty.", nameof(ProjectId));

        if (ArchivedBy.Value == Guid.Empty)
            throw new ArgumentException("Archived by user ID cannot be empty.", nameof(ArchivedBy));
    }
}

public class ArchiveProjectCommandHandler : IRequestHandler<ArchiveProjectCommand>
{
    public async Task Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement project archiving logic
        // 1. Load project from repository
        // 2. Load user from repository to verify permissions
        // 3. Archive project (soft delete) using domain methods
        // 4. Save project to repository
        
        await Task.CompletedTask;
        throw new NotImplementedException("ArchiveProjectCommandHandler requires Infrastructure layer implementation");
    }
}