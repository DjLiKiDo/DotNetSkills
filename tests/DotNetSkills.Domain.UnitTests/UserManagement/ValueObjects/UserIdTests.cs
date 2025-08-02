namespace DotNetSkills.Domain.UnitTests.UserManagement.ValueObjects;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class UserIdTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidGuid_ShouldCreateUserId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var userId = new UserId(guid);

        // Assert
        userId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void New_ShouldCreateUniqueUserIds()
    {
        // Act
        var userId1 = UserId.New();
        var userId2 = UserId.New();

        // Assert
        userId1.Should().NotBe(userId2);
        userId1.Value.Should().NotBe(Guid.Empty);
        userId2.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void ImplicitOperator_ShouldConvertToGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);

        // Act
        Guid result = userId;

        // Assert
        result.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void ExplicitOperator_ShouldConvertFromGuid()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var userId = (UserId)guid;

        // Assert
        userId.Value.Should().Be(guid);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId = new UserId(guid);
        var expectedString = guid.ToString();

        // Act
        var result = userId.ToString();

        // Assert
        result.Should().Be(expectedString);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var userId1 = new UserId(guid);
        var userId2 = new UserId(guid);

        // Act & Assert
        userId1.Should().Be(userId2);
        userId1.GetHashCode().Should().Be(userId2.GetHashCode());
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var userId1 = UserId.New();
        var userId2 = UserId.New();

        // Act & Assert
        userId1.Should().NotBe(userId2);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithEmptyGuid_ShouldCreateUserId()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var userId = new UserId(emptyGuid);

        // Assert
        userId.Value.Should().Be(emptyGuid);
    }
}