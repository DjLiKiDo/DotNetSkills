namespace DotNetSkills.Application.TaskExecution.Features.UpdateTask;

/// <summary>
/// Validator for UpdateTaskCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
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
            .IsInEnum()
            .WithMessage("Priority must be a valid TaskPriority value.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .WithMessage("Estimated hours must be positive.")
            .LessThanOrEqualTo(1000)
            .WithMessage("Estimated hours cannot exceed 1000 hours.")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage("Updated by user ID is required.");

        // Note: Due date validation is handled in domain logic as tasks may be updated with past due dates
    }
}
