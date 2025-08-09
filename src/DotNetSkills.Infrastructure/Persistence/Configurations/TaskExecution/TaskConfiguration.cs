namespace DotNetSkills.Infrastructure.Persistence.Configurations.TaskExecution;

/// <summary>
/// Entity Framework configuration for the Task aggregate root.
/// Configures table mapping, relationships, indexes, and constraints for the Tasks table,
/// including self-referencing parent-child relationships for subtasks.
/// </summary>
public class TaskConfiguration : BaseEntityConfiguration<DotNetSkills.Domain.TaskExecution.Entities.Task, TaskId>
{
    /// <summary>
    /// Creates a TaskId from a Guid value for strongly-typed ID conversion.
    /// </summary>
    /// <param name="value">The Guid value.</param>
    /// <returns>A new TaskId instance.</returns>
    protected override TaskId CreateIdFromGuid(Guid value)
    {
        return new TaskId(value);
    }

    /// <summary>
    /// Configures entity-specific settings for the Task aggregate.
    /// </summary>
    /// <param name="builder">The entity type builder for Task.</param>
    protected override void ConfigureEntity(EntityTypeBuilder<DotNetSkills.Domain.TaskExecution.Entities.Task> builder)
    {
        // Table mapping
        builder.ToTable("Tasks", "dbo", t => 
            t.HasComment("Stores task information with hierarchy support for project management"));

        // Properties configuration
        ConfigureProperties(builder);
        
        // Relationships configuration  
        ConfigureRelationships(builder);
        
        // Indexes configuration
        ConfigureIndexes(builder);
        
        // Constraints configuration
        ConfigureConstraints(builder);
    }

    /// <summary>
    /// Configures the Task entity properties and value conversions.
    /// </summary>
    /// <param name="builder">The entity type builder for Task.</param>
    private static void ConfigureProperties(EntityTypeBuilder<DotNetSkills.Domain.TaskExecution.Entities.Task> builder)
    {
        // Task Title - required field with length constraint
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(ValidationConstants.StringLengths.TaskTitleMaxLength)
            .IsUnicode()
            .HasComment("The title/name of the task");

        // Task Description - optional field with length constraint
        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(ValidationConstants.StringLengths.DescriptionMaxLength)
            .IsUnicode()
            .HasComment("Detailed description of the task requirements and goals");

        // ProjectId - foreign key to Projects table
        builder.Property(t => t.ProjectId)
            .IsRequired()
            .HasConversion(ValueConverters.CreateProjectIdConverter())
            .HasComment("The ID of the project this task belongs to");

        // AssignedUserId - nullable foreign key to Users table
        builder.Property(t => t.AssignedUserId)
            .IsRequired(false)
            .HasConversion(ValueConverters.CreateNullableUserIdConverter())
            .HasComment("The ID of the user assigned to this task (null if unassigned)");

        // ParentTaskId - nullable self-referencing foreign key for subtasks
        builder.Property(t => t.ParentTaskId)
            .IsRequired(false)
            .HasConversion(ValueConverters.CreateNullableTaskIdConverter())
            .HasComment("The ID of the parent task (null for top-level tasks)");

        // Task Status - enum stored as string
        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("Current status of the task (ToDo, InProgress, InReview, Done, Cancelled)");

        // Task Priority - enum stored as string
        builder.Property(t => t.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("Priority level of the task (Low, Medium, High, Critical)");

        // Estimated Hours - nullable integer for effort estimation
        builder.Property(t => t.EstimatedHours)
            .IsRequired(false)
            .HasComment("Estimated effort in hours to complete the task");

        // Actual Hours - nullable integer for actual effort tracking
        builder.Property(t => t.ActualHours)
            .IsRequired(false)
            .HasComment("Actual effort in hours spent on the task (set when completed)");

        // Due Date - nullable DateTime
        builder.Property(t => t.DueDate)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The target completion date for the task");

        // Started At - nullable DateTime
        builder.Property(t => t.StartedAt)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The date and time when work on the task began");

        // Completed At - nullable DateTime
        builder.Property(t => t.CompletedAt)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The date and time when the task was completed");
    }

    /// <summary>
    /// Configures relationships between Task and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Task.</param>
    private static void ConfigureRelationships(EntityTypeBuilder<DotNetSkills.Domain.TaskExecution.Entities.Task> builder)
    {
        // Many-to-One relationship with Project
        // A task belongs to exactly one project
        builder.HasOne<Project>()
            .WithMany() // Navigation property not exposed on Project aggregate
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade) // Delete tasks when project is deleted
            .HasConstraintName("FK_Tasks_Projects");

        // Many-to-One relationship with User (nullable - for assigned user)
        // A task can be assigned to one user or none
        builder.HasOne<User>()
            .WithMany() // Navigation property not exposed on User aggregate
            .HasForeignKey(t => t.AssignedUserId)
            .OnDelete(DeleteBehavior.SetNull) // Unassign task when user is deleted
            .HasConstraintName("FK_Tasks_Users_AssignedUser");

        // Self-referencing One-to-Many relationship for parent-child tasks
        // A task can have multiple subtasks, but each subtask has only one parent
        builder.HasOne<DotNetSkills.Domain.TaskExecution.Entities.Task>()
            .WithMany(t => t.Subtasks)
            .HasForeignKey(t => t.ParentTaskId)
            .OnDelete(DeleteBehavior.Restrict) // Prevent cascade delete - must handle subtasks explicitly
            .HasConstraintName("FK_Tasks_Tasks_Parent");
    }

