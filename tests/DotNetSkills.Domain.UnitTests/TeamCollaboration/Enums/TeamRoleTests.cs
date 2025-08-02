namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.Enums;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamRoleTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void TeamRole_ShouldHaveCorrectEnumValues()
    {
        // Act & Assert
        ((int)TeamRole.Developer).Should().Be(1);
        ((int)TeamRole.ProjectManager).Should().Be(2);
        ((int)TeamRole.TeamLead).Should().Be(3);
        ((int)TeamRole.Viewer).Should().Be(4);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamRole_ShouldHaveAllExpectedValues()
    {
        // Arrange
        var expectedValues = new[]
        {
            TeamRole.Developer,
            TeamRole.ProjectManager,
            TeamRole.TeamLead,
            TeamRole.Viewer
        };

        // Act
        var actualValues = Enum.GetValues<TeamRole>();

        // Assert
        actualValues.Should().BeEquivalentTo(expectedValues);
        actualValues.Should().HaveCount(4);
    }

    [Theory]
    [InlineData(TeamRole.Developer, "Developer")]
    [InlineData(TeamRole.ProjectManager, "ProjectManager")]
    [InlineData(TeamRole.TeamLead, "TeamLead")]
    [InlineData(TeamRole.Viewer, "Viewer")]
    [Trait("TestType", "BusinessLogic")]
    public void ToString_ShouldReturnCorrectStringRepresentation(TeamRole role, string expected)
    {
        // Act
        var result = role.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("Developer", TeamRole.Developer)]
    [InlineData("ProjectManager", TeamRole.ProjectManager)]
    [InlineData("TeamLead", TeamRole.TeamLead)]
    [InlineData("Viewer", TeamRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void Parse_WithValidString_ShouldReturnCorrectEnum(string input, TeamRole expected)
    {
        // Act
        var result = Enum.Parse<TeamRole>(input);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("DEVELOPER")]
    [InlineData("developer")]
    [InlineData("Dev")]
    [InlineData("InvalidRole")]
    [InlineData("")]
    [Trait("TestType", "Validation")]
    public void Parse_WithInvalidString_ShouldThrowArgumentException(string input)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Enum.Parse<TeamRole>(input));
        exception.Should().NotBeNull();
    }

    [Theory]
    [InlineData("Developer", true, TeamRole.Developer)]
    [InlineData("ProjectManager", true, TeamRole.ProjectManager)]
    [InlineData("TeamLead", true, TeamRole.TeamLead)]
    [InlineData("Viewer", true, TeamRole.Viewer)]
    [InlineData("InvalidRole", false, default(TeamRole))]
    [InlineData("", false, default(TeamRole))]
    [Trait("TestType", "BusinessLogic")]
    public void TryParse_ShouldReturnCorrectResults(string input, bool expectedSuccess, TeamRole expectedValue)
    {
        // Act
        var success = Enum.TryParse<TeamRole>(input, out var result);

        // Assert
        success.Should().Be(expectedSuccess);
        if (expectedSuccess)
        {
            result.Should().Be(expectedValue);
        }
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetNames_ShouldReturnAllRoleNames()
    {
        // Act
        var names = Enum.GetNames<TeamRole>();

        // Assert
        names.Should().BeEquivalentTo(new[]
        {
            "Developer",
            "ProjectManager", 
            "TeamLead",
            "Viewer"
        });
        names.Should().HaveCount(4);
    }

    [Theory]
    [InlineData(1, TeamRole.Developer)]
    [InlineData(2, TeamRole.ProjectManager)]
    [InlineData(3, TeamRole.TeamLead)]
    [InlineData(4, TeamRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void Cast_FromInt_ShouldReturnCorrectEnum(int value, TeamRole expected)
    {
        // Act
        var result = (TeamRole)value;

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(-1)]
    [InlineData(100)]
    [Trait("TestType", "EdgeCase")]
    public void Cast_FromInvalidInt_ShouldCreateInvalidEnum(int value)
    {
        // Act
        var result = (TeamRole)value;

        // Assert
        Enum.IsDefined(typeof(TeamRole), result).Should().BeFalse();
    }

    [Theory]
    [InlineData(TeamRole.Developer)]
    [InlineData(TeamRole.ProjectManager)]
    [InlineData(TeamRole.TeamLead)]
    [InlineData(TeamRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void IsDefined_WithValidEnum_ShouldReturnTrue(TeamRole role)
    {
        // Act
        var result = Enum.IsDefined(typeof(TeamRole), role);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void IsDefined_WithInvalidEnum_ShouldReturnFalse()
    {
        // Arrange
        var invalidRole = (TeamRole)999;

        // Act
        var result = Enum.IsDefined(typeof(TeamRole), invalidRole);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamRole_ShouldBeComparable()
    {
        // Arrange
        var developer = TeamRole.Developer;
        var projectManager = TeamRole.ProjectManager;
        var teamLead = TeamRole.TeamLead;
        var viewer = TeamRole.Viewer;

        // Act & Assert
        (developer < projectManager).Should().BeTrue();
        (projectManager < teamLead).Should().BeTrue();
        (teamLead < viewer).Should().BeTrue();
        
        (viewer > teamLead).Should().BeTrue();
        (teamLead > projectManager).Should().BeTrue();
        (projectManager > developer).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamRole_EqualityOperators_ShouldWorkCorrectly()
    {
        // Arrange
        var role1 = TeamRole.Developer;
        var role2 = TeamRole.Developer;
        var role3 = TeamRole.ProjectManager;

        // Act & Assert
        (role1 == role2).Should().BeTrue();
        (role1 != role3).Should().BeTrue();
        (role1 == role3).Should().BeFalse();
        role1.Equals(role2).Should().BeTrue();
        role1.Equals(role3).Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var role1 = TeamRole.Developer;
        var role2 = TeamRole.Developer;

        // Act
        var hash1 = role1.GetHashCode();
        var hash2 = role2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }
}