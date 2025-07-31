using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Services;

public interface IUserDomainService
{
    bool CanCreateUserWithEmail(EmailAddress email);
    ValidationResult ValidateUserCreation(CreateUserParameters parameters);
    ValidationResult ValidateUserUpdate(User user, UpdateUserParameters parameters);
    bool CanUserPerformAction(User user, UserAction action, User? targetUser = null);
}

public readonly record struct CreateUserParameters(
    string FirstName,
    string LastName,
    EmailAddress Email,
    UserRole Role,
    string Password,
    UserId CreatedBy);

public readonly record struct UpdateUserParameters(
    string FirstName,
    string LastName,
    EmailAddress Email);

public enum UserAction
{
    ManageUsers,
    ManageTeams,
    ManageProjects,
    ManageTasks,
    ViewReports,
    AssignTasks,
    UpdateProfile
}