namespace DotNetSkills.Domain.TeamCollaboration.Entities;

/// <summary>
/// Represents a team in the DotNetSkills system.
/// This is an aggregate root that manages team members and team-related operations.
/// </summary>
public class Team : AggregateRoot<TeamId>
{
    /// <summary>
    /// Maximum number of members allowed in a team.
    /// </summary>
    public const int MaxMembers = 50;

    private readonly List<TeamMember> _members = [];

    /// <summary>
    /// Gets the team name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the team description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the current status of the team.
    /// </summary>
    public TeamStatus Status { get; private set; }

    /// <summary>
    /// Gets the read-only collection of team members.
    /// </summary>
    public IReadOnlyList<TeamMember> Members => _members.AsReadOnly();

    /// <summary>
    /// Gets the number of current team members.
    /// </summary>
    public int MemberCount => _members.Count;

    /// <summary>
    /// Private constructor for EF Core.
    /// </summary>
    private Team() : base(TeamId.New())
    {
        Name = string.Empty;
        Status = TeamStatus.Active; // Default status
    }

    /// <summary>
    /// Initializes a new instance of the Team class.
    /// </summary>
    /// <param name="name">The team name.</param>
    /// <param name="description">The team description (optional).</param>
    /// <param name="createdBy">The user creating the team.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when createdBy is null.</exception>
    /// <exception cref="DomainException">Thrown when the creator doesn't have permission to create teams.</exception>
    public Team(string name, string? description, User createdBy) : base(TeamId.New())
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));
        Ensure.NotNull(createdBy, nameof(createdBy));

        Ensure.BusinessRule(
            createdBy.CanManageTeams(),
            ValidationMessages.Team.NoPermissionToCreate);

        Name = name.Trim();
        Description = description?.Trim();
        Status = TeamStatus.Active; // New teams start as active

        // Raise domain event
        RaiseDomainEvent(new TeamCreatedDomainEvent(Id, Name, createdBy.Id));
    }

    /// <summary>
    /// Creates a new team with the specified details.
    /// </summary>
    /// <param name="name">The team name.</param>
    /// <param name="description">The team description (optional).</param>
    /// <param name="createdBy">The user creating the team.</param>
    /// <returns>A new Team instance.</returns>
    public static Team Create(string name, string? description, User createdBy)
    {
        return new Team(name, description, createdBy);
    }

    /// <summary>
    /// Updates the team information.
    /// </summary>
    /// <param name="name">The new team name.</param>
    /// <param name="description">The new team description (optional).</param>
    /// <exception cref="ArgumentException">Thrown when name is empty or whitespace.</exception>
    public void UpdateInfo(string name, string? description)
    {
        Ensure.NotNullOrWhiteSpace(name, nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }

    /// <summary>
    /// Adds a user to the team with the specified role.
    /// </summary>
    /// <param name="user">The user to add to the team.</param>
    /// <param name="role">The role to assign to the user in the team.</param>
    /// <param name="addedBy">The user adding the member (must have appropriate permissions).</param>
    /// <exception cref="ArgumentNullException">Thrown when user or addedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    /// <remarks>
    /// This method uses BusinessRules for static validation. For complex validations that require
    /// checking against external data (like active projects), use ITeamDomainService in the Application layer.
    /// </remarks>
    public void AddMember(User user, TeamRole role, User addedBy)
    {
        Ensure.NotNull(user, nameof(user));
        Ensure.NotNull(addedBy, nameof(addedBy));

        // Use BusinessRules for static authorization and validation
        Ensure.BusinessRule(
            BusinessRules.TeamManagement.CanAddMemberToTeam(
                user.Status, user.Role, _members.Count, addedBy.Role),
            ValidationMessages.Team.NoPermissionToAddMembers);

        Ensure.BusinessRule(
            !_members.Any(m => m.UserId == user.Id),
            ValidationMessages.User.AlreadyTeamMember);

        var teamMember = new TeamMember(user.Id, Id, role);
        _members.Add(teamMember);

        // Add the membership to the user as well
        user.AddTeamMembership(teamMember);

        // Raise domain event
        RaiseDomainEvent(new UserJoinedTeamDomainEvent(user.Id, Id, role));
    }

    /// <summary>
    /// Removes a user from the team.
    /// </summary>
    /// <param name="user">The user to remove from the team.</param>
    /// <param name="removedBy">The user removing the member (must have appropriate permissions).</param>
    /// <exception cref="ArgumentNullException">Thrown when user or removedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void RemoveMember(User user, User removedBy)
    {
        Ensure.NotNull(user, nameof(user));
        Ensure.NotNull(removedBy, nameof(removedBy));

        var member = _members.FirstOrDefault(m => m.UserId == user.Id);
        Ensure.BusinessRule(
            member != null,
            ValidationMessages.User.NotTeamMember);

        Ensure.BusinessRule(
            CanRemoveMembers(removedBy, member!),
            ValidationMessages.Team.NoPermissionToRemoveMembers);

        _members.Remove(member!);

        // Remove the membership from the user as well
        user.RemoveTeamMembership(Id);

        // Raise domain event
        RaiseDomainEvent(new UserLeftTeamDomainEvent(user.Id, Id, member!.Role));
    }

    /// <summary>
    /// Changes the role of a team member.
    /// </summary>
    /// <param name="user">The user whose role to change.</param>
    /// <param name="newRole">The new role to assign.</param>
    /// <param name="changedBy">The user making the change (must have appropriate permissions).</param>
    /// <exception cref="ArgumentNullException">Thrown when user or changedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when business rules are violated.</exception>
    public void ChangeMemberRole(User user, TeamRole newRole, User changedBy)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(changedBy);

        var member = _members.FirstOrDefault(m => m.UserId == user.Id);
        if (member == null)
            throw new DomainException("User is not a member of this team");

        member.ChangeRole(newRole, changedBy);
    }

    /// <summary>
    /// Checks if the specified user can add members to the team.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <returns>True if the user can add members, false otherwise.</returns>
    private bool CanAddMembers(User user)
    {
        // Admin can always add members
        if (user.Role == UserRole.Admin)
            return true;

        // Project managers and team leads can add members to their teams
        var userRole = user.GetRoleInTeam(Id);
        return userRole == TeamRole.ProjectManager || userRole == TeamRole.TeamLead;
    }

    /// <summary>
    /// Checks if the specified user can remove the given member from the team.
    /// </summary>
    /// <param name="user">The user attempting to remove the member.</param>
    /// <param name="memberToRemove">The member to be removed.</param>
    /// <returns>True if the user can remove the member, false otherwise.</returns>
    private bool CanRemoveMembers(User user, TeamMember memberToRemove)
    {
        // Admin can always remove members
        if (user.Role == UserRole.Admin)
            return true;

        // Users can remove themselves
        if (user.Id == memberToRemove.UserId)
            return true;

        // Project managers can remove any member
        if (user.GetRoleInTeam(Id) == TeamRole.ProjectManager)
            return true;

        // Team leads can remove developers and viewers
        if (user.GetRoleInTeam(Id) == TeamRole.TeamLead &&
            (memberToRemove.Role == TeamRole.Developer || memberToRemove.Role == TeamRole.Viewer))
            return true;

        return false;
    }

    /// <summary>
    /// Gets all members with the specified role.
    /// </summary>
    /// <param name="role">The role to filter by.</param>
    /// <returns>A collection of team members with the specified role.</returns>
    public IEnumerable<TeamMember> GetMembersByRole(TeamRole role)
    {
        return _members.Where(m => m.Role == role);
    }

    /// <summary>
    /// Gets all project managers in the team.
    /// </summary>
    /// <returns>A collection of project manager team members.</returns>
    public IEnumerable<TeamMember> GetProjectManagers()
    {
        return GetMembersByRole(TeamRole.ProjectManager);
    }

    /// <summary>
    /// Gets all team leads in the team.
    /// </summary>
    /// <returns>A collection of team lead members.</returns>
    public IEnumerable<TeamMember> GetTeamLeads()
    {
        return GetMembersByRole(TeamRole.TeamLead);
    }

    /// <summary>
    /// Gets all developers in the team.
    /// </summary>
    /// <returns>A collection of developer team members.</returns>
    public IEnumerable<TeamMember> GetDevelopers()
    {
        return GetMembersByRole(TeamRole.Developer);
    }

    /// <summary>
    /// Checks if the team has room for more members.
    /// </summary>
    /// <returns>True if the team can accept more members, false otherwise.</returns>
    public bool HasRoom()
    {
        return _members.Count < MaxMembers;
    }

    /// <summary>
    /// Checks if the team has any members with leadership roles.
    /// </summary>
    /// <returns>True if the team has at least one project manager or team lead, false otherwise.</returns>
    public bool HasLeadership()
    {
        return _members.Any(m => m.HasLeadershipPrivileges());
    }

    /// <summary>
    /// Gets the member record for the specified user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The team member record, or null if the user is not a member.</returns>
    public TeamMember? GetMember(UserId userId)
    {
        return _members.FirstOrDefault(m => m.UserId == userId);
    }

    /// <summary>
    /// Changes the team status to the specified new status.
    /// </summary>
    /// <param name="newStatus">The new status to transition to.</param>
    /// <param name="changedBy">The user making the status change.</param>
    /// <exception cref="ArgumentNullException">Thrown when changedBy is null.</exception>
    /// <exception cref="DomainException">Thrown when the status transition is invalid or user lacks permission.</exception>
    public void ChangeStatus(TeamStatus newStatus, User changedBy)
    {
        Ensure.NotNull(changedBy, nameof(changedBy));

        Ensure.BusinessRule(
            changedBy.CanManageTeams(),
            "Only users with team management privileges can change team status");

        Ensure.BusinessRule(
            Status.CanTransitionTo(newStatus),
            $"Cannot transition team status from {Status} to {newStatus}");

        if (Status == newStatus)
            return; // No change needed

        var previousStatus = Status;
        Status = newStatus;

        // Raise domain event for status change
        RaiseDomainEvent(new TeamStatusChangedDomainEvent(Id, previousStatus, newStatus, changedBy.Id));
    }

    /// <summary>
    /// Activates the team if it's currently inactive or pending.
    /// </summary>
    /// <param name="activatedBy">The user activating the team.</param>
    public void Activate(User activatedBy)
    {
        ChangeStatus(TeamStatus.Active, activatedBy);
    }

    /// <summary>
    /// Deactivates the team, preventing new members from joining.
    /// </summary>
    /// <param name="deactivatedBy">The user deactivating the team.</param>
    public void Deactivate(User deactivatedBy)
    {
        ChangeStatus(TeamStatus.Inactive, deactivatedBy);
    }

    /// <summary>
    /// Archives the team for historical purposes.
    /// </summary>
    /// <param name="archivedBy">The user archiving the team.</param>
    public void Archive(User archivedBy)
    {
        ChangeStatus(TeamStatus.Archived, archivedBy);
    }

    /// <summary>
    /// Determines if the team can accept new members based on its current status and member count.
    /// </summary>
    /// <returns>True if new members can join; otherwise, false.</returns>
    public bool CanAcceptNewMembers()
    {
        return Status.AllowsMemberAddition() && _members.Count < MaxMembers;
    }

    /// <summary>
    /// Determines if the team can create new projects based on its current status.
    /// </summary>
    /// <returns>True if new projects can be created; otherwise, false.</returns>
    public bool CanCreateProjects()
    {
        return Status.AllowsProjectCreation();
    }

    /// <summary>
    /// Determines if the team can perform operational activities based on its current status.
    /// </summary>
    /// <returns>True if operational activities are allowed; otherwise, false.</returns>
    public bool IsOperational()
    {
        return Status.AllowsOperations();
    }
}
