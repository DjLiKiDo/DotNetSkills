namespace DotNetSkills.Domain.UnitTests.UserManagement.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class UserRoleTests : TestBase
{
    [Theory]
    [InlineData(UserRole.Viewer, 1)]
    [InlineData(UserRole.Developer, 2)]
    [InlineData(UserRole.ProjectManager, 3)]
    [InlineData(UserRole.Admin, 4)]
    [Trait("TestType", "BusinessLogic")]
    public void UserRole_ShouldHaveCorrectValues(UserRole role, int expectedValue)
    {
        // Act & Assert
        ((int)role).Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(UserRole.Viewer, "Viewer")]
    [InlineData(UserRole.Developer, "Developer")]
    [InlineData(UserRole.ProjectManager, "Project Manager")]
    [InlineData(UserRole.Admin, "Administrator")]
    [Trait("TestType", "BusinessLogic")]
    public void GetDisplayName_ShouldReturnCorrectDisplayName(UserRole role, string expectedDisplayName)
    {
        // Act
        var displayName = role.GetDisplayName();

        // Assert
        displayName.Should().Be(expectedDisplayName);
    }

    [Theory]
    [InlineData(UserRole.Viewer)]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.ProjectManager)]
    [InlineData(UserRole.Admin)]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_ShouldReturnNonEmptyPermissions(UserRole role)
    {
        // Act
        var permissions = role.GetPermissions();

        // Assert
        permissions.Should().NotBeNull();
        permissions.Should().NotBeEmpty();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_Viewer_ShouldHaveReadOnlyPermissions()
    {
        // Act
        var permissions = UserRole.Viewer.GetPermissions();

        // Assert
        permissions.Should().Contain("read:projects");
        permissions.Should().Contain("read:tasks");
        permissions.Should().NotContain(p => p.StartsWith("create:"));
        permissions.Should().NotContain(p => p.StartsWith("update:"));
        permissions.Should().NotContain(p => p.StartsWith("delete:"));
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_Developer_ShouldHaveTaskPermissions()
    {
        // Act
        var permissions = UserRole.Developer.GetPermissions();

        // Assert
        permissions.Should().Contain("read:projects");
        permissions.Should().Contain("read:tasks");
        permissions.Should().Contain("update:tasks");
        permissions.Should().Contain("create:tasks");
        permissions.Should().NotContain("create:projects");
        permissions.Should().NotContain("manage:users");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_ProjectManager_ShouldHaveProjectAndTeamPermissions()
    {
        // Act
        var permissions = UserRole.ProjectManager.GetPermissions();

        // Assert
        permissions.Should().Contain("read:projects");
        permissions.Should().Contain("create:projects");
        permissions.Should().Contain("update:projects");
        permissions.Should().Contain("manage:teams");
        permissions.Should().Contain("assign:tasks");
        permissions.Should().NotContain("manage:users");
        permissions.Should().NotContain("manage:system");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_Admin_ShouldHaveAllPermissions()
    {
        // Act
        var permissions = UserRole.Admin.GetPermissions();

        // Assert
        permissions.Should().Contain("read:*");
        permissions.Should().Contain("create:*");
        permissions.Should().Contain("update:*");
        permissions.Should().Contain("delete:*");
        permissions.Should().Contain("manage:users");
        permissions.Should().Contain("manage:system");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_ShouldReturnReadOnlyCollection()
    {
        // Act
        var permissions = UserRole.Developer.GetPermissions();

        // Assert
        permissions.Should().BeAssignableTo<IReadOnlyList<string>>();
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetDisplayName_WithInvalidRole_ShouldReturnToString()
    {
        // Arrange
        var invalidRole = (UserRole)999;

        // Act
        var displayName = invalidRole.GetDisplayName();

        // Assert
        displayName.Should().Be(invalidRole.ToString());
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetPermissions_WithInvalidRole_ShouldReturnEmptyArray()
    {
        // Arrange
        var invalidRole = (UserRole)999;

        // Act
        var permissions = invalidRole.GetPermissions();

        // Assert
        permissions.Should().BeEmpty();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UserRole_EnumValues_ShouldBeInHierarchicalOrder()
    {
        // Assert - Values should be in ascending order of authority
        ((int)UserRole.Viewer).Should().BeLessThan((int)UserRole.Developer);
        ((int)UserRole.Developer).Should().BeLessThan((int)UserRole.ProjectManager);
        ((int)UserRole.ProjectManager).Should().BeLessThan((int)UserRole.Admin);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetPermissions_HigherRoles_ShouldHaveMorePermissions()
    {
        // Act
        var viewerPermissions = UserRole.Viewer.GetPermissions();
        var developerPermissions = UserRole.Developer.GetPermissions();
        var pmPermissions = UserRole.ProjectManager.GetPermissions();
        var adminPermissions = UserRole.Admin.GetPermissions();

        // Assert - Higher roles should generally have more permissions
        // Note: Admin uses wildcards so it has fewer strings but broader permissions
        developerPermissions.Count.Should().BeGreaterThan(viewerPermissions.Count);
        pmPermissions.Count.Should().BeGreaterThan(developerPermissions.Count);
        adminPermissions.Count.Should().BeGreaterThan(0); // Admin has wildcard permissions
        
        // Verify admin has the most powerful permissions (wildcards)
        adminPermissions.Should().Contain("read:*");
        adminPermissions.Should().Contain("create:*");
        adminPermissions.Should().Contain("update:*");
        adminPermissions.Should().Contain("delete:*");
    }
}