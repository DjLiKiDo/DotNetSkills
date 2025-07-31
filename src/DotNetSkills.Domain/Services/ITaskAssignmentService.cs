using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;
using Task = DotNetSkills.Domain.Entities.Task;

namespace DotNetSkills.Domain.Services;

public interface ITaskAssignmentService
{
    bool CanAssignTaskToUser(Task task, User user);
    bool CanUserAccessTask(User user, Task task);
    ValidationResult ValidateTaskAssignment(Task task, User user);
}

public readonly record struct ValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }
    
    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }
    
    public static ValidationResult Valid() => new(true);
    public static ValidationResult Invalid(string errorMessage) => new(false, errorMessage);
}