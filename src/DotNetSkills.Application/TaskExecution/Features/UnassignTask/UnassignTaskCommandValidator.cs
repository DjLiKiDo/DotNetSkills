namespace DotNetSkills.Application.TaskExecution.Features.UnassignTask;

/// <summary>
/// Validator for UnassignTaskCommand.
/// </summary>
public class UnassignTaskCommandValidator : AbstractValidator<UnassignTaskCommand>
{
    public UnassignTaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required.");

        RuleFor(x => x.TaskId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("TaskId cannot be empty GUID.");

        RuleFor(x => x.UnassignedByUserId)
            .NotEmpty()
            .WithMessage("UnassignedByUserId is required.");

        RuleFor(x => x.UnassignedByUserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UnassignedByUserId cannot be empty GUID.");
    }
}
