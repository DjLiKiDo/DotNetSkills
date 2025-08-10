using AutoMapper;
using DotNetSkills.Application.TaskExecution.Contracts.Responses;
using DotNetSkills.Application.TaskExecution.Mappings;
using DotNetSkills.Application.Common.Mappings;
using DotNetSkills.Domain.TaskExecution.Entities;
using DotNetSkills.Domain.TaskExecution.Enums;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using FluentAssertions;
using Xunit;

namespace DotNetSkills.Application.UnitTests.TaskExecution.Mappings;

public class TaskAssignmentMappingTests
{
    private readonly IMapper _mapper;

    public TaskAssignmentMappingTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SharedValueObjectMappingProfile>();
            cfg.AddProfile<TaskMappingProfile>();
        });
        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void Map_Task_To_TaskAssignmentResponse_With_Context_Items_Should_Populate_Assigned_Names()
    {
        // Arrange
        var task = CreateSampleTask();
        var context = new Dictionary<string, object>
        {
            ["AssignedUserName"] = "Jane Doe",
            ["AssignedByUserId"] = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            ["AssignedByUserName"] = "John Admin"
        };

        // Act
        var result = _mapper.Map<TaskAssignmentResponse>(task, opt =>
        {
            foreach (var kvp in context)
            {
                opt.Items[kvp.Key] = kvp.Value;
            }
        });

        // Assert
        result.TaskId.Should().Be(task.Id.Value);
        result.TaskTitle.Should().Be(task.Title);
        result.AssignedUserName.Should().Be("Jane Doe");
        result.AssignedByUserName.Should().Be("John Admin");
        result.AssignedByUserId.Should().Be(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
    }

    [Fact]
    public void Map_Task_To_TaskAssignmentResponse_Without_Context_Items_Should_Have_Defaults()
    {
        // Arrange
        var task = CreateSampleTask();

        // Act
        var result = _mapper.Map<TaskAssignmentResponse>(task);

        // Assert
        result.TaskId.Should().Be(task.Id.Value);
        result.AssignedUserName.Should().BeNull();
        result.AssignedByUserName.Should().BeEmpty();
        result.AssignedByUserId.Should().Be(Guid.Empty);
    }

    private static Domain.TaskExecution.Entities.Task CreateSampleTask()
    {
        var projectId = new ProjectId(Guid.NewGuid());
        var user = DotNetSkills.Domain.UserManagement.Entities.User.Create(
            name: "Creator",
            email: new EmailAddress("creator@example.com"),
            role: DotNetSkills.Domain.UserManagement.Enums.UserRole.Admin,
            createdByUser: null
        );
        var task = Domain.TaskExecution.Entities.Task.Create(
            title: "Sample Task",
            description: "Desc",
            projectId: projectId,
            priority: TaskPriority.Medium,
            parentTask: null,
            estimatedHours: null,
            dueDate: null,
            createdBy: user
        );
        return task;
    }
}
