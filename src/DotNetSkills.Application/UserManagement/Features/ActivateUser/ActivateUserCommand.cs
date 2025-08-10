namespace DotNetSkills.Application.UserManagement.Features.ActivateUser;

/// <summary>
/// Command to activate a user account, changing their status from inactive to active.
/// </summary>
public record ActivateUserCommand(UserId UserId) : IRequest<UserResponse>;
