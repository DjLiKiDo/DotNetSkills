namespace DotNetSkills.Domain.UserManagement.Entities;

/// <summary>
/// Represents a user in the DotNetSkills system.
/// This is an aggregate root that manages user identity, roles, and team memberships.
/// </summary>
public class User : AggregateRoot<UserId>
{
    private readonly List<TeamMember> _teamMemberships = [];

    /// <summary>
    /// Gets the user's full name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    public EmailAddress Email { get; private set; }

    /// <summary>
    /// Gets the user's role in the system.
    /// </summary>
    public UserRole Role { get; private set; }

    /// <summary>
    /// Gets the user's current status.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Gets the read-only collection of team memberships for this user.
    /// </summary>
    public IReadOnlyList<TeamMember> TeamMemberships => _teamMemberships.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private User() : base(UserId.New())
    {
        Name = string.Empty;
        Email = null!;
    }

    /// <summary>
    /// Initializes a new instance of the User class.
    /// </summary>
    /// <param name="name">The user's full name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="role">The user's role in the system.</param>
    /// <param name="createdBy">The ID of the user creating this user (must be an admin).</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public User(string name, EmailAddress email, UserRole role, UserId? createdBy = null) : base(UserId.New())
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));
        Ensure.NotNull(email, nameof(email));

        Name = name.Trim();
        Email = email;
        Role = role;
        Status = UserStatus.Active;

        // Raise domain event
        RaiseDomainEvent(new UserCreatedDomainEvent(Id, Email, Name, Role, createdBy));
    }

    /// <summary>
    /// Creates a new user with the specified details.
    /// Only admin users can create new users.
    /// </summary>
    /// <param name="name">The user's full name.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="role">The user's role in the system.</param>
    /// <param name="createdByUser">The user creating this user (must be an admin).</param>
    /// <returns>A new User instance.</returns>
    /// <exception cref="DomainException">Thrown when the creator is not an admin.</exception>
    /// <remarks>
    /// This method uses BusinessRules for static authorization validation.
    /// For complex validations like email uniqueness, use IUserDomainService in the Application layer.
    /// </remarks>
    public static User Create(string name, EmailAddress email, UserRole role, User? createdByUser = null)
    {
        // Use BusinessRules for static authorization validation (fast, no dependencies)
        Ensure.BusinessRule(
            BusinessRules.Authorization.CanCreateUser(createdByUser?.Role),
            ValidationMessages.User.OnlyAdminCanCreate);

        return new User(name, email, role, createdByUser?.Id);
    }

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    /// <param name="name">The new name for the user.</param>
    /// <param name="email">The new email address for the user.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    public void UpdateProfile(string name, EmailAddress email)
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));
        Ensure.NotNull(email, nameof(email));

        Name = name.Trim();
        Email = email;
    }

    /// <summary>
    /// Changes the user's role in the system.
    /// </summary>
    /// <param name="newRole">The new role to assign to the user.</param>
    /// <param name="changedBy">The user making the role change (must be an admin).</param>
    /// <exception cref="DomainException">Thrown when the changer is not an admin or when trying to change own role.</exception>
    public void ChangeRole(UserRole newRole, User changedBy)
    {
        Ensure.NotNull(changedBy, nameof(changedBy));

        // Use BusinessRules for static authorization validation
        Ensure.BusinessRule(
            BusinessRules.Authorization.CanModifyUserRole(changedBy.Role, Role),
            ValidationMessages.User.OnlyAdminCanChangeRole);
        Ensure.BusinessRule(
            changedBy.Id != Id,
            ValidationMessages.User.CannotChangeSelfRole);

        Role = newRole;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
    }

    /// <summary>
    /// Deactivates the user account.
    /// </summary>
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    /// <summary>
    /// Suspends the user account.
    /// </summary>
    public void Suspend()
    {
        Status = UserStatus.Suspended;
    }

    /// <summary>
    /// Checks if the user is currently active.
    /// </summary>
    /// <returns>True if the user is active, false otherwise.</returns>
    public bool IsActive() => Status == UserStatus.Active;

    /// <summary>
    /// Adds a team membership for this user.
    /// This method should only be called from the Team aggregate.
    /// </summary>
    /// <param name="teamMember">The team membership to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when teamMember is null.</exception>
    /// <exception cref="DomainException">Thrown when the user is already a member of the team.</exception>
    internal void AddTeamMembership(TeamMember teamMember)
    {
        ArgumentNullException.ThrowIfNull(teamMember);

        if (_teamMemberships.Any(tm => tm.TeamId == teamMember.TeamId))
            throw new DomainException("User is already a member of this team");

        _teamMemberships.Add(teamMember);
    }

    /// <summary>
    /// Removes a team membership for this user.
    /// This method should only be called from the Team aggregate.
    /// </summary>
    /// <param name="teamId">The ID of the team to remove membership from.</param>
    /// <exception cref="DomainException">Thrown when the user is not a member of the team.</exception>
    internal void RemoveTeamMembership(TeamId teamId)
    {
        var membership = _teamMemberships.FirstOrDefault(tm => tm.TeamId == teamId);
        if (membership == null)
            throw new DomainException("User is not a member of this team");

        _teamMemberships.Remove(membership);
    }

    /// <summary>
    /// Checks if the user is a member of the specified team.
    /// </summary>
    /// <param name="teamId">The ID of the team to check.</param>
    /// <returns>True if the user is a member of the team, false otherwise.</returns>
    public bool IsMemberOfTeam(TeamId teamId)
    {
        return _teamMemberships.Any(tm => tm.TeamId == teamId);
    }

    /// <summary>
    /// Gets the user's role in the specified team.
    /// </summary>
    /// <param name="teamId">The ID of the team.</param>
    /// <returns>The user's role in the team, or null if not a member.</returns>
    public TeamRole? GetRoleInTeam(TeamId teamId)
    {
        return _teamMemberships.FirstOrDefault(tm => tm.TeamId == teamId)?.Role;
    }

    /// <summary>
    /// Checks if the user can be assigned tasks (must be active and have appropriate role).
    /// </summary>
    /// <returns>True if the user can be assigned tasks, false otherwise.</returns>
    public bool CanBeAssignedTasks()
    {
        return IsActive() && (Role == UserRole.Developer || Role == UserRole.ProjectManager || Role == UserRole.Admin);
    }

    /// <summary>
    /// Checks if the user can manage projects (must be active and have appropriate role).
    /// </summary>
    /// <returns>True if the user can manage projects, false otherwise.</returns>
    public bool CanManageProjects()
    {
        return IsActive() && (Role == UserRole.ProjectManager || Role == UserRole.Admin);
    }

    /// <summary>
    /// Checks if the user can manage teams (must be active admin or project manager).
    /// </summary>
    /// <returns>True if the user can manage teams, false otherwise.</returns>
    public bool CanManageTeams()
    {
        return IsActive() && (Role == UserRole.Admin || Role == UserRole.ProjectManager);
    }
}
