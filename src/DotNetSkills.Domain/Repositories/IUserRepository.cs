using DotNetSkills.Domain.Entities;
using DotNetSkills.Domain.ValueObjects;
using DotNetSkills.Domain.Enums;

namespace DotNetSkills.Domain.Repositories;

/// <summary>
/// Repository contract for User aggregate operations.
/// Defines domain-specific data access patterns for user management.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their unique identifier.
    /// </summary>
    /// <param name="id">The user identifier.</param>
    /// <returns>The user if found, null otherwise.</returns>
    System.Threading.Tasks.Task<User?> GetByIdAsync(UserId id);
    
    /// <summary>
    /// Gets a user by their email address.
    /// Used for authentication and duplicate email validation.
    /// </summary>
    /// <param name="email">The email address to search for.</param>
    /// <returns>The user if found, null otherwise.</returns>
    System.Threading.Tasks.Task<User?> GetByEmailAsync(EmailAddress email);
    
    /// <summary>
    /// Gets all active users in the system.
    /// Excludes deactivated or deleted users.
    /// </summary>
    /// <returns>A read-only collection of active users.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<User>> GetActiveUsersAsync();
    
    /// <summary>
    /// Gets users by their role for authorization scenarios.
    /// </summary>
    /// <param name="role">The user role to filter by.</param>
    /// <returns>Users with the specified role.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<User>> GetUsersByRoleAsync(UserRole role);
    
    /// <summary>
    /// Gets users who are members of a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier.</param>
    /// <returns>Users who belong to the specified team.</returns>
    System.Threading.Tasks.Task<IReadOnlyList<User>> GetTeamMembersAsync(TeamId teamId);
    
    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <returns>The added user with any generated values.</returns>
    System.Threading.Tasks.Task<User> AddAsync(User user);
    
    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user to update.</param>
    System.Threading.Tasks.Task UpdateAsync(User user);
    
    /// <summary>
    /// Removes a user from the repository.
    /// This should handle business rules around user deletion.
    /// </summary>
    /// <param name="id">The identifier of the user to remove.</param>
    System.Threading.Tasks.Task DeleteAsync(UserId id);
    
    /// <summary>
    /// Checks if a user exists with the given email address.
    /// Used for validation during user creation.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>True if a user exists with the email, false otherwise.</returns>
    System.Threading.Tasks.Task<bool> ExistsWithEmailAsync(EmailAddress email);
}
