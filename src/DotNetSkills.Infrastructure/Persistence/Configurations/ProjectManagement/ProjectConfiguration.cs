namespace DotNetSkills.Infrastructure.Persistence.Configurations.ProjectManagement;

/// <summary>
/// Entity Framework configuration for the Project aggregate root.
/// Configures table mapping, relationships, indexes, and constraints for the Projects table.
/// </summary>
public class ProjectConfiguration : BaseEntityConfiguration<Project, ProjectId>
{
    /// <summary>
    /// Creates a ProjectId from a Guid value for strongly-typed ID conversion.
    /// </summary>
    /// <param name="value">The Guid value.</param>
    /// <returns>A new ProjectId instance.</returns>
    protected override ProjectId CreateIdFromGuid(Guid value)
    {
        return new ProjectId(value);
    }

    /// <summary>
    /// Configures entity-specific settings for the Project aggregate.
    /// </summary>
    /// <param name="builder">The entity type builder for Project.</param>
    protected override void ConfigureEntity(EntityTypeBuilder<Project> builder)
    {
        // Table mapping
        builder.ToTable("Projects", "dbo", t => 
            t.HasComment("Stores project information and metadata for the project management system"));

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
    /// Configures the Project entity properties and value conversions.
    /// </summary>
    /// <param name="builder">The entity type builder for Project.</param>
    private static void ConfigureProperties(EntityTypeBuilder<Project> builder)
    {
        // Project Name - required field with length constraint
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.StringLengths.ProjectNameMaxLength)
            .IsUnicode()
            .HasComment("The display name of the project");

        // Project Description - optional field with length constraint
        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(ValidationConstants.StringLengths.DescriptionMaxLength)
            .IsUnicode()
            .HasComment("Optional detailed description of the project");

        // TeamId - foreign key to Teams table
        builder.Property(p => p.TeamId)
            .IsRequired()
            .HasConversion(ValueConverters.CreateTeamIdConverter())
            .HasComment("The ID of the team responsible for this project");

        // Project Status - enum stored as string
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("Current status of the project (Active, Completed, OnHold, Cancelled, Planning)");

        // Start Date - nullable DateTime
        builder.Property(p => p.StartDate)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The actual start date of project work");

        // End Date - nullable DateTime  
        builder.Property(p => p.EndDate)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The actual completion date of the project");

        // Planned End Date - nullable DateTime
        builder.Property(p => p.PlannedEndDate)
            .IsRequired(false)
            .HasColumnType("datetime2")
            .HasPrecision(7)
            .HasComment("The originally planned completion date");
    }

    /// <summary>
    /// Configures relationships between Project and other entities.
    /// </summary>
    /// <param name="builder">The entity type builder for Project.</param>
    private static void ConfigureRelationships(EntityTypeBuilder<Project> builder)
    {
        // Many-to-One relationship with Team
        // A project belongs to exactly one team
        builder.HasOne<Team>()
            .WithMany() // Navigation property not exposed on Team aggregate
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict) // Prevent cascade delete - projects must be explicitly handled
            .HasConstraintName("FK_Projects_Teams");

        // One-to-Many relationship with Tasks will be configured in TaskConfiguration
        // Projects can have many tasks, but tasks belong to exactly one project
    }

    /// <summary>
    /// Configures database indexes for optimal query performance.
    /// </summary>
    /// <param name="builder">The entity type builder for Project.</param>
    private static void ConfigureIndexes(EntityTypeBuilder<Project> builder)
    {
        // Unique index on project name for business rule enforcement
        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("IX_Projects_Name_Unique")
            .HasFilter("[Name] IS NOT NULL");

        // Index on TeamId for efficient team-based project queries
        builder.HasIndex(p => p.TeamId)
            .HasDatabaseName("IX_Projects_TeamId")
            .HasFilter("[TeamId] IS NOT NULL");

        // Composite index on Status and TeamId for dashboard queries
        builder.HasIndex(p => new { p.Status, p.TeamId })
            .HasDatabaseName("IX_Projects_Status_TeamId")
            .HasFilter("[Status] IS NOT NULL AND [TeamId] IS NOT NULL");

        // Index on PlannedEndDate for deadline tracking
        builder.HasIndex(p => p.PlannedEndDate)
            .HasDatabaseName("IX_Projects_PlannedEndDate")
            .HasFilter("[PlannedEndDate] IS NOT NULL");

        // Composite index on Status and PlannedEndDate for project monitoring
        builder.HasIndex(p => new { p.Status, p.PlannedEndDate })
            .HasDatabaseName("IX_Projects_Status_PlannedEndDate")
            .HasFilter("[Status] IS NOT NULL");
    }

    /// <summary>
    /// Configures database constraints for data integrity.
    /// </summary>
    /// <param name="builder">The entity type builder for Project.</param>
    private static void ConfigureConstraints(EntityTypeBuilder<Project> builder)
    {
        // Check constraint to ensure StartDate is not in the future when set
        // This will be handled by application logic, but we document the business rule
        
        // Check constraint to ensure EndDate is after StartDate when both are set
        // This will be enforced by domain logic in the Project entity
        
        // The actual constraints will be implemented as database check constraints
        // in a future migration if needed for additional data integrity
    }
}
