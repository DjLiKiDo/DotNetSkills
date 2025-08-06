namespace DotNetSkills.Application.ProjectManagement.Features.CreateProject;

/// <summary>
/// Handler for CreateProjectCommand that orchestrates project creation using domain factory methods.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
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
