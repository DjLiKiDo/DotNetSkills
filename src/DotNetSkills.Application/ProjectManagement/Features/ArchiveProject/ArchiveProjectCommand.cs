namespace DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;

/// <summary>
/// Command for archiving (soft deleting) a project.
/// </summary>
public record ArchiveProjectCommand(
    ProjectId ProjectId,
    UserId ArchivedBy) : IRequest;
