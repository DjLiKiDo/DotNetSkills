namespace DotNetSkills.Infrastructure.Persistence.Configurations.UserManagement;

/// <summary>
/// Entity configuration for the User entity.
/// Configures the User aggregate root with proper table mapping, value conversions, and relationships.
/// </summary>
public class UserConfiguration : BaseEntityConfiguration<User, UserId>
{
    /// <summary>
    /// Configures the User entity with specific database mappings and constraints.
    /// </summary>
    /// <param name="builder">The entity type builder for User.</param>
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        // Configure table name
        builder.ToTable("Users");
        
        // Configure Name property
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(ValidationConstants.StringLengths.UserNameMaxLength)
            .HasComment("The user's full name");
        
        // Configure Email value object as owned entity
        ConfigureEmailAddress(builder);
        
        // Configure Role enum as string
        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("The user's role in the system (Admin, ProjectManager, Developer, Viewer)");
        
        // Configure Status enum as string
        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("The user's current status (Active, Inactive, Suspended, Pending)");
        
        // Configure navigation properties
        ConfigureNavigationProperties(builder);
        
        // Configure indexes for performance
        ConfigureIndexes(builder);
    }
    
    /// <summary>
    /// Creates a UserId from a Guid value for EF Core value conversion.
    /// </summary>
    /// <param name="value">The Guid value.</param>
    /// <returns>A new UserId instance.</returns>
    protected override UserId CreateIdFromGuid(Guid value) => new(value);
    
    /// <summary>
    /// Configures the EmailAddress value object as an owned entity.
    /// </summary>
    /// <param name="builder">The entity type builder for User.</param>
    private static void ConfigureEmailAddress(EntityTypeBuilder<User> builder)
    {
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(ValidationConstants.StringLengths.EmailMaxLength)
                .HasComment("The user's email address");
                
            // Unique constraint on email
            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email_Unique");
        });
    }
    
    /// <summary>
    /// Configures navigation properties for the User entity.
    /// </summary>
    /// <param name="builder">The entity type builder for User.</param>
    private static void ConfigureNavigationProperties(EntityTypeBuilder<User> builder)
    {
        // Configure relationship with TeamMember entities
        // Note: TeamMember is part of the Team aggregate, so this is a cross-aggregate relationship
        builder.HasMany<TeamMember>()
            .WithOne()
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Users_UserId");
        
        // Configure backing field for team memberships collection
        builder.Navigation(u => u.TeamMemberships)
            .HasField("_teamMemberships")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
    
    /// <summary>
    /// Configures database indexes for optimal query performance.
    /// </summary>
    /// <param name="builder">The entity type builder for User.</param>
    private static void ConfigureIndexes(EntityTypeBuilder<User> builder)
    {
        // Index on Role for role-based queries
        builder.HasIndex(u => u.Role)
            .HasDatabaseName("IX_Users_Role");
        
        // Index on Status for status filtering
        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");
        
        // Composite index on Role and Status for common filtering scenarios
        builder.HasIndex(u => new { u.Role, u.Status })
            .HasDatabaseName("IX_Users_Role_Status");
        
        // Index on Name for user search functionality
        builder.HasIndex(u => u.Name)
            .HasDatabaseName("IX_Users_Name");
    }
}
