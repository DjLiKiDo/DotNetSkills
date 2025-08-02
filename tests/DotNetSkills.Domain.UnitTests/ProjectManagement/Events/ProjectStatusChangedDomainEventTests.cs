namespace DotNetSkills.Domain.UnitTests.ProjectManagement.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectStatusChangedDomainEventTests : TestBase
{
    #region Constructor Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
        domainEvent.ChangedBy.Should().Be(changedBy);
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
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.Should().BeAssignableTo<BaseDomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Theory]
    [InlineData(ProjectStatus.Planning, ProjectStatus.Active)]
    [InlineData(ProjectStatus.Active, ProjectStatus.OnHold)]
    [InlineData(ProjectStatus.OnHold, ProjectStatus.Active)]
    [InlineData(ProjectStatus.Active, ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Active, ProjectStatus.Cancelled)]
    [InlineData(ProjectStatus.Planning, ProjectStatus.Cancelled)]
    [InlineData(ProjectStatus.OnHold, ProjectStatus.Cancelled)]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidStatusTransitions_ShouldCreateEvent(ProjectStatus previousStatus, ProjectStatus newStatus)
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    [Theory]
    [InlineData(ProjectStatus.Completed, ProjectStatus.Planning)]
    [InlineData(ProjectStatus.Cancelled, ProjectStatus.Active)]
    [InlineData(ProjectStatus.Completed, ProjectStatus.OnHold)]
    [Trait("TestType", "Creation")]
    public void Constructor_WithInvalidStatusTransitions_ShouldStillCreateEvent(ProjectStatus previousStatus, ProjectStatus newStatus)
    {
        // Arrange - Domain events record what happened, validation happens elsewhere
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithSameStatus_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var status = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, status, status, changedBy);

        // Assert
        domainEvent.PreviousStatus.Should().Be(status);
        domainEvent.NewStatus.Should().Be(status);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    #endregion

    #region Equality Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValues_ShouldHaveEqualBusinessProperties()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);
        var event2 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Act & Assert - Business properties should be equal
        event1.ProjectId.Should().Be(event2.ProjectId);
        event1.PreviousStatus.Should().Be(event2.PreviousStatus);
        event1.NewStatus.Should().Be(event2.NewStatus);
        event1.ChangedBy.Should().Be(event2.ChangedBy);
        
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
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectStatusChangedDomainEvent(projectId1, previousStatus, newStatus, changedBy);
        var event2 = new ProjectStatusChangedDomainEvent(projectId2, previousStatus, newStatus, changedBy);

        // Act & Assert
        event1.Should().NotBe(event2);
        (event1 == event2).Should().BeFalse();
        (event1 != event2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentPreviousStatus_ShouldNotBeEqual()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Planning, newStatus, changedBy);
        var event2 = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.OnHold, newStatus, changedBy);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentNewStatus_ShouldNotBeEqual()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var changedBy = UserIdBuilder.Create().Build();

        var event1 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, ProjectStatus.Active, changedBy);
        var event2 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, ProjectStatus.Cancelled, changedBy);

        // Act & Assert
        event1.Should().NotBe(event2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentChangedBy_ShouldNotBeEqual()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy1 = UserIdBuilder.Create().Build();
        var changedBy2 = UserIdBuilder.Create().Build();

        var event1 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy1);
        var event2 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy2);

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
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var originalEvent = new ProjectStatusChangedDomainEvent(originalProjectId, previousStatus, newStatus, changedBy);

        // Act
        var newEvent = originalEvent with { ProjectId = newProjectId };

        // Assert
        originalEvent.ProjectId.Should().Be(originalProjectId);
        newEvent.ProjectId.Should().Be(newProjectId);
        newEvent.PreviousStatus.Should().Be(previousStatus);
        newEvent.NewStatus.Should().Be(newStatus);
        newEvent.ChangedBy.Should().Be(changedBy);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportWithExpressionForStatus()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var originalPreviousStatus = ProjectStatus.Planning;
        var originalNewStatus = ProjectStatus.Active;
        var newPreviousStatus = ProjectStatus.Active;
        var newNewStatus = ProjectStatus.Completed;
        var changedBy = UserIdBuilder.Create().Build();

        var originalEvent = new ProjectStatusChangedDomainEvent(projectId, originalPreviousStatus, originalNewStatus, changedBy);

        // Act
        var newEvent = originalEvent with { PreviousStatus = newPreviousStatus, NewStatus = newNewStatus };

        // Assert
        originalEvent.PreviousStatus.Should().Be(originalPreviousStatus);
        originalEvent.NewStatus.Should().Be(originalNewStatus);
        newEvent.PreviousStatus.Should().Be(newPreviousStatus);
        newEvent.NewStatus.Should().Be(newNewStatus);
        newEvent.ProjectId.Should().Be(projectId);
        newEvent.ChangedBy.Should().Be(changedBy);
        originalEvent.Should().NotBe(newEvent);
    }

    [Fact]
    [Trait("TestType", "RecordFeatures")]
    public void Record_ShouldSupportDeconstruction()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Act
        var (extractedProjectId, extractedPreviousStatus, extractedNewStatus, extractedChangedBy) = domainEvent;

        // Assert
        extractedProjectId.Should().Be(projectId);
        extractedPreviousStatus.Should().Be(previousStatus);
        extractedNewStatus.Should().Be(newStatus);
        extractedChangedBy.Should().Be(changedBy);
    }

    #endregion

    #region Serialization Tests

    [Fact]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnMeaningfulRepresentation()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Act
        var result = domainEvent.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(nameof(ProjectStatusChangedDomainEvent));
    }

    #endregion

    #region Infrastructure Property Tests

    [Fact]
    [Trait("TestType", "Infrastructure")]
    public void DomainEvent_ShouldHaveUniqueCorrelationIds()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);
        var event2 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        event1.CorrelationId.Should().NotBe(event2.CorrelationId);
    }

    [Fact]
    [Trait("TestType", "Infrastructure")]
    public void DomainEvent_ShouldHaveSequentialOccurredAtTimes()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);
        Thread.Sleep(1); // Ensure different timestamps
        var event2 = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        event2.OccurredAt.Should().BeOnOrAfter(event1.OccurredAt);
    }

    #endregion

    #region Business Context Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldRepresentStatusTransitionContext()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
        domainEvent.ChangedBy.Should().Be(changedBy);
        
        // Event should capture the essential context for status change
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldCaptureStatusProgressionDirection()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();

        // Act - Create events for different progression directions
        var startEvent = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Planning, ProjectStatus.Active, changedBy);
        var pauseEvent = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.OnHold, changedBy);
        var resumeEvent = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.OnHold, ProjectStatus.Active, changedBy);
        var completeEvent = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.Completed, changedBy);
        var cancelEvent = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.Cancelled, changedBy);

        // Assert
        startEvent.PreviousStatus.Should().Be(ProjectStatus.Planning);
        startEvent.NewStatus.Should().Be(ProjectStatus.Active);
        
        pauseEvent.PreviousStatus.Should().Be(ProjectStatus.Active);
        pauseEvent.NewStatus.Should().Be(ProjectStatus.OnHold);
        
        resumeEvent.PreviousStatus.Should().Be(ProjectStatus.OnHold);
        resumeEvent.NewStatus.Should().Be(ProjectStatus.Active);
        
        completeEvent.PreviousStatus.Should().Be(ProjectStatus.Active);
        completeEvent.NewStatus.Should().Be(ProjectStatus.Completed);
        
        cancelEvent.PreviousStatus.Should().Be(ProjectStatus.Active);
        cancelEvent.NewStatus.Should().Be(ProjectStatus.Cancelled);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldSupportMultipleProjectsChangingStatus()
    {
        // Arrange
        var project1Id = ProjectIdBuilder.Create().Build();
        var project2Id = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var project1Event = new ProjectStatusChangedDomainEvent(project1Id, previousStatus, newStatus, changedBy);
        var project2Event = new ProjectStatusChangedDomainEvent(project2Id, previousStatus, newStatus, changedBy);

        // Assert
        project1Event.ProjectId.Should().Be(project1Id);
        project2Event.ProjectId.Should().Be(project2Id);
        project1Event.Should().NotBe(project2Event);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldSupportMultipleUsersChangingProjectStatus()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var user1Id = UserIdBuilder.Create().Build();
        var user2Id = UserIdBuilder.Create().Build();

        // Act
        var user1Event = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, user1Id);
        var user2Event = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, user2Id);

        // Assert
        user1Event.ChangedBy.Should().Be(user1Id);
        user2Event.ChangedBy.Should().Be(user2Id);
        user1Event.Should().NotBe(user2Event);
    }

    #endregion

    #region Status Transition Analysis Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldIdentifyProgressiveTransitions()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var planningToActive = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Planning, ProjectStatus.Active, changedBy);
        var activeToCompleted = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.Completed, changedBy);

        // Assert
        planningToActive.PreviousStatus.IsActive().Should().BeTrue();
        planningToActive.NewStatus.IsActive().Should().BeTrue();
        
        activeToCompleted.PreviousStatus.IsActive().Should().BeTrue();
        activeToCompleted.NewStatus.IsFinalized().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldIdentifyRegressiveTransitions()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var activeToOnHold = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.OnHold, changedBy);
        var activeToCancelled = new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.Cancelled, changedBy);

        // Assert
        activeToOnHold.PreviousStatus.IsActive().Should().BeTrue();
        activeToOnHold.NewStatus.IsActive().Should().BeTrue(); // OnHold is still considered active
        
        activeToCancelled.PreviousStatus.IsActive().Should().BeTrue();
        activeToCancelled.NewStatus.IsFinalized().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Event_ShouldTrackStatusChangeFrequency()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var changedBy = UserIdBuilder.Create().Build();
        var statusChanges = new List<ProjectStatusChangedDomainEvent>();

        // Act - Simulate multiple status changes
        statusChanges.Add(new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Planning, ProjectStatus.Active, changedBy));
        statusChanges.Add(new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.OnHold, changedBy));
        statusChanges.Add(new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.OnHold, ProjectStatus.Active, changedBy));
        statusChanges.Add(new ProjectStatusChangedDomainEvent(projectId, ProjectStatus.Active, ProjectStatus.Completed, changedBy));

        // Assert
        statusChanges.Should().HaveCount(4);
        statusChanges.All(e => e.ProjectId == projectId).Should().BeTrue();
        statusChanges.All(e => e.ChangedBy == changedBy).Should().BeTrue();
        
        // Verify progression
        statusChanges[0].PreviousStatus.Should().Be(ProjectStatus.Planning);
        statusChanges[0].NewStatus.Should().Be(ProjectStatus.Active);
        statusChanges[3].PreviousStatus.Should().Be(ProjectStatus.Active);
        statusChanges[3].NewStatus.Should().Be(ProjectStatus.Completed);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithInvalidEnumValues_ShouldCreateEvent()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var invalidPreviousStatus = (ProjectStatus)999;
        var invalidNewStatus = (ProjectStatus)(-1);
        var changedBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, invalidPreviousStatus, invalidNewStatus, changedBy);

        // Assert
        domainEvent.PreviousStatus.Should().Be(invalidPreviousStatus);
        domainEvent.NewStatus.Should().Be(invalidNewStatus);
        domainEvent.ProjectId.Should().Be(projectId);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithEmptyGuidIds_ShouldCreateEvent()
    {
        // Arrange
        var projectId = new ProjectId(Guid.Empty);
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = new UserId(Guid.Empty);

        // Act
        var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);

        // Assert
        domainEvent.ProjectId.Value.Should().Be(Guid.Empty);
        domainEvent.ChangedBy.Value.Should().Be(Guid.Empty);
        domainEvent.PreviousStatus.Should().Be(previousStatus);
        domainEvent.NewStatus.Should().Be(newStatus);
    }

    #endregion

    #region Performance Tests

    [Fact]
    [Trait("TestType", "Performance")]
    public void EventCreation_ShouldBeFast()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var previousStatus = ProjectStatus.Planning;
        var newStatus = ProjectStatus.Active;
        var changedBy = UserIdBuilder.Create().Build();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            var domainEvent = new ProjectStatusChangedDomainEvent(projectId, previousStatus, newStatus, changedBy);
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    #endregion
}