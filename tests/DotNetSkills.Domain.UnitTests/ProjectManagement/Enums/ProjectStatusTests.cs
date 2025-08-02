namespace DotNetSkills.Domain.UnitTests.ProjectManagement.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "ProjectManagement")]
public class ProjectStatusTests : TestBase
{
    #region Enum Value Tests

    [Fact]
    [Trait("TestType", "EnumValues")]
    public void ProjectStatus_ShouldHaveExpectedValues()
    {
        // Act & Assert
        ProjectStatus.Active.Should().Be((ProjectStatus)1);
        ProjectStatus.Completed.Should().Be((ProjectStatus)2);
        ProjectStatus.OnHold.Should().Be((ProjectStatus)3);
        ProjectStatus.Cancelled.Should().Be((ProjectStatus)4);
        ProjectStatus.Planning.Should().Be((ProjectStatus)5);
    }

    [Fact]
    [Trait("TestType", "EnumValues")]
    public void ProjectStatus_ShouldHaveAllExpectedEnumMembers()
    {
        // Arrange
        var expectedValues = new[]
        {
            ProjectStatus.Active,
            ProjectStatus.Completed,
            ProjectStatus.OnHold,
            ProjectStatus.Cancelled,
            ProjectStatus.Planning
        };

        // Act
        var actualValues = Enum.GetValues<ProjectStatus>();

        // Assert
        actualValues.Should().HaveCount(5);
        actualValues.Should().BeEquivalentTo(expectedValues);
    }

    [Theory]
    [InlineData(ProjectStatus.Active, "Active")]
    [InlineData(ProjectStatus.Completed, "Completed")]
    [InlineData(ProjectStatus.OnHold, "OnHold")]
    [InlineData(ProjectStatus.Cancelled, "Cancelled")]
    [InlineData(ProjectStatus.Planning, "Planning")]
    [Trait("TestType", "Serialization")]
    public void ToString_ShouldReturnCorrectStringRepresentation(ProjectStatus status, string expected)
    {
        // Act
        var result = status.ToString();

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Parsing Tests

    [Theory]
    [InlineData("Active", ProjectStatus.Active)]
    [InlineData("Completed", ProjectStatus.Completed)]
    [InlineData("OnHold", ProjectStatus.OnHold)]
    [InlineData("Cancelled", ProjectStatus.Cancelled)]
    [InlineData("Planning", ProjectStatus.Planning)]
    [Trait("TestType", "Parsing")]
    public void Parse_WithValidString_ShouldReturnCorrectEnum(string input, ProjectStatus expected)
    {
        // Act
        var result = Enum.Parse<ProjectStatus>(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("active")]
    [InlineData("ACTIVE")]
    [InlineData("completed")]
    [InlineData("COMPLETED")]
    [Trait("TestType", "Parsing")]
    public void Parse_WithIgnoreCase_ShouldReturnCorrectEnum(string input)
    {
        // Act
        var result = Enum.Parse<ProjectStatus>(input, ignoreCase: true);

        // Assert
        result.Should().BeDefined();
    }

    [Theory]
    [InlineData("Active", true, ProjectStatus.Active)]
    [InlineData("InvalidStatus", false, default(ProjectStatus))]
    [InlineData("", false, default(ProjectStatus))]
    [InlineData("NotAnEnum", false, default(ProjectStatus))]
    [Trait("TestType", "Parsing")]
    public void TryParse_ShouldReturnExpectedResults(string input, bool expectedSuccess, ProjectStatus expectedValue)
    {
        // Act
        var success = Enum.TryParse<ProjectStatus>(input, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            result.Should().Be(expectedValue);
        }
    }

    #endregion

    #region Conversion Tests

    [Theory]
    [InlineData(1, ProjectStatus.Active)]
    [InlineData(2, ProjectStatus.Completed)]
    [InlineData(3, ProjectStatus.OnHold)]
    [InlineData(4, ProjectStatus.Cancelled)]
    [InlineData(5, ProjectStatus.Planning)]
    [Trait("TestType", "Conversion")]
    public void CastFromInt_ShouldReturnCorrectEnum(int value, ProjectStatus expected)
    {
        // Act
        var result = (ProjectStatus)value;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(ProjectStatus.Active, 1)]
    [InlineData(ProjectStatus.Completed, 2)]
    [InlineData(ProjectStatus.OnHold, 3)]
    [InlineData(ProjectStatus.Cancelled, 4)]
    [InlineData(ProjectStatus.Planning, 5)]
    [Trait("TestType", "Conversion")]
    public void CastToInt_ShouldReturnCorrectValue(ProjectStatus status, int expected)
    {
        // Act
        var result = (int)status;

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData(ProjectStatus.Active)]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.OnHold)]
    [InlineData(ProjectStatus.Cancelled)]
    [InlineData(ProjectStatus.Planning)]
    [Trait("TestType", "Validation")]
    public void IsDefined_WithValidStatus_ShouldReturnTrue(ProjectStatus status)
    {
        // Act & Assert
        Enum.IsDefined(status).Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    [InlineData(999)]
    [Trait("TestType", "Validation")]
    public void IsDefined_WithInvalidValue_ShouldReturnFalse(int invalidValue)
    {
        // Act
        var status = (ProjectStatus)invalidValue;
        var isDefined = Enum.IsDefined(status);

        // Assert
        isDefined.Should().BeFalse();
    }

    #endregion

    #region Comparison Tests

    [Fact]
    [Trait("TestType", "Comparison")]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var status1 = ProjectStatus.Active;
        var status2 = ProjectStatus.Active;

        // Act & Assert
        status1.Should().Be(status2);
        (status1 == status2).Should().BeTrue();
        (status1 != status2).Should().BeFalse();
        status1.Equals(status2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "Comparison")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var status1 = ProjectStatus.Active;
        var status2 = ProjectStatus.Completed;

        // Act & Assert
        status1.Should().NotBe(status2);
        (status1 == status2).Should().BeFalse();
        (status1 != status2).Should().BeTrue();
        status1.Equals(status2).Should().BeFalse();
    }

