namespace DotNetSkills.Domain.UnitTests.TaskExecution.Entities;

using Task = DotNetSkills.Domain.TaskExecution.Entities.Task;
using TaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TaskExecution")]
public class TaskTests : TestBase
{
    #region Constructor Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateTask()
    {
        // Arrange
        var title = "Test Task";
        var description = "Test Description";
        var projectId = ProjectIdBuilder.Create().Build();
        var priority = TaskPriority.High;
        var estimatedHours = 8;
        var dueDate = DateTime.UtcNow.AddDays(7);
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var task = new Task(title, description, projectId, priority, null, estimatedHours, dueDate, createdBy);

        // Assert
        task.Should().NotBeNull();
        task.Id.Should().NotBe(TaskId.New());
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.ProjectId.Should().Be(projectId);
        task.Priority.Should().Be(priority);
        task.ParentTaskId.Should().BeNull();
        task.EstimatedHours.Should().Be(estimatedHours);
        task.DueDate.Should().Be(dueDate);
        task.Status.Should().Be(TaskStatus.ToDo);
        task.AssignedUserId.Should().BeNull();
        task.StartedAt.Should().BeNull();
        task.CompletedAt.Should().BeNull();
        task.ActualHours.Should().BeNull();
        task.Subtasks.Should().BeEmpty();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithSubtask_ShouldCreateSubtask()
    {
        // Arrange
        var parentTaskId = TaskIdBuilder.Create().Build();
        var title = "Subtask";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var subtask = new Task(title, null, projectId, TaskPriority.Medium, parentTaskId, null, null, createdBy);

        // Assert
        subtask.ParentTaskId.Should().Be(parentTaskId);
        subtask.IsSubtask().Should().BeTrue();
        subtask.HasSubtasks().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithMinimalParameters_ShouldCreateTask()
    {
        // Arrange
        var title = "Simple Task";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var task = new Task(title, null, projectId, TaskPriority.Low, null, null, null, createdBy);

        // Assert
        task.Title.Should().Be(title);
        task.Description.Should().BeNull();
        task.EstimatedHours.Should().BeNull();
        task.DueDate.Should().BeNull();
        task.Priority.Should().Be(TaskPriority.Low);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyOrWhitespaceTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => new Task(invalidTitle, "Description", projectId, TaskPriority.Medium, null, null, null, createdBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("title");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullProjectId_ShouldThrowArgumentNullException()
    {
        // Arrange
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => new Task("Title", "Description", null!, TaskPriority.Medium, null, null, null, createdBy);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("projectId");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullCreatedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();

        // Act & Assert
        var action = () => new Task("Title", "Description", projectId, TaskPriority.Medium, null, null, null, null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("createdBy");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    [Trait("TestType", "Creation")]
    public void Constructor_WithInvalidEstimatedHours_ShouldThrowArgumentException(int invalidHours)
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => new Task("Title", "Description", projectId, TaskPriority.Medium, null, invalidHours, null, createdBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("estimatedHours");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithPastDueDate_ShouldThrowArgumentException()
    {
        // Arrange
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var action = () => new Task("Title", "Description", projectId, TaskPriority.Medium, null, null, pastDate, createdBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("dueDate");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldTrimTitleAndDescription()
    {
        // Arrange
        var title = "  Task Title  ";
        var description = "  Task Description  ";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var task = new Task(title, description, projectId, TaskPriority.Medium, null, null, null, createdBy);

        // Assert
        task.Title.Should().Be("Task Title");
        task.Description.Should().Be("Task Description");
    }

    [Fact]
    [Trait("TestType", "DomainEvents")]
    public void Constructor_ShouldRaiseTaskCreatedDomainEvent()
    {
        // Arrange
        var title = "Test Task";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var task = new Task(title, "Description", projectId, TaskPriority.Medium, null, null, null, createdBy);

        // Assert
        var domainEvents = task.DomainEvents;
        domainEvents.Should().HaveCount(1);
        var taskCreatedEvent = domainEvents.First().Should().BeOfType<TaskCreatedDomainEvent>().Subject;
        taskCreatedEvent.TaskId.Should().Be(task.Id);
        taskCreatedEvent.Title.Should().Be(title);
        taskCreatedEvent.ProjectId.Should().Be(projectId);
        taskCreatedEvent.ParentTaskId.Should().BeNull();
        taskCreatedEvent.CreatedBy.Should().Be(createdBy.Id);
    }

    #endregion

    #region Create Factory Method Tests

    [Fact]
    [Trait("TestType", "Creation")]
    public void Create_WithValidParameters_ShouldCreateTask()
    {
        // Arrange
        var title = "Factory Task";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var task = Task.Create(title, "Description", projectId, TaskPriority.High, null, 8, DateTime.UtcNow.AddDays(7), createdBy);

        // Assert
        task.Should().NotBeNull();
        task.Title.Should().Be(title);
        task.Priority.Should().Be(TaskPriority.High);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithParentTask_ShouldCreateSubtask()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var title = "Child Task";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        var subtask = Task.Create(title, "Description", projectId, TaskPriority.Medium, parentTask, null, null, createdBy);

        // Assert
        subtask.ParentTaskId.Should().Be(parentTask.Id);
        subtask.IsSubtask().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithSubtaskAsParent_ShouldThrowDomainException()
    {
        // Arrange
        var grandparentTask = CreateValidTask();
        var parentSubtask = Task.Create("Parent Subtask", null, ProjectIdBuilder.Create().Build(), TaskPriority.Medium, grandparentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        var title = "Invalid Deep Child Task";
        var projectId = ProjectIdBuilder.Create().Build();
        var createdBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => Task.Create(title, null, projectId, TaskPriority.Low, parentSubtask, null, null, createdBy);
        action.Should().Throw<DomainException>()
              .WithMessage(ValidationMessages.Task.SubtaskNestingLimit);
    }

    #endregion

    #region UpdateInfo Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithValidParameters_ShouldUpdateTask()
    {
        // Arrange
        var task = CreateValidTask();
        var newTitle = "Updated Task";
        var newDescription = "Updated Description";
        var newPriority = TaskPriority.Critical;
        var newEstimatedHours = 16;
        var newDueDate = DateTime.UtcNow.AddDays(14);
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.UpdateInfo(newTitle, newDescription, newPriority, newEstimatedHours, newDueDate, updatedBy);

        // Assert
        task.Title.Should().Be(newTitle);
        task.Description.Should().Be(newDescription);
        task.Priority.Should().Be(newPriority);
        task.EstimatedHours.Should().Be(newEstimatedHours);
        task.DueDate.Should().Be(newDueDate);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_ShouldTrimTitleAndDescription()
    {
        // Arrange
        var task = CreateValidTask();
        var newTitle = "  Updated Title  ";
        var newDescription = "  Updated Description  ";
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.UpdateInfo(newTitle, newDescription, TaskPriority.Medium, null, null, updatedBy);

        // Assert
        task.Title.Should().Be("Updated Title");
        task.Description.Should().Be("Updated Description");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithCompletedTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var completedBy = UserBuilder.Create().AsDeveloper().Build();
        task.Start(completedBy); // Must start task first
        task.Complete(completedBy);
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.UpdateInfo("New Title", "New Description", TaskPriority.High, null, null, updatedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("*modify*tasks*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithEmptyTitle_ShouldThrowArgumentException(string invalidTitle)
    {
        // Arrange
        var task = CreateValidTask();
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.UpdateInfo(invalidTitle, "Description", TaskPriority.Medium, null, null, updatedBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("title");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithNullUpdatedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        var action = () => task.UpdateInfo("New Title", "Description", TaskPriority.Medium, null, null, null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("updatedBy");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithInvalidEstimatedHours_ShouldThrowArgumentException(int invalidHours)
    {
        // Arrange
        var task = CreateValidTask();
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.UpdateInfo("New Title", "Description", TaskPriority.Medium, invalidHours, null, updatedBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("estimatedHours");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateInfo_WithPastDueDate_ShouldThrowArgumentException()
    {
        // Arrange
        var task = CreateValidTask();
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var updatedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.UpdateInfo("New Title", "Description", TaskPriority.Medium, null, pastDate, updatedBy);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("dueDate");
    }

    #endregion

    #region Assignment Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithValidUser_ShouldAssignTask()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act
        task.AssignTo(assignee, assignedBy);

        // Assert
        task.AssignedUserId.Should().Be(assignee.Id);
        task.IsAssigned().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "DomainEvents")]
    public void AssignTo_ShouldRaiseTaskAssignedDomainEvent()
    {
        // Arrange
        var task = CreateValidTask();
        task.ClearDomainEvents(); // Clear creation event
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act
        task.AssignTo(assignee, assignedBy);

        // Assert
        var domainEvents = task.DomainEvents;
        domainEvents.Should().HaveCount(1);
        var assignedEvent = domainEvents.First().Should().BeOfType<TaskAssignedDomainEvent>().Subject;
        assignedEvent.TaskId.Should().Be(task.Id);
        assignedEvent.AssigneeId.Should().Be(assignee.Id);
        assignedEvent.AssignedBy.Should().Be(assignedBy.Id);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithCompletedTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var completedBy = UserBuilder.Create().AsDeveloper().Build();
        task.Start(completedBy); // Must start task first
        task.Complete(completedBy);
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.AssignTo(assignee, assignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Cannot assign completed tasks");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithCancelledTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var cancelledBy = UserBuilder.Create().AsDeveloper().Build();
        task.Cancel(cancelledBy);
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.AssignTo(assignee, assignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Cannot assign cancelled tasks");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithUserWhoCannotBeAssignedTasks_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsViewer().Build(); // Viewers cannot be assigned tasks
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.AssignTo(assignee, assignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("User cannot be assigned tasks");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithSameUserTwice_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();
        task.AssignTo(assignee, assignedBy);

        // Act & Assert
        var action = () => task.AssignTo(assignee, assignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Task is already assigned to this user");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithNullAssignee_ShouldThrowArgumentNullException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.AssignTo(null!, assignedBy);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("assignee");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AssignTo_WithNullAssignedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.AssignTo(assignee, null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("assignedBy");
    }

    #endregion

    #region Unassignment Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Unassign_WithAssignedTask_ShouldUnassignTask()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();
        task.AssignTo(assignee, assignedBy);
        var unassignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act
        task.Unassign(unassignedBy);

        // Assert
        task.AssignedUserId.Should().BeNull();
        task.IsAssigned().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Unassign_WithUnassignedTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var unassignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.Unassign(unassignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Task is not assigned to anyone");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Unassign_WithCompletedTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();
        task.AssignTo(assignee, assignedBy);
        task.Start(assignee); // Must start task first
        task.Complete(assignee);
        var unassignedBy = UserBuilder.Create().AsProjectManager().Build();

        // Act & Assert
        var action = () => task.Unassign(unassignedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Cannot unassign completed tasks");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Unassign_WithNullUnassignedBy_ShouldThrowArgumentNullException()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();
        task.AssignTo(assignee, assignedBy);

        // Act & Assert
        var action = () => task.Unassign(null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("unassignedBy");
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_FromToDoStatus_ShouldChangeStatusToInProgress()
    {
        // Arrange
        var task = CreateValidTask();
        var startedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Start(startedBy);

        // Assert
        task.Status.Should().Be(TaskStatus.InProgress);
        task.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.AssignedUserId.Should().Be(startedBy.Id); // Auto-assigned
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_WithAssignedUser_ShouldOnlyAllowAssignedUserToStart()
    {
        // Arrange
        var task = CreateValidTask();
        var assignee = UserBuilder.Create().AsDeveloper().Build();
        var assignedBy = UserBuilder.Create().AsProjectManager().Build();
        task.AssignTo(assignee, assignedBy);
        var differentUser = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.Start(differentUser);
        action.Should().Throw<DomainException>()
              .WithMessage("Only the assigned user can start this task");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Start_WithUnassignedTask_ShouldAutoAssignToStarter()
    {
        // Arrange
        var task = CreateValidTask();
        var startedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Start(startedBy);

        // Assert
        task.AssignedUserId.Should().Be(startedBy.Id);
        task.Status.Should().Be(TaskStatus.InProgress);
    }

    [Fact]
    [Trait("TestType", "DomainEvents")]
    public void Start_ShouldRaiseTaskStatusChangedAndAssignedEvents()
    {
        // Arrange
        var task = CreateValidTask();
        task.ClearDomainEvents(); // Clear creation event
        var startedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Start(startedBy);

        // Assert
        var domainEvents = task.DomainEvents;
        domainEvents.Should().HaveCount(2);
        
        var assignedEvent = domainEvents.First().Should().BeOfType<TaskAssignedDomainEvent>().Subject;
        assignedEvent.AssigneeId.Should().Be(startedBy.Id);
        
        var statusChangedEvent = domainEvents.Last().Should().BeOfType<TaskStatusChangedDomainEvent>().Subject;
        statusChangedEvent.PreviousStatus.Should().Be(TaskStatus.ToDo);
        statusChangedEvent.NewStatus.Should().Be(TaskStatus.InProgress);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void SubmitForReview_FromInProgressStatus_ShouldChangeStatusToInReview()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user);

        // Act
        task.SubmitForReview(user);

        // Assert
        task.Status.Should().Be(TaskStatus.InReview);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Complete_WithValidConditions_ShouldMarkTaskAsCompleted()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user);
        var actualHours = 6;

        // Act
        task.Complete(user, actualHours);

        // Assert
        task.Status.Should().Be(TaskStatus.Done);
        task.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        task.ActualHours.Should().Be(actualHours);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Complete_WithSubtasksIncomplete_ShouldThrowDomainException()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask = Task.Create("Subtask", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        parentTask.AddSubtask(subtask);
        var user = UserBuilder.Create().AsDeveloper().Build();
        parentTask.Start(user); // Start parent task first

        // Act & Assert
        var action = () => parentTask.Complete(user);
        action.Should().Throw<DomainException>()
              .WithMessage(ValidationMessages.Task.IncompleteSubtasks);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Cancel_WithValidTask_ShouldCancelTask()
    {
        // Arrange
        var task = CreateValidTask();
        var cancelledBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Cancel(cancelledBy);

        // Assert
        task.Status.Should().Be(TaskStatus.Cancelled);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Cancel_WithSubtasks_ShouldCancelAllSubtasks()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask1 = Task.Create("Subtask 1", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        var subtask2 = Task.Create("Subtask 2", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        parentTask.AddSubtask(subtask1);
        parentTask.AddSubtask(subtask2);
        var cancelledBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        parentTask.Cancel(cancelledBy);

        // Assert
        parentTask.Status.Should().Be(TaskStatus.Cancelled);
        subtask1.Status.Should().Be(TaskStatus.Cancelled);
        subtask2.Status.Should().Be(TaskStatus.Cancelled);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Cancel_WithCompletedTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user); // Must start task first
        task.Complete(user);
        var cancelledBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.Cancel(cancelledBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Cannot cancel completed tasks");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Reopen_WithCompletedTask_ShouldReopenTask()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user);
        task.Complete(user);
        var reopenedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Reopen(reopenedBy);

        // Assert
        task.Status.Should().Be(TaskStatus.InProgress); // Returns to InProgress since it was started
        task.CompletedAt.Should().BeNull();
        task.ActualHours.Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Reopen_WithCancelledTask_ShouldReopenTask()
    {
        // Arrange
        var task = CreateValidTask();
        var cancelledBy = UserBuilder.Create().AsDeveloper().Build();
        task.Cancel(cancelledBy);
        var reopenedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act
        task.Reopen(reopenedBy);

        // Assert
        task.Status.Should().Be(TaskStatus.ToDo); // Returns to ToDo since it was never started
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Reopen_WithActiveTask_ShouldThrowDomainException()
    {
        // Arrange
        var task = CreateValidTask();
        var reopenedBy = UserBuilder.Create().AsDeveloper().Build();

        // Act & Assert
        var action = () => task.Reopen(reopenedBy);
        action.Should().Throw<DomainException>()
              .WithMessage("Can only reopen completed or cancelled tasks");
    }

    #endregion

    #region Subtask Management Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddSubtask_WithValidSubtask_ShouldAddSubtask()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask = Task.Create("Subtask", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());

        // Act
        parentTask.AddSubtask(subtask);

        // Assert
        parentTask.Subtasks.Should().HaveCount(1);
        parentTask.Subtasks.First().Should().Be(subtask);
        parentTask.HasSubtasks().Should().BeTrue();
        subtask.IsSubtask().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddSubtask_ToSubtask_ShouldThrowDomainException()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask = Task.Create("Subtask", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        parentTask.AddSubtask(subtask);

        // Act & Assert - The error is thrown during creation, not addition
        var action = () => Task.Create("Nested Subtask", null, parentTask.ProjectId, TaskPriority.Medium, subtask, null, null, UserBuilder.Create().AsDeveloper().Build());
        action.Should().Throw<DomainException>()
              .WithMessage("*single-level nesting*");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddSubtask_WithMismatchedParentId_ShouldThrowDomainException()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var differentParentTask = CreateValidTask();
        var subtask = Task.Create("Subtask", null, parentTask.ProjectId, TaskPriority.Medium, differentParentTask, null, null, UserBuilder.Create().AsDeveloper().Build());

        // Act & Assert
        var action = () => parentTask.AddSubtask(subtask);
        action.Should().Throw<DomainException>()
              .WithMessage("Subtask parent ID does not match this task");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddSubtask_WithSameSubtaskTwice_ShouldThrowDomainException()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask = Task.Create("Subtask", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        parentTask.AddSubtask(subtask);

        // Act & Assert
        var action = () => parentTask.AddSubtask(subtask);
        action.Should().Throw<DomainException>()
              .WithMessage("Subtask is already added to this task");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AddSubtask_WithNullSubtask_ShouldThrowArgumentNullException()
    {
        // Arrange
        var parentTask = CreateValidTask();

        // Act & Assert
        var action = () => parentTask.AddSubtask(null!);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("subtask");
    }

    #endregion

    #region Query Methods Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithPastDueDate_ShouldReturnTrue()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(1); // Start with future date for creation
        var task = TaskBuilder.Create()
            .WithDueDate(pastDate)
            .Build();
        
        // Use reflection to set a past due date after creation to bypass validation
        var dueDateField = typeof(Task).GetField("<DueDate>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dueDateField?.SetValue(task, DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        task.IsOverdue().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithFutureDueDate_ShouldReturnFalse()
    {
        // Arrange
        var task = TaskBuilder.Create()
            .WithDueDate(DateTime.UtcNow.AddDays(1))
            .Build();

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithNoDueDate_ShouldReturnFalse()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsOverdue_WithCompletedTask_ShouldReturnFalse()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1); // Start with future date for creation
        var task = TaskBuilder.Create()
            .WithDueDate(futureDate)
            .Build();
        
        // Use reflection to set a past due date after creation
        var dueDateField = typeof(Task).GetField("<DueDate>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        dueDateField?.SetValue(task, DateTime.UtcNow.AddDays(-1));
        
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user); // Must start task first
        task.Complete(user);

        // Act & Assert
        task.IsOverdue().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetCompletionPercentage_WithoutSubtasks_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        task.GetCompletionPercentage().Should().Be(0m);

        // Complete the task
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user); // Must start task first
        task.Complete(user);
        task.GetCompletionPercentage().Should().Be(100m);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetCompletionPercentage_WithSubtasks_ShouldReturnCorrectPercentage()
    {
        // Arrange
        var parentTask = CreateValidTask();
        var subtask1 = Task.Create("Subtask 1", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        var subtask2 = Task.Create("Subtask 2", null, parentTask.ProjectId, TaskPriority.Medium, parentTask, null, null, UserBuilder.Create().AsDeveloper().Build());
        parentTask.AddSubtask(subtask1);
        parentTask.AddSubtask(subtask2);

        // Act & Assert
        parentTask.GetCompletionPercentage().Should().Be(0m);

        // Complete one subtask
        var user = UserBuilder.Create().AsDeveloper().Build();
        subtask1.Start(user); // Must start task first
        subtask1.Complete(user);
        parentTask.GetCompletionPercentage().Should().Be(50m);

        // Complete second subtask
        subtask2.Start(user); // Must start task first
        subtask2.Complete(user);
        parentTask.GetCompletionPercentage().Should().Be(100m);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDuration_WithNotStartedTask_ShouldReturnNull()
    {
        // Arrange
        var task = CreateValidTask();

        // Act & Assert
        task.GetDuration().Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDuration_WithStartedTask_ShouldReturnDuration()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user);

        // Act
        var duration = task.GetDuration();

        // Assert
        duration.Should().NotBeNull();
        duration!.Value.Should().BeCloseTo(TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetDuration_WithCompletedTask_ShouldReturnCompleteDuration()
    {
        // Arrange
        var task = CreateValidTask();
        var user = UserBuilder.Create().AsDeveloper().Build();
        task.Start(user);
        
        // Simulate some time passing (we'll use reflection to set an earlier start time)
        var startedAtField = typeof(Task).GetField("<StartedAt>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        startedAtField?.SetValue(task, DateTime.UtcNow.AddHours(-2));
        
        task.Complete(user);

        // Act
        var duration = task.GetDuration();

        // Assert
        duration.Should().NotBeNull();
        duration!.Value.Should().BeCloseTo(TimeSpan.FromHours(2), TimeSpan.FromMinutes(1));
    }

    #endregion

    #region Helper Methods

    private static Task CreateValidTask()
    {
        return TaskBuilder.Create()
            .WithTitle("Test Task")
            .WithDescription("Test Description")
            .WithPriority(TaskPriority.Medium)
            .Build();
    }

    #endregion
}