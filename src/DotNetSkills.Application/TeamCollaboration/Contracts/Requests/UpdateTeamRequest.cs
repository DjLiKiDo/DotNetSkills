namespace DotNetSkills.Application.TeamCollaboration.Contracts.Requests;

/// <summary>
/// Request DTO for updating an existing team.
/// This DTO represents the data required to update a team.
/// </summary>
public record UpdateTeamRequest
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
    /// Validates the update team request.
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
