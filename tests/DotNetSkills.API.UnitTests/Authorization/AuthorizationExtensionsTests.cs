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
    public async Task AddApiAuthorization_ShouldRegisterAllPolicies()
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
    var adminPolicy = await authorizationOptions.GetPolicyAsync(Policies.AdminOnly);
    var teamManagerPolicy = await authorizationOptions.GetPolicyAsync(Policies.TeamManager);
    var projectManagerPolicy = await authorizationOptions.GetPolicyAsync(Policies.ProjectManagerOrAdmin);
    var projectMemberPolicy = await authorizationOptions.GetPolicyAsync(Policies.ProjectMemberOrAdmin);

        adminPolicy.Should().NotBeNull();
        teamManagerPolicy.Should().NotBeNull();
        projectManagerPolicy.Should().NotBeNull();
        projectMemberPolicy.Should().NotBeNull();
    }

    [Fact]
    public async Task AdminOnlyPolicy_ShouldRequireAdminRole()
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
    var adminResult = await authService.AuthorizeAsync(adminUser, Policies.AdminOnly);
    var nonAdminResult = await authService.AuthorizeAsync(nonAdminUser, Policies.AdminOnly);

        adminResult.Succeeded.Should().BeTrue();
        nonAdminResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task TeamManagerPolicy_ShouldAllowAdminAndProjectManager()
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
    var adminResult = await authService.AuthorizeAsync(adminUser, Policies.TeamManager);
    var projectManagerResult = await authService.AuthorizeAsync(projectManagerUser, Policies.TeamManager);
    var developerResult = await authService.AuthorizeAsync(developerUser, Policies.TeamManager);

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        developerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ProjectManagerOrAdminPolicy_ShouldAllowAdminAndProjectManager()
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
    var adminResult = await authService.AuthorizeAsync(adminUser, Policies.ProjectManagerOrAdmin);
    var projectManagerResult = await authService.AuthorizeAsync(projectManagerUser, Policies.ProjectManagerOrAdmin);
    var viewerResult = await authService.AuthorizeAsync(viewerUser, Policies.ProjectManagerOrAdmin);

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        viewerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ProjectMemberOrAdminPolicy_ShouldAllowElevatedRoles()
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
    var adminResult = await authService.AuthorizeAsync(adminUser, Policies.ProjectMemberOrAdmin);
    var projectManagerResult = await authService.AuthorizeAsync(projectManagerUser, Policies.ProjectMemberOrAdmin);
    var developerResult = await authService.AuthorizeAsync(developerUser, Policies.ProjectMemberOrAdmin);
    var viewerResult = await authService.AuthorizeAsync(viewerUser, Policies.ProjectMemberOrAdmin);

        adminResult.Succeeded.Should().BeTrue();
        projectManagerResult.Succeeded.Should().BeTrue();
        developerResult.Succeeded.Should().BeTrue();
        viewerResult.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task ProjectMemberOrAdminPolicy_ShouldAllowProjectMembershipClaim()
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
    var withProjectResult = await authService.AuthorizeAsync(viewerWithProjectClaim, Policies.ProjectMemberOrAdmin);
    var withoutProjectResult = await authService.AuthorizeAsync(viewerWithoutProjectClaim, Policies.ProjectMemberOrAdmin);

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