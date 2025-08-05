namespace DotNetSkills.Application.UserManagement.Commands;

/// <summary>
/// Command to create a new user in the system.
/// This command represents a request to create a user account.
/// TODO: Implement IRequest&lt;UserResponse&gt; when MediatR is added.
/// </summary>
public record CreateUserCommand(
    string Name,
    string Email,
    string Role)
{
    /// <summary>
    /// Validates that required properties have values.
    /// TODO: Implement proper FluentValidation when validator is created.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Name is required", nameof(Name));

        if (string.IsNullOrWhiteSpace(Email))
            throw new ArgumentException("Email is required", nameof(Email));

        if (string.IsNullOrWhiteSpace(Role))
            throw new ArgumentException("Role is required", nameof(Role));
    }
}
