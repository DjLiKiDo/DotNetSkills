using DotNetSkills.Application.TaskExecution.Features.AssignTask;
using DotNetSkills.Application.TaskExecution.Contracts;
using DotNetSkills.Application.TaskExecution.Contracts.Responses;
using DotNetSkills.Application.UserManagement.Contracts;
using DomainTaskEntity = DotNetSkills.Domain.TaskExecution.Entities.Task;
using DotNetSkills.Domain.TaskExecution.Enums;
using DotNetSkills.Domain.ProjectManagement.ValueObjects;
using DotNetSkills.Domain.TaskExecution.ValueObjects;
using DotNetSkills.Domain.UserManagement.Entities;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using DotNetSkills.Application.Common.Abstractions;
using DotNetSkills.Application.Common.Exceptions;
using System.Threading.Tasks;
using DotNetSkills.Domain.UserManagement.Enums;
using Moq;
using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace DotNetSkills.Application.UnitTests.TaskExecution.AssignTask;

public class AssignTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<AssignTaskCommandHandler>> _logger = new();

    public AssignTaskCommandHandlerTests()
    {
        // Use real mapping profile
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(AssignTaskCommandHandler).Assembly));
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_Assigns_Task_Successfully()
    {
        // Arrange
        var projectId = ProjectId.New();
        var creator = User.Create("Creator", new EmailAddress("creator@example.com"), UserRole.Admin);
    var task = DomainTaskEntity.Create("Task A", "Desc", projectId, TaskPriority.Medium, null, null, null, creator);
        var assignee = User.Create("Assignee", new EmailAddress("assignee@example.com"), UserRole.Developer, creator);
        var assigner = User.Create("Assigner", new EmailAddress("assigner@example.com"), UserRole.ProjectManager, creator);

        _taskRepo.Setup(r => r.GetByIdAsync(It.IsAny<TaskId>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(task);
        _userRepo.Setup(r => r.GetByIdAsync(assignee.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(assignee);
        _userRepo.Setup(r => r.GetByIdAsync(assigner.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(assigner);

        var handler = new AssignTaskCommandHandler(_taskRepo.Object, _userRepo.Object, _uow.Object, _mapper, _logger.Object);
        var command = new AssignTaskCommand(task.Id, assignee.Id, assigner.Id);

        // Act
        var response = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(task.Id.Value, response.TaskId);
        Assert.Equal(assignee.Id.Value, response.AssignedUserId);
        Assert.Equal(assigner.Id.Value, response.AssignedByUserId);
        _taskRepo.Verify(r => r.Update(task), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ThrowsNotFoundException_WhenTaskNotFound()
    {
        // Arrange
        var taskId = TaskId.New();
        var assigneeId = UserId.New();
        var assignerId = UserId.New();

        _taskRepo.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((DomainTaskEntity?)null);

        var handler = new AssignTaskCommandHandler(_taskRepo.Object, _userRepo.Object, _uow.Object, _mapper, _logger.Object);
        var command = new AssignTaskCommand(taskId, assigneeId, assignerId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Task", exception.Message);
        Assert.Contains(taskId.Value.ToString(), exception.Message);
        _taskRepo.Verify(r => r.Update(It.IsAny<DomainTaskEntity>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
