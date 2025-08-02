namespace DotNetSkills.Domain.UnitTests.UserManagement.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class UserStatusTests : TestBase
{
    [Theory]
    [InlineData(UserStatus.Active, 1)]
    [InlineData(UserStatus.Inactive, 2)]
    [InlineData(UserStatus.Suspended, 3)]
    [InlineData(UserStatus.Pending, 4)]
    [Trait("TestType", "BusinessLogic")]
    public void UserStatus_ShouldHaveCorrectValues(UserStatus status, int expectedValue)
    {
        // Act & Assert
        ((int)status).Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(UserStatus.Active, "Active")]
    [InlineData(UserStatus.Inactive, "Inactive")]
    [InlineData(UserStatus.Suspended, "Suspended")]
    [InlineData(UserStatus.Pending, "Pending Activation")]
    [Trait("TestType", "BusinessLogic")]
    public void GetDisplayName_ShouldReturnCorrectDisplayName(UserStatus status, string expectedDisplayName)
    {
        // Act
        var displayName = status.GetDisplayName();

        // Assert
        displayName.Should().Be(expectedDisplayName);
    }

    [Theory]
    [InlineData(UserStatus.Active, "#28a745")]
    [InlineData(UserStatus.Inactive, "#6c757d")]
    [InlineData(UserStatus.Suspended, "#dc3545")]
    [InlineData(UserStatus.Pending, "#ffc107")]
    [Trait("TestType", "BusinessLogic")]
    public void GetColorCode_ShouldReturnCorrectColorCode(UserStatus status, string expectedColorCode)
    {
        // Act
        var colorCode = status.GetColorCode();

        // Assert
        colorCode.Should().Be(expectedColorCode);
    }

    [Theory]
    [InlineData(UserStatus.Active, "User can perform all authorized operations")]
    [InlineData(UserStatus.Inactive, "User account is deactivated and cannot access the system")]
    [InlineData(UserStatus.Suspended, "User account is temporarily suspended due to policy violations")]
    [InlineData(UserStatus.Pending, "User account is newly created and pending activation")]
    [Trait("TestType", "BusinessLogic")]
    public void GetDescription_ShouldReturnCorrectDescription(UserStatus status, string expectedDescription)
    {
        // Act
        var description = status.GetDescription();

        // Assert
        description.Should().Be(expectedDescription);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetDisplayName_WithInvalidStatus_ShouldReturnToString()
    {
        // Arrange
        var invalidStatus = (UserStatus)999;

        // Act
        var displayName = invalidStatus.GetDisplayName();

        // Assert
        displayName.Should().Be(invalidStatus.ToString());
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetColorCode_WithInvalidStatus_ShouldReturnDefaultGray()
    {
        // Arrange
        var invalidStatus = (UserStatus)999;

        // Act
        var colorCode = invalidStatus.GetColorCode();

        // Assert
        colorCode.Should().Be("#6c757d"); // Default gray
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void GetDescription_WithInvalidStatus_ShouldReturnUnknownStatus()
    {
        // Arrange
        var invalidStatus = (UserStatus)999;

        // Act
        var description = invalidStatus.GetDescription();

        // Assert
        description.Should().Be("Unknown status");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UserStatus_EnumValues_ShouldFollowLogicalProgression()
    {
        // Assert - Values should be in logical order: Active, Inactive, Suspended, Pending
        ((int)UserStatus.Active).Should().Be(1);
        ((int)UserStatus.Inactive).Should().Be(2);
        ((int)UserStatus.Suspended).Should().Be(3);
        ((int)UserStatus.Pending).Should().Be(4);
    }
}