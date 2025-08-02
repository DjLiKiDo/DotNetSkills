namespace DotNetSkills.Application.UserManagement.DTOs;

public record UpdateUserRoleRequest
{
    public UserRole Role { get; init; }
}