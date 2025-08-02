namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamCreatedDomainEventTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.Name.Should().Be(name);
        domainEvent.CreatedBy.Should().Be(createdBy);
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
        domainEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.OccurredAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_ShouldInheritFromBaseDomainEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Should().BeAssignableTo<BaseDomainEvent>();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyName_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = string.Empty;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Name.Should().Be(string.Empty);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithWhitespaceName_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "   ";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Name.Should().Be("   ");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithLongName_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = new string('A', 1000);
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Name.Should().Be(name);
        domainEvent.Name.Length.Should().Be(1000);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValues_ShouldHaveEqualBusinessProperties()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new TeamCreatedDomainEvent(teamId, name, createdBy);
        var event2 = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Act & Assert - Business properties should be equal
        event1.TeamId.Should().Be(event2.TeamId);
        event1.Name.Should().Be(event2.Name);
        event1.CreatedBy.Should().Be(event2.CreatedBy);
        
        // Infrastructure properties will be different
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
        event1.Should().NotBe(event2); // Records are different due to CorrelationId/OccurredAt
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentTeamIds_ShouldNotBeEqual()
    {
        // Arrange
        var teamId1 = TeamIdBuilder.Create().Build();
        var teamId2 = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new TeamCreatedDomainEvent(teamId1, name, createdBy);
        var event2 = new TeamCreatedDomainEvent(teamId2, name, createdBy);

        // Act & Assert
        event1.Should().NotBe(event2);
        (event1 == event2).Should().BeFalse();
        (event1 != event2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentNames_ShouldNotBeEqual()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new TeamCreatedDomainEvent(teamId, "Development Team", createdBy);
        var event2 = new TeamCreatedDomainEvent(teamId, "Testing Team", createdBy);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentCreatedBy_ShouldNotBeEqual()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy1 = UserIdBuilder.Create().Build();
        var createdBy2 = UserIdBuilder.Create().Build();

        var event1 = new TeamCreatedDomainEvent(teamId, name, createdBy1);
        var event2 = new TeamCreatedDomainEvent(teamId, name, createdBy2);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalTeamId = TeamIdBuilder.Create().Build();
        var newTeamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        var originalEvent = new TeamCreatedDomainEvent(originalTeamId, name, createdBy);

        // Act
        var newEvent = originalEvent with { TeamId = newTeamId };

        // Assert
        originalEvent.TeamId.Should().Be(originalTeamId);
        newEvent.TeamId.Should().Be(newTeamId);
        newEvent.Name.Should().Be(name);
        newEvent.CreatedBy.Should().Be(createdBy);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnMeaningfulRepresentation()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Act
        var result = domainEvent.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(nameof(TeamCreatedDomainEvent));
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveUniqueCorrelationIds()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new TeamCreatedDomainEvent(teamId, name, createdBy);
        var event2 = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveSequentialOccurredOnTimes()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development Team";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new TeamCreatedDomainEvent(teamId, name, createdBy);
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        event2.OccurredAt.Should().BeOnOrAfter(event1.OccurredAt);
    }

    [Theory]
    [InlineData(null)]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithNullName_ShouldCreateEvent(string? nullName)
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, nullName!, createdBy);

        // Assert
        domainEvent.Name.Should().BeNull();
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithSpecialCharacters_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Development-Team_2024 (Phase-1) & Testing!";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Name.Should().Be(name);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithUnicodeCharacters_ShouldCreateEvent()
    {
        // Arrange
        var teamId = TeamIdBuilder.Create().Build();
        var name = "Équipe de Développement 开发团队";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new TeamCreatedDomainEvent(teamId, name, createdBy);

        // Assert
        domainEvent.Name.Should().Be(name);
    }
}