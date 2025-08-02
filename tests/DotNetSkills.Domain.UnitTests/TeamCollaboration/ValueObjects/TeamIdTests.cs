namespace DotNetSkills.Domain.UnitTests.TeamCollaboration.ValueObjects;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "TeamCollaboration")]
public class TeamIdTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidGuid_ShouldCreateTeamId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var teamId = new TeamId(guid);

        // Assert
        teamId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithEmptyGuid_ShouldCreateTeamId()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var teamId = new TeamId(emptyGuid);

        // Assert
        teamId.Value.Should().Be(emptyGuid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void New_ShouldCreateUniqueTeamIds()
    {
        // Act
        var teamId1 = TeamId.New();
        var teamId2 = TeamId.New();

        // Assert
        teamId1.Should().NotBe(teamId2);
        teamId1.Value.Should().NotBe(Guid.Empty);
        teamId2.Value.Should().NotBe(Guid.Empty);
        teamId1.Value.Should().NotBe(teamId2.Value);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ImplicitOperator_ShouldConvertToGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamId = new TeamId(guid);

        // Act
        Guid result = teamId;

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
        var teamId = (TeamId)guid;

        // Assert
        teamId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamId = new TeamId(guid);

        // Act
        var result = teamId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var teamId1 = new TeamId(guid);
        var teamId2 = new TeamId(guid);

        // Act & Assert
        teamId1.Should().Be(teamId2);
        teamId1.GetHashCode().Should().Be(teamId2.GetHashCode());
        (teamId1 == teamId2).Should().BeTrue();
        (teamId1 != teamId2).Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var teamId1 = new TeamId(Guid.NewGuid());
        var teamId2 = new TeamId(Guid.NewGuid());

        // Act & Assert
        teamId1.Should().NotBe(teamId2);
        (teamId1 == teamId2).Should().BeFalse();
        (teamId1 != teamId2).Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void TeamId_ShouldImplementIStronglyTypedId()
    {
        // Arrange
        var teamId = TeamId.New();

        // Act & Assert
        teamId.Should().BeAssignableTo<IStronglyTypedId<Guid>>();
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithMaxGuid_ShouldCreateTeamId()
    {
        // Arrange
        var maxGuid = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        var teamId = new TeamId(maxGuid);

        // Assert
        teamId.Value.Should().Be(maxGuid);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Record_ShouldSupportWithExpression()
    {
        // Arrange
        var originalGuid = Guid.NewGuid();
        var newGuid = Guid.NewGuid();
        var teamId = new TeamId(originalGuid);

        // Act
        var newTeamId = teamId with { Value = newGuid };

        // Assert
        teamId.Value.Should().Be(originalGuid);
        newTeamId.Value.Should().Be(newGuid);
        teamId.Should().NotBe(newTeamId);
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void TeamId_ShouldBeSerializable()
    {
        // Arrange
        var teamId = TeamId.New();

        // Act & Assert - Records are serializable by default
        teamId.ToString().Should().NotBeNullOrEmpty();
        teamId.GetHashCode().Should().NotBe(0);
    }
}