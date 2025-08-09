using DotNetSkills.Application.ProjectManagement.Contracts;
using DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;
using DotNetSkills.Application.TaskExecution.Contracts;
using DotNetSkills.Application.UserManagement.Contracts;

namespace DotNetSkills.Application.UnitTests.ProjectManagement.Features.GetProjectTasks;

public class GetProjectTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock = new();
    private readonly Mock<IProjectRepository> _projectRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    
    private GetProjectTasksQueryHandler CreateHandler()
    {
        return new GetProjectTasksQueryHandler(
            _taskRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenProjectNotFound_ThrowsDomainException()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var query = new GetProjectTasksQuery(projectId);
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var handler = CreateHandler();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(
            () => handler.Handle(query, CancellationToken.None));
        
        exception.Message.Should().Contain($"Project with ID {projectId.Value} not found");
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenValidRequest_ReturnsPagedProjectTaskResponse()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var teamId = new TeamId(Guid.NewGuid());
        var userId = new UserId(Guid.NewGuid());
        
        var user = User.Create("Test User", new EmailAddress("test@example.com"), UserRole.Admin);
        var project = Project.Create("Test Project", "Description", teamId, null, user);
        var task1 = DomainTask.Create("Task 1", "Description 1", projectId, Domain.TaskExecution.Enums.TaskPriority.High, null, null, null, user);
        var task2 = DomainTask.Create("Task 2", "Description 2", projectId, Domain.TaskExecution.Enums.TaskPriority.Medium, null, null, null, user);
        
        var query = new GetProjectTasksQuery(projectId);
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<ProjectId?>(),
                It.IsAny<UserId?>(),
                It.IsAny<DomainTaskStatus?>(),
                It.IsAny<Domain.TaskExecution.Enums.TaskPriority?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { task1, task2 }, 2));

        _taskRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { task1, task2 });

        _taskRepositoryMock
            .Setup(r => r.HasSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _taskRepositoryMock
            .Setup(r => r.GetSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<DomainTask>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProjectId.Should().Be(projectId.Value);
        result.ProjectName.Should().Be(project.Name);
        result.Tasks.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTasksHaveAssignedUsers_ReturnsTasksWithUserNames()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var teamId = new TeamId(Guid.NewGuid());
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());
        
        var creatorUser = User.Create("Creator", new EmailAddress("creator@example.com"), UserRole.Admin);
        var project = Project.Create("Test Project", "Description", teamId, null, creatorUser);
        var user1 = User.Create("User One", new EmailAddress("user1@example.com"), UserRole.Developer);
        var user2 = User.Create("User Two", new EmailAddress("user2@example.com"), UserRole.Developer);
        var task1 = DomainTask.Create("Task 1", "Description 1", projectId, Domain.TaskExecution.Enums.TaskPriority.High, null, null, null, user1);
        var task2 = DomainTask.Create("Task 2", "Description 2", projectId, Domain.TaskExecution.Enums.TaskPriority.Medium, null, null, null, user2);
        
        // Assign tasks to users
        task1.AssignTo(user1, user1);
        task2.AssignTo(user2, user2);
        
        var query = new GetProjectTasksQuery(projectId);
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<ProjectId?>(),
                It.IsAny<UserId?>(),
                It.IsAny<DomainTaskStatus?>(),
                It.IsAny<Domain.TaskExecution.Enums.TaskPriority?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { task1, task2 }, 2));

        _taskRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { task1, task2 });

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user1);

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user2);

        _taskRepositoryMock
            .Setup(r => r.HasSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _taskRepositoryMock
            .Setup(r => r.GetSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<DomainTask>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Tasks.Should().HaveCount(2);
        
        var firstTask = result.Tasks.First(t => t.TaskId == task1.Id.Value);
        firstTask.AssignedUserId.Should().Be(user1.Id.Value);
        firstTask.AssignedUserName.Should().Be("User One");
        firstTask.IsAssigned.Should().BeTrue();
        
        var secondTask = result.Tasks.First(t => t.TaskId == task2.Id.Value);
        secondTask.AssignedUserId.Should().Be(user2.Id.Value);
        secondTask.AssignedUserName.Should().Be("User Two");
        secondTask.IsAssigned.Should().BeTrue();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WhenTasksHaveSubtasks_ReturnsCorrectSubtaskCounts()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var teamId = new TeamId(Guid.NewGuid());
        
        var creatorUser = User.Create("Creator", new EmailAddress("creator@example.com"), UserRole.Admin);
        var project = Project.Create("Test Project", "Description", teamId, null, creatorUser);
        var user = User.Create("Test User", new EmailAddress("test@example.com"), UserRole.Admin);
        var parentTask = DomainTask.Create("Parent Task", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.High, null, null, null, user);
        var subtask1 = DomainTask.Create("Subtask 1", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.Medium, parentTask, null, null, user);
        var subtask2 = DomainTask.Create("Subtask 2", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.Low, parentTask, null, null, user);
        
        // Complete one subtask
        subtask1.Start(user);
        subtask1.Complete(user);
        
        var query = new GetProjectTasksQuery(projectId);
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<ProjectId?>(),
                It.IsAny<UserId?>(),
                It.IsAny<DomainTaskStatus?>(),
                It.IsAny<Domain.TaskExecution.Enums.TaskPriority?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new[] { parentTask }, 1));

        _taskRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { parentTask, subtask1, subtask2 });

        _taskRepositoryMock
            .Setup(r => r.HasSubtasksAsync(parentTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _taskRepositoryMock
            .Setup(r => r.GetSubtasksAsync(parentTask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { subtask1, subtask2 });

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Tasks.Should().HaveCount(1);
        
        var task = result.Tasks.First();
        task.HasSubtasks.Should().BeTrue();
        task.SubtaskCount.Should().Be(2);
        task.CompletionPercentage.Should().Be(50m); // 1 out of 2 subtasks completed
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_WithFilteringOptions_PassesFiltersToRepository()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var teamId = new TeamId(Guid.NewGuid());
        var assignedUserId = new UserId(Guid.NewGuid());
        
        var creatorUser = User.Create("Creator", new EmailAddress("creator@example.com"), UserRole.Admin);
        var project = Project.Create("Test Project", "Description", teamId, null, creatorUser);
        
        var query = new GetProjectTasksQuery(
            ProjectId: projectId,
            Page: 2,
            PageSize: 10,
            Status: DomainTaskStatus.InProgress,
            AssignedUserId: assignedUserId,
            Priority: Domain.TaskExecution.Enums.TaskPriority.High,
            DueDateFrom: DateTime.Today.AddDays(-7),
            DueDateTo: DateTime.Today.AddDays(7),
            IsOverdue: true,
            IsSubtask: false,
            Search: "test search"
        );
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<ProjectId?>(),
                It.IsAny<UserId?>(),
                It.IsAny<DomainTaskStatus?>(),
                It.IsAny<Domain.TaskExecution.Enums.TaskPriority?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enumerable.Empty<DomainTask>(), 0));

        _taskRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<DomainTask>());

        var handler = CreateHandler();

        // Act
        await handler.Handle(query, CancellationToken.None);

        // Assert
        _taskRepositoryMock.Verify(r => r.GetPagedAsync(
            2, // page
            10, // pageSize
            "test search", // searchTerm
            projectId, // projectId
            assignedUserId, // assignedUserId
            DomainTaskStatus.InProgress, // status
            Domain.TaskExecution.Enums.TaskPriority.High, // priority
            It.Is<DateTime>(d => d.Date == DateTime.Today.AddDays(-7).Date), // dueDateFrom
            It.Is<DateTime>(d => d.Date == DateTime.Today.AddDays(7).Date), // dueDateTo
            true, // isOverdue
            false, // isSubtask
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_CalculatesTaskCounts_ReturnsCorrectAggregates()
    {
        // Arrange
        var projectId = new ProjectId(Guid.NewGuid());
        var teamId = new TeamId(Guid.NewGuid());
        
        var creatorUser = User.Create("Creator", new EmailAddress("creator@example.com"), UserRole.Admin);
        var project = Project.Create("Test Project", "Description", teamId, null, creatorUser);
        var user = User.Create("Test User", new EmailAddress("test@example.com"), UserRole.Admin);
        
        var activeTask1 = DomainTask.Create("Active 1", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.High, null, null, null, user);
        var activeTask2 = DomainTask.Create("Active 2", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.Medium, null, null, null, user);
        var completedTask = DomainTask.Create("Completed", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.Low, null, null, null, user);
        var overdueTask = DomainTask.Create("Overdue", "Description", projectId, Domain.TaskExecution.Enums.TaskPriority.High, null, null, DateTime.UtcNow.AddDays(1), user);
        
        // Manually set due date to past to make it overdue (simulating a task that was created earlier but is now overdue)
        typeof(DomainTask).GetProperty("DueDate")!.SetValue(overdueTask, DateTime.UtcNow.AddDays(-1));
        
        completedTask.Start(user);
        completedTask.Complete(user);
        activeTask2.Start(user);
        
        var allTasks = new[] { activeTask1, activeTask2, completedTask, overdueTask };
        
        var query = new GetProjectTasksQuery(projectId);
        
        _projectRepositoryMock
            .Setup(r => r.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _taskRepositoryMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string?>(),
                It.IsAny<ProjectId?>(),
                It.IsAny<UserId?>(),
                It.IsAny<DomainTaskStatus?>(),
                It.IsAny<Domain.TaskExecution.Enums.TaskPriority?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool?>(),
                It.IsAny<bool?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((allTasks, 4));

        _taskRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(allTasks);

        _taskRepositoryMock
            .Setup(r => r.HasSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _taskRepositoryMock
            .Setup(r => r.GetSubtasksAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<DomainTask>());

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.ActiveTaskCount.Should().Be(3); // activeTask1, activeTask2, overdueTask (not done)
        result.CompletedTaskCount.Should().Be(1); // completedTask
        result.OverdueTaskCount.Should().Be(1); // overdueTask (has due date in past and not done)
    }
}