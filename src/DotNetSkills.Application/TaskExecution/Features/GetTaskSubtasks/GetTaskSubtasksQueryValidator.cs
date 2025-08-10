namespace DotNetSkills.Application.TaskExecution.Features.GetTaskSubtasks;

/// <summary>
/// Validator for GetTaskSubtasksQuery.
/// </summary>
public class GetTaskSubtasksQueryValidator : AbstractValidator<GetTaskSubtasksQuery>
{
    public GetTaskSubtasksQueryValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required.");

        RuleFor(x => x.TaskId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("TaskId cannot be empty GUID.");
    }
}
