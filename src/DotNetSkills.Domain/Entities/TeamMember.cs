using DotNetSkills.Domain.Common;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Entities;

public class TeamMember : IEquatable<TeamMember>
{
    private TeamMember() { }

    public TeamMember(UserId userId, TeamId teamId, DateTime joinedAt)
    {
        UserId = userId;
        TeamId = teamId;
        JoinedAt = joinedAt;
    }

    public UserId UserId { get; private set; }
    public TeamId TeamId { get; private set; }
    public DateTime JoinedAt { get; private set; }

    public User User { get; private set; } = null!;
    public Team Team { get; private set; } = null!;

    public TimeSpan MembershipDuration => DateTime.UtcNow - JoinedAt;
    public int DaysAsMember => (int)MembershipDuration.TotalDays;

    public bool IsRecentMember(int daysThreshold = 30)
    {
        return DaysAsMember <= daysThreshold;
    }

    public bool IsLongTermMember(int daysThreshold = 365)
    {
        return DaysAsMember >= daysThreshold;
    }

    internal void SetUser(User user)
    {
        User = user;
    }

    internal void SetTeam(Team team)
    {
        Team = team;
    }

    public bool Equals(TeamMember? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return UserId.Equals(other.UserId) && TeamId.Equals(other.TeamId);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TeamMember);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(UserId, TeamId);
    }

    public static bool operator ==(TeamMember? left, TeamMember? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TeamMember? left, TeamMember? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"TeamMember(UserId: {UserId}, TeamId: {TeamId}, JoinedAt: {JoinedAt:yyyy-MM-dd})";
    }
}