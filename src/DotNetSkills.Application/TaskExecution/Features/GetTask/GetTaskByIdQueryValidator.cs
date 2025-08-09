namespace DotNetSkills.Application.TaskExecution.Features.GetTask;

/// <summary>
/// Validator for GetTaskByIdQuery that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}
