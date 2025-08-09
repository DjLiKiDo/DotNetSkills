namespace DotNetSkills.Application.ProjectManagement.Contracts.Requests;

/// <summary>
/// Request DTO for updating an existing project.
/// This DTO represents the data required to update a project.
/// </summary>
public record UpdateProjectRequest
{
    /// <summary>
    /// Gets or sets the project name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the project description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the planned completion date (optional).
    /// </summary>
    public DateTime? PlannedEndDate { get; init; }

    /// <summary>
    /// Validates the update project request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Project name is required.", nameof(Name));

        if (Name.Length > 200)
            throw new ArgumentException("Project name cannot exceed 200 characters.", nameof(Name));

        if (Description?.Length > 1000)
            throw new ArgumentException("Project description cannot exceed 1000 characters.", nameof(Description));

        if (PlannedEndDate.HasValue && PlannedEndDate.Value <= DateTime.UtcNow)
            throw new ArgumentException("Planned end date must be in the future.", nameof(PlannedEndDate));
    }
}
