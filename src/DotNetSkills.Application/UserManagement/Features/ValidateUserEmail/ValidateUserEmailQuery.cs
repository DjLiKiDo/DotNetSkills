namespace DotNetSkills.Application.UserManagement.Features.ValidateUserEmail;

/// <summary>
/// Query to validate if an email address is unique in the system.
/// Returns false if the email is already taken by another user.
/// Used by validators for cross-entity validation during user creation and updates.
/// </summary>
/// <param name="Email">The email address to validate for uniqueness.</param>
/// <param name="ExcludeUserId">Optional user ID to exclude from the uniqueness check (for updates).</param>
public record ValidateUserEmailQuery(string Email, UserId? ExcludeUserId = null) : IRequest<Result<bool>>;