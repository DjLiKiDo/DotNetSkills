using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using DotNetSkills.API.Authorization;
using FluentAssertions;
using Xunit;
using System.Security.Claims;
using DotNetSkills.Domain.UserManagement.Enums;

namespace DotNetSkills.API.UnitTests.Authorization;

/// <summary>
/// Unit tests for authorization policy registration and configuration.
/// </summary>
public class AuthorizationExtensionsTests
{
    [Fact]
    public void AddApiAuthorization_ShouldRegisterAllPolicies()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authorizationOptions = serviceProvider.GetRequiredService<IAuthorizationPolicyProvider>();
        authorizationOptions.Should().NotBeNull();

        // Verify policies are registered by attempting to retrieve them
        var adminPolicy = authorizationOptions.GetPolicyAsync(Policies.AdminOnly).Result;
        var teamManagerPolicy = authorizationOptions.GetPolicyAsync(Policies.TeamManager).Result;
        var projectManagerPolicy = authorizationOptions.GetPolicyAsync(Policies.ProjectManagerOrAdmin).Result;
        var projectMemberPolicy = authorizationOptions.GetPolicyAsync(Policies.ProjectMemberOrAdmin).Result;

        adminPolicy.Should().NotBeNull();
        teamManagerPolicy.Should().NotBeNull();
        projectManagerPolicy.Should().NotBeNull();
        projectMemberPolicy.Should().NotBeNull();
    }

    [Fact]
    public void AdminOnlyPolicy_ShouldRequireAdminRole()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act - Create test user with Admin role
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
        }));

        var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Developer.ToString())
        }));

        // Assert
        var adminResult = authService.AuthorizeAsync(adminUser, Policies.AdminOnly).Result;
        var nonAdminResult = authService.AuthorizeAsync(nonAdminUser, Policies.AdminOnly).Result;

        adminResult.Succeeded.Should().BeTrue();
        nonAdminResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void TeamManagerPolicy_ShouldAllowAdminAndProjectManager()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act - Create test users
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
        }));

        var projectManagerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.ProjectManager.ToString())
        }));

        var developerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Developer.ToString())
        }));

        // Assert
        var adminResult = authService.AuthorizeAsync(adminUser, Policies.TeamManager).Result;
        var projectManagerResult = authService.AuthorizeAsync(projectManagerUser, Policies.TeamManager).Result;
        var developerResult = authService.AuthorizeAsync(developerUser, Policies.TeamManager).Result;

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        developerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void ProjectManagerOrAdminPolicy_ShouldAllowAdminAndProjectManager()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act - Create test users
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
        }));

        var projectManagerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.ProjectManager.ToString())
        }));

        var viewerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Viewer.ToString())
        }));

        // Assert
        var adminResult = authService.AuthorizeAsync(adminUser, Policies.ProjectManagerOrAdmin).Result;
        var projectManagerResult = authService.AuthorizeAsync(projectManagerUser, Policies.ProjectManagerOrAdmin).Result;
        var viewerResult = authService.AuthorizeAsync(viewerUser, Policies.ProjectManagerOrAdmin).Result;

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        viewerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void ProjectMemberOrAdminPolicy_ShouldAllowElevatedRoles()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act - Create test users with elevated roles
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Admin.ToString())
        }));

        var projectManagerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.ProjectManager.ToString())
        }));

        var developerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Developer.ToString())
        }));

        var viewerUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Viewer.ToString())
        }));

        // Assert
        var adminResult = authService.AuthorizeAsync(adminUser, Policies.ProjectMemberOrAdmin).Result;
        var projectManagerResult = authService.AuthorizeAsync(projectManagerUser, Policies.ProjectMemberOrAdmin).Result;
        var developerResult = authService.AuthorizeAsync(developerUser, Policies.ProjectMemberOrAdmin).Result;
        var viewerResult = authService.AuthorizeAsync(viewerUser, Policies.ProjectMemberOrAdmin).Result;

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        developerResult.Succeeded.Should().BeTrue();
        viewerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void ProjectMemberOrAdminPolicy_ShouldAllowProjectMembershipClaim()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApiAuthorization();
        var serviceProvider = services.BuildServiceProvider();
        var authService = serviceProvider.GetRequiredService<IAuthorizationService>();

        // Act - Create test user with project membership claim but no elevated role
        var viewerWithProjectClaim = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Viewer.ToString()),
            new Claim("project", "project-123")
        }));

        var viewerWithoutProjectClaim = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, UserRole.Viewer.ToString())
        }));

        // Assert
        var withProjectResult = authService.AuthorizeAsync(viewerWithProjectClaim, Policies.ProjectMemberOrAdmin).Result;
        var withoutProjectResult = authService.AuthorizeAsync(viewerWithoutProjectClaim, Policies.ProjectMemberOrAdmin).Result;

        withProjectResult.Succeeded.Should().BeTrue();
        withoutProjectResult.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData(Policies.AdminOnly)]
    [InlineData(Policies.TeamManager)]
    [InlineData(Policies.ProjectManagerOrAdmin)]
    [InlineData(Policies.ProjectMemberOrAdmin)]
    public void PolicyConstants_ShouldNotBeNullOrEmpty(string policyName)
    {
        // Assert
        policyName.Should().NotBeNullOrEmpty();
    }
}