namespace DotNetSkills.Application.ProjectManagement.DTOs;

/// <summary>
/// Response DTO for project operations.
/// This DTO represents project data returned from API endpoints.
/// </summary>
public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    Guid TeamId,
    string TeamName,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime? PlannedEndDate,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    /// <summary>
    /// Gets whether the project is overdue.
    /// </summary>
    public bool IsOverdue => PlannedEndDate.HasValue &&
                            PlannedEndDate.Value < DateTime.UtcNow &&
                            Status != "Completed" &&
                            Status != "Cancelled";

    /// <summary>
    /// Gets whether the project is active.
    /// </summary>
    public bool IsActive => Status == "Active";

    /// <summary>
    /// Creates a ProjectResponse from domain entity.
    /// TODO: Replace with AutoMapper when properly configured.
    /// </summary>
    public static ProjectResponse FromDomain(Project project, string teamName = "Unknown")
    {
        return new ProjectResponse(
            project.Id.Value,
            project.Name,
            project.Description,
            project.Status.ToString(),
            project.TeamId.Value,
            teamName, // TODO: Load team name from repository or include in query
            project.StartDate,
            project.EndDate,
            project.PlannedEndDate,
            project.CreatedAt,
            project.UpdatedAt);
    }
}
