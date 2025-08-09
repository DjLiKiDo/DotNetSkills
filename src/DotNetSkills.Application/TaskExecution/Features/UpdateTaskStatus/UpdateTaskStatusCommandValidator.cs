namespace DotNetSkills.Application.TaskExecution.Features.UpdateTaskStatus;

/// <summary>
/// Validator for UpdateTaskStatusCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class UpdateTaskStatusCommandValidator : AbstractValidator<UpdateTaskStatusCommand>
{
    public UpdateTaskStatusCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("Task ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid TaskStatus value.");

        RuleFor(x => x.ActualHours)
            .GreaterThan(0)
            .WithMessage("Actual hours must be positive.")
            .LessThanOrEqualTo(2000)
            .WithMessage("Actual hours cannot exceed 2000 hours.")
            .When(x => x.ActualHours.HasValue);

        RuleFor(x => x.UpdatedBy)
            .NotEmpty()
            .WithMessage("Updated by user ID is required.");
    }
}
