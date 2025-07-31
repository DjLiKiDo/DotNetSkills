using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;
using DotNetSkills.Domain.Enums;

namespace DotNetSkills.Domain.Repositories;

/// <summary>
/// Repository contract for Project aggregate operations.
/// Handles project management and team relationships.
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Gets a project by its unique identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Project?> GetByIdAsync(ProjectId id);
    
    /// <summary>
    /// Gets a project by its name within a team.
    /// Used for duplicate name validation within team scope.
    /// </summary>
    /// <param name="name">The project name to search for.</param>
    /// <param name="teamId">The team identifier to scope the search.</param>
    /// <returns>The project if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Project?> GetByNameInTeamAsync(string name, TeamId teamId);
    
    /// <summary>
    /// Gets all projects belonging to a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier.</param>
    /// <returns>Projects belonging to the specified team.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Project>> GetByTeamIdAsync(TeamId teamId);
    
    /// <summary>
    /// Gets a project with all its tasks loaded.
    /// Used when project task operations are needed.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project with tasks if found, null otherwise.</returns>
    System.Threading.Tasks.Task<Project?> GetProjectWithTasksAsync(ProjectId id);
    
    /// <summary>
    /// Gets all active projects in the system.
    /// Excludes completed, cancelled, or on-hold projects.
    /// </summary>
    /// <returns>A read-only collection of active projects.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Project>> GetActiveProjectsAsync();
    
    /// <summary>
    /// Gets projects by their status.
    /// </summary>
    /// <param name="status">The project status to filter by.</param>
    /// <returns>Projects with the specified status.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Project>> GetByStatusAsync(ProjectStatus status);
    
    /// <summary>
    /// Gets projects approaching their deadline.
    /// </summary>
    /// <param name="daysFromNow">Number of days from now to consider as approaching.</param>
    /// <returns>Projects with deadlines within the specified timeframe.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Project>> GetProjectsApproachingDeadlineAsync(int daysFromNow);
    
    /// <summary>
    /// Gets projects that have overdue tasks.
    /// Used for project health monitoring.
    /// </summary>
    /// <returns>Projects containing overdue tasks.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<Project>> GetProjectsWithOverdueTasksAsync();
    
    /// <summary>
    /// Adds a new project to the repository.
    /// </summary>
    /// <param name="project">The project to add.</param>
    /// <returns>The added project with any generated values.</returns>
    System.Threading.Tasks.Task<Project> AddAsync(Project project);
    
    /// <summary>
    /// Updates an existing project in the repository.
    /// </summary>
    /// <param name="project">The project to update.</param>
    System.Threading.Tasks.Task UpdateAsync(Project project);
    
    /// <summary>
    /// Removes a project from the repository.
    /// Should validate business rules before deletion.
    /// </summary>
    /// <param name="id">The identifier of the project to remove.</param>
    System.Threading.Tasks.Task DeleteAsync(ProjectId id);
    
    /// <summary>
    /// Checks if a project exists with the given name in the specified team.
    /// Used for validation during project creation.
    /// </summary>
    /// <param name="name">The project name to check.</param>
    /// <param name="teamId">The team identifier to scope the check.</param>
    /// <returns>True if a project exists with the name in the team, false otherwise.</returns>
    System.Threading.Tasks.Task<bool> ExistsWithNameInTeamAsync(string name, TeamId teamId);
}