    /// <summary>
    /// Configures database indexes for optimal query performance.
    /// </summary>
    /// <param name="builder">The entity type builder for Task.</param>
    private static void ConfigureIndexes(EntityTypeBuilder<DotNetSkills.Domain.TaskExecution.Entities.Task> builder)
    {
        // Index on ProjectId for efficient project-based task queries
        builder.HasIndex(t => t.ProjectId)
            .HasDatabaseName("IX_Tasks_ProjectId")
            .HasFilter("[ProjectId] IS NOT NULL");

        // Index on AssignedUserId for efficient user assignment queries
        builder.HasIndex(t => t.AssignedUserId)
            .HasDatabaseName("IX_Tasks_AssignedUserId")
            .HasFilter("[AssignedUserId] IS NOT NULL");

        // Index on ParentTaskId for efficient subtask queries
        builder.HasIndex(t => t.ParentTaskId)
            .HasDatabaseName("IX_Tasks_ParentTaskId")
            .HasFilter("[ParentTaskId] IS NOT NULL");

        // Composite index on Status and Priority for task filtering and sorting
        builder.HasIndex(t => new { t.Status, t.Priority })
            .HasDatabaseName("IX_Tasks_Status_Priority")
            .HasFilter("[Status] IS NOT NULL AND [Priority] IS NOT NULL");

        // Composite index on ProjectId and Status for project task management
        builder.HasIndex(t => new { t.ProjectId, t.Status })
            .HasDatabaseName("IX_Tasks_ProjectId_Status")
            .HasFilter("[ProjectId] IS NOT NULL AND [Status] IS NOT NULL");

        // Composite index on AssignedUserId and Status for user task lists
        builder.HasIndex(t => new { t.AssignedUserId, t.Status })
            .HasDatabaseName("IX_Tasks_AssignedUserId_Status")
            .HasFilter("[AssignedUserId] IS NOT NULL AND [Status] IS NOT NULL");

        // Index on DueDate for deadline tracking and overdue task identification
        builder.HasIndex(t => t.DueDate)
            .HasDatabaseName("IX_Tasks_DueDate")
            .HasFilter("[DueDate] IS NOT NULL");

        // Composite index on Status and DueDate for task scheduling queries
        builder.HasIndex(t => new { t.Status, t.DueDate })
            .HasDatabaseName("IX_Tasks_Status_DueDate")
            .HasFilter("[Status] IS NOT NULL");

        // Index on Title for task search functionality
        builder.HasIndex(t => t.Title)
            .HasDatabaseName("IX_Tasks_Title")
            .HasFilter("[Title] IS NOT NULL");
    }

    /// <summary>
    /// Configures database constraints for data integrity.
    /// </summary>
    /// <param name="builder">The entity type builder for Task.</param>
    private static void ConfigureConstraints(EntityTypeBuilder<DotNetSkills.Domain.TaskExecution.Entities.Task> builder)
    {
        // Business rule constraints will be enforced by domain logic:
        // - EstimatedHours must be positive when set
        // - ActualHours must be positive when set  
        // - CompletedAt must be after StartedAt when both are set
        // - DueDate should not be in the past when set
        // - Subtasks cannot have their own subtasks (one level only)
        
        // These constraints could be implemented as database check constraints
        // in future migrations if additional data integrity enforcement is needed
    }
}
