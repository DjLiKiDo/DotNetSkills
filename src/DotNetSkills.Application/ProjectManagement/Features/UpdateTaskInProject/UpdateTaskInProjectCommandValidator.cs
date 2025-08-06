namespace DotNetSkills.Application.ProjectManagement.Features.UpdateTaskInProject;

/// <summary>
/// Validator for UpdateTaskInProjectCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class UpdateTaskInProjectCommandValidator : AbstractValidator<UpdateTaskInProjectCommand>
{
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };

    public UpdateTaskInProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("Task ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Task title is required.")
            .MaximumLength(200)
            .WithMessage("Task title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Task description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .Must(priority => ValidPriorities.Contains(priority))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .WithMessage("Estimated hours must be positive.")
            .LessThanOrEqualTo(1000)
            .WithMessage("Estimated hours cannot exceed 1000 hours.")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage("Updated by user ID is required.");

        // Note: Due date validation against current time is handled in domain logic
        // since completed tasks can have past due dates
    }
}
