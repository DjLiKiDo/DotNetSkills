namespace DotNetSkills.Application.ProjectManagement.Features.CreateProject;

/// <summary>
/// Command for creating a new project.
/// </summary>
public record CreateProjectCommand(
    string Name,
    string? Description,
    TeamId TeamId,
    DateTime? PlannedEndDate,
    UserId CreatedBy) : IRequest<ProjectResponse>;
