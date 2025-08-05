namespace DotNetSkills.Application.UserManagement.Commands;

/// <summary>
/// Command to create a new user in the system.
/// This command represents a request to create a user account using CQRS pattern with MediatR.
/// </summary>
public record CreateUserCommand(
    string Name,
    string Email,
    string Role,
    UserId? CreatedById = null) : IRequest<Result<UserResponse>>;
