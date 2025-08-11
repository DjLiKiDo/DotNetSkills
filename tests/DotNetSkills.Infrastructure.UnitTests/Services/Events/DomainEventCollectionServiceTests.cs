using DotNetSkills.Domain.Common.Entities;
using DotNetSkills.Domain.UserManagement.Entities;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using DotNetSkills.Infrastructure.Services.Events;
using Microsoft.Extensions.Logging;
using Moq;

namespace DotNetSkills.Infrastructure.UnitTests.Services.Events;

/// <summary>
/// Unit tests for DomainEventCollectionService.
/// Tests the tracking of modified aggregate roots and domain event collection.
/// </summary>
public class DomainEventCollectionServiceTests
{
    private readonly Mock<ILogger<DomainEventCollectionService>> _loggerMock;
    private readonly DomainEventCollectionService _service;

    public DomainEventCollectionServiceTests()
    {
        _loggerMock = new Mock<ILogger<DomainEventCollectionService>>();
        _service = new DomainEventCollectionService(_loggerMock.Object);
    }

    [Fact]
    public void RegisterModifiedAggregate_WithValidAggregate_ShouldTrackAggregate()
    {
        // Arrange
        var user = User.Create("John Doe", new EmailAddress("john@example.com"), UserRole.Developer);

        // Act
        _service.RegisterModifiedAggregate(user);

        // Assert
        var trackedAggregates = _service.GetModifiedAggregates();
        trackedAggregates.Should().HaveCount(1);
        trackedAggregates.First().GetId().Should().Be(user.Id.Value);
    }

    [Fact]
    public void RegisterModifiedAggregate_WithNullAggregate_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => _service.RegisterModifiedAggregate<UserId>(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("aggregateRoot");
    }

    [Fact]
    public void GetModifiedAggregates_WhenEmpty_ShouldReturnEmptyCollection()
    {
        // Act
        var result = _service.GetModifiedAggregates();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void ClearTrackedAggregates_WithTrackedAggregates_ShouldClearAll()
    {
        // Arrange
        var user = User.Create("John Doe", new EmailAddress("john@example.com"), UserRole.Developer);
        _service.RegisterModifiedAggregate(user);

        // Act
        _service.ClearTrackedAggregates();

        // Assert
        var trackedAggregates = _service.GetModifiedAggregates();
        trackedAggregates.Should().BeEmpty();
    }

    [Fact]
    public void GetDomainEvents_WithNoTrackedAggregates_ShouldReturnEmptyCollection()
    {
        // Act
        var domainEvents = _service.GetDomainEvents();

        // Assert
        domainEvents.Should().NotBeNull();
        domainEvents.Should().BeEmpty();
    }
}
