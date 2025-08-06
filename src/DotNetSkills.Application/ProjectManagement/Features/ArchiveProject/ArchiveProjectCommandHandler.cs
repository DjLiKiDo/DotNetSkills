namespace DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;

/// <summary>
/// Handler for ArchiveProjectCommand that orchestrates project archiving using domain methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
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
