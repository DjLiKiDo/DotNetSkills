namespace DotNetSkills.Domain.UnitTests.ProjectManagement.Entities;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectTests : TestBase
{
    #region Creation Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateProject()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project for development";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = new Project(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        project.Id.Should().NotBe(ProjectId.New());
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.TeamId.Should().Be(teamId);
        project.PlannedEndDate.Should().Be(plannedEndDate);
        project.Status.Should().Be(ProjectStatus.Planning);
        project.StartDate.Should().BeNull();
        project.EndDate.Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Create_WithValidParameters_ShouldCreateProject()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project for development";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = Project.Create(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        project.Id.Should().NotBe(ProjectId.New());
        project.Name.Should().Be(name);
        project.Description.Should().Be(description);
        project.TeamId.Should().Be(teamId);
        project.PlannedEndDate.Should().Be(plannedEndDate);
        project.Status.Should().Be(ProjectStatus.Planning);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullDescription_ShouldCreateProject()
    {
        // Arrange
        var name = "Development Project";
        string? description = null;
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = new Project(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        project.Description.Should().BeNull();
        project.Name.Should().Be(name);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullPlannedEndDate_ShouldCreateProject()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        DateTime? plannedEndDate = null;
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = new Project(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        project.PlannedEndDate.Should().BeNull();
        project.Name.Should().Be(name);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldRaiseProjectCreatedDomainEvent()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = new Project(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        var domainEvent = project.DomainEvents.OfType<ProjectCreatedDomainEvent>().FirstOrDefault();
        domainEvent.Should().NotBeNull();
        domainEvent!.ProjectId.Should().Be(project.Id);
        domainEvent.Name.Should().Be(name);
        domainEvent.TeamId.Should().Be(teamId);
        domainEvent.CreatedBy.Should().Be(createdBy.Id);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithWhitespaceInName_ShouldTrimName()
    {
        // Arrange
        var name = "  Development Project  ";
        var description = "  A test project  ";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act
        var project = new Project(name, description, teamId, plannedEndDate, createdBy);

        // Assert
        project.Name.Should().Be("Development Project");
        project.Description.Should().Be("A test project");
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("TestType", "Validation")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act & Assert
        var action = () => new Project(invalidName!, description, teamId, plannedEndDate, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithParameterName("name");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullTeamId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        TeamId teamId = null!;
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act & Assert
        var action = () => new Project(name, description, teamId, plannedEndDate, createdBy);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName(nameof(teamId));
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullCreatedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        User createdBy = null!;

        // Act & Assert
        var action = () => new Project(name, description, teamId, plannedEndDate, createdBy);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName(nameof(createdBy));
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithUserWithoutPermission_ShouldThrowDomainException()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(30);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.Viewer)
            .Build();

        // Act & Assert
        var action = () => new Project(name, description, teamId, plannedEndDate, createdBy);
        action.Should().Throw<DomainException>()
            .WithMessage("User does not have permission to create projects");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithPastPlannedEndDate_ShouldThrowArgumentException()
    {
        // Arrange
        var name = "Development Project";
        var description = "A test project";
        var teamId = TeamIdBuilder.Create().Build();
        var plannedEndDate = DateTime.UtcNow.AddDays(-10);
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        // Act & Assert
        var action = () => new Project(name, description, teamId, plannedEndDate, createdBy);
        action.Should().Throw<ArgumentException>()
            .WithParameterName(nameof(plannedEndDate));
    }

    #endregion

    #region Status Management Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_WithValidUser_ShouldChangeStatusToActive()
    {
        // Arrange
        var project = CreateValidProject();
        var startedBy = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();

        // Act
        project.Start(startedBy);

        // Assert
        project.Status.Should().Be(ProjectStatus.Active);
        project.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        project.StartDate?.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_ShouldRaiseProjectStatusChangedDomainEvent()
    {
        // Arrange
        var project = CreateValidProject();
        var startedBy = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.ClearDomainEvents(); // Clear creation event

        // Act
        project.Start(startedBy);

        // Assert
        var domainEvent = project.DomainEvents.OfType<ProjectStatusChangedDomainEvent>().FirstOrDefault();
        domainEvent.Should().NotBeNull();
        domainEvent!.ProjectId.Should().Be(project.Id);
        domainEvent.PreviousStatus.Should().Be(ProjectStatus.Planning);
        domainEvent.NewStatus.Should().Be(ProjectStatus.Active);
        domainEvent.ChangedBy.Should().Be(startedBy.Id);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void PutOnHold_WithActiveProject_ShouldChangeStatusToOnHold()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);

        // Act
        project.PutOnHold(user);

        // Assert
        project.Status.Should().Be(ProjectStatus.OnHold);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Resume_WithOnHoldProject_ShouldChangeStatusToActive()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);
        project.PutOnHold(user);

        // Act
        project.Resume(user);

        // Assert
        project.Status.Should().Be(ProjectStatus.Active);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Complete_WithActiveProject_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);

        // Act
        project.Complete(user, hasActiveTasks: false);

        // Assert
        project.Status.Should().Be(ProjectStatus.Completed);
        project.EndDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        project.EndDate?.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Cancel_WithAnyStatus_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();

        // Act
        project.Cancel(user);

        // Assert
        project.Status.Should().Be(ProjectStatus.Cancelled);
        project.EndDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        project.EndDate?.Kind.Should().Be(DateTimeKind.Utc);
    }

    #endregion

    #region Permission Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_WithAdminRole_ShouldSucceed()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();

        // Act & Assert
        var action = () => project.Start(user);
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.Viewer)]
    [Trait("TestType", "Validation")]
    public void Start_WithUnauthorizedRole_ShouldThrowDomainException(UserRole role)
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(role)
            .Build();

        // Act & Assert
        var action = () => project.Start(user);
        action.Should().Throw<DomainException>()
            .WithMessage("User does not have permission to start this project");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Complete_WithActiveTasks_ShouldThrowDomainException()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);

        // Act & Assert
        var action = () => project.Complete(user, hasActiveTasks: true);
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot complete project with active tasks");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Resume_WithNonOnHoldProject_ShouldThrowDomainException()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();

        // Act & Assert
        var action = () => project.Resume(user);
        action.Should().Throw<DomainException>()
            .WithMessage("Can only resume projects that are on hold");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Cancel_WithCompletedProject_ShouldThrowDomainException()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);
        project.Complete(user, hasActiveTasks: false);

        // Act & Assert
        var action = () => project.Cancel(user);
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel completed projects");
    }

    #endregion

    #region Information Update Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithValidParameters_ShouldUpdateProjectInformation()
    {
        // Arrange
        var project = CreateValidProject();
        var newName = "Updated Project Name";
        var newDescription = "Updated description";
        var newPlannedEndDate = DateTime.UtcNow.AddDays(60);
        var updatedBy = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();

        // Act
        project.UpdateInfo(newName, newDescription, newPlannedEndDate, updatedBy);

        // Assert
        project.Name.Should().Be(newName);
        project.Description.Should().Be(newDescription);
        project.PlannedEndDate.Should().Be(newPlannedEndDate);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void UpdateInfo_WithCompletedProject_ShouldThrowDomainException()
    {
        // Arrange
        var project = CreateValidProject();
        var user = UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
        project.Start(user);
        project.Complete(user, hasActiveTasks: false);

        // Act & Assert
        var action = () => project.UpdateInfo("New Name", "New Description", null, user);
        action.Should().Throw<DomainException>()
            .WithMessage("Cannot modify completed projects");
    }

    #endregion

    #region Business Logic Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsActive_WithActiveStatus_ShouldReturnTrue()
    {
        // Arrange
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        project.Start(user);

        // Act & Assert
        project.IsActive().Should().BeTrue();
    }

    [Theory]
    [InlineData(ProjectStatus.Planning)]
    [InlineData(ProjectStatus.OnHold)]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Cancelled)]
    [Trait("TestType", "BusinessLogic")]
    public void IsActive_WithNonActiveStatus_ShouldReturnFalse(ProjectStatus status)
    {
        // Arrange
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        
        // Set the desired status
        if (status == ProjectStatus.OnHold)
        {
            project.Start(user);
            project.PutOnHold(user);
        }
        else if (status == ProjectStatus.Completed)
        {
            project.Start(user);
            project.Complete(user, false);
        }
        else if (status == ProjectStatus.Cancelled)
        {
            project.Cancel(user);
        }
        // Planning is the default status

        // Act & Assert
        project.IsActive().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsCompleted_WithCompletedStatus_ShouldReturnTrue()
    {
        // Arrange
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        project.Start(user);
        project.Complete(user, false);

        // Act & Assert
        project.IsCompleted().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithOverduePlannedEndDate_ShouldReturnTrue()
    {
        // Arrange - Create project with future date first, then start it and modify internally for testing
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        project.Start(user);
        
        // Use reflection to simulate an overdue situation for testing
        var plannedEndDateField = typeof(Project).GetField("<PlannedEndDate>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        plannedEndDateField?.SetValue(project, DateTime.UtcNow.AddDays(-10));

        // Act & Assert
        project.IsOverdue().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithCompletedProject_ShouldReturnFalse()
    {
        // Arrange - Create project, start it, set overdue date, then complete
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        project.Start(user);
        
        // Use reflection to set past planned end date
        var plannedEndDateField = typeof(Project).GetField("<PlannedEndDate>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        plannedEndDateField?.SetValue(project, DateTime.UtcNow.AddDays(-10));
        
        project.Complete(user, false);

        // Act & Assert
        project.IsOverdue().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDuration_WithStartedProject_ShouldReturnDuration()
    {
        // Arrange
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        project.Start(user);

        // Act
        var duration = project.GetDuration();

        // Assert
        duration.Should().NotBeNull();
        duration!.Value.Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDuration_WithNotStartedProject_ShouldReturnNull()
    {
        // Arrange
        var project = CreateValidProject();

        // Act
        var duration = project.GetDuration();

        // Assert
        duration.Should().BeNull();
    }

    [Theory]
    [InlineData(ProjectStatus.Planning, false)]
    [InlineData(ProjectStatus.Cancelled, false)]
    [InlineData(ProjectStatus.Active, true)]
    [InlineData(ProjectStatus.Completed, true)]
    [InlineData(ProjectStatus.OnHold, true)]
    [Trait("TestType", "BusinessLogic")]
    public void CanBeDeleted_ShouldReturnCorrectValue(ProjectStatus status, bool hasTasks)
    {
        // Arrange
        var project = CreateValidProject();
        var user = CreateAuthorizedUser(project.TeamId);
        
        // Set the desired status
        if (status == ProjectStatus.Active)
        {
            project.Start(user);
        }
        else if (status == ProjectStatus.OnHold)
        {
            project.Start(user);
            project.PutOnHold(user);
        }
        else if (status == ProjectStatus.Completed)
        {
            project.Start(user);
            project.Complete(user, false);
        }
        else if (status == ProjectStatus.Cancelled)
        {
            project.Cancel(user);
        }

        // Act
        var canBeDeleted = project.CanBeDeleted(hasTasks);

        // Assert
        var expectedResult = !hasTasks && (status == ProjectStatus.Planning || status == ProjectStatus.Cancelled);
        canBeDeleted.Should().Be(expectedResult);
    }

    #endregion

    #region Helper Methods

    private Project CreateValidProject()
    {
        var createdBy = UserBuilder.Create()
            .WithRole(UserRole.ProjectManager)
            .Build();

        return Project.Create(
            "Test Project",
            "Test Description",
            TeamIdBuilder.Create().Build(),
            DateTime.UtcNow.AddDays(30),
            createdBy);
    }


    private User CreateAuthorizedUser(TeamId teamId)
    {
        return UserBuilder.Create()
            .WithRole(UserRole.Admin)
            .Build();
    }

    #endregion
}