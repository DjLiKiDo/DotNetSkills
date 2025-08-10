namespace DotNetSkills.Application.UserManagement.Features.GetUser;

/// <summary>
/// FluentValidation validator for GetUserByIdQuery.
/// Provides validation for user ID format and ensures proper strongly-typed ID validation.
/// </summary>
public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
{
    public GetUserByIdQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        // UserId validation - ensure the underlying Guid is not empty
        RuleFor(x => x.UserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage(string.Format(ValidationMessages.ValueObjects.EmptyGuid, "User ID"));
    }
}