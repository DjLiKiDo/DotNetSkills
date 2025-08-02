namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class UserLeftTeamDomainEventTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var domainEvent = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.Role.Should().Be(role);
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
        domainEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.OccurredAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_ShouldInheritFromBaseDomainEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var domainEvent = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert
        domainEvent.Should().BeAssignableTo<BaseDomainEvent>();
    }

    [Theory]
    [InlineData(TeamRole.Developer)]
    [InlineData(TeamRole.ProjectManager)]
    [InlineData(TeamRole.TeamLead)]
    [InlineData(TeamRole.Viewer)]
    [Trait("TestType", "Creation")]
    public void Constructor_WithDifferentRoles_ShouldCreateEvent(TeamRole role)
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert
        domainEvent.Role.Should().Be(role);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TeamId.Should().Be(teamId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValues_ShouldHaveEqualBusinessProperties()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        var event1 = new UserLeftTeamDomainEvent(userId, teamId, role);
        var event2 = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Act & Assert - Business properties should be equal
        event1.UserId.Should().Be(event2.UserId);
        event1.TeamId.Should().Be(event2.TeamId);
        event1.Role.Should().Be(event2.Role);
        
        // Infrastructure properties will be different
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
        event1.Should().NotBe(event2); // Records are different due to CorrelationId/OccurredAt
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentUserIds_ShouldNotBeEqual()
    {
        // Arrange
        var userId1 = UserIdBuilder.Create().Build();
        var userId2 = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        var event1 = new UserLeftTeamDomainEvent(userId1, teamId, role);
        var event2 = new UserLeftTeamDomainEvent(userId2, teamId, role);

        // Act & Assert
        event1.Should().NotBe(event2);
        (event1 == event2).Should().BeFalse();
        (event1 != event2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentTeamIds_ShouldNotBeEqual()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId1 = TeamIdBuilder.Create().Build();
        var teamId2 = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        var event1 = new UserLeftTeamDomainEvent(userId, teamId1, role);
        var event2 = new UserLeftTeamDomainEvent(userId, teamId2, role);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentRoles_ShouldNotBeEqual()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();

        var event1 = new UserLeftTeamDomainEvent(userId, teamId, TeamRole.Developer);
        var event2 = new UserLeftTeamDomainEvent(userId, teamId, TeamRole.ProjectManager);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalUserId = UserIdBuilder.Create().Build();
        var newUserId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        var originalEvent = new UserLeftTeamDomainEvent(originalUserId, teamId, role);

        // Act
        var newEvent = originalEvent with { UserId = newUserId };

        // Assert
        originalEvent.UserId.Should().Be(originalUserId);
        newEvent.UserId.Should().Be(newUserId);
        newEvent.TeamId.Should().Be(teamId);
        newEvent.Role.Should().Be(role);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Record_ShouldSupportWithExpressionForRole()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var originalRole = TeamRole.Developer;
        var newRole = TeamRole.TeamLead;

        var originalEvent = new UserLeftTeamDomainEvent(userId, teamId, originalRole);

        // Act
        var newEvent = originalEvent with { Role = newRole };

        // Assert
        originalEvent.Role.Should().Be(originalRole);
        newEvent.Role.Should().Be(newRole);
        newEvent.UserId.Should().Be(userId);
        newEvent.TeamId.Should().Be(teamId);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnMeaningfulRepresentation()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        var domainEvent = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Act
        var result = domainEvent.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(nameof(UserLeftTeamDomainEvent));
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveUniqueCorrelationIds()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserLeftTeamDomainEvent(userId, teamId, role);
        var event2 = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveSequentialOccurredOnTimes()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserLeftTeamDomainEvent(userId, teamId, role);
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert
        event2.OccurredAt.Should().BeOnOrAfter(event1.OccurredAt);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithInvalidRole_ShouldCreateEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var invalidRole = (TeamRole)999;

        // Act
        var domainEvent = new UserLeftTeamDomainEvent(userId, teamId, invalidRole);

        // Assert
        domainEvent.Role.Should().Be(invalidRole);
        domainEvent.UserId.Should().Be(userId);
        domainEvent.TeamId.Should().Be(teamId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithSameTeamAndUser_DifferentRoles_ShouldCreateDifferentEvents()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();

        // Act
        var developerEvent = new UserLeftTeamDomainEvent(userId, teamId, TeamRole.Developer);
        var leadEvent = new UserLeftTeamDomainEvent(userId, teamId, TeamRole.TeamLead);

        // Assert
        developerEvent.Should().NotBe(leadEvent);
        developerEvent.UserId.Should().Be(leadEvent.UserId);
        developerEvent.TeamId.Should().Be(leadEvent.TeamId);
        developerEvent.Role.Should().NotBe(leadEvent.Role);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithMultipleUsersLeavingSameTeam_ShouldCreateDistinctEvents()
    {
        // Arrange
        var user1Id = UserIdBuilder.Create().Build();
        var user2Id = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserLeftTeamDomainEvent(user1Id, teamId, role);
        var event2 = new UserLeftTeamDomainEvent(user2Id, teamId, role);

        // Assert
        event1.Should().NotBe(event2);
        event1.TeamId.Should().Be(event2.TeamId);
        event1.Role.Should().Be(event2.Role);
        event1.UserId.Should().NotBe(event2.UserId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithSameUserLeavingMultipleTeams_ShouldCreateDistinctEvents()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var team1Id = TeamIdBuilder.Create().Build();
        var team2Id = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserLeftTeamDomainEvent(userId, team1Id, role);
        var event2 = new UserLeftTeamDomainEvent(userId, team2Id, role);

        // Assert
        event1.Should().NotBe(event2);
        event1.UserId.Should().Be(event2.UserId);
        event1.Role.Should().Be(event2.Role);
        event1.TeamId.Should().NotBe(event2.TeamId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void JoinAndLeaveEvents_WithSameParameters_ShouldNotBeEqual()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var joinedEvent = new UserJoinedTeamDomainEvent(userId, teamId, role);
        var leftEvent = new UserLeftTeamDomainEvent(userId, teamId, role);

        // Assert - Different event types should not be equal even with same data
        joinedEvent.Should().NotBe(leftEvent);
        joinedEvent.UserId.Should().Be(leftEvent.UserId);
        joinedEvent.TeamId.Should().Be(leftEvent.TeamId);
        joinedEvent.Role.Should().Be(leftEvent.Role);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UserLeftTeamEvent_ShouldRepresentPreviousRole()
    {
        // Arrange - User was previously a TeamLead but left the team
        var userId = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var previousRole = TeamRole.TeamLead;

        // Act
        var leftEvent = new UserLeftTeamDomainEvent(userId, teamId, previousRole);

        // Assert
        leftEvent.Role.Should().Be(previousRole);
        leftEvent.UserId.Should().Be(userId);
        leftEvent.TeamId.Should().Be(teamId);
    }
}