using System.Reflection;

namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Entities;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamMemberTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateTeamMember()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var teamMember = CreateTeamMember(userId, teamId, role);

        // Assert
        teamMember.UserId.Should().Be(userId);
        teamMember.TeamId.Should().Be(teamId);
        teamMember.Role.Should().Be(role);
        teamMember.Id.Should().NotBe(TeamMemberId.New());
        teamMember.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        teamMember.JoinedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullUserId_ShouldThrowArgumentNullException()
    {
        // Arrange
        UserId userId = null!;
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => CreateTeamMember(userId, teamId, role));
        exception.InnerException.Should().BeOfType<ArgumentNullException>();
        ((ArgumentNullException)exception.InnerException!).ParamName.Should().Be("userId");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullTeamId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        TeamId teamId = null!;
        var role = TeamRole.Developer;

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => CreateTeamMember(userId, teamId, role));
        exception.InnerException.Should().BeOfType<ArgumentNullException>();
        ((ArgumentNullException)exception.InnerException!).ParamName.Should().Be("teamId");
    }

    [Theory]
    [InlineData(TeamRole.Developer)]
    [InlineData(TeamRole.ProjectManager)]
    [InlineData(TeamRole.TeamLead)]
    [InlineData(TeamRole.Viewer)]
    [Trait("TestType", "Creation")]
    public void Constructor_WithDifferentRoles_ShouldCreateTeamMember(TeamRole role)
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();

        // Act
        var teamMember = CreateTeamMember(userId, teamId, role);

        // Assert
        teamMember.Role.Should().Be(role);
        teamMember.UserId.Should().Be(userId);
        teamMember.TeamId.Should().Be(teamId);
    }

    [Theory]
    [InlineData(TeamRole.TeamLead, true)]
    [InlineData(TeamRole.ProjectManager, true)]
    [InlineData(TeamRole.Developer, false)]
    [InlineData(TeamRole.Viewer, false)]
    [Trait("TestType", "BusinessLogic")]
    public void HasLeadershipPrivileges_ShouldReturnCorrectValue(TeamRole role, bool expectedResult)
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), role);

        // Act
        var result = teamMember.HasLeadershipPrivileges();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(TeamRole.Developer, true)]
    [InlineData(TeamRole.ProjectManager, true)]
    [InlineData(TeamRole.TeamLead, true)]
    [InlineData(TeamRole.Viewer, false)]
    [Trait("TestType", "BusinessLogic")]
    public void CanBeAssignedTasks_ShouldReturnCorrectValue(TeamRole role, bool expectedResult)
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), role);

        // Act
        var result = teamMember.CanBeAssignedTasks();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(TeamRole.ProjectManager, true)]
    [InlineData(TeamRole.TeamLead, true)]
    [InlineData(TeamRole.Developer, false)]
    [InlineData(TeamRole.Viewer, false)]
    [Trait("TestType", "BusinessLogic")]
    public void CanAssignTasks_ShouldReturnCorrectValue(TeamRole role, bool expectedResult)
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), role);

        // Act
        var result = teamMember.CanAssignTasks();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WithAdminUser_ShouldChangeRole()
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), TeamRole.Developer);
        var adminUser = UserBuilder.Create().AsAdmin().Build();
        var newRole = TeamRole.TeamLead;

        // Act
        ChangeTeamMemberRole(teamMember, newRole, adminUser);

        // Assert
        teamMember.Role.Should().Be(newRole);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WithNonProjectManagerUser_ShouldThrowDomainException()
    {
        // Arrange  
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), TeamRole.Developer);
        var developerUser = UserBuilder.Create().AsDeveloper().Build(); // Not a project manager
        var newRole = TeamRole.TeamLead;

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => ChangeTeamMemberRole(teamMember, newRole, developerUser));
        exception.InnerException.Should().BeOfType<DomainException>();
        exception.InnerException!.Message.Should().Be("User does not have permission to change team member roles");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WithNonAdminAndNonTeamMember_ShouldThrowDomainException()
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), TeamRole.Developer);
        var nonAdminUser = UserBuilder.Create().AsDeveloper().Build();
        var newRole = TeamRole.TeamLead;

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => ChangeTeamMemberRole(teamMember, newRole, nonAdminUser));
        exception.InnerException.Should().BeOfType<DomainException>();
        exception.InnerException!.Message.Should().Be("User does not have permission to change team member roles");
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void JoinedAt_ShouldBeSetToCurrentUtcTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var teamMember = CreateTeamMember(userId, teamId, role);
        var afterCreation = DateTime.UtcNow;

        // Assert
        teamMember.JoinedAt.Should().BeOnOrAfter(beforeCreation);
        teamMember.JoinedAt.Should().BeOnOrBefore(afterCreation);
        teamMember.JoinedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamMember_ShouldInheritFromBaseEntity()
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), TeamRole.Developer);

        // Act & Assert
        teamMember.Should().BeAssignableTo<BaseEntity<TeamMemberId>>();
        teamMember.Id.Should().NotBeNull();
        teamMember.Id.Value.Should().NotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(TeamRole.Developer)]
    [InlineData(TeamRole.ProjectManager)]
    [InlineData(TeamRole.TeamLead)]
    [InlineData(TeamRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void RolePermissions_ShouldBeConsistentAcrossAllRoles(TeamRole role)
    {
        // Arrange
        var teamMember = CreateTeamMember(UserIdBuilder.Create(), TeamIdBuilder.Create(), role);

        // Act & Assert - Test consistency of role-based permissions
        var hasLeadership = teamMember.HasLeadershipPrivileges();
        var canBeAssigned = teamMember.CanBeAssignedTasks();
        var canAssign = teamMember.CanAssignTasks();

        // Leadership roles should also be able to assign tasks
        if (hasLeadership)
        {
            canAssign.Should().BeTrue("Leadership roles should be able to assign tasks");
        }

        // Anyone who can assign tasks should also be able to be assigned tasks (except viewers)
        if (canAssign && role != TeamRole.Viewer)
        {
            canBeAssigned.Should().BeTrue("Users who can assign tasks should also be able to be assigned tasks");
        }
    }

    /// <summary>
    /// Helper method to create TeamMember instances since the constructor is internal.
    /// This uses reflection to simulate how the Team aggregate would create TeamMember instances.
    /// </summary>
    private static TeamMember CreateTeamMember(UserId userId, TeamId teamId, TeamRole role)
    {
        var constructor = typeof(TeamMember).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(UserId), typeof(TeamId), typeof(TeamRole) },
            null);

        if (constructor == null)
            throw new InvalidOperationException("Could not find TeamMember internal constructor");

        return (TeamMember)constructor.Invoke(new object[] { userId, teamId, role });
    }

    /// <summary>
    /// Helper method to change TeamMember role since the method is internal.
    /// This uses reflection to simulate how the Team aggregate would change member roles.
    /// </summary>
    private static void ChangeTeamMemberRole(TeamMember teamMember, TeamRole newRole, User changedBy)
    {
        var changeRoleMethod = typeof(TeamMember).GetMethod(
            "ChangeRole",
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(TeamRole), typeof(User) },
            null);

        if (changeRoleMethod == null)
            throw new InvalidOperationException("Could not find TeamMember ChangeRole internal method");

        changeRoleMethod.Invoke(teamMember, new object[] { newRole, changedBy });
    }
}