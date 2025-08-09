using System.Security.Claims;
using DotNetSkills.Domain.UserManagement.Enums;

namespace DotNetSkills.API.Authorization;

/// <summary>
/// Extension methods for configuring authorization policies.
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds authorization policies to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add policies to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApiAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // AdminOnly policy - requires Admin role
            options.AddPolicy(Policies.AdminOnly, policy =>
                policy.RequireClaim(ClaimTypes.Role, UserRole.Admin.ToString()));

            // TeamManager policy - requires Admin or ProjectManager role  
            // (Note: Using ProjectManager since there's no TeamManager role in UserRole enum)
            options.AddPolicy(Policies.TeamManager, policy =>
                policy.RequireClaim(ClaimTypes.Role, 
                    UserRole.Admin.ToString(), 
                    UserRole.ProjectManager.ToString()));

            // ProjectManagerOrAdmin policy - requires Admin or ProjectManager role
            options.AddPolicy(Policies.ProjectManagerOrAdmin, policy =>
                policy.RequireClaim(ClaimTypes.Role, 
                    UserRole.Admin.ToString(), 
                    UserRole.ProjectManager.ToString()));

            // ProjectMemberOrAdmin policy - requires Admin, ProjectManager, Developer role OR project membership claim
            options.AddPolicy(Policies.ProjectMemberOrAdmin, policy =>
                policy.RequireAssertion(context =>
                {
                    // Check if user has elevated roles
                    var userRoles = context.User.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    if (userRoles.Contains(UserRole.Admin.ToString()) ||
                        userRoles.Contains(UserRole.ProjectManager.ToString()) ||
                        userRoles.Contains(UserRole.Developer.ToString()))
                    {
                        return true;
                    }

                    // Check for explicit project membership claim
                    // Format: "project:{projectId}" 
                    var projectClaims = context.User.Claims
                        .Where(c => c.Type == "project")
                        .Select(c => c.Value)
                        .ToList();

                    return projectClaims.Count > 0;
                }));
        });

        return services;
    }
}