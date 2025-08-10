namespace DotNetSkills.Application.TaskExecution.Features.UpdateSubtask;

/// <summary>
/// Validator for UpdateSubtaskCommand.
/// </summary>
public class UpdateSubtaskCommandValidator : AbstractValidator<UpdateSubtaskCommand>
{
    public UpdateSubtaskCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required.");

        RuleFor(x => x.TaskId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("TaskId cannot be empty GUID.");

        RuleFor(x => x.UpdatedByUserId)
            .NotEmpty()
            .WithMessage("UpdatedByUserId is required.");

        RuleFor(x => x.UpdatedByUserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("UpdatedByUserId cannot be empty GUID.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => x.Description != null)
            .WithMessage("Description cannot exceed 2000 characters.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .When(x => x.EstimatedHours.HasValue)
            .WithMessage("Estimated hours must be positive.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Priority must be a valid TaskPriority value.");
    }
}
