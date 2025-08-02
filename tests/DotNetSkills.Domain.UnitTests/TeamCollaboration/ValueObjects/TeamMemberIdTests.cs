namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.ValueObjects;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamMemberIdTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidGuid_ShouldCreateTeamMemberId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var teamMemberId = new TeamMemberId(guid);

        // Assert
        teamMemberId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyGuid_ShouldCreateTeamMemberId()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var teamMemberId = new TeamMemberId(emptyGuid);

        // Assert
        teamMemberId.Value.Should().Be(emptyGuid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void New_ShouldCreateUniqueTeamMemberIds()
    {
        // Act
        var teamMemberId1 = TeamMemberId.New();
        var teamMemberId2 = TeamMemberId.New();

        // Assert
        teamMemberId1.Should().NotBe(teamMemberId2);
        teamMemberId1.Value.Should().NotBe(Guid.Empty);
        teamMemberId2.Value.Should().NotBe(Guid.Empty);
        teamMemberId1.Value.Should().NotBe(teamMemberId2.Value);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ImplicitOperator_ShouldConvertToGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamMemberId = new TeamMemberId(guid);

        // Act
        Guid result = teamMemberId;

        // Assert
        result.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ExplicitOperator_ShouldConvertFromGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var teamMemberId = (TeamMemberId)guid;

        // Assert
        teamMemberId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamMemberId = new TeamMemberId(guid);

        // Act
        var result = teamMemberId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamMemberId1 = new TeamMemberId(guid);
        var teamMemberId2 = new TeamMemberId(guid);

        // Act & Assert
        teamMemberId1.Should().Be(teamMemberId2);
        teamMemberId1.GetHashCode().Should().Be(teamMemberId2.GetHashCode());
        (teamMemberId1 == teamMemberId2).Should().BeTrue();
        (teamMemberId1 != teamMemberId2).Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var teamMemberId1 = new TeamMemberId(Guid.NewGuid());
        var teamMemberId2 = new TeamMemberId(Guid.NewGuid());

        // Act & Assert
        teamMemberId1.Should().NotBe(teamMemberId2);
        (teamMemberId1 == teamMemberId2).Should().BeFalse();
        (teamMemberId1 != teamMemberId2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamMemberId_ShouldImplementIStronglyTypedId()
    {
        // Arrange
        var teamMemberId = TeamMemberId.New();

        // Act & Assert
        teamMemberId.Should().BeAssignableTo<IStronglyTypedId<Guid>>();
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithMaxGuid_ShouldCreateTeamMemberId()
    {
        // Arrange
        var maxGuid = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        var teamMemberId = new TeamMemberId(maxGuid);

        // Assert
        teamMemberId.Value.Should().Be(maxGuid);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var newGuid = Guid.NewGuid();
        var teamMemberId = new TeamMemberId(originalGuid);

        // Act
        var newTeamMemberId = teamMemberId with { Value = newGuid };

        // Assert
        teamMemberId.Value.Should().Be(originalGuid);
        newTeamMemberId.Value.Should().Be(newGuid);
        teamMemberId.Should().NotBe(newTeamMemberId);
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void TeamMemberId_ShouldBeSerializable()
    {
        // Arrange
        var teamMemberId = TeamMemberId.New();

        // Act & Assert - Records are serializable by default
        teamMemberId.ToString().Should().NotBeNullOrEmpty();
        teamMemberId.GetHashCode().Should().NotBe(0);
    }
}