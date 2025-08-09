namespace DotNetSkills.Application.ProjectManagement.Features.UpdateProject;

/// <summary>
/// Command for updating an existing project.
/// </summary>
public record UpdateProjectCommand(
    ProjectId ProjectId,
    string Name,
    string? Description,
    DateTime? PlannedEndDate,
    UserId UpdatedBy) : IRequest<ProjectResponse>;
