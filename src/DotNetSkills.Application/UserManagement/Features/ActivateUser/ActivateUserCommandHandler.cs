namespace DotNetSkills.Application.UserManagement.Features.ActivateUser;

/// <summary>
/// Handler for activating a user account.
/// TODO: Replace with actual implementation when Infrastructure layer is available.
/// </summary>
public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement user activation logic
        // 1. Get user from repository
        // 2. Call user.Activate() domain method
        // 3. Save changes
        // 4. Map to DTO and return

        await Task.CompletedTask;
        throw new NotImplementedException("ActivateUserCommandHandler requires Infrastructure layer implementation");
    }
}
