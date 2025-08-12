namespace DotNetSkills.Infrastructure.UnitTests.Persistence;

public class TeamMemberMappingTests
{
    [Fact(DisplayName = "EF Model - TeamMember has no shadow FKs and correct navigations")]
    public void TeamMemberModel_NoShadowFKs_CorrectNavigations()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var ctx = new ApplicationDbContext(options);

        // Act
        var entityType = ctx.Model.FindEntityType(typeof(TeamMember));
        entityType.Should().NotBeNull();

        var propertyNames = entityType!.GetProperties().Select(p => p.Name).ToList();

        // Assert: no shadow FK columns
        propertyNames.Should().NotContain("TeamId1");
        propertyNames.Should().NotContain("UserId1");

        // Assert: FKs configured correctly
        var fks = entityType.GetForeignKeys().ToList();
        fks.Should().HaveCount(2);

        // One FK should be for TeamId with Dependent navigation 'Team'
        fks.Should().ContainSingle(fk => fk.Properties.Any(p => p.Name == "TeamId"))
            .Which.DependentToPrincipal!.Name.Should().Be("Team");

        // One FK should be for UserId with Dependent navigation 'User'
        fks.Should().ContainSingle(fk => fk.Properties.Any(p => p.Name == "UserId"))
            .Which.DependentToPrincipal!.Name.Should().Be("User");
    }

    [Fact(DisplayName = "EF CRUD - Team/User/TeamMember end-to-end mapping works without shadow FKs")]
    public async Task TeamUserTeamMember_CRUD_Smoke_NoShadowFKs()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var ctx = new ApplicationDbContext(options);

        var admin = new User("Admin", new EmailAddress("admin@example.com"), UserRole.Admin);
        var team = Team.Create("Alpha Team", "First team", admin);

        // create a developer user and add to team via domain
        var dev = new User("Dev One", new EmailAddress("dev1@example.com"), UserRole.Developer, admin.Id);
        team.AddMember(dev, TeamRole.Developer, admin);

        // Act
        ctx.Users.Add(admin);
        ctx.Users.Add(dev);
        ctx.Teams.Add(team);
        await ctx.SaveChangesAsync();

        // Assert - query back
        var savedTeam = await ctx.Teams.Include(t => t.Members).FirstAsync();
        savedTeam.Members.Should().HaveCount(1);
        savedTeam.Members.First().UserId.Should().Be(dev.Id);
        savedTeam.Members.First().TeamId.Should().Be(team.Id);

        // Assert model still has no shadow FKs
        var entityType = ctx.Model.FindEntityType(typeof(TeamMember))!;
        entityType.GetProperties().Select(p => p.Name).Should().NotContain(new[] { "TeamId1", "UserId1" });
    }
}
