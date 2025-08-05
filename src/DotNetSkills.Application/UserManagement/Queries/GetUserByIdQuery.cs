namespace DotNetSkills.Application.UserManagement.Queries;

/// <summary>
/// Query to retrieve a user by their unique identifier.
/// Returns null if the user is not found, wrapped in a Result pattern.
/// </summary>
/// <param name="UserId">The unique identifier of the user to retrieve.</param>
public record GetUserByIdQuery(UserId UserId) : IRequest<Result<UserResponse?>>;
