namespace DotNetSkills.Application.TeamCollaboration.DTOs;

/// <summary>
/// Request DTO for adding a team member.
/// This DTO represents the data required to add a user to a team.
/// </summary>
public record AddTeamMemberRequest
{
    /// <summary>
    /// Gets or sets the ID of the user to add to the team.
    /// </summary>
    public required Guid UserId { get; init; }

    /// <summary>
    /// Gets or sets the role to assign to the user in the team.
    /// </summary>
    public required TeamRole Role { get; init; }

    /// <summary>
    /// Validates the add team member request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (UserId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(UserId));

        if (!Enum.IsDefined(typeof(TeamRole), Role))
            throw new ArgumentException("Invalid team role.", nameof(Role));
    }
}
