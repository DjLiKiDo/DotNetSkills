using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Services;

public interface IEmailValidationService
{
    bool IsValidEmail(string email);
    EmailAddress CreateEmailAddress(string email);
    bool IsEmailInUse(EmailAddress email);
}