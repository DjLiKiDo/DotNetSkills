namespace DotNetSkills.Application.TaskExecution.Features.CreateSubtask;

/// <summary>
/// Validator for CreateSubtaskCommand.
/// </summary>
public class CreateSubtaskCommandValidator : AbstractValidator<CreateSubtaskCommand>
{
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };

    public CreateSubtaskCommandValidator()
    {
        RuleFor(x => x.ParentTaskId)
            .NotEmpty()
            .WithMessage("ParentTaskId is required.");

        RuleFor(x => x.ParentTaskId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("ParentTaskId cannot be empty GUID.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty()
            .WithMessage("CreatedByUserId is required.");

        RuleFor(x => x.CreatedByUserId.Value)
            .NotEqual(Guid.Empty)
            .WithMessage("CreatedByUserId cannot be empty GUID.");

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
            .Must(priority => ValidPriorities.Contains(priority))
            .WithMessage($"Priority must be one of: {string.Join(", ", ValidPriorities)}.");

        RuleFor(x => x.AssignedUserId)
            .Must(userId => userId != null && userId.Value != Guid.Empty)
            .When(x => x.AssignedUserId != null)
            .WithMessage("AssignedUserId cannot be empty GUID when provided.");
    }
}
