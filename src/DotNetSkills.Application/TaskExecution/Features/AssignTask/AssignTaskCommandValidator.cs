namespace DotNetSkills.Application.TaskExecution.Features.AssignTask;

/// <summary>
/// Validator for AssignTaskCommand.
/// </summary>
public class AssignTaskCommandValidator : AbstractValidator<AssignTaskCommand>
{
    public AssignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required.");

        RuleFor(x => x.TaskId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("TaskId cannot be empty GUID.");

        RuleFor(x => x.AssignedUserId)
            .NotEmpty()
            .WithMessage("AssignedUserId is required.");

        RuleFor(x => x.AssignedUserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("AssignedUserId cannot be empty GUID.");

        RuleFor(x => x.AssignedByUserId)
            .NotEmpty()
            .WithMessage("AssignedByUserId is required.");

        RuleFor(x => x.AssignedByUserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("AssignedByUserId cannot be empty GUID.");
    }
}
