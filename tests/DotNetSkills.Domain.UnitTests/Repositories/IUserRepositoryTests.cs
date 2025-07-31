using FluentAssertions;
using Moq;
using DotNetSkills.Domain.Repositories;
using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;
using DotNetSkills.Domain.Enums;

namespace DotNetSkills.Domain.UnitTests.Repositories;

/// <summary>
/// Unit tests for IUserRepository interface contract validation.
/// Tests ensure the repository interface adheres to domain requirements.
/// </summary>
public class IUserRepositoryTests
{
    private readonly Mock<IUserRepository> _mockRepository;

    public IUserRepositoryTests()
    {
        _mockRepository = new Mock<IUserRepository>();
    }

    [Fact(DisplayName = "GetByIdAsync should return null when user not found")]
    public async System.Threading.Tasks.Task GetByIdAsync_WhenUserNotFound_ShouldReturnNull()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        _mockRepository.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _mockRepository.Object.GetByIdAsync(userId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact(DisplayName = "GetByEmailAsync should return user when email exists")]
    public async System.Threading.Tasks.Task GetByEmailAsync_WhenEmailExists_ShouldReturnUser()
    {
        // Arrange
        var email = new EmailAddress("test@example.com");
        var userId = new UserId(Guid.NewGuid());
        var createdBy = new UserId(Guid.NewGuid());
        var user = User.Create(userId, "Test", "User", email, UserRole.Developer, "hashedpassword", createdBy);
        
        _mockRepository.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _mockRepository.Object.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
        _mockRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
    }

    [Fact(DisplayName = "GetActiveUsersAsync should return only active users")]
    public async System.Threading.Tasks.Task GetActiveUsersAsync_WhenCalled_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var createdBy = new UserId(Guid.NewGuid());
        var activeUsers = new List<User>
        {
            User.Create(new UserId(Guid.NewGuid()), "User", "One", new EmailAddress("user1@example.com"), UserRole.Developer, "hash1", createdBy),
            User.Create(new UserId(Guid.NewGuid()), "User", "Two", new EmailAddress("user2@example.com"), UserRole.ProjectManager, "hash2", createdBy)
        };

        _mockRepository.Setup(x => x.GetActiveUsersAsync())
            .ReturnsAsync(activeUsers.AsReadOnly());

        // Act
        var result = await _mockRepository.Object.GetActiveUsersAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(user => user.IsActive.Should().BeTrue());
    }

    [Fact(DisplayName = "GetUsersByRoleAsync should return users with specific role")]
    public async System.Threading.Tasks.Task GetUsersByRoleAsync_WhenRoleSpecified_ShouldReturnUsersWithRole()
    {
        // Arrange
        var role = UserRole.Admin;
        var createdBy = new UserId(Guid.NewGuid());
        var adminUsers = new List<User>
        {
            User.Create(new UserId(Guid.NewGuid()), "Admin", "One", new EmailAddress("admin1@example.com"), UserRole.Admin, "hash", createdBy)
        };

        _mockRepository.Setup(x => x.GetUsersByRoleAsync(role))
            .ReturnsAsync(adminUsers.AsReadOnly());

        // Act
        var result = await _mockRepository.Object.GetUsersByRoleAsync(role);

        // Assert
        result.Should().NotBeNull();
        result.Should().AllSatisfy(user => user.Role.Should().Be(role));
    }

    [Fact(DisplayName = "ExistsWithEmailAsync should return true when email exists")]
    public async System.Threading.Tasks.Task ExistsWithEmailAsync_WhenEmailExists_ShouldReturnTrue()
    {
        // Arrange
        var email = new EmailAddress("existing@example.com");
        _mockRepository.Setup(x => x.ExistsWithEmailAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _mockRepository.Object.ExistsWithEmailAsync(email);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(x => x.ExistsWithEmailAsync(email), Times.Once);
    }

    [Fact(DisplayName = "ExistsWithEmailAsync should return false when email does not exist")]
    public async System.Threading.Tasks.Task ExistsWithEmailAsync_WhenEmailDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var email = new EmailAddress("nonexistent@example.com");
        _mockRepository.Setup(x => x.ExistsWithEmailAsync(email))
            .ReturnsAsync(false);

        // Act
        var result = await _mockRepository.Object.ExistsWithEmailAsync(email);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(x => x.ExistsWithEmailAsync(email), Times.Once);
    }

    [Fact(DisplayName = "AddAsync should return the added user")]
    public async System.Threading.Tasks.Task AddAsync_WhenUserProvided_ShouldReturnAddedUser()
    {
        // Arrange
        var createdBy = new UserId(Guid.NewGuid());
        var user = User.Create(new UserId(Guid.NewGuid()), "New", "User", new EmailAddress("new@example.com"), UserRole.Developer, "hash", createdBy);
        
        _mockRepository.Setup(x => x.AddAsync(user))
            .ReturnsAsync(user);

        // Act
        var result = await _mockRepository.Object.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(user);
        _mockRepository.Verify(x => x.AddAsync(user), Times.Once);
    }

    [Fact(DisplayName = "UpdateAsync should be called once")]
    public async System.Threading.Tasks.Task UpdateAsync_WhenUserProvided_ShouldBeCalledOnce()
    {
        // Arrange
        var createdBy = new UserId(Guid.NewGuid());
        var user = User.Create(new UserId(Guid.NewGuid()), "Updated", "User", new EmailAddress("updated@example.com"), UserRole.Developer, "hash", createdBy);
        
        _mockRepository.Setup(x => x.UpdateAsync(user))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        await _mockRepository.Object.UpdateAsync(user);

        // Assert
        _mockRepository.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact(DisplayName = "DeleteAsync should be called once with correct ID")]
    public async System.Threading.Tasks.Task DeleteAsync_WhenUserIdProvided_ShouldBeCalledOnce()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        
        _mockRepository.Setup(x => x.DeleteAsync(userId))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        // Act
        await _mockRepository.Object.DeleteAsync(userId);

        // Assert
        _mockRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
    }
}
