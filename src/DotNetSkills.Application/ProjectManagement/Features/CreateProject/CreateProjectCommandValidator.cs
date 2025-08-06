namespace DotNetSkills.Application.ProjectManagement.Features.CreateProject;

/// <summary>
/// Validator for CreateProjectCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required.")
            .MaximumLength(200)
            .WithMessage("Project name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Project description cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.TeamId)
            .NotEmpty()
            .WithMessage("Team ID is required.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Created by user ID is required.");

        RuleFor(x => x.PlannedEndDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Planned end date must be in the future.")
            .When(x => x.PlannedEndDate.HasValue);
    }
}
