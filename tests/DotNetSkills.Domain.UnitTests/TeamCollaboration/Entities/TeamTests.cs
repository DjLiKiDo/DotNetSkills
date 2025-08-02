namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Entities;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateTeam()
    {
        // Arrange
        var name = "Development Team";
        var description = "Primary development team";
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act
        var team = new Team(name, description, createdBy);

        // Assert
        team.Name.Should().Be(name);
        team.Description.Should().Be(description);
        team.Id.Should().NotBe(TeamId.New());
        team.Members.Should().BeEmpty();
        team.MemberCount.Should().Be(0);
        team.HasRoom().Should().BeTrue();
        team.HasLeadership().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldRaiseTeamCreatedDomainEvent()
    {
        // Arrange
        var name = "Development Team";
        var description = "Primary development team";
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act
        var team = new Team(name, description, createdBy);

        // Assert
        var domainEvent = DomainEventHelper.AssertEventRaised<TeamCreatedDomainEvent, TeamId>(team);
        domainEvent.TeamId.Should().Be(team.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.CreatedBy.Should().Be(createdBy.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestType", "Validation")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var description = "Test description";
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act & Assert
        AssertArgumentException(() => new Team(invalidName, description, createdBy), "name");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullCreatedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = "Test Team";
        var description = "Test description";
        User createdBy = null!;

        // Act & Assert
        AssertArgumentNullException(() => new Team(name, description, createdBy), "createdBy");
    }

    [Theory]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.ProjectManager)]
    [InlineData(UserRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithNonAdminCreator_ShouldThrowDomainException(UserRole nonAdminRole)
    {
        // Arrange
        var name = "Test Team";
        var description = "Test description";
        var createdBy = UserBuilder.Create().WithRole(nonAdminRole).Build();

        // Act & Assert
        AssertDomainException(
            () => new Team(name, description, createdBy),
            ValidationMessages.Team.NoPermissionToCreate);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldTrimNameAndDescription()
    {
        // Arrange
        var nameWithWhitespace = "  Test Team  ";
        var descriptionWithWhitespace = "  Test description  ";
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act
        var team = new Team(nameWithWhitespace, descriptionWithWhitespace, createdBy);

        // Assert
        team.Name.Should().Be("Test Team");
        team.Description.Should().Be("Test description");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullDescription_ShouldCreateTeam()
    {
        // Arrange
        var name = "Test Team";
        string? description = null;
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act
        var team = new Team(name, description, createdBy);

        // Assert
        team.Name.Should().Be(name);
        team.Description.Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithValidParameters_ShouldCreateTeam()
    {
        // Arrange
        var name = "Test Team";
        var description = "Test description";
        var createdBy = UserBuilder.Create().AsAdmin().Build();

        // Act
        var team = Team.Create(name, description, createdBy);

        // Assert
        team.Name.Should().Be(name);
        team.Description.Should().Be(description);
        team.Members.Should().BeEmpty();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithValidParameters_ShouldUpdateTeamInfo()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var newName = "Updated Team Name";
        var newDescription = "Updated team description";

        // Act
        team.UpdateInfo(newName, newDescription);

        // Assert
        team.Name.Should().Be(newName);
        team.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [Trait("TestType", "Validation")]
    public void UpdateInfo_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var description = "Valid description";

        // Act & Assert
        AssertArgumentException(() => team.UpdateInfo(invalidName, description), "name");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_ShouldTrimNameAndDescription()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var nameWithWhitespace = "  Updated Team  ";
        var descriptionWithWhitespace = "  Updated description  ";

        // Act
        team.UpdateInfo(nameWithWhitespace, descriptionWithWhitespace);

        // Assert
        team.Name.Should().Be("Updated Team");
        team.Description.Should().Be("Updated description");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddMember_WithValidParameters_ShouldAddMemberToTeam()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var addedBy = UserBuilder.Create().AsAdmin().Build();
        var role = TeamRole.Developer;

        // Act
        team.AddMember(user, role, addedBy);

        // Assert
        team.Members.Should().HaveCount(1);
        team.MemberCount.Should().Be(1);
        var member = team.Members.First();
        member.UserId.Should().Be(user.Id);
        member.Role.Should().Be(role);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddMember_WithValidParameters_ShouldRaiseUserJoinedTeamDomainEvent()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var addedBy = UserBuilder.Create().AsAdmin().Build();
        var role = TeamRole.Developer;

        // Act
        team.AddMember(user, role, addedBy);

        // Assert
        var domainEvent = DomainEventHelper.AssertEventRaised<UserJoinedTeamDomainEvent, TeamId>(team);
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.TeamId.Should().Be(team.Id);
        domainEvent.Role.Should().Be(role);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void AddMember_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        User user = null!;
        var addedBy = UserBuilder.Create().AsAdmin().Build();
        var role = TeamRole.Developer;

        // Act & Assert
        AssertArgumentNullException(() => team.AddMember(user, role, addedBy), "user");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void AddMember_WithNullAddedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        User addedBy = null!;
        var role = TeamRole.Developer;

        // Act & Assert
        AssertArgumentNullException(() => team.AddMember(user, role, addedBy), "addedBy");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddMember_WhenUserAlreadyMember_ShouldThrowDomainException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var addedBy = UserBuilder.Create().AsAdmin().Build();
        var role = TeamRole.Developer;

        // Add member first time
        team.AddMember(user, role, addedBy);

        // Act & Assert - Try to add same user again
        AssertDomainException(() => team.AddMember(user, role, addedBy), ValidationMessages.User.AlreadyTeamMember);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void RemoveMember_WithValidParameters_ShouldRemoveMemberFromTeam()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        // Add member first
        team.AddMember(user, TeamRole.Developer, admin);
        team.Members.Should().HaveCount(1);

        // Act
        team.RemoveMember(user, admin);

        // Assert
        team.Members.Should().BeEmpty();
        team.MemberCount.Should().Be(0);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void RemoveMember_WithValidParameters_ShouldRaiseUserLeftTeamDomainEvent()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();
        var role = TeamRole.Developer;

        // Add member first
        team.AddMember(user, role, admin);

        // Act
        team.RemoveMember(user, admin);

        // Assert
        var leftEvents = DomainEventHelper.GetEvents<UserLeftTeamDomainEvent, TeamId>(team);
        var leftEvent = leftEvents.Should().ContainSingle().Subject;
        leftEvent.UserId.Should().Be(user.Id);
        leftEvent.TeamId.Should().Be(team.Id);
        leftEvent.Role.Should().Be(role);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void RemoveMember_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        User user = null!;
        var removedBy = UserBuilder.Create().AsAdmin().Build();

        // Act & Assert
        AssertArgumentNullException(() => team.RemoveMember(user, removedBy), "user");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void RemoveMember_WithNullRemovedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        User removedBy = null!;

        // Act & Assert
        AssertArgumentNullException(() => team.RemoveMember(user, removedBy), "removedBy");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void RemoveMember_WhenUserNotMember_ShouldThrowDomainException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var removedBy = UserBuilder.Create().AsAdmin().Build();

        // Act & Assert
        AssertDomainException(() => team.RemoveMember(user, removedBy), ValidationMessages.User.NotTeamMember);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetMembersByRole_ShouldReturnMembersWithSpecifiedRole()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var dev1 = UserBuilder.Create().WithName("Dev 1").AsDeveloper().Build();
        var dev2 = UserBuilder.Create().WithName("Dev 2").AsDeveloper().Build();
        var pm = UserBuilder.Create().WithName("PM").AsProjectManager().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(dev1, TeamRole.Developer, admin);
        team.AddMember(dev2, TeamRole.Developer, admin);
        team.AddMember(pm, TeamRole.ProjectManager, admin);

        // Act
        var developers = team.GetMembersByRole(TeamRole.Developer);
        var projectManagers = team.GetMembersByRole(TeamRole.ProjectManager);

        // Assert
        developers.Should().HaveCount(2);
        developers.Should().OnlyContain(m => m.Role == TeamRole.Developer);
        projectManagers.Should().HaveCount(1); 
        projectManagers.Should().OnlyContain(m => m.Role == TeamRole.ProjectManager);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetProjectManagers_ShouldReturnAllProjectManagers()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var pm1 = UserBuilder.Create().WithName("PM 1").AsProjectManager().Build();
        var pm2 = UserBuilder.Create().WithName("PM 2").AsProjectManager().Build();
        var dev = UserBuilder.Create().WithName("Dev").AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(pm1, TeamRole.ProjectManager, admin);
        team.AddMember(pm2, TeamRole.ProjectManager, admin);
        team.AddMember(dev, TeamRole.Developer, admin);

        // Act
        var projectManagers = team.GetProjectManagers();

        // Assert
        projectManagers.Should().HaveCount(2);
        projectManagers.Should().OnlyContain(m => m.Role == TeamRole.ProjectManager);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetTeamLeads_ShouldReturnAllTeamLeads()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var lead1 = UserBuilder.Create().WithName("Lead 1").AsDeveloper().Build();
        var lead2 = UserBuilder.Create().WithName("Lead 2").AsDeveloper().Build();
        var dev = UserBuilder.Create().WithName("Dev").AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(lead1, TeamRole.TeamLead, admin);
        team.AddMember(lead2, TeamRole.TeamLead, admin);
        team.AddMember(dev, TeamRole.Developer, admin);

        // Act
        var teamLeads = team.GetTeamLeads();

        // Assert
        teamLeads.Should().HaveCount(2);
        teamLeads.Should().OnlyContain(m => m.Role == TeamRole.TeamLead);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDevelopers_ShouldReturnAllDevelopers()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var dev1 = UserBuilder.Create().WithName("Dev 1").AsDeveloper().Build();
        var dev2 = UserBuilder.Create().WithName("Dev 2").AsDeveloper().Build();
        var pm = UserBuilder.Create().WithName("PM").AsProjectManager().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(dev1, TeamRole.Developer, admin);
        team.AddMember(dev2, TeamRole.Developer, admin);
        team.AddMember(pm, TeamRole.ProjectManager, admin);

        // Act
        var developers = team.GetDevelopers();

        // Assert
        developers.Should().HaveCount(2);
        developers.Should().OnlyContain(m => m.Role == TeamRole.Developer);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void HasRoom_WhenBelowMaxMembers_ShouldReturnTrue()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();

        // Act & Assert
        team.HasRoom().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]  
    public void HasRoom_WhenAtMaxMembers_ShouldReturnFalse()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        // Add maximum number of members
        for (int i = 0; i < Team.MaxMembers; i++)
        {
            var user = UserBuilder.Create().WithName($"User {i}").WithEmail($"user{i}@example.com").Build();
            team.AddMember(user, TeamRole.Developer, admin);
        }

        // Act & Assert
        team.HasRoom().Should().BeFalse();
        team.MemberCount.Should().Be(Team.MaxMembers);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void HasLeadership_WhenNoLeaders_ShouldReturnFalse()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var dev = UserBuilder.Create().AsDeveloper().Build();
        var viewer = UserBuilder.Create().AsViewer().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(dev, TeamRole.Developer, admin);
        team.AddMember(viewer, TeamRole.Viewer, admin);

        // Act & Assert
        team.HasLeadership().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void HasLeadership_WhenHasProjectManager_ShouldReturnTrue()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var pm = UserBuilder.Create().AsProjectManager().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(pm, TeamRole.ProjectManager, admin);

        // Act & Assert
        team.HasLeadership().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void HasLeadership_WhenHasTeamLead_ShouldReturnTrue()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var lead = UserBuilder.Create().AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(lead, TeamRole.TeamLead, admin);

        // Act & Assert
        team.HasLeadership().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetMember_WhenUserIsMember_ShouldReturnTeamMember()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(user, TeamRole.Developer, admin);

        // Act
        var member = team.GetMember(user.Id);

        // Assert
        member.Should().NotBeNull();
        member!.UserId.Should().Be(user.Id);
        member.Role.Should().Be(TeamRole.Developer);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetMember_WhenUserNotMember_ShouldReturnNull()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var userId = UserIdBuilder.Create().Build();

        // Act
        var member = team.GetMember(userId);

        // Assert
        member.Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void MaxMembers_ShouldBe50()
    {
        // Act & Assert
        Team.MaxMembers.Should().Be(50);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeMemberRole_WithValidParameters_ShouldChangeRole()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var admin = UserBuilder.Create().AsAdmin().Build();

        team.AddMember(user, TeamRole.Developer, admin);
        var newRole = TeamRole.TeamLead;

        // Act
        team.ChangeMemberRole(user, newRole, admin);

        // Assert
        var member = team.GetMember(user.Id);
        member.Should().NotBeNull();
        member!.Role.Should().Be(newRole);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void ChangeMemberRole_WithNullUser_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        User user = null!;
        var changedBy = UserBuilder.Create().AsAdmin().Build();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => team.ChangeMemberRole(user, TeamRole.TeamLead, changedBy));
        exception.ParamName.Should().Be("user");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void ChangeMemberRole_WithNullChangedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        User changedBy = null!;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => team.ChangeMemberRole(user, TeamRole.TeamLead, changedBy));
        exception.ParamName.Should().Be("changedBy");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeMemberRole_WhenUserNotMember_ShouldThrowDomainException()
    {
        // Arrange
        var team = TeamBuilder.Create().Build();
        var user = UserBuilder.Create().AsDeveloper().Build();
        var changedBy = UserBuilder.Create().AsAdmin().Build();

        // Act & Assert
        AssertDomainException(() => team.ChangeMemberRole(user, TeamRole.TeamLead, changedBy), "User is not a member of this team");
    }
}