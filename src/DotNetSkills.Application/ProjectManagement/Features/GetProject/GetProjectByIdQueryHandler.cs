namespace DotNetSkills.Application.ProjectManagement.Features.GetProject;

/// <summary>
/// Handler for GetProjectByIdQuery that retrieves a project by its ID.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectResponse?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;

    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        ITeamRepository teamRepository)
    {
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
    }

    public async Task<ProjectResponse?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken).ConfigureAwait(false);
        if (project == null)
            return null;

        var team = await _teamRepository.GetByIdAsync(project.TeamId, cancellationToken).ConfigureAwait(false);
        return ProjectResponse.FromDomain(project, team?.Name ?? "Unknown");
    }
}
