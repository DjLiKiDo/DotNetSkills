namespace DotNetSkills.Application.UserManagement.Queries;

/// <summary>
/// Query to get a user by their ID.
/// This query returns a single user or null if not found.
/// TODO: Implement IRequest&lt;UserResponse?&gt; when MediatR is added.
/// </summary>
public record GetUserByIdQuery(Guid UserId)
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
