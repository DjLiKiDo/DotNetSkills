namespace DotNetSkills.Application.UserManagement.Features.DeactivateUser;

/// <summary>
/// Command for deactivating a user account (soft delete).
/// This command implements the CQRS pattern with MediatR and follows Clean Architecture principles.
/// </summary>
/// <param name="UserId">The ID of the user to deactivate.</param>
/// <param name="DeactivatedById">The ID of the user performing the deactivation (must be an admin).</param>
public record DeactivateUserCommand(
    UserId UserId,
    UserId DeactivatedById) : IRequest<Result<UserResponse>>;
