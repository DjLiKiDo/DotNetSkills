namespace DotNetSkills.Domain.Enums;

public enum TaskType
{
    Task = 1,
    Story = 2,
    Bug = 3,
    Epic = 4,
    Subtask = 5
}

public static class TaskTypeExtensions
{
    public static bool CanHaveSubtasks(this TaskType taskType) => taskType switch
    {
        TaskType.Task => true,
        TaskType.Story => true,
        TaskType.Bug => true,
        TaskType.Epic => true,
        TaskType.Subtask => false, // Subtasks cannot have their own subtasks (single-level nesting)
        _ => false
    };

    public static string GetDisplayName(this TaskType taskType) => taskType switch
    {
        TaskType.Task => "Task",
        TaskType.Story => "User Story",
        TaskType.Bug => "Bug",
        TaskType.Epic => "Epic",
        TaskType.Subtask => "Subtask",
        _ => taskType.ToString()
    };

    public static string GetIconClass(this TaskType taskType) => taskType switch
    {
        TaskType.Task => "fa-tasks",
        TaskType.Story => "fa-book",
        TaskType.Bug => "fa-bug",
        TaskType.Epic => "fa-mountain",
        TaskType.Subtask => "fa-minus",
        _ => "fa-question"
    };
}