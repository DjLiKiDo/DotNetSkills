using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Services;

public class UserDomainService : IUserDomainService
{
    private readonly IEmailValidationService _emailValidationService;
    private readonly IPasswordHashingService _passwordHashingService;

    public UserDomainService(
        IEmailValidationService emailValidationService,
        IPasswordHashingService passwordHashingService)
    {
        _emailValidationService = emailValidationService;
        _passwordHashingService = passwordHashingService;
    }

    public bool CanCreateUserWithEmail(EmailAddress email)
    {
        return !_emailValidationService.IsEmailInUse(email);
    }

    public ValidationResult ValidateUserCreation(CreateUserParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.FirstName))
            return ValidationResult.Invalid("First name is required.");

        if (string.IsNullOrWhiteSpace(parameters.LastName))
            return ValidationResult.Invalid("Last name is required.");

        if (string.IsNullOrWhiteSpace(parameters.Password))
            return ValidationResult.Invalid("Password is required.");

        if (_emailValidationService.IsEmailInUse(parameters.Email))
            return ValidationResult.Invalid("Email address is already in use.");

        return ValidationResult.Valid();
    }

    public ValidationResult ValidateUserUpdate(User user, UpdateUserParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.FirstName))
            return ValidationResult.Invalid("First name is required.");

        if (string.IsNullOrWhiteSpace(parameters.LastName))
            return ValidationResult.Invalid("Last name is required.");

        if (!user.Email.Equals(parameters.Email) && _emailValidationService.IsEmailInUse(parameters.Email))
            return ValidationResult.Invalid("Email address is already in use.");

        return ValidationResult.Valid();
    }

    public bool CanUserPerformAction(User user, UserAction action, User? targetUser = null)
    {
        if (!user.IsActive)
            return false;

        return action switch
        {
            UserAction.ManageUsers => user.Role.CanManageUsers() && 
                                    (targetUser == null || user.CanManageUser(targetUser)),
            UserAction.ManageTeams => user.Role.CanManageTeams(),
            UserAction.ManageProjects => user.Role.CanManageProjects(),
            UserAction.ManageTasks => user.Role.CanManageTasks(),
            UserAction.AssignTasks => user.Role.CanManageTasks(),
            UserAction.ViewReports => user.Role >= UserRole.ProjectManager,
            UserAction.UpdateProfile => true, // All users can update their own profile
            _ => false
        };
    }
}