namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamRoleExtensionsTests : TestBase
{
    [Theory]
    [InlineData(TeamRole.Developer, "Developer")]
    [InlineData(TeamRole.ProjectManager, "Project Manager")]
    [InlineData(TeamRole.TeamLead, "Team Lead")]
    [InlineData(TeamRole.Viewer, "Viewer")]
    [Trait("TestType", "BusinessLogic")]
    public void GetDisplayName_ShouldReturnCorrectDisplayName(TeamRole role, string expectedDisplayName)
    {
        // Act
        var result = role.GetDisplayName();

        // Assert
        result.Should().Be(expectedDisplayName);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetDisplayName_WithInvalidRole_ShouldReturnToStringValue()
    {
        // Arrange
        var invalidRole = (TeamRole)999;

        // Act
        var result = invalidRole.GetDisplayName();

        // Assert
        result.Should().Be(invalidRole.ToString());
    }

    [Theory]
    [InlineData(TeamRole.ProjectManager, "#dc3545")] // Red
    [InlineData(TeamRole.TeamLead, "#fd7e14")]       // Orange
    [InlineData(TeamRole.Developer, "#007bff")]      // Blue
    [InlineData(TeamRole.Viewer, "#6c757d")]         // Gray
    [Trait("TestType", "BusinessLogic")]
    public void GetColorCode_ShouldReturnCorrectColorCode(TeamRole role, string expectedColor)
    {
        // Act
        var result = role.GetColorCode();

        // Assert
        result.Should().Be(expectedColor);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetColorCode_WithInvalidRole_ShouldReturnDefaultGray()
    {
        // Arrange
        var invalidRole = (TeamRole)999;

        // Act
        var result = invalidRole.GetColorCode();

        // Assert
        result.Should().Be("#6c757d"); // Default gray
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetColorCode_AllColorsShouldBeValidHexCodes()
    {
        // Arrange
        var allRoles = Enum.GetValues<TeamRole>();

        // Act & Assert
        foreach (var role in allRoles)
        {
            var colorCode = role.GetColorCode();
            colorCode.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$", 
                $"Color code for {role} should be a valid hex color");
        }
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_ProjectManager_ShouldReturnCorrectResponsibilities()
    {
        // Act
        var responsibilities = TeamRole.ProjectManager.GetResponsibilities();

        // Assert
        responsibilities.Should().NotBeNull();
        responsibilities.Should().HaveCount(4);
        responsibilities.Should().Contain("Manage project scope and timeline");
        responsibilities.Should().Contain("Assign and reassign tasks");
        responsibilities.Should().Contain("Manage team membership and roles");
        responsibilities.Should().Contain("Report to stakeholders");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_TeamLead_ShouldReturnCorrectResponsibilities()
    {
        // Act
        var responsibilities = TeamRole.TeamLead.GetResponsibilities();

        // Assert
        responsibilities.Should().NotBeNull();
        responsibilities.Should().HaveCount(4);
        responsibilities.Should().Contain("Lead technical decisions");
        responsibilities.Should().Contain("Mentor team members");
        responsibilities.Should().Contain("Assign tasks to developers");
        responsibilities.Should().Contain("Review code and technical work");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_Developer_ShouldReturnCorrectResponsibilities()
    {
        // Act
        var responsibilities = TeamRole.Developer.GetResponsibilities();

        // Assert
        responsibilities.Should().NotBeNull();
        responsibilities.Should().HaveCount(4);
        responsibilities.Should().Contain("Complete assigned tasks");
        responsibilities.Should().Contain("Participate in code reviews");
        responsibilities.Should().Contain("Collaborate with team members");
        responsibilities.Should().Contain("Contribute to technical discussions");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_Viewer_ShouldReturnCorrectResponsibilities()
    {
        // Act
        var responsibilities = TeamRole.Viewer.GetResponsibilities();

        // Assert
        responsibilities.Should().NotBeNull();
        responsibilities.Should().HaveCount(3);
        responsibilities.Should().Contain("View project progress");
        responsibilities.Should().Contain("Access reports and metrics");
        responsibilities.Should().Contain("Observe team activities");
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetResponsibilities_WithInvalidRole_ShouldReturnEmptyArray()
    {
        // Arrange
        var invalidRole = (TeamRole)999;

        // Act
        var responsibilities = invalidRole.GetResponsibilities();

        // Assert
        responsibilities.Should().NotBeNull();
        responsibilities.Should().BeEmpty();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_ShouldReturnReadOnlyList()
    {
        // Act
        var responsibilities = TeamRole.Developer.GetResponsibilities();

        // Assert
        responsibilities.Should().BeAssignableTo<IReadOnlyList<string>>();
    }

    [Theory]
    [InlineData(TeamRole.ProjectManager, "briefcase")]
    [InlineData(TeamRole.TeamLead, "users")]
    [InlineData(TeamRole.Developer, "code")]
    [InlineData(TeamRole.Viewer, "eye")]
    [Trait("TestType", "BusinessLogic")]
    public void GetIconName_ShouldReturnCorrectIconName(TeamRole role, string expectedIcon)
    {
        // Act
        var result = role.GetIconName();

        // Assert
        result.Should().Be(expectedIcon);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetIconName_WithInvalidRole_ShouldReturnDefaultIcon()
    {
        // Arrange
        var invalidRole = (TeamRole)999;

        // Act
        var result = invalidRole.GetIconName();

        // Assert
        result.Should().Be("user");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetIconName_AllIconsShouldBeNonEmpty()
    {
        // Arrange
        var allRoles = Enum.GetValues<TeamRole>();

        // Act & Assert
        foreach (var role in allRoles)
        {
            var iconName = role.GetIconName();
            iconName.Should().NotBeNullOrWhiteSpace($"Icon name for {role} should not be null or empty");
        }
    }

    [Fact]
    [Trait("TestType", "Performance")]
    public void ExtensionMethods_ShouldPerformWell()
    {
        // Arrange
        var role = TeamRole.ProjectManager;
        var iterations = 1000;

        // Act & Assert - These should complete quickly
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < iterations; i++)
        {
            _ = role.GetDisplayName();
            _ = role.GetColorCode();
            _ = role.GetResponsibilities();
            _ = role.GetIconName();
        }
        
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "Extension methods should be fast");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AllExtensionMethods_ShouldReturnNonNullValues()
    {
        // Arrange
        var allRoles = Enum.GetValues<TeamRole>();

        // Act & Assert
        foreach (var role in allRoles)
        {
            role.GetDisplayName().Should().NotBeNull($"DisplayName for {role} should not be null");
            role.GetColorCode().Should().NotBeNull($"ColorCode for {role} should not be null");
            role.GetResponsibilities().Should().NotBeNull($"Responsibilities for {role} should not be null");
            role.GetIconName().Should().NotBeNull($"IconName for {role} should not be null");
        }
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetResponsibilities_AllRoles_ShouldHaveAtLeastOneResponsibility()
    {
        // Arrange
        var allRoles = Enum.GetValues<TeamRole>();

        // Act & Assert
        foreach (var role in allRoles)
        {
            var responsibilities = role.GetResponsibilities();
            responsibilities.Should().NotBeEmpty($"{role} should have at least one responsibility");
        }
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ExtensionMethods_ShouldBeConsistent()
    {
        // Arrange
        var role = TeamRole.Developer;

        // Act - Call methods multiple times
        var displayName1 = role.GetDisplayName();
        var displayName2 = role.GetDisplayName();
        var colorCode1 = role.GetColorCode();
        var colorCode2 = role.GetColorCode();
        var responsibilities1 = role.GetResponsibilities();
        var responsibilities2 = role.GetResponsibilities();
        var iconName1 = role.GetIconName();
        var iconName2 = role.GetIconName();

        // Assert - Results should be consistent
        displayName1.Should().Be(displayName2);
        colorCode1.Should().Be(colorCode2);
        responsibilities1.Should().BeEquivalentTo(responsibilities2);
        iconName1.Should().Be(iconName2);
    }
}