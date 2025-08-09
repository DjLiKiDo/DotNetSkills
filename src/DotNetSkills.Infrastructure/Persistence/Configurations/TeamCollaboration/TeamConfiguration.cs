namespace DotNetSkills.Infrastructure.Persistence.Configurations.TeamCollaboration;

/// <summary>
/// Entity configuration for the Team entity.
/// Configures the Team aggregate root with proper table mapping, value conversions, and relationships.
/// </summary>
public class TeamConfiguration : BaseEntityConfiguration<Team, TeamId>
{
    /// <summary>
    /// Configures the Team entity with specific database mappings and constraints.
    /// </summary>
    /// <param name="builder">The entity type builder for Team.</param>
    protected override void ConfigureEntity(EntityTypeBuilder<Team> builder)
    {
        // Configure table name
        builder.ToTable("Teams");
        
        // Configure Name property
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.StringLengths.TeamNameMaxLength)
            .HasComment("The team name");
        
        // Configure Description property
        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(ValidationConstants.StringLengths.DescriptionMaxLength)
            .HasComment("Optional team description");
        
        // Configure navigation properties
        ConfigureNavigationProperties(builder);
        
        // Configure indexes and constraints
        ConfigureIndexesAndConstraints(builder);
    }
    
    /// <summary>
    /// Creates a TeamId from a Guid value for EF Core value conversion.
    /// </summary>
    /// <param name="value">The Guid value.</param>
    /// <returns>A new TeamId instance.</returns>
    protected override TeamId CreateIdFromGuid(Guid value) => new(value);
    
    /// <summary>
    /// Configures navigation properties for the Team entity.
    /// </summary>
    /// <param name="builder">The entity type builder for Team.</param>
    private static void ConfigureNavigationProperties(EntityTypeBuilder<Team> builder)
    {
        // Configure one-to-many relationship with TeamMember entities
        // TeamMember is part of the Team aggregate
        builder.HasMany<TeamMember>()
            .WithOne()
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Teams_TeamId");
        
        // Configure backing field for members collection
        builder.Navigation(t => t.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        // Configure one-to-many relationship with Project entities
        builder.HasMany<Project>()
            .WithOne()
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict) // Don't allow team deletion if projects exist
            .HasConstraintName("FK_Projects_Teams_TeamId");
    }
    
    /// <summary>
    /// Configures database indexes and constraints for optimal query performance.
    /// </summary>
    /// <param name="builder">The entity type builder for Team.</param>
    private static void ConfigureIndexesAndConstraints(EntityTypeBuilder<Team> builder)
    {
        // Unique constraint on team name (business rule)
        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Teams_Name_Unique");
        
        // Index on CreatedAt for team listings and filtering
        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Teams_CreatedAt");
        
        // Add check constraint for team name length
        builder.ToTable(t => 
            t.HasCheckConstraint(
                "CK_Teams_Name_MinLength", 
                $"LEN([Name]) >= {ValidationConstants.StringLengths.TeamNameMinLength}"));
    }
}
