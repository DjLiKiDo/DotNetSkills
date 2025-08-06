namespace DotNetSkills.Application.TaskExecution.Features.CreateTask;

/// <summary>
/// Validator for CreateTaskCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    private static readonly string[] ValidPriorities = { "Low", "Medium", "High", "Critical" };

    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Task title is required.")
            .MaximumLength(200)
            .WithMessage("Task title cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Task description cannot exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.Priority)
            .Must(priority => ValidPriorities.Contains(priority))
            .WithMessage("Priority must be one of: Low, Medium, High, Critical.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .WithMessage("Estimated hours must be positive.")
            .LessThanOrEqualTo(1000)
            .WithMessage("Estimated hours cannot exceed 1000 hours.")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.ParentTaskId)
            .NotEqual(new TaskId(Guid.Empty))
            .WithMessage("Parent task ID cannot be empty.")
            .When(x => x.ParentTaskId != null);

        RuleFor(x => x.AssignedUserId)
            .NotEqual(new UserId(Guid.Empty))
            .WithMessage("Assigned user ID cannot be empty.")
            .When(x => x.AssignedUserId != null);

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Created by user ID is required.");
    }
}
