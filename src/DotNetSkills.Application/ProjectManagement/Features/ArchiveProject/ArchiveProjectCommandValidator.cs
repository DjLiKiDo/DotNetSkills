namespace DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;

/// <summary>
/// Validator for ArchiveProjectCommand that ensures all business rules are satisfied before execution.
/// Uses FluentValidation to provide comprehensive input validation with clear error messages.
/// </summary>
public class ArchiveProjectCommandValidator : AbstractValidator<ArchiveProjectCommand>
{
    public ArchiveProjectCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("Project ID is required.");

        RuleFor(x => x.ArchivedBy)
            .NotEmpty()
            .WithMessage("Archived by user ID is required.");
    }
}
