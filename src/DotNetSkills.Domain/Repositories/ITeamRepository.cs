using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;

namespace DotNetSkills.Domain.Repositories;

/// <summary>
/// Repository contract for Team aggregate operations.
/// Handles team management and member relationships.
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Gets a team by its unique identifier.
    /// </summary>
    /// <param name="id">The team identifier.</param>
    /// <returns>The team if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Team?> GetByIdAsync(TeamId id);
    
    /// <summary>
    /// Gets a team by its name.
    /// Used for duplicate name validation.
    /// </summary>
    /// <param name="name">The team name to search for.</param>
    /// <returns>The team if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Team?> GetByNameAsync(string name);
    
    /// <summary>
    /// Gets all teams where the specified user is a member.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Teams where the user is a member.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Team>> GetTeamsForUserAsync(UserId userId);
    
    /// <summary>
    /// Gets a team with all its members loaded.
    /// Used when team member operations are needed.
    /// </summary>
    /// <param name="id">The team identifier.</param>
    /// <returns>The team with members if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Team?> GetTeamWithMembersAsync(TeamId id);
    
    /// <summary>
    /// Gets all active teams in the system.
    /// </summary>
    /// <returns>A read-only collection of active teams.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Team>> GetActiveTeamsAsync();
    
    /// <summary>
    /// Gets teams that have no active projects.
    /// Used for cleanup or reallocation scenarios.
    /// </summary>
    /// <returns>Teams without active projects.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Team>> GetTeamsWithoutActiveProjectsAsync();
    
    /// <summary>
    /// Adds a new team to the repository.
    /// </summary>
    /// <param name="team">The team to add.</param>
    /// <returns>The added team with any generated values.</returns>
    System.Threading.Tasks.Task<Team> AddAsync(Team team);
    
    /// <summary>
    /// Updates an existing team in the repository.
    /// </summary>
    /// <param name="team">The team to update.</param>
    System.Threading.Tasks.Task UpdateAsync(Team team);
    
    /// <summary>
    /// Removes a team from the repository.
    /// Should validate business rules before deletion.
    /// </summary>
    /// <param name="id">The identifier of the team to remove.</param>
    System.Threading.Tasks.Task DeleteAsync(TeamId id);
    
    /// <summary>
    /// Checks if a team exists with the given name.
    /// Used for validation during team creation.
    /// </summary>
    /// <param name="name">The team name to check.</param>
    /// <returns>True if a team exists with the name, false otherwise.</returns>
    System.Threading.Tasks.Task<bool> ExistsWithNameAsync(string name);
}
