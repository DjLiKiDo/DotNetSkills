namespace DotNetSkills.Application.UserManagement.Queries;

/// <summary>
/// Query to check if a user exists in the system by their unique identifier.
/// Returns true if the user exists, false otherwise.
/// Used by validators for cross-entity validation and authorization checks.
/// </summary>
/// <param name="UserId">The unique identifier of the user to check for existence.</param>
public record CheckUserExistsQuery(UserId UserId) : IRequest<Result<bool>>;