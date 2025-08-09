namespace DotNetSkills.Application.ProjectManagement.Features.GetProject;

/// <summary>
/// Query for retrieving a single project by its ID.
/// </summary>
public record GetProjectByIdQuery(ProjectId ProjectId) : IRequest<ProjectResponse?>;
