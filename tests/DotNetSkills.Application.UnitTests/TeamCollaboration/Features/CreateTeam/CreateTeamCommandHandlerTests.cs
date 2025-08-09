using AutoMapper;
using DotNetSkills.Application.Common.Abstractions;
using DotNetSkills.Application.TeamCollaboration.Contracts;
using DotNetSkills.Application.TeamCollaboration.Contracts.Responses;
using DotNetSkills.Application.TeamCollaboration.Features.CreateTeam;
using DotNetSkills.Application.UserManagement.Contracts;
using DotNetSkills.Domain.TeamCollaboration.Entities;
using DotNetSkills.Domain.UserManagement.Entities;
using DotNetSkills.Domain.UserManagement.Enums;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using FluentAssertions;
using Moq;

namespace DotNetSkills.Application.UnitTests.TeamCollaboration.Features.CreateTeam;

[Trait("Category", "Unit")]
public class CreateTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<ICurrentUserService> _currentUserService;
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IMapper> _mapper;
    private readonly CreateTeamCommandHandler _handler;
    private readonly User _projectManager;
    private readonly UserId _currentUserId;

    public CreateTeamCommandHandlerTests()
    {
        _teamRepository = new Mock<ITeamRepository>();
        _userRepository = new Mock<IUserRepository>();
        _currentUserService = new Mock<ICurrentUserService>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        
        _handler = new CreateTeamCommandHandler(
            _teamRepository.Object,
            _userRepository.Object,
            _currentUserService.Object,
            _unitOfWork.Object,
            _mapper.Object);

        _currentUserId = new UserId(Guid.NewGuid());
        _projectManager = User.Create("Project Manager", new EmailAddress("pm@test.com"), UserRole.ProjectManager);
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldCreateTeamSuccessfully()
    {
        // Arrange
        var command = new CreateTeamCommand("Test Team", "Test Description", _currentUserId);
        var expectedResponse = new TeamResponse(
            Guid.NewGuid(),
            "Test Team",
            "Test Description",
            0,
            DateTime.UtcNow,
            DateTime.UtcNow,
            Array.Empty<TeamMemberResponse>());

        _currentUserService.Setup(x => x.GetCurrentUserId()).Returns(_currentUserId);
        _userRepository.Setup(x => x.GetByIdAsync(_currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_projectManager);
        _teamRepository.Setup(x => x.GetByNameAsync("Test Team", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Team?)null);
        _mapper.Setup(x => x.Map<TeamResponse>(It.IsAny<Team>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Team");
        result.Description.Should().Be("Test Description");

        _teamRepository.Verify(x => x.Add(It.IsAny<Team>()), Times.Once);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TeamWithSameNameExists_ShouldThrowException()
    {
        // Arrange
        var command = new CreateTeamCommand("Existing Team", "Test Description", _currentUserId);
        var existingTeam = Team.Create("Existing Team", "Description", _projectManager);

        _currentUserService.Setup(x => x.GetCurrentUserId()).Returns(_currentUserId);
        _userRepository.Setup(x => x.GetByIdAsync(_currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_projectManager);
        _teamRepository.Setup(x => x.GetByNameAsync("Existing Team", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingTeam);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Team with name 'Existing Team' already exists");
    }

    [Fact]
    public async Task Handle_UserNotAuthenticated_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new CreateTeamCommand("Test Team", "Test Description", _currentUserId);

        _currentUserService.Setup(x => x.GetCurrentUserId()).Returns((UserId?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User must be authenticated to create teams");
    }

    [Fact]
    public async Task Handle_CurrentUserNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var command = new CreateTeamCommand("Test Team", "Test Description", _currentUserId);

        _currentUserService.Setup(x => x.GetCurrentUserId()).Returns(_currentUserId);
        _userRepository.Setup(x => x.GetByIdAsync(_currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Current user not found");
    }
}