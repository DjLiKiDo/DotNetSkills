using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.ValueObjects;
using Task = DotNetSkills.Domain.Entities.Task;

namespace DotNetSkills.Domain.Services;

public class TaskAssignmentService : ITaskAssignmentService
{
    public bool CanAssignTaskToUser(Task task, User user)
    {
        var result = ValidateTaskAssignment(task, user);
        return result.IsValid;
    }

    public bool CanUserAccessTask(User user, Task task)
    {
        if (!user.IsActive)
            return false;

        if (user.Role.CanManageProjects())
            return true;

        // For now, we need the Project to get TeamId
        // This will be resolved when we implement proper aggregate boundaries
        return task.Project != null && user.CanAccessTeam(task.Project.TeamId);
    }

    public ValidationResult ValidateTaskAssignment(Task task, User user)
    {
        if (!user.IsActive)
            return ValidationResult.Invalid("Cannot assign task to inactive user.");

        if (task.Status == Enums.TaskStatus.Done)
            return ValidationResult.Invalid("Cannot assign completed tasks.");

        if (!user.Role.CanBeAssignedTasks())
            return ValidationResult.Invalid($"Users with role {user.Role.GetDisplayName()} cannot be assigned tasks.");

        if (!CanUserAccessTask(user, task))
            return ValidationResult.Invalid("User does not have access to this task's project.");

        if (task.IsSubtask && task.ParentTask?.AssignedToId != null && task.ParentTask.AssignedToId != user.Id)
            return ValidationResult.Invalid("Subtasks should typically be assigned to the same user as the parent task.");

        return ValidationResult.Valid();
    }
}