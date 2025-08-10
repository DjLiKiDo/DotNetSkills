namespace DotNetSkills.Application.ProjectManagement.Contracts.Responses;

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

}
