using AutoMapper;
using DotNetSkills.Application.UserManagement.Mappings;
using DotNetSkills.Application.TeamCollaboration.Mappings;
using DotNetSkills.Application.ProjectManagement.Mappings;
using DotNetSkills.Application.TaskExecution.Mappings;
using DotNetSkills.Application.Common.Mappings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetSkills.Application.UnitTests.Mappings;

[Trait("Category", "Unit")]
public class AutoMapperConfigurationTests
{
    [Fact]
    public void UserMappingProfile_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void TeamMappingProfile_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TeamMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void ProjectMappingProfile_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProjectMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void TaskMappingProfile_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TaskMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void SharedValueObjectMappingProfile_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SharedValueObjectMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void All_MappingProfiles_Configuration_Should_Be_Valid()
    {
        // Arrange
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SharedValueObjectMappingProfile>();
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<TeamMappingProfile>();
            cfg.AddProfile<ProjectMappingProfile>();
            cfg.AddProfile<TaskMappingProfile>();
        });

        // Act & Assert
        Action act = () => configuration.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }

    [Fact]
    public void DependencyInjection_AutoMapper_Should_Work_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddApplicationServices();

        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        var mapper = serviceProvider.GetService<IMapper>();
        mapper.Should().NotBeNull();

        // Verify configuration is valid
        Action act = () => mapper.ConfigurationProvider.AssertConfigurationIsValid();
        act.Should().NotThrow();
    }
}