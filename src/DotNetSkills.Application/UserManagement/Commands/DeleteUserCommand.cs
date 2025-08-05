namespace DotNetSkills.Application.UserManagement.Commands;

/// <summary>
/// Command to delete (soft delete) a user from the system.
/// This command represents a request to deactivate a user account.
/// TODO: Implement IRequest&lt;bool&gt; when MediatR is added.
/// </summary>
public record DeleteUserCommand(Guid UserId)
{
    /// <summary>
    /// Validates that required properties have values.
    /// </summary>
    public void Validate()
    {
        if (UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(UserId));
    }
}
