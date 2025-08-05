namespace DotNetSkills.Domain.ProjectManagement.Events;

/// <summary>
/// Domain event raised when a project is created.
/// </summary>
/// <param name="ProjectId">The ID of the created project.</param>
/// <param name="Name">The name of the created project.</param>
/// <param name="TeamId">The ID of the team associated with the project.</param>
/// <param name="CreatedBy">The ID of the user who created the project.</param>
public record ProjectCreatedDomainEvent(
    ProjectId ProjectId,
    string Name,
    TeamId TeamId,
    UserId CreatedBy) : BaseDomainEvent;
