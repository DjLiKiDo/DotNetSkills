namespace DotNetSkills.Application.TeamCollaboration.DTOs;

/// <summary>
/// Request DTO for creating a new team.
/// This DTO represents the data required to create a new team.
/// </summary>
public record CreateTeamRequest
{
    /// <summary>
    /// Gets or sets the team name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the team description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Validates the create team request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Team name is required.", nameof(Name));

        if (Name.Length > 100)
            throw new ArgumentException("Team name cannot exceed 100 characters.", nameof(Name));

        if (Description?.Length > 500)
            throw new ArgumentException("Team description cannot exceed 500 characters.", nameof(Description));
    }
}
