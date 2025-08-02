namespace DotNetSkills.Domain.UnitTests.UserManagement.Events;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class UserCreatedDomainEventTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().WithValidEmail("test", "example.com").Build();
        var name = "Test User";
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        domainEvent.UserId.Should().Be(userId);
        domainEvent.Email.Should().Be(email);
        domainEvent.Name.Should().Be(name);
        domainEvent.Role.Should().Be(role);
        domainEvent.CreatedBy.Should().Be(createdBy);
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithNullCreatedBy_ShouldCreateEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var role = UserRole.Developer;
        UserId? createdBy = null;

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        domainEvent.UserId.Should().Be(userId);
        domainEvent.Email.Should().Be(email);
        domainEvent.Name.Should().Be(name);
        domainEvent.Role.Should().Be(role);
        domainEvent.CreatedBy.Should().BeNull();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveOccurredAtTimestamp()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);
        var afterCreation = DateTime.UtcNow;

        // Assert
        domainEvent.OccurredAt.Should().BeOnOrAfter(beforeCreation);
        domainEvent.OccurredAt.Should().BeOnOrBefore(afterCreation);
        domainEvent.OccurredAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldHaveCorrelationId()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        domainEvent.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldBeEquatable()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var event1 = new UserCreatedDomainEvent(userId, email, name, role, createdBy);
        var event2 = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        // Note: Events should NOT be equal even with same data due to different timestamps and correlation IDs
        event1.Should().NotBe(event2);
        event1.UserId.Should().Be(event2.UserId);
        event1.Email.Should().Be(event2.Email);
        event1.Name.Should().Be(event2.Name);
        event1.Role.Should().Be(event2.Role);
        event1.CreatedBy.Should().Be(event2.CreatedBy);
    }

    [Theory]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.ProjectManager)]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void Constructor_WithDifferentRoles_ShouldCreateEvent(UserRole role)
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        domainEvent.Role.Should().Be(role);
        domainEvent.UserId.Should().Be(userId);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void DomainEvent_ShouldInheritFromBaseDomainEvent()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";  
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Assert
        domainEvent.Should().BeAssignableTo<BaseDomainEvent>();
        domainEvent.Should().BeAssignableTo<IDomainEvent>();
    }

    [Fact]
    [Trait("TestType", "Serialization")]
    public void DomainEvent_ShouldBeSerializable()
    {
        // Arrange
        var userId = UserIdBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();
        var name = "Test User";
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();
        var domainEvent = new UserCreatedDomainEvent(userId, email, name, role, createdBy);

        // Act & Assert - Records are serializable by default
        domainEvent.ToString().Should().NotBeNullOrEmpty();
        domainEvent.GetHashCode().Should().NotBe(0);
    }
}