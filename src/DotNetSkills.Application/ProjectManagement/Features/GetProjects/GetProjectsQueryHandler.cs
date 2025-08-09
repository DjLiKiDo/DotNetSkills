namespace DotNetSkills.Application.ProjectManagement.Features.GetProjects;

/// <summary>
/// Handler for GetProjectsQuery that retrieves projects with pagination and filtering.
/// Implements CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, PagedProjectResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITeamRepository _teamRepository;

    public GetProjectsQueryHandler(
        IProjectRepository projectRepository,
        ITeamRepository teamRepository)
    {
        _projectRepository = projectRepository;
        _teamRepository = teamRepository;
    }

    public async Task<PagedProjectResponse> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var (projects, totalCount) = await _projectRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            request.TeamId,
            request.Status,
            cancellationToken).ConfigureAwait(false);

        var projectResponses = new List<ProjectResponse>();
        foreach (var project in projects)
        {
            var team = await _teamRepository.GetByIdAsync(project.TeamId, cancellationToken).ConfigureAwait(false);
            projectResponses.Add(ProjectResponse.FromDomain(project, team?.Name ?? "Unknown"));
        }

        return new PagedProjectResponse(
            projectResponses.AsReadOnly(),
            totalCount,
            request.Page,
            request.PageSize);
    }
}
