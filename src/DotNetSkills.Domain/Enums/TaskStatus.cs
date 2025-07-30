namespace DotNetSkills.Domain.Enums;

public enum TaskStatus
{
    ToDo = 1,
    InProgress = 2,
    Done = 3
}

public static class TaskStatusExtensions
{
    public static bool CanTransitionTo(this TaskStatus currentStatus, TaskStatus newStatus)
    {
        return currentStatus switch
        {
            TaskStatus.ToDo => newStatus is TaskStatus.InProgress or TaskStatus.Done,
            TaskStatus.InProgress => newStatus is TaskStatus.ToDo or TaskStatus.Done,
            TaskStatus.Done => false, // Cannot move from Done back to any other status
            _ => false
        };
    }

    public static string GetDisplayName(this TaskStatus status) => status switch
    {
        TaskStatus.ToDo => "To Do",
        TaskStatus.InProgress => "In Progress",
        TaskStatus.Done => "Done",
        _ => status.ToString()
    };

    public static bool IsComplete(this TaskStatus status) => 
        status == TaskStatus.Done;
}