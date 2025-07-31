using FluentAssertions;
using DotNetSkills.Domain.Repositories;

namespace DotNetSkills.Domain.UnitTests.Repositories;

/// <summary>
/// Contract validation tests for all repository interfaces.
/// Ensures interface signatures are correctly defined according to domain requirements.
/// </summary>
public class RepositoryContractTests
{
    [Fact(DisplayName = "IUserRepository interface should have all required methods")]
    public void IUserRepository_InterfaceDefinition_ShouldHaveAllRequiredMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(IUserRepository);
        var methods = interfaceType.GetMethods();

        // Assert
        interfaceType.IsInterface.Should().BeTrue();
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetByEmailAsync");
        methods.Should().Contain(m => m.Name == "GetActiveUsersAsync");
        methods.Should().Contain(m => m.Name == "GetUsersByRoleAsync");
        methods.Should().Contain(m => m.Name == "GetTeamMembersAsync");
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "UpdateAsync");
        methods.Should().Contain(m => m.Name == "DeleteAsync");
        methods.Should().Contain(m => m.Name == "ExistsWithEmailAsync");
    }

    [Fact(DisplayName = "ITeamRepository interface should have all required methods")]
    public void ITeamRepository_InterfaceDefinition_ShouldHaveAllRequiredMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(ITeamRepository);
        var methods = interfaceType.GetMethods();

        // Assert
        interfaceType.IsInterface.Should().BeTrue();
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetByNameAsync");
        methods.Should().Contain(m => m.Name == "GetTeamsForUserAsync");
        methods.Should().Contain(m => m.Name == "GetTeamWithMembersAsync");
        methods.Should().Contain(m => m.Name == "GetActiveTeamsAsync");
        methods.Should().Contain(m => m.Name == "GetTeamsWithoutActiveProjectsAsync");
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "UpdateAsync");
        methods.Should().Contain(m => m.Name == "DeleteAsync");
        methods.Should().Contain(m => m.Name == "ExistsWithNameAsync");
    }

    [Fact(DisplayName = "IProjectRepository interface should have all required methods")]
    public void IProjectRepository_InterfaceDefinition_ShouldHaveAllRequiredMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(IProjectRepository);
        var methods = interfaceType.GetMethods();

        // Assert
        interfaceType.IsInterface.Should().BeTrue();
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetByNameInTeamAsync");
        methods.Should().Contain(m => m.Name == "GetByTeamIdAsync");
        methods.Should().Contain(m => m.Name == "GetProjectWithTasksAsync");
        methods.Should().Contain(m => m.Name == "GetActiveProjectsAsync");
        methods.Should().Contain(m => m.Name == "GetByStatusAsync");
        methods.Should().Contain(m => m.Name == "GetProjectsApproachingDeadlineAsync");
        methods.Should().Contain(m => m.Name == "GetProjectsWithOverdueTasksAsync");
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "UpdateAsync");
        methods.Should().Contain(m => m.Name == "DeleteAsync");
        methods.Should().Contain(m => m.Name == "ExistsWithNameInTeamAsync");
    }

    [Fact(DisplayName = "ITaskRepository interface should have all required methods")]
    public void ITaskRepository_InterfaceDefinition_ShouldHaveAllRequiredMethods()
    {
        // Arrange & Act
        var interfaceType = typeof(ITaskRepository);
        var methods = interfaceType.GetMethods();

        // Assert
        interfaceType.IsInterface.Should().BeTrue();
        methods.Should().Contain(m => m.Name == "GetByIdAsync");
        methods.Should().Contain(m => m.Name == "GetByProjectIdAsync");
        methods.Should().Contain(m => m.Name == "GetByAssigneeIdAsync");
        methods.Should().Contain(m => m.Name == "GetTaskWithSubtasksAsync");
        methods.Should().Contain(m => m.Name == "GetSubtasksAsync");
        methods.Should().Contain(m => m.Name == "GetByStatusAsync");
        methods.Should().Contain(m => m.Name == "GetByPriorityAsync");
        methods.Should().Contain(m => m.Name == "GetOverdueTasksAsync");
        methods.Should().Contain(m => m.Name == "GetTasksDueWithinAsync");
        methods.Should().Contain(m => m.Name == "GetUnassignedTasksInProjectAsync");
        methods.Should().Contain(m => m.Name == "GetByTypeAsync");
        methods.Should().Contain(m => m.Name == "GetTaskCountByStatusAsync");
        methods.Should().Contain(m => m.Name == "AddAsync");
        methods.Should().Contain(m => m.Name == "UpdateAsync");
        methods.Should().Contain(m => m.Name == "DeleteAsync");
        methods.Should().Contain(m => m.Name == "CanDeleteAsync");
    }

    [Fact(DisplayName = "IUnitOfWork interface should have transaction and repository properties")]
    public void IUnitOfWork_InterfaceDefinition_ShouldHaveTransactionAndRepositoryProperties()
    {
        // Arrange & Act
        var interfaceType = typeof(IUnitOfWork);
        var methods = interfaceType.GetMethods();
        var properties = interfaceType.GetProperties();

        // Assert
        interfaceType.IsInterface.Should().BeTrue();
        
        // Transaction methods
        methods.Should().Contain(m => m.Name == "SaveChangesAsync");
        methods.Should().Contain(m => m.Name == "BeginTransactionAsync");
        methods.Should().Contain(m => m.Name == "CommitTransactionAsync");
        methods.Should().Contain(m => m.Name == "RollbackTransactionAsync");
        
        // Repository properties
        properties.Should().Contain(p => p.Name == "Users");
        properties.Should().Contain(p => p.Name == "Teams");
        properties.Should().Contain(p => p.Name == "Projects");
        properties.Should().Contain(p => p.Name == "Tasks");
        
        // IDisposable inheritance
        typeof(IDisposable).IsAssignableFrom(interfaceType).Should().BeTrue();
    }

    [Theory(DisplayName = "All repository interfaces should be in correct namespace")]
    [InlineData(typeof(IUserRepository))]
    [InlineData(typeof(ITeamRepository))]
    [InlineData(typeof(IProjectRepository))]
    [InlineData(typeof(ITaskRepository))]
    [InlineData(typeof(IUnitOfWork))]
    public void RepositoryInterfaces_Namespace_ShouldBeCorrect(Type repositoryType)
    {
        // Arrange & Act & Assert
        repositoryType.Namespace.Should().Be("DotNetSkills.Domain.Repositories");
    }

    [Theory(DisplayName = "All repository interfaces should follow naming convention")]
    [InlineData(typeof(IUserRepository), "IUserRepository")]
    [InlineData(typeof(ITeamRepository), "ITeamRepository")]
    [InlineData(typeof(IProjectRepository), "IProjectRepository")]
    [InlineData(typeof(ITaskRepository), "ITaskRepository")]
    [InlineData(typeof(IUnitOfWork), "IUnitOfWork")]
    public void RepositoryInterfaces_NamingConvention_ShouldBeCorrect(Type repositoryType, string expectedName)
    {
        // Arrange & Act & Assert
        repositoryType.Name.Should().Be(expectedName);
        repositoryType.IsInterface.Should().BeTrue();
    }
}
