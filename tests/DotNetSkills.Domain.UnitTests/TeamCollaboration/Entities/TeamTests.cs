using DotNetSkills.Domain.TeamCollaboration.Entities;
using DotNetSkills.Domain.TeamCollaboration.Enums;
using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
using DotNetSkills.Domain.UserManagement.Entities;
using DotNetSkills.Domain.UserManagement.Enums;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using FluentAssertions;

namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Entities;

[Trait("Category", "Unit")]
public class TeamTests
{
    private readonly User _admin;
    private readonly User _projectManager;
    private readonly User _developer;

    public TeamTests()
    {
        _admin = User.Create("Admin User", new EmailAddress("admin@test.com"), UserRole.Admin);
        _projectManager = User.Create("PM User", new EmailAddress("pm@test.com"), UserRole.ProjectManager);
        _developer = User.Create("Dev User", new EmailAddress("dev@test.com"), UserRole.Developer);
    }

    [Fact]
    public void Create_ValidInput_ShouldCreateTeamSuccessfully()
    {
        // Arrange
        var teamName = "Test Team";
        var description = "A test team";

        // Act
        var team = Team.Create(teamName, description, _projectManager);

        // Assert
        team.Should().NotBeNull();
        team.Name.Should().Be(teamName);
        team.Description.Should().Be(description);
        team.Status.Should().Be(TeamStatus.Active);
        team.MemberCount.Should().Be(0);
        team.Id.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNullCreator_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var act = () => Team.Create("Test Team", "Description", null!);
        
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange & Act & Assert
        var act = () => Team.Create("", "Description", _projectManager);
        
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void UpdateInfo_ValidInput_ShouldUpdateTeamInfo()
    {
        // Arrange
        var team = Team.Create("Original Name", "Original Description", _projectManager);
        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        team.UpdateInfo(newName, newDescription);

        // Assert
        team.Name.Should().Be(newName);
        team.Description.Should().Be(newDescription);
    }

    [Fact]
    public void AddMember_ValidInput_ShouldAddMemberToTeam()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);

        // Act
        team.AddMember(_developer, TeamRole.Developer, _projectManager);

        // Assert
        team.MemberCount.Should().Be(1);
        team.Members.Should().Contain(m => m.UserId == _developer.Id && m.Role == TeamRole.Developer);
    }

    [Fact]
    public void AddMember_DuplicateUser_ShouldThrowException()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);
        team.AddMember(_developer, TeamRole.Developer, _projectManager);

        // Act & Assert
        var act = () => team.AddMember(_developer, TeamRole.TeamLead, _projectManager);
        
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RemoveMember_ExistingMember_ShouldRemoveMemberFromTeam()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);
        team.AddMember(_developer, TeamRole.Developer, _projectManager);

        // Act
        team.RemoveMember(_developer, _projectManager);

        // Assert
        team.MemberCount.Should().Be(0);
        team.Members.Should().NotContain(m => m.UserId == _developer.Id);
    }

    [Fact]
    public void RemoveMember_NonExistingMember_ShouldThrowException()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);

        // Act & Assert
        var act = () => team.RemoveMember(_developer, _projectManager);
        
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ChangeMemberRole_ExistingMember_ShouldUpdateMemberRole()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);
        team.AddMember(_developer, TeamRole.Developer, _projectManager);

        // Act
        team.ChangeMemberRole(_developer, TeamRole.TeamLead, _projectManager);

        // Assert
        var member = team.GetMember(_developer.Id);
        member.Should().NotBeNull();
        member!.Role.Should().Be(TeamRole.TeamLead);
    }

    [Fact]
    public void ChangeStatus_ValidTransition_ShouldUpdateStatus()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);

        // Act
        team.ChangeStatus(TeamStatus.Inactive, _admin);

        // Assert
        team.Status.Should().Be(TeamStatus.Inactive);
    }

    [Fact]
    public void HasRoom_BelowMaxMembers_ShouldReturnTrue()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);

        // Act
        var hasRoom = team.HasRoom();

        // Assert
        hasRoom.Should().BeTrue();
    }

    [Fact]
    public void CanAcceptNewMembers_ActiveTeamWithSpace_ShouldReturnTrue()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);

        // Act
        var canAccept = team.CanAcceptNewMembers();

        // Assert
        canAccept.Should().BeTrue();
    }

    [Fact]
    public void GetMembersByRole_ReturnsCorrectMembers()
    {
        // Arrange
        var team = Team.Create("Test Team", "Description", _projectManager);
        var dev1 = User.Create("Dev1", new EmailAddress("dev1@test.com"), UserRole.Developer);
        var dev2 = User.Create("Dev2", new EmailAddress("dev2@test.com"), UserRole.Developer);
        
        team.AddMember(dev1, TeamRole.Developer, _projectManager);
        team.AddMember(dev2, TeamRole.Developer, _projectManager);
        team.AddMember(_developer, TeamRole.TeamLead, _projectManager);

        // Act
        var developers = team.GetMembersByRole(TeamRole.Developer);
        var teamLeads = team.GetMembersByRole(TeamRole.TeamLead);

        // Assert
        developers.Should().HaveCount(2);
        teamLeads.Should().HaveCount(1);
    }
}