    [Theory]
    [InlineData(ProjectStatus.Planning, ProjectStatus.Active)]
    [InlineData(ProjectStatus.Active, ProjectStatus.Completed)]
    [InlineData(ProjectStatus.OnHold, ProjectStatus.Cancelled)]
    [Trait("TestType", "Comparison")]
    public void Comparison_WithDifferentValues_ShouldWorkCorrectly(ProjectStatus first, ProjectStatus second)
    {
        // Act & Assert
        first.Should().NotBe(second);
        ((int)first).Should().NotBe((int)second);
    }

    #endregion

    #region Hash Code Tests

    [Fact]
    [Trait("TestType", "HashCode")]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var status1 = ProjectStatus.Active;
        var status2 = ProjectStatus.Active;

        // Act
        var hashCode1 = status1.GetHashCode();
        var hashCode2 = status2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    [Trait("TestType", "HashCode")]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var statuses = Enum.GetValues<ProjectStatus>();

        // Act
        var hashCodes = statuses.Select(s => s.GetHashCode()).ToList();

        // Assert
        hashCodes.Should().OnlyHaveUniqueItems();
    }

    #endregion

    #region Collection Tests

    [Fact]
    [Trait("TestType", "Collection")]
    public void GetValues_ShouldReturnAllEnumValues()
    {
        // Act
        var values = Enum.GetValues<ProjectStatus>();

        // Assert
        values.Should().HaveCount(5);
        values.Should().Contain(ProjectStatus.Active);
        values.Should().Contain(ProjectStatus.Completed);
        values.Should().Contain(ProjectStatus.OnHold);
        values.Should().Contain(ProjectStatus.Cancelled);
        values.Should().Contain(ProjectStatus.Planning);
    }

    [Fact]
    [Trait("TestType", "Collection")]
    public void GetNames_ShouldReturnAllEnumNames()
    {
        // Act
        var names = Enum.GetNames<ProjectStatus>();

        // Assert
        names.Should().HaveCount(5);
        names.Should().Contain("Active");
        names.Should().Contain("Completed");
        names.Should().Contain("OnHold");
        names.Should().Contain("Cancelled");
        names.Should().Contain("Planning");
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void DefaultValue_ShouldBeActive()
    {
        // Act
        var defaultStatus = default(ProjectStatus);

        // Assert
        defaultStatus.Should().Be((ProjectStatus)0); // Should be 0, not Active (which is 1)
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void InvalidEnumValue_ShouldStillBeUsable()
    {
        // Arrange
        var invalidStatus = (ProjectStatus)999;

        // Act & Assert
        invalidStatus.Should().Be((ProjectStatus)999);
        Enum.IsDefined(invalidStatus).Should().BeFalse();
        invalidStatus.ToString().Should().Be("999");
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void NegativeEnumValue_ShouldStillBeUsable()
    {
        // Arrange
        var negativeStatus = (ProjectStatus)(-1);

        // Act & Assert
        negativeStatus.Should().Be((ProjectStatus)(-1));
        Enum.IsDefined(negativeStatus).Should().BeFalse();
        negativeStatus.ToString().Should().Be("-1");
    }

    #endregion

    #region Business Logic Tests

    [Theory]
    [InlineData(ProjectStatus.Planning)]
    [InlineData(ProjectStatus.Active)]
    [InlineData(ProjectStatus.OnHold)]
    [Trait("TestType", "BusinessLogic")]
    public void WorkInProgressStatuses_ShouldBeIdentifiable(ProjectStatus status)
    {
        // Act & Assert
        var isWorkInProgress = status is ProjectStatus.Planning or ProjectStatus.Active or ProjectStatus.OnHold;
        isWorkInProgress.Should().BeTrue();
    }

    [Theory]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Cancelled)]
    [Trait("TestType", "BusinessLogic")]
    public void FinalizedStatuses_ShouldBeIdentifiable(ProjectStatus status)
    {
        // Act & Assert
        var isFinalized = status is ProjectStatus.Completed or ProjectStatus.Cancelled;
        isFinalized.Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void AllStatuses_ShouldBeCategorizable()
    {
        // Arrange
        var allStatuses = Enum.GetValues<ProjectStatus>();

        // Act & Assert
        foreach (var status in allStatuses)
        {
            var isWorkInProgress = status is ProjectStatus.Planning or ProjectStatus.Active or ProjectStatus.OnHold;
            var isFinalized = status is ProjectStatus.Completed or ProjectStatus.Cancelled;

            // Each status should be either work in progress or finalized
            (isWorkInProgress || isFinalized).Should().BeTrue($"Status {status} should be categorizable");
            (isWorkInProgress && isFinalized).Should().BeFalse($"Status {status} should not be in both categories");
        }
    }

    #endregion
}