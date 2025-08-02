namespace DotNetSkills.Domain.UnitTests.ProjectManagement.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectCreatedDomainEventTests : TestBase
{
    #region Constructor Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.Name.Should().Be(name);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy);
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
        domainEvent.OccurredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        domainEvent.OccurredAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldInheritFromBaseDomainEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.Should().BeAssignableTo<BaseDomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyName_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = string.Empty;
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.Name.Should().Be(string.Empty);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullName_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        string? name = null;
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name!, teamId, createdBy);

        // Assert
        domainEvent.Name.Should().BeNull();
        domainEvent.ProjectId.Should().Be(projectId);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithLongName_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = new string('A', 1000);
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.Name.Should().Be(name);
        domainEvent.Name.Length.Should().Be(1000);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithSpecialCharacters_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Project-Name_2024 (Phase-1) & Testing! 开发项目";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.Name.Should().Be(name);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValues_ShouldHaveEqualBusinessProperties()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);
        var event2 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Act & Assert - Business properties should be equal
        event1.ProjectId.Should().Be(event2.ProjectId);
        event1.Name.Should().Be(event2.Name);
        event1.TeamId.Should().Be(event2.TeamId);
        event1.CreatedBy.Should().Be(event2.CreatedBy);
        
        // Infrastructure properties will be different
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
        event1.Should().NotBe(event2); // Records are different due to CorrelationId/OccurredAt
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentProjectIds_ShouldNotBeEqual()
    {
        // Arrange
        var projectId1 = ProjectIdBuilder.Create().Build();
        var projectId2 = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectCreatedDomainEvent(projectId1, name, teamId, createdBy);
        var event2 = new ProjectCreatedDomainEvent(projectId2, name, teamId, createdBy);

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
        var projectId = ProjectIdBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectCreatedDomainEvent(projectId, "Project A", teamId, createdBy);
        var event2 = new ProjectCreatedDomainEvent(projectId, "Project B", teamId, createdBy);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentTeamIds_ShouldNotBeEqual()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId1 = TeamIdBuilder.Create().Build();
        var teamId2 = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectCreatedDomainEvent(projectId, name, teamId1, createdBy);
        var event2 = new ProjectCreatedDomainEvent(projectId, name, teamId2, createdBy);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentCreatedBy_ShouldNotBeEqual()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy1 = UserIdBuilder.Create().Build();
        var createdBy2 = UserIdBuilder.Create().Build();

        var event1 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy1);
        var event2 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy2);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    #endregion

    #region Record Features Tests

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalProjectId = ProjectIdBuilder.Create().Build();
        var newProjectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var originalEvent = new ProjectCreatedDomainEvent(originalProjectId, name, teamId, createdBy);

        // Act
        var newEvent = originalEvent with { ProjectId = newProjectId };

        // Assert
        originalEvent.ProjectId.Should().Be(originalProjectId);
        newEvent.ProjectId.Should().Be(newProjectId);
        newEvent.Name.Should().Be(name);
        newEvent.TeamId.Should().Be(teamId);
        newEvent.CreatedBy.Should().Be(createdBy);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportWithExpressionForName()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var originalName = "Original Project";
        var newName = "Updated Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var originalEvent = new ProjectCreatedDomainEvent(projectId, originalName, teamId, createdBy);

        // Act
        var newEvent = originalEvent with { Name = newName };

        // Assert
        originalEvent.Name.Should().Be(originalName);
        newEvent.Name.Should().Be(newName);
        newEvent.ProjectId.Should().Be(projectId);
        newEvent.TeamId.Should().Be(teamId);
        newEvent.CreatedBy.Should().Be(createdBy);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportDeconstruction()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Act
        var (extractedProjectId, extractedName, extractedTeamId, extractedCreatedBy) = domainEvent;

        // Assert
        extractedProjectId.Should().Be(projectId);
        extractedName.Should().Be(name);
        extractedTeamId.Should().Be(teamId);
        extractedCreatedBy.Should().Be(createdBy);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnMeaningfulRepresentation()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Act
        var result = domainEvent.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(nameof(ProjectCreatedDomainEvent));
    }

    #endregion

    #region Infrastructure Property Tests

    [Fact]
    [Trait("TestType", "Infrastructure")]
    public void DomainEvent_ShouldHaveUniqueCorrelationIds()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);
        var event2 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
    }

    [Fact]
    [Trait("TestType", "Infrastructure")]
    public void DomainEvent_ShouldHaveSequentialOccurredAtTimes()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        event2.OccurredAt.Should().BeOnOrAfter(event1.OccurredAt);
    }

    [Fact]
    [Trait("TestType", "Infrastructure")]
    public void CorrelationId_ShouldBeValidGuid()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
        Guid.TryParse(domainEvent.CorrelationId.ToString(), out _).Should().BeTrue();
    }

    #endregion

    #region Business Context Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldRepresentProjectCreationContext()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "E-Commerce Platform";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.Name.Should().Be(name);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy);
        
        // Event should capture the essential context for project creation
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldAllowDifferentProjectTypes()
    {
        // Arrange & Act
        var webProject = new ProjectCreatedDomainEvent(
            ProjectIdBuilder.Create().Build(),
            "Web Application",
            TeamIdBuilder.Create().Build(),
            UserIdBuilder.Create().Build());

        var mobileProject = new ProjectCreatedDomainEvent(
            ProjectIdBuilder.Create().Build(),
            "Mobile App",
            TeamIdBuilder.Create().Build(),
            UserIdBuilder.Create().Build());

        var apiProject = new ProjectCreatedDomainEvent(
            ProjectIdBuilder.Create().Build(),
            "API Service",
            TeamIdBuilder.Create().Build(),
            UserIdBuilder.Create().Build());

        // Assert
        webProject.Name.Should().Be("Web Application");
        mobileProject.Name.Should().Be("Mobile App");
        apiProject.Name.Should().Be("API Service");

        // All should be distinct events
        webProject.Should().NotBe(mobileProject);
        mobileProject.Should().NotBe(apiProject);
        webProject.Should().NotBe(apiProject);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldSupportMultipleTeamsCreatingProjects()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Shared Project Template";
        var team1Id = TeamIdBuilder.Create().Build();
        var team2Id = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var team1Event = new ProjectCreatedDomainEvent(projectId, name, team1Id, createdBy);
        var team2Event = new ProjectCreatedDomainEvent(projectId, name, team2Id, createdBy);

        // Assert
        team1Event.TeamId.Should().Be(team1Id);
        team2Event.TeamId.Should().Be(team2Id);
        team1Event.Should().NotBe(team2Event);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldSupportMultipleUsersCreatingProjects()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Development Project";
        var teamId = TeamIdBuilder.Create().Build();
        var user1Id = UserIdBuilder.Create().Build();
        var user2Id = UserIdBuilder.Create().Build();

        // Act
        var user1Event = new ProjectCreatedDomainEvent(projectId, name, teamId, user1Id);
        var user2Event = new ProjectCreatedDomainEvent(projectId, name, teamId, user2Id);

        // Assert
        user1Event.CreatedBy.Should().Be(user1Id);
        user2Event.CreatedBy.Should().Be(user2Id);
        user1Event.Should().NotBe(user2Event);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithWhitespaceOnlyName_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "   ";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.Name.Should().Be("   ");
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithEmptyGuidIds_ShouldCreateEvent()
    {
        // Arrange
        var projectId = new ProjectId(Guid.Empty);
        var name = "Test Project";
        var teamId = new TeamId(Guid.Empty);
        var createdBy = new UserId(Guid.Empty);

        // Act
        var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);

        // Assert
        domainEvent.ProjectId.Value.Should().Be(Guid.Empty);
        domainEvent.TeamId.Value.Should().Be(Guid.Empty);
        domainEvent.CreatedBy.Value.Should().Be(Guid.Empty);
        domainEvent.Name.Should().Be(name);
    }

    #endregion

    #region Performance Tests

    [Fact]
    [Trait("TestType", "Performance")]
    public void EventCreation_ShouldBeFast()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var name = "Performance Test Project";
        var teamId = TeamIdBuilder.Create().Build();
        var createdBy = UserIdBuilder.Create().Build();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            var domainEvent = new ProjectCreatedDomainEvent(projectId, name, teamId, createdBy);
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    #endregion
}