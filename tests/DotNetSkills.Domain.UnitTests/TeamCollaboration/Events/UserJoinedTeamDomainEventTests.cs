namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class UserJoinedTeamDomainEventTests : TestBase
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
        var domainEvent = new UserJoinedTeamDomainEvent(userId, teamId, role);

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
        var domainEvent = new UserJoinedTeamDomainEvent(userId, teamId, role);

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
        var domainEvent = new UserJoinedTeamDomainEvent(userId, teamId, role);

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

        var event1 = new UserJoinedTeamDomainEvent(userId, teamId, role);
        var event2 = new UserJoinedTeamDomainEvent(userId, teamId, role);

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

        var event1 = new UserJoinedTeamDomainEvent(userId1, teamId, role);
        var event2 = new UserJoinedTeamDomainEvent(userId2, teamId, role);

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

        var event1 = new UserJoinedTeamDomainEvent(userId, teamId1, role);
        var event2 = new UserJoinedTeamDomainEvent(userId, teamId2, role);

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

        var event1 = new UserJoinedTeamDomainEvent(userId, teamId, TeamRole.Developer);
        var event2 = new UserJoinedTeamDomainEvent(userId, teamId, TeamRole.ProjectManager);

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

        var originalEvent = new UserJoinedTeamDomainEvent(originalUserId, teamId, role);

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

        var originalEvent = new UserJoinedTeamDomainEvent(userId, teamId, originalRole);

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

        var domainEvent = new UserJoinedTeamDomainEvent(userId, teamId, role);

        // Act
        var result = domainEvent.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(nameof(UserJoinedTeamDomainEvent));
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
        var event1 = new UserJoinedTeamDomainEvent(userId, teamId, role);
        var event2 = new UserJoinedTeamDomainEvent(userId, teamId, role);

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
        var event1 = new UserJoinedTeamDomainEvent(userId, teamId, role);
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new UserJoinedTeamDomainEvent(userId, teamId, role);

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
        var domainEvent = new UserJoinedTeamDomainEvent(userId, teamId, invalidRole);

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
        var developerEvent = new UserJoinedTeamDomainEvent(userId, teamId, TeamRole.Developer);
        var leadEvent = new UserJoinedTeamDomainEvent(userId, teamId, TeamRole.TeamLead);

        // Assert
        developerEvent.Should().NotBe(leadEvent);
        developerEvent.UserId.Should().Be(leadEvent.UserId);
        developerEvent.TeamId.Should().Be(leadEvent.TeamId);
        developerEvent.Role.Should().NotBe(leadEvent.Role);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithMultipleUsersJoiningSameTeam_ShouldCreateDistinctEvents()
    {
        // Arrange
        var user1Id = UserIdBuilder.Create().Build();
        var user2Id = UserIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserJoinedTeamDomainEvent(user1Id, teamId, role);
        var event2 = new UserJoinedTeamDomainEvent(user2Id, teamId, role);

        // Assert
        event1.Should().NotBe(event2);
        event1.TeamId.Should().Be(event2.TeamId);
        event1.Role.Should().Be(event2.Role);
        event1.UserId.Should().NotBe(event2.UserId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithSameUserJoiningMultipleTeams_ShouldCreateDistinctEvents()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var team1Id = TeamIdBuilder.Create().Build();
        var team2Id = TeamIdBuilder.Create().Build();
        var role = TeamRole.Developer;

        // Act
        var event1 = new UserJoinedTeamDomainEvent(userId, team1Id, role);
        var event2 = new UserJoinedTeamDomainEvent(userId, team2Id, role);

        // Assert
        event1.Should().NotBe(event2);
        event1.UserId.Should().Be(event2.UserId);
        event1.Role.Should().Be(event2.Role);
        event1.TeamId.Should().NotBe(event2.TeamId);
    }
}