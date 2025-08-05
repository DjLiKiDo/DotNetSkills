namespace DotNetSkills.Application.TeamCollaboration.DTOs;

/// <summary>
/// Request DTO for updating a team member's role.
/// This DTO represents the data required to change a team member's role.
/// </summary>
public record UpdateMemberRoleRequest
{
    /// <summary>
    /// Gets or sets the new role to assign to the team member.
    /// </summary>
    public required TeamRole Role { get; init; }

    /// <summary>
    /// Validates the update member role request.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public void Validate()
    {
        if (!Enum.IsDefined(typeof(TeamRole), Role))
            throw new ArgumentException("Invalid team role.", nameof(Role));
    }
}
