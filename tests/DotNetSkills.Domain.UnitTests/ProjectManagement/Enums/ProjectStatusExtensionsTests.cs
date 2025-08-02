namespace DotNetSkills.Domain.UnitTests.ProjectManagement.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectStatusExtensionsTests : TestBase
{
    #region GetDisplayName Tests

    [Theory]
    [InlineData(ProjectStatus.Planning, "Planning")]
    [InlineData(ProjectStatus.Active, "Active")]
    [InlineData(ProjectStatus.OnHold, "On Hold")]
    [InlineData(ProjectStatus.Completed, "Completed")]
    [InlineData(ProjectStatus.Cancelled, "Cancelled")]
    [Trait("TestType", "Extension")]
    public void GetDisplayName_WithValidStatus_ShouldReturnCorrectDisplayName(ProjectStatus status, string expected)
    {
        // Act
        var result = status.GetDisplayName();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetDisplayName_WithInvalidStatus_ShouldReturnToStringValue()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act
        var result = invalidStatus.GetDisplayName();

        // Assert
        result.Should().Be("999");
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetDisplayName_WithAllValidStatuses_ShouldReturnNonEmptyStrings()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var displayName = status.GetDisplayName();
            displayName.Should().NotBeNullOrEmpty($"Display name for {status} should not be null or empty");
        }
    }

    #endregion

    #region IsActive Tests

    [Theory]
    [InlineData(ProjectStatus.Planning, true)]
    [InlineData(ProjectStatus.Active, true)]
    [InlineData(ProjectStatus.OnHold, true)]
    [InlineData(ProjectStatus.Completed, false)]
    [InlineData(ProjectStatus.Cancelled, false)]
    [Trait("TestType", "Extension")]
    public void IsActive_ShouldReturnCorrectValue(ProjectStatus status, bool expected)
    {
        // Act
        var result = status.IsActive();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsActive_WithInvalidStatus_ShouldReturnFalse()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act
        var result = invalidStatus.IsActive();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsActive_AllActiveStatuses_ShouldReturnTrue()
    {
        // Arrange
        var activeStatuses = new[] { ProjectStatus.Planning, ProjectStatus.Active, ProjectStatus.OnHold };

        // Act & Assert
        foreach (var status in activeStatuses)
        {
            status.IsActive().Should().BeTrue($"{status} should be considered active");
        }
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsActive_AllFinalizedStatuses_ShouldReturnFalse()
    {
        // Arrange
        var finalizedStatuses = new[] { ProjectStatus.Completed, ProjectStatus.Cancelled };

        // Act & Assert
        foreach (var status in finalizedStatuses)
        {
            status.IsActive().Should().BeFalse($"{status} should not be considered active");
        }
    }

    #endregion

    #region IsFinalized Tests

    [Theory]
    [InlineData(ProjectStatus.Planning, false)]
    [InlineData(ProjectStatus.Active, false)]
    [InlineData(ProjectStatus.OnHold, false)]
    [InlineData(ProjectStatus.Completed, true)]
    [InlineData(ProjectStatus.Cancelled, true)]
    [Trait("TestType", "Extension")]
    public void IsFinalized_ShouldReturnCorrectValue(ProjectStatus status, bool expected)
    {
        // Act
        var result = status.IsFinalized();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsFinalized_WithInvalidStatus_ShouldReturnFalse()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act
        var result = invalidStatus.IsFinalized();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsFinalized_AllFinalizedStatuses_ShouldReturnTrue()
    {
        // Arrange
        var finalizedStatuses = new[] { ProjectStatus.Completed, ProjectStatus.Cancelled };

        // Act & Assert
        foreach (var status in finalizedStatuses)
        {
            status.IsFinalized().Should().BeTrue($"{status} should be considered finalized");
        }
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsFinalized_AllActiveStatuses_ShouldReturnFalse()
    {
        // Arrange
        var activeStatuses = new[] { ProjectStatus.Planning, ProjectStatus.Active, ProjectStatus.OnHold };

        // Act & Assert
        foreach (var status in activeStatuses)
        {
            status.IsFinalized().Should().BeFalse($"{status} should not be considered finalized");
        }
    }

    #endregion

    #region IsActive vs IsFinalized Mutual Exclusion Tests

    [Fact]
    [Trait("TestType", "Extension")]
    public void IsActive_And_IsFinalized_ShouldBeMutuallyExclusive()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var isActive = status.IsActive();
            var isFinalized = status.IsFinalized();

            // Should not be both active and finalized
            (isActive && isFinalized).Should().BeFalse($"{status} should not be both active and finalized");
            
            // Should be either active or finalized (for valid statuses)
            if (Enum.IsDefined(status))
            {
                (isActive || isFinalized).Should().BeTrue($"{status} should be either active or finalized");
            }
        }
    }

    #endregion

    #region GetColorCode Tests

    [Theory]
    [InlineData(ProjectStatus.Planning, "#6f42c1")]   // Purple
    [InlineData(ProjectStatus.Active, "#28a745")]     // Green
    [InlineData(ProjectStatus.OnHold, "#ffc107")]     // Yellow
    [InlineData(ProjectStatus.Completed, "#17a2b8")]  // Cyan
    [InlineData(ProjectStatus.Cancelled, "#dc3545")]  // Red
    [Trait("TestType", "Extension")]
    public void GetColorCode_WithValidStatus_ShouldReturnCorrectColorCode(ProjectStatus status, string expectedColor)
    {
        // Act
        var result = status.GetColorCode();

        // Assert
        result.Should().Be(expectedColor);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetColorCode_WithInvalidStatus_ShouldReturnDefaultGrayColor()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act
        var result = invalidStatus.GetColorCode();

        // Assert
        result.Should().Be("#6c757d"); // Gray (default)
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetColorCode_AllResults_ShouldBeValidHexColors()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();
        var hexColorPattern = @"^#[0-9a-fA-F]{6}$";

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var colorCode = status.GetColorCode();
            colorCode.Should().MatchRegex(hexColorPattern, $"Color code for {status} should be a valid hex color");
        }
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetColorCode_AllValidStatuses_ShouldReturnUniqueColors()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act
        var colorCodes = allStatuses.Select(s => s.GetColorCode()).ToList();

        // Assert
        colorCodes.Should().OnlyHaveUniqueItems("Each status should have a unique color code");
    }

    #endregion

    #region GetProgressWeight Tests

    [Theory]
    [InlineData(ProjectStatus.Planning, 0.0)]
    [InlineData(ProjectStatus.Active, 0.5)]
    [InlineData(ProjectStatus.OnHold, 0.5)]
    [InlineData(ProjectStatus.Completed, 1.0)]
    [InlineData(ProjectStatus.Cancelled, 0.0)]
    [Trait("TestType", "Extension")]
    public void GetProgressWeight_WithValidStatus_ShouldReturnCorrectWeight(ProjectStatus status, decimal expectedWeight)
    {
        // Act
        var result = status.GetProgressWeight();

        // Assert
        result.Should().Be(expectedWeight);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetProgressWeight_WithInvalidStatus_ShouldReturnZero()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act
        var result = invalidStatus.GetProgressWeight();

        // Assert
        result.Should().Be(0.0m);
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetProgressWeight_AllResults_ShouldBeBetweenZeroAndOne()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var weight = status.GetProgressWeight();
            weight.Should().BeGreaterOrEqualTo(0.0m, $"Progress weight for {status} should be >= 0");
            weight.Should().BeLessOrEqualTo(1.0m, $"Progress weight for {status} should be <= 1");
        }
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetProgressWeight_CompletedStatus_ShouldHaveHighestWeight()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act
        var completedWeight = ProjectStatus.Completed.GetProgressWeight();
        var otherWeights = allStatuses.Where(s => s != ProjectStatus.Completed)
                                     .Select(s => s.GetProgressWeight());

        // Assert
        completedWeight.Should().Be(1.0m);
        otherWeights.Should().AllSatisfy(weight => weight.Should().BeLessThan(completedWeight));
    }

    [Fact]
    [Trait("TestType", "Extension")]
    public void GetProgressWeight_ActiveAndOnHold_ShouldHaveSameWeight()
    {
        // Act
        var activeWeight = ProjectStatus.Active.GetProgressWeight();
        var onHoldWeight = ProjectStatus.OnHold.GetProgressWeight();

        // Assert
        activeWeight.Should().Be(onHoldWeight, "Active and OnHold should have the same progress weight");
    }

    #endregion

    #region Business Logic Consistency Tests

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void FinalizedStatuses_ShouldHaveDefinitiveProgressWeights()
    {
        // Act & Assert
        ProjectStatus.Completed.GetProgressWeight().Should().Be(1.0m, "Completed projects are 100% done");
        ProjectStatus.Cancelled.GetProgressWeight().Should().Be(0.0m, "Cancelled projects have no progress");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ActiveStatuses_ShouldHaveDistinctColorCodes()
    {
        // Arrange
        var activeStatuses = new[] { ProjectStatus.Planning, ProjectStatus.Active, ProjectStatus.OnHold };

        // Act
        var colorCodes = activeStatuses.Select(s => s.GetColorCode()).ToList();

        // Assert
        colorCodes.Should().OnlyHaveUniqueItems("Active statuses should have distinct colors for UI differentiation");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AllExtensionMethods_ShouldHandleInvalidStatusesGracefully()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)(-1);

        // Act & Assert
        invalidStatus.GetDisplayName().Should().NotBeNullOrEmpty();
        invalidStatus.IsActive().Should().BeFalse();
        invalidStatus.IsFinalized().Should().BeFalse();
        invalidStatus.GetColorCode().Should().Be("#6c757d"); // Default gray
        invalidStatus.GetProgressWeight().Should().Be(0.0m);
    }

    #endregion

    #region Performance Tests

    [Fact]
    [Trait("TestType", "Performance")]
    public void ExtensionMethods_ShouldBeFast()
    {
        // Arrange
        var statuses = Enum.GetValues<ProjectStatus>().ToList();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 10000; i++)
        {
            var status = statuses[i % statuses.Count];
            _ = status.GetDisplayName();
            _ = status.IsActive();
            _ = status.IsFinalized();
            _ = status.GetColorCode();
            _ = status.GetProgressWeight();
        }

        // Assert
        stopwatch.Stop();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "Extension methods should be fast");
    }

    #endregion

    #region Integration Tests

    [Fact]
    [Trait("TestType", "Integration")]
    public void DisplayNames_ShouldBeAppropriateForUserInterface()
    {
        // Act & Assert
        ProjectStatus.OnHold.GetDisplayName().Should().Be("On Hold", "Should be human-readable with proper spacing");
        
        var allDisplayNames = Enum.GetValues<ProjectStatus>()
                                  .Select(s => s.GetDisplayName())
                                  .ToList();
        
        allDisplayNames.Should().AllSatisfy(name => 
        {
            name.Should().NotBeNullOrWhiteSpace();
            name.Length.Should().BeGreaterThan(0);
            name.Length.Should().BeLessThan(50); // Reasonable UI limit
        });
    }

    [Fact]
    [Trait("TestType", "Integration")]
    public void ColorCodes_ShouldFollowBootstrapConventions()
    {
        // Act & Assert
        ProjectStatus.Active.GetColorCode().Should().Be("#28a745", "Should use Bootstrap success color");
        ProjectStatus.Cancelled.GetColorCode().Should().Be("#dc3545", "Should use Bootstrap danger color");
        ProjectStatus.OnHold.GetColorCode().Should().Be("#ffc107", "Should use Bootstrap warning color");
        ProjectStatus.Completed.GetColorCode().Should().Be("#17a2b8", "Should use Bootstrap info color");
        ProjectStatus.Planning.GetColorCode().Should().Be("#6f42c1", "Should use Bootstrap secondary/purple color");
    }

    #endregion
}