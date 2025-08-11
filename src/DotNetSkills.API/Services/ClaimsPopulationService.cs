using System.Security.Claims;
using MediatR;
using DotNetSkills.Application.UserManagement.Features.GetUserTeamMemberships;
using DotNetSkills.Application.Common.Abstractions;
using DotNetSkills.Domain.UserManagement.Entities;
using DotNetSkills.Domain.UserManagement.ValueObjects;
using DotNetSkills.Domain.UserManagement.Enums;
using DotNetSkills.Domain.TeamCollaboration.Enums;
using DotNetSkills.Application.UserManagement.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace DotNetSkills.API.Services;

/// <summary>
/// Service for populating user claims with additional membership and permission information.
/// Used during JWT token generation to add team and project membership claims.
/// </summary>
public interface IClaimsPopulationService
{
    /// <summary>
    /// Populates claims for a user with team and project membership information.
    /// </summary>
    Task<IEnumerable<Claim>> PopulateUserClaimsAsync(UserId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates basic identity claims for a user.
    /// </summary>
    Task<ClaimsIdentity> CreateUserIdentityAsync(UserId userId, string authenticationType = "jwt", CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of claims population service that retrieves membership data from application layer.
/// </summary>
public sealed class ClaimsPopulationService : IClaimsPopulationService
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ClaimsPopulationService> _logger;
    private readonly IMemoryCache _cache;

    // Namespaced custom claim types
    private const string Namespace = "dotnetskills:";
    private static class ClaimTypesExt
    {
        public const string UserId = Namespace + "user_id";
        public const string Role = Namespace + "role";
        public const string Status = Namespace + "status";
        public const string Team = Namespace + "team";             // value: teamId:role
        public const string TeamMember = Namespace + "team_member"; // value: teamId
        public const string TeamRole = Namespace + "team_role";     // value: teamId:role
        public const string TeamLeader = Namespace + "team_leader"; // value: teamId
        public const string Project = Namespace + "project";        // value: projectId or team-{teamId}
        public const string Permission = Namespace + "perm";        // value: permission identifier
    }

    private static readonly TimeSpan MembershipCacheTtl = TimeSpan.FromMinutes(5);

    public ClaimsPopulationService(
        IMediator mediator,
        IUserRepository userRepository,
        ILogger<ClaimsPopulationService> logger,
        IMemoryCache cache)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// Populates claims for a user including basic info, role, team memberships, and permissions.
    /// </summary>
    public async Task<IEnumerable<Claim>> PopulateUserClaimsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>();

        try
        {
            // Get user basic information
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found for claims population: {UserId}", userId.Value);
                return claims;
            }

            // Basic identity claims
            claims.AddRange(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()),
                new Claim("sub", userId.Value.ToString()),
                new Claim(ClaimTypesExt.UserId, userId.Value.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypesExt.Role, user.Role.ToString()),
                new Claim(ClaimTypesExt.Status, user.Status.ToString())
            });

            // Get team memberships (cached)
            var cacheKey = $"claims:team-memberships:{userId.Value}";
            TeamMembershipListDto teamMemberships = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = MembershipCacheTtl;
                return await _mediator.Send(new GetUserTeamMembershipsQuery(userId), cancellationToken);
            }) ?? new TeamMembershipListDto
            {
                UserId = user.Id,
                UserName = user.Name,
                TeamMemberships = Array.Empty<TeamMembershipDto>()
            };
            
            foreach (var membership in teamMemberships.TeamMemberships)
            {
                claims.Add(new Claim(ClaimTypesExt.Team, $"{membership.TeamId}:{membership.Role}"));
                claims.Add(new Claim(ClaimTypesExt.TeamMember, membership.TeamId.ToString()));
                claims.Add(new Claim(ClaimTypesExt.TeamRole, $"{membership.TeamId}:{membership.Role}"));
            }

            // Add permission-based claims
            AppendPermissionClaims(claims, user, teamMemberships);

            _logger.LogDebug("Populated {ClaimCount} claims for user {UserId}", claims.Count, userId.Value);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating claims for user {UserId}", userId.Value);
            // Preserve already added basic claims; avoid clearing to retain identity context.
        }

        return claims;
    }

    /// <summary>
    /// Creates a complete ClaimsIdentity for a user with all populated claims.
    /// </summary>
    public async Task<ClaimsIdentity> CreateUserIdentityAsync(UserId userId, string authenticationType = "jwt", CancellationToken cancellationToken = default)
    {
        var claims = await PopulateUserClaimsAsync(userId, cancellationToken);
        return new ClaimsIdentity(claims, authenticationType);
    }

    /// <summary>
    /// Adds permission-based claims based on user's role and memberships.
    /// </summary>
    private static void AppendPermissionClaims(
        List<Claim> claims,
        User user,
        TeamMembershipListDto teamMemberships)
    {
        if (user.Role == UserRole.Admin)
        {
            claims.Add(new Claim(ClaimTypesExt.Permission, "admin"));
            claims.Add(new Claim(ClaimTypesExt.Permission, "manage_all_teams"));
            claims.Add(new Claim(ClaimTypesExt.Permission, "manage_all_projects"));
            claims.Add(new Claim(ClaimTypesExt.Permission, "manage_users"));
        }

        if (user.Role == UserRole.ProjectManager || user.Role == UserRole.Admin)
        {
            claims.Add(new Claim(ClaimTypesExt.Permission, "manage_projects"));
            claims.Add(new Claim(ClaimTypesExt.Permission, "assign_tasks"));
        }

        var leadershipTeams = teamMemberships.TeamMemberships
            .Where(tm => tm.Role == TeamRole.TeamLead || tm.Role == TeamRole.ProjectManager)
            .ToList();
        foreach (var leaderTeam in leadershipTeams)
        {
            claims.Add(new Claim(ClaimTypesExt.TeamLeader, leaderTeam.TeamId.ToString()));
            claims.Add(new Claim(ClaimTypesExt.Permission, $"manage_team:{leaderTeam.TeamId}"));
        }

        if (user.Role == UserRole.Developer)
        {
            claims.Add(new Claim(ClaimTypesExt.Permission, "view_assigned_tasks"));
            claims.Add(new Claim(ClaimTypesExt.Permission, "update_task_status"));
        }

        foreach (var membership in teamMemberships.TeamMemberships)
        {
            claims.Add(new Claim(ClaimTypesExt.Project, $"team-{membership.TeamId}"));
        }
    }
}