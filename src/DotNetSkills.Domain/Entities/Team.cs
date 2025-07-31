using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.Enums;
using DotNetSkills.Domain.Events;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Entities;

public class Team : BaseEntity<TeamId>
{
    private readonly List<TeamMember> _members = new();
    private readonly List<Project> _projects = new();

    private Team() : base() { }

    public Team(TeamId id, string name, string? description = null)
        : base(id)
    {
        Name = ValidateAndSetName(name);
        Description = description?.Trim();
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public IReadOnlyList<TeamMember> Members => _members.AsReadOnly();
    public IReadOnlyList<Project> Projects => _projects.AsReadOnly();

    public int MemberCount => _members.Count;
    public int ActiveProjectCount => _projects.Count(p => p.Status.IsActive());

    public void UpdateDetails(string name, string? description)
    {
        Name = ValidateAndSetName(name);
        Description = description?.Trim();
        UpdateTimestamp();
    }

    public void AddMember(User user, UserRole addedByRole, UserId addedBy)
    {
        if (!addedByRole.CanManageTeams())
            throw new DomainException("Only project managers and administrators can add team members.");

        if (!IsActive)
            throw new DomainException("Cannot add members to an inactive team.");

        if (_members.Any(m => m.UserId == user.Id))
            throw new DomainException($"User {user.FullName} is already a member of this team.");

        var joinedAt = GetCurrentTime();
        var teamMember = new TeamMember(user.Id, Id, joinedAt);
        _members.Add(teamMember);
        user.AddTeamMembership(teamMember);
        
        UpdateTimestamp();

        RaiseDomainEvent(new TeamMemberAddedDomainEvent(
            Id,
            Name,
            user.Id,
            user.FullName,
            user.Email.Value,
            addedBy,
            joinedAt
        ));
    }

    public void RemoveMember(UserId userId, UserRole removedByRole, UserId removedBy)
    {
        if (!removedByRole.CanManageTeams())
            throw new DomainException("Only project managers and administrators can remove team members.");

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
            throw new DomainException("User is not a member of this team.");

        // Check if user has active tasks in team projects
        var hasActiveTasks = _projects.Any(p => 
            p.Tasks.Any(t => t.AssignedToId == userId && !t.Status.IsComplete()));

        if (hasActiveTasks)
            throw new DomainException("Cannot remove team member who has active tasks assigned.");

        var membershipDuration = member.MembershipDuration;
        var removedAt = GetCurrentTime();

        _members.Remove(member);
        UpdateTimestamp();

        RaiseDomainEvent(new TeamMemberRemovedDomainEvent(
            Id,
            Name,
            member.UserId,
            member.User.FullName,
            member.User.Email.Value,
            removedBy,
            removedAt,
            membershipDuration
        ));
    }

    public void Deactivate(UserRole deactivatedByRole, UserId deactivatedBy)
    {
        if (!deactivatedByRole.CanManageTeams())
            throw new DomainException("Only project managers and administrators can deactivate teams.");

        if (!IsActive)
            return;

        if (HasActiveProjects())
            throw new DomainException("Cannot deactivate team with active projects.");

        IsActive = false;
        var deactivatedAt = GetCurrentTime();
        UpdateTimestamp();

        RaiseDomainEvent(new TeamDeactivatedDomainEvent(
            Id,
            Name,
            Description,
            MemberCount,
            deactivatedBy,
            deactivatedAt
        ));
    }

    public void Activate(UserRole activatedByRole, UserId activatedBy)
    {
        if (!activatedByRole.CanManageTeams())
            throw new DomainException("Only project managers and administrators can activate teams.");

        if (IsActive)
            return;

        IsActive = true;
        var activatedAt = GetCurrentTime();
        UpdateTimestamp();

        RaiseDomainEvent(new TeamActivatedDomainEvent(
            Id,
            Name,
            Description,
            MemberCount,
            activatedBy,
            activatedAt
        ));
    }

    public bool HasMember(UserId userId)
    {
        return _members.Any(m => m.UserId == userId);
    }

    public bool HasActiveProjects()
    {
        return _projects.Any(p => p.Status.IsActive());
    }

    public bool CanUserManageTeam(User user)
    {
        return user.IsActive && user.Role.CanManageTeams();
    }

    public bool CanUserAccessTeam(User user)
    {
        if (!user.IsActive)
            return false;

        if (user.Role.CanManageTeams())
            return true;

        return HasMember(user.Id);
    }

    public TeamMember? GetMember(UserId userId)
    {
        return _members.FirstOrDefault(m => m.UserId == userId);
    }

    public IEnumerable<User> GetMembersWithRole(UserRole role)
    {
        return _members
            .Where(m => m.User.Role == role)
            .Select(m => m.User);
    }

    internal void AddProject(Project project)
    {
        if (_projects.Any(p => p.Id == project.Id))
            return;

        _projects.Add(project);
    }

    internal void RemoveProject(ProjectId projectId)
    {
        var project = _projects.FirstOrDefault(p => p.Id == projectId);
        if (project != null)
            _projects.Remove(project);
    }

    private static string ValidateAndSetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Team name is required.");

        if (name.Length < DomainConstants.Team.MinNameLength)
            throw new DomainException($"Team name must be at least {DomainConstants.Team.MinNameLength} characters long.");

        if (name.Length > DomainConstants.Team.MaxNameLength)
            throw new DomainException($"Team name cannot exceed {DomainConstants.Team.MaxNameLength} characters.");

        return name.Trim();
    }
}