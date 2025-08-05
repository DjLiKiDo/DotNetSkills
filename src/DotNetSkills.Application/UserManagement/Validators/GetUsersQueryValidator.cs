namespace DotNetSkills.Application.UserManagement.Validators;

/// <summary>
/// FluentValidation validator for GetUsersQuery.
/// Provides validation for pagination parameters and search criteria with performance optimization.
/// </summary>
public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        ConfigureValidationRules();
    }

    private void ConfigureValidationRules()
    {
        // Page validation - must be positive
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage(string.Format(ValidationMessages.Common.MustBePositive, "Page number"));

        // PageSize validation - must be within allowed range (1-100)
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(string.Format(ValidationMessages.Common.MustBeInRange, "Page size", 1, 100));

        // SearchTerm validation - if provided, must not be whitespace only and reasonable length
        When(x => !string.IsNullOrEmpty(x.SearchTerm), () =>
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty()
                .WithMessage("Search term cannot be whitespace only")
                .Length(1, 100)
                .WithMessage(string.Format(ValidationMessages.Common.MustBeInRange, "Search term length", 1, 100));
        });

        // Role validation - if provided, must be valid enum value
        When(x => x.Role.HasValue, () =>
        {
            RuleFor(x => x.Role!.Value)
                .IsInEnum()
                .WithMessage("Invalid user role specified");
        });

        // Status validation - if provided, must be valid enum value  
        When(x => x.Status.HasValue, () =>
        {
            RuleFor(x => x.Status!.Value)
                .IsInEnum()
                .WithMessage("Invalid user status specified");
        });
    }
}