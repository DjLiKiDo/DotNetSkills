namespace DotNetSkills.Infrastructure.Persistence.Configurations.TeamCollaboration;

/// <summary>
/// Entity configuration for the TeamMember entity.
/// Configures the TeamMember join entity as part of the Team aggregate.
/// </summary>
public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    /// <summary>
    /// Configures the TeamMember entity with specific database mappings and constraints.
    /// </summary>
    /// <param name="builder">The entity type builder for TeamMember.</param>
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        // Configure table name
        builder.ToTable("TeamMembers");
        
        // Configure primary key with strongly-typed ID conversion
        builder.HasKey(tm => tm.Id);
        builder.Property(tm => tm.Id)
            .HasConversion(ValueConverters.CreateTeamMemberIdConverter())
            .IsRequired()
            .ValueGeneratedNever();
        
        // Configure audit fields
        ConfigureAuditFields(builder);
        
        // Configure UserId property with value conversion
        builder.Property(tm => tm.UserId)
            .HasConversion(ValueConverters.CreateUserIdConverter())
            .IsRequired()
            .HasComment("The ID of the user who is a member of the team");
        
        // Configure TeamId property with value conversion
        builder.Property(tm => tm.TeamId)
            .HasConversion(ValueConverters.CreateTeamIdConverter())
            .IsRequired()
            .HasComment("The ID of the team the user belongs to");
        
        // Configure Role enum as string
        builder.Property(tm => tm.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasComment("The role of the user within the team (Developer, ProjectManager, TeamLead, Viewer)");
        
        // Configure JoinedAt property
        builder.Property(tm => tm.JoinedAt)
            .IsRequired()
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("The date and time when the user joined the team");
        
    // Configure relationships centrally to avoid duplicate mappings
    ConfigureRelationships(builder);
        
        // Configure indexes and constraints
        ConfigureIndexesAndConstraints(builder);
    }
    
    /// <summary>
    /// Configures audit fields manually since we're not inheriting from BaseEntityConfiguration.
    /// </summary>
    /// <param name="builder">The entity type builder for TeamMember.</param>
    private static void ConfigureAuditFields(EntityTypeBuilder<TeamMember> builder)
    {
        builder.Property(tm => tm.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(tm => tm.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2(7)")
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(tm => tm.CreatedBy)
            .IsRequired(false)
            .HasColumnType("uniqueidentifier");
            
        builder.Property(tm => tm.UpdatedBy)
            .IsRequired(false)
            .HasColumnType("uniqueidentifier");
    }
    
    /// <summary>
    /// Configures relationships for the TeamMember entity.
    /// Defined here to avoid duplicate configuration from principals.
    /// </summary>
    /// <param name="builder">The entity type builder for TeamMember.</param>
    private static void ConfigureRelationships(EntityTypeBuilder<TeamMember> builder)
    {
        // TeamMember -> User (many-to-one) using dependent navigation tm.User
        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Users_UserId");

        // TeamMember -> Team (many-to-one) using dependent navigation tm.Team
        builder.HasOne(tm => tm.Team)
            .WithMany(t => t.Members)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TeamMembers_Teams_TeamId");
    }
    
    /// <summary>
    /// Configures database indexes and constraints for optimal query performance.
    /// </summary>
    /// <param name="builder">The entity type builder for TeamMember.</param>
    private static void ConfigureIndexesAndConstraints(EntityTypeBuilder<TeamMember> builder)
    {
        // Unique constraint on UserId + TeamId (business rule: user can only be in a team once)
        builder.HasIndex(tm => new { tm.UserId, tm.TeamId })
            .IsUnique()
            .HasDatabaseName("IX_TeamMembers_UserId_TeamId_Unique");
        
        // Index on UserId for user-based queries (find all teams for a user)
        builder.HasIndex(tm => tm.UserId)
            .HasDatabaseName("IX_TeamMembers_UserId");
        
        // Index on TeamId for team-based queries (find all members of a team)
        builder.HasIndex(tm => tm.TeamId)
            .HasDatabaseName("IX_TeamMembers_TeamId");
        
        // Index on Role for role-based filtering
        builder.HasIndex(tm => tm.Role)
            .HasDatabaseName("IX_TeamMembers_Role");
        
        // Composite index on TeamId and Role for team role queries
        builder.HasIndex(tm => new { tm.TeamId, tm.Role })
            .HasDatabaseName("IX_TeamMembers_TeamId_Role");
        
        // Index on JoinedAt for temporal queries and reporting
        builder.HasIndex(tm => tm.JoinedAt)
            .HasDatabaseName("IX_TeamMembers_JoinedAt");
    }
}
