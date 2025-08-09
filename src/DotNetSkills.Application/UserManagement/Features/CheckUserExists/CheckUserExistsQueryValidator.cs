namespace DotNetSkills.Application.UserManagement.Features.CheckUserExists;

/// <summary>
/// Validator for CheckUserExistsQuery.
/// </summary>
public class CheckUserExistsQueryValidator : AbstractValidator<CheckUserExistsQuery>
{
    public CheckUserExistsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.");

        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UserId cannot be empty GUID.");
    }
}
