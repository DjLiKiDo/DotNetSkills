namespace DotNetSkills.Domain.UserManagement.Services;

/// <summary>
/// Domain service for complex user management operations that require external dependencies
/// or cross-aggregate coordination. This service handles operations that don't naturally
/// belong to a single entity and require access to repositories or other external services.
/// </summary>
/// <remarks>
/// This interface defines contracts for operations that:
/// - Require database access (email uniqueness validation)
/// - Involve cross-aggregate business logic
/// - Orchestrate complex domain operations
/// 
/// Simple authorization rules remain in BusinessRules.Authorization for performance
/// and to maintain domain independence.
/// </remarks>
public interface IUserDomainService
{
    /// <summary>
    /// Validates that an email address is unique across all users in the system.
    /// This operation requires repository access to check existing users.
    /// </summary>
    /// <param name="email">The email address to validate for uniqueness.</param>
    /// <param name="excludeUserId">Optional user ID to exclude from the check (for updates).</param>
    /// <returns>True if the email is unique, false if it already exists.</returns>
    Task<bool> IsEmailUniqueAsync(EmailAddress email, UserId? excludeUserId = null);

    /// <summary>
    /// Validates if a user can be safely deleted from the system.
    /// This involves checking for active assignments, team memberships, and other dependencies.
    /// </summary>
    /// <param name="userId">The ID of the user to validate for deletion.</param>
    /// <param name="requestingUser">The user requesting the deletion (for authorization).</param>
    /// <returns>True if the user can be deleted, false otherwise.</returns>
    Task<bool> CanDeleteUserAsync(UserId userId, User requestingUser);

    /// <summary>
    /// Validates if a user has any active task assignments that would prevent certain operations.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <returns>True if the user has active task assignments, false otherwise.</returns>
    Task<bool> HasActiveTaskAssignmentsAsync(UserId userId);

    /// <summary>
    /// Validates if a user can be assigned to a specific team considering current workload,
    /// team capacity, and business rules.
    /// </summary>
    /// <param name="userId">The ID of the user to assign.</param>
    /// <param name="teamId">The ID of the team to assign to.</param>
    /// <param name="requestingUser">The user making the assignment request.</param>
    /// <returns>True if the user can be assigned to the team, false otherwise.</returns>
    Task<bool> CanAssignToTeamAsync(UserId userId, TeamId teamId, User requestingUser);

    /// <summary>
    /// Performs comprehensive validation for user creation, including all business rules
    /// and data consistency checks that require external dependencies.
    /// </summary>
    /// <param name="request">The user creation request details.</param>
    /// <param name="createdByUser">The user creating the new user.</param>
    /// <returns>A domain validation result with any validation errors.</returns>
    Task<DomainValidationResult> ValidateUserCreationAsync(CreateUserRequest request, User? createdByUser);
}

/// <summary>
/// Request object for user creation validation.
/// </summary>
public record CreateUserRequest(
    string Name,
    EmailAddress Email,
    UserRole Role);

/// <summary>
/// Result object for domain validation operations.
/// </summary>
public class DomainValidationResult
{
    private readonly List<string> _errors = new();

    /// <summary>
    /// Gets whether the validation was successful (no errors).
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyList<string> Errors => _errors.AsReadOnly();

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    /// <param name="error">The error message to add.</param>
    public void AddError(string error)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            _errors.Add(error);
        }
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A validation result with no errors.</returns>
    public static DomainValidationResult Success() => new();

    /// <summary>
    /// Creates a failed validation result with the specified error.
    /// </summary>
    /// <param name="error">The validation error.</param>
    /// <returns>A validation result with the specified error.</returns>
    public static DomainValidationResult Failure(string error)
    {
        var result = new DomainValidationResult();
        result.AddError(error);
        return result;
    }

    /// <summary>
    /// Creates a failed validation result with multiple errors.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A validation result with the specified errors.</returns>
    public static DomainValidationResult Failure(IEnumerable<string> errors)
    {
        var result = new DomainValidationResult();
        foreach (var error in errors)
        {
            result.AddError(error);
        }
        return result;
    }
}
