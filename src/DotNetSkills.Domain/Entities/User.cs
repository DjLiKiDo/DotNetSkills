using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.Events;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Entities;

public class User : BaseEntity<UserId>
{
    private readonly List<TeamMember> _teamMemberships = new();
    private readonly List<Task> _assignedTasks = new();

    private User() : base() { }

    public User(UserId id, string firstName, string lastName, EmailAddress email, UserRole role, string passwordHash)
        : base(id)
    {
        FirstName = ValidateAndSetFirstName(firstName);
        LastName = ValidateAndSetLastName(lastName);
        Email = email;
        Role = role;
        PasswordHash = ValidateAndSetPasswordHash(passwordHash);
        IsActive = true;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public EmailAddress Email { get; private set; }
    public UserRole Role { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    public IReadOnlyList<TeamMember> TeamMemberships => _teamMemberships.AsReadOnly();
    public IReadOnlyList<Task> AssignedTasks => _assignedTasks.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    public static User Create(UserId id, string firstName, string lastName, EmailAddress email, UserRole role, string passwordHash, UserId createdBy)
    {
        var user = new User(id, firstName, lastName, email, role, passwordHash);
        
        user.RaiseDomainEvent(new UserCreatedDomainEvent(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            user.CreatedAt,
            createdBy
        ));

        return user;
    }

    public void UpdateProfile(string firstName, string lastName, EmailAddress email)
    {
        FirstName = ValidateAndSetFirstName(firstName);
        LastName = ValidateAndSetLastName(lastName);
        Email = email;
        UpdateTimestamp();
    }

    public void UpdateRole(UserRole newRole, UserRole requestedByRole, UserId updatedBy)
    {
        if (!requestedByRole.CanManageUsers())
            throw new DomainException("Only administrators can update user roles.");

        if (Role == newRole)
            return;

        var oldRole = Role;
        Role = newRole;
        var updatedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new UserRoleUpdatedDomainEvent(
            Id,
            FirstName,
            LastName,
            Email,
            oldRole,
            newRole,
            updatedBy,
            updatedAt
        ));
    }

    public void UpdatePassword(string newPasswordHash, UserId changedBy)
    {
        PasswordHash = ValidateAndSetPasswordHash(newPasswordHash);
        var changedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new UserPasswordChangedDomainEvent(
            Id,
            FirstName,
            LastName,
            Email,
            changedBy,
            changedAt
        ));
    }

    public void Deactivate(UserRole requestedByRole, UserId deactivatedBy)
    {
        if (!requestedByRole.CanManageUsers())
            throw new DomainException("Only administrators can deactivate users.");

        if (!IsActive)
            return;

        IsActive = false;
        var deactivatedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new UserDeactivatedDomainEvent(
            Id,
            FirstName,
            LastName,
            Email,
            Role,
            deactivatedBy,
            deactivatedAt
        ));
    }

    public void Activate(UserRole requestedByRole, UserId activatedBy)
    {
        if (!requestedByRole.CanManageUsers())
            throw new DomainException("Only administrators can activate users.");

        if (IsActive)
            return;

        IsActive = true;
        var activatedAt = DateTime.UtcNow;
        UpdateTimestamp();

        RaiseDomainEvent(new UserActivatedDomainEvent(
            Id,
            FirstName,
            LastName,
            Email,
            Role,
            activatedBy,
            activatedAt
        ));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool CanManageUser(User otherUser)
    {
        if (!IsActive)
            return false;

        return Role.CanManageUsers() && Role.HasHigherOrEqualAuthorityThan(otherUser.Role);
    }

    public bool CanAccessTeam(TeamId teamId)
    {
        if (!IsActive)
            return false;

        if (Role.CanManageTeams())
            return true;

        return _teamMemberships.Any(tm => tm.TeamId == teamId);
    }

    public bool IsInTeam(TeamId teamId)
    {
        return _teamMemberships.Any(tm => tm.TeamId == teamId);
    }

    internal void AddTeamMembership(TeamMember teamMember)
    {
        if (_teamMemberships.Any(tm => tm.TeamId == teamMember.TeamId))
            throw new DomainException($"User is already a member of team {teamMember.TeamId}.");

        _teamMemberships.Add(teamMember);
    }

    internal void RemoveTeamMembership(TeamId teamId)
    {
        var membership = _teamMemberships.FirstOrDefault(tm => tm.TeamId == teamId);
        if (membership == null)
            throw new DomainException($"User is not a member of team {teamId}.");

        _teamMemberships.Remove(membership);
    }

    internal void AssignTask(Task task)
    {
        if (_assignedTasks.Any(t => t.Id == task.Id))
            return;

        _assignedTasks.Add(task);
    }

    internal void UnassignTask(TaskId taskId)
    {
        var task = _assignedTasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
            _assignedTasks.Remove(task);
    }

    private static string ValidateAndSetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");

        if (firstName.Length > DomainConstants.User.MaxFirstNameLength)
            throw new DomainException($"First name cannot exceed {DomainConstants.User.MaxFirstNameLength} characters.");

        return firstName.Trim();
    }

    private static string ValidateAndSetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        if (lastName.Length > DomainConstants.User.MaxLastNameLength)
            throw new DomainException($"Last name cannot exceed {DomainConstants.User.MaxLastNameLength} characters.");

        return lastName.Trim();
    }

    private static string ValidateAndSetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("Password hash is required.");

        return passwordHash;
    }
}