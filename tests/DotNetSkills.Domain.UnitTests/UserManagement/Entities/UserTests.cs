namespace DotNetSkills.Domain.UnitTests.UserManagement.Entities;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class UserTests : TestBase
{
    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldCreateUser()
    {
        // Arrange
        var name = "John Doe";
        var email = EmailAddressBuilder.Create().WithValidEmail("john", "example.com").Build();
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create();

        // Act
        var user = new User(name, email, role, createdBy);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Role.Should().Be(role);
        user.Status.Should().Be(UserStatus.Active);
        user.Id.Should().NotBe(UserId.New());
        user.TeamMemberships.Should().BeEmpty();
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidParameters_ShouldRaiseUserCreatedDomainEvent()
    {
        // Arrange
        var name = "John Doe";
        var email = EmailAddressBuilder.Create().WithValidEmail("john", "example.com").Build();
        var role = UserRole.Developer;
        var createdBy = UserIdBuilder.Create().Build();

        // Act
        var user = new User(name, email, role, createdBy);

        // Assert
        var domainEvent = DomainEventHelper.AssertEventRaised<UserCreatedDomainEvent, UserId>(user);
        domainEvent.UserId.Should().Be(user.Id);
        domainEvent.Email.Should().Be(email);
        domainEvent.Name.Should().Be(name);
        domainEvent.Role.Should().Be(role);
        domainEvent.CreatedBy.Should().Be(createdBy);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestType", "Validation")]
    public void Constructor_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var email = EmailAddressBuilder.Create().Build();
        var role = UserRole.Developer;

        // Act & Assert
        AssertArgumentException(() => new User(invalidName, email, role), "name");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var name = "John Doe";
        EmailAddress email = null!;
        var role = UserRole.Developer;

        // Act & Assert
        AssertArgumentNullException(() => new User(name, email, role), "email");
    }

    [Fact]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldTrimName()
    {
        // Arrange
        var nameWithWhitespace = "  John Doe  ";
        var email = EmailAddressBuilder.Create().Build();
        var role = UserRole.Developer;

        // Act
        var user = new User(nameWithWhitespace, email, role);

        // Assert
        user.Name.Should().Be("John Doe");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithAdminCreator_ShouldCreateUser()
    {
        // Arrange
        var adminUser = UserBuilder.Create().AsAdmin().Build();
        var name = "John Doe";
        var email = EmailAddressBuilder.Create().WithValidEmail("john", "example.com").Build();
        var role = UserRole.Developer;

        // Act
        var user = User.Create(name, email, role, adminUser);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Role.Should().Be(role);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Theory]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.ProjectManager)]
    [InlineData(UserRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithNonAdminCreator_ShouldThrowDomainException(UserRole nonAdminRole)
    {
        // Arrange
        var nonAdminUser = UserBuilder.Create().WithRole(nonAdminRole).Build();
        var name = "John Doe";
        var email = EmailAddressBuilder.Create().Build();
        var role = UserRole.Developer;

        // Act & Assert
        AssertDomainException(
            () => User.Create(name, email, role, nonAdminUser),
            ValidationMessages.User.OnlyAdminCanCreate);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Create_WithoutCreator_ShouldCreateUser()
    {
        // Arrange
        var name = "John Doe";
        var email = EmailAddressBuilder.Create().Build();
        var role = UserRole.Developer;

        // Act
        var user = User.Create(name, email, role, null);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.Role.Should().Be(role);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void UpdateProfile_WithValidParameters_ShouldUpdateUserProfile()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var newName = "Jane Doe";
        var newEmail = EmailAddressBuilder.Create().WithValidEmail("jane", "example.com").Build();

        // Act
        user.UpdateProfile(newName, newEmail);

        // Assert
        user.Name.Should().Be(newName);
        user.Email.Should().Be(newEmail);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("TestType", "Validation")]
    public void UpdateProfile_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var email = EmailAddressBuilder.Create().Build();

        // Act & Assert
        AssertArgumentException(() => user.UpdateProfile(invalidName, email), "name");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void UpdateProfile_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var name = "John Doe";
        EmailAddress email = null!;

        // Act & Assert
        AssertArgumentNullException(() => user.UpdateProfile(name, email), "email");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WithAdminChanger_ShouldChangeRole()
    {
        // Arrange
        var user = UserBuilder.Create().AsDeveloper().Build();
        var adminUser = UserBuilder.Create().AsAdmin().Build();
        var newRole = UserRole.ProjectManager;

        // Act
        user.ChangeRole(newRole, adminUser);

        // Assert
        user.Role.Should().Be(newRole);
    }

    [Theory]
    [InlineData(UserRole.Developer)]
    [InlineData(UserRole.ProjectManager)]
    [InlineData(UserRole.Viewer)]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WithNonAdminChanger_ShouldThrowDomainException(UserRole nonAdminRole)
    {
        // Arrange
        var user = UserBuilder.Create().AsDeveloper().Build();
        var nonAdminUser = UserBuilder.Create().WithRole(nonAdminRole).Build();
        var newRole = UserRole.ProjectManager;

        // Act & Assert
        AssertDomainException(
            () => user.ChangeRole(newRole, nonAdminUser),
            ValidationMessages.User.OnlyAdminCanChangeRole);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ChangeRole_WhenTryingToChangeSelfRole_ShouldThrowDomainException()
    {
        // Arrange
        var adminUser = UserBuilder.Create().AsAdmin().Build();
        var newRole = UserRole.Developer;

        // Act & Assert
        // The business rule check happens first, so admin can't change admin role
        AssertDomainException(
            () => adminUser.ChangeRole(newRole, adminUser),
            ValidationMessages.User.OnlyAdminCanChangeRole);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void ChangeRole_WithNullChanger_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var newRole = UserRole.ProjectManager;
        User changer = null!;

        // Act & Assert
        AssertArgumentNullException(() => user.ChangeRole(newRole, changer), "changedBy");
    }

    [Fact]
    [Trait("TestType", "StateTransition")]
    public void Activate_ShouldSetStatusToActive()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        user.Deactivate(); // First deactivate

        // Act
        user.Activate();

        // Assert
        user.Status.Should().Be(UserStatus.Active);
        user.IsActive().Should().BeTrue();
    }

    [Fact]
    [Trait("TestType", "StateTransition")]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        // Arrange
        var user = UserBuilder.Create().Build();

        // Act
        user.Deactivate();

        // Assert
        user.Status.Should().Be(UserStatus.Inactive);
        user.IsActive().Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "StateTransition")]
    public void Suspend_ShouldSetStatusToSuspended()
    {
        // Arrange
        var user = UserBuilder.Create().Build();

        // Act
        user.Suspend();

        // Assert
        user.Status.Should().Be(UserStatus.Suspended);
        user.IsActive().Should().BeFalse();
    }

    [Theory]
    [InlineData(UserStatus.Active, true)]
    [InlineData(UserStatus.Inactive, false)]
    [InlineData(UserStatus.Suspended, false)]
    [Trait("TestType", "BusinessLogic")]
    public void IsActive_ShouldReturnCorrectValue(UserStatus status, bool expectedResult)
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        switch (status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Inactive:
                user.Deactivate();
                break;
            case UserStatus.Suspended:
                user.Suspend();
                break;
        }

        // Act
        var result = user.IsActive();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(UserRole.Developer, UserStatus.Active, true)]
    [InlineData(UserRole.ProjectManager, UserStatus.Active, true)]
    [InlineData(UserRole.Admin, UserStatus.Active, true)]
    [InlineData(UserRole.Viewer, UserStatus.Active, false)]
    [InlineData(UserRole.Developer, UserStatus.Inactive, false)]
    [InlineData(UserRole.Developer, UserStatus.Suspended, false)]
    [Trait("TestType", "BusinessLogic")]
    public void CanBeAssignedTasks_ShouldReturnCorrectValue(UserRole role, UserStatus status, bool expectedResult)
    {
        // Arrange
        var user = UserBuilder.Create().WithRole(role).Build();
        switch (status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Inactive:
                user.Deactivate();
                break;
            case UserStatus.Suspended:
                user.Suspend();
                break;
        }

        // Act
        var result = user.CanBeAssignedTasks();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(UserRole.ProjectManager, UserStatus.Active, true)]
    [InlineData(UserRole.Admin, UserStatus.Active, true)]
    [InlineData(UserRole.Developer, UserStatus.Active, false)]
    [InlineData(UserRole.Viewer, UserStatus.Active, false)]
    [InlineData(UserRole.ProjectManager, UserStatus.Inactive, false)]
    [Trait("TestType", "BusinessLogic")]
    public void CanManageProjects_ShouldReturnCorrectValue(UserRole role, UserStatus status, bool expectedResult)
    {
        // Arrange
        var user = UserBuilder.Create().WithRole(role).Build();
        switch (status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Inactive:
                user.Deactivate();
                break;
            case UserStatus.Suspended:
                user.Suspend();
                break;
        }

        // Act
        var result = user.CanManageProjects();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(UserRole.Admin, UserStatus.Active, true)]
    [InlineData(UserRole.ProjectManager, UserStatus.Active, false)]
    [InlineData(UserRole.Developer, UserStatus.Active, false)]
    [InlineData(UserRole.Viewer, UserStatus.Active, false)]
    [InlineData(UserRole.Admin, UserStatus.Inactive, false)]
    [Trait("TestType", "BusinessLogic")]
    public void CanManageTeams_ShouldReturnCorrectValue(UserRole role, UserStatus status, bool expectedResult)
    {
        // Arrange
        var user = UserBuilder.Create().WithRole(role).Build();
        switch (status)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Inactive:
                user.Deactivate();
                break;
            case UserStatus.Suspended:
                user.Suspend();
                break;
        }

        // Act
        var result = user.CanManageTeams();

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void IsMemberOfTeam_WhenNotMember_ShouldReturnFalse()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create();

        // Act
        var result = user.IsMemberOfTeam(teamId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void GetRoleInTeam_WhenNotMember_ShouldReturnNull()
    {
        // Arrange
        var user = UserBuilder.Create().Build();
        var teamId = TeamIdBuilder.Create();

        // Act
        var result = user.GetRoleInTeam(teamId);

        // Assert
        result.Should().BeNull();
    }
}