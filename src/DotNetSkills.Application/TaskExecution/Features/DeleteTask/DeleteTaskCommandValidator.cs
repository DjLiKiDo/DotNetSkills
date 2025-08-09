namespace DotNetSkills.Application.TaskExecution.Features.DeleteTask;

/// <summary>
/// Validator for DeleteTaskCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("Task ID is required.");

        RuleFor(x => x.DeletedBy)
            .NotEmpty()
            .WithMessage("Deleted by user ID is required.");
    }
}
