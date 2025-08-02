namespace DotNetSkills.Application.UserManagement.Commands;

public record DeactivateUserCommand(UserId UserId) : IRequest<UserResponse>;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement user deactivation logic
        // 1. Get user from repository
        // 2. Call user.Deactivate() domain method
        // 3. Save changes
        // 4. Map to DTO and return
        
        await Task.CompletedTask;
        throw new NotImplementedException("DeactivateUserCommandHandler requires Infrastructure layer implementation");
    }
}