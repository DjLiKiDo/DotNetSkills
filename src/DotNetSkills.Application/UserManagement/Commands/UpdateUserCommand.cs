namespace DotNetSkills.Application.UserManagement.Commands;

/// <summary>
/// Command to update an existing user in the system.
/// This command represents a request to update user profile information.
/// TODO: Implement IRequest&lt;UserResponse&gt; when MediatR is added.
/// </summary>
public record UpdateUserCommand(
    Guid UserId,
    string Name,
    string Email)
{
    /// <summary>
    /// Validates that required properties have values.
    /// TODO: Implement proper FluentValidation when validator is created.
    /// </summary>
    public void Validate()
    {
        if (UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(UserId));

        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required", nameof(Name));

        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("Email is required", nameof(Email));
    }
}
