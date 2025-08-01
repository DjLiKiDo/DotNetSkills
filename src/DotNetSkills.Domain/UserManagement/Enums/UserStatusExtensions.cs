namespace DotNetSkills.Domain.UserManagement.Enums;

/// <summary>
/// Extension methods for UserStatus enum providing business logic and utility methods.
/// </summary>
public static class UserStatusExtensions
{
    /// <summary>
    /// Gets the display name for the user status.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>A human-readable display name.</returns>
    public static string GetDisplayName(this UserStatus status) => status switch
    {
        UserStatus.Active => "Active",
        UserStatus.Inactive => "Inactive",
        UserStatus.Suspended => "Suspended",
        UserStatus.Pending => "Pending Activation",
        _ => status.ToString()
    };

    /// <summary>
    /// Checks if the user status allows login.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>True if the user can login, false otherwise.</returns>
    public static bool CanLogin(this UserStatus status) =>
        status == UserStatus.Active;

    /// <summary>
    /// Checks if the user status allows being assigned tasks.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>True if the user can be assigned tasks, false otherwise.</returns>
    public static bool CanBeAssignedTasks(this UserStatus status) =>
        status == UserStatus.Active;

    /// <summary>
    /// Checks if the user status is in an active state.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>True if the user is active, false otherwise.</returns>
    public static bool IsActive(this UserStatus status) =>
        status == UserStatus.Active;

    /// <summary>
    /// Checks if the user status is temporarily restricted.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>True if the user is temporarily restricted, false otherwise.</returns>
    public static bool IsTemporarilyRestricted(this UserStatus status) =>
        status is UserStatus.Suspended or UserStatus.Pending;

    /// <summary>
    /// Checks if the user status is permanently inactive.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>True if the user is permanently inactive, false otherwise.</returns>
    public static bool IsPermanentlyInactive(this UserStatus status) =>
        status == UserStatus.Inactive;

    /// <summary>
    /// Gets the color code associated with the user status for UI purposes.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>A hexadecimal color code.</returns>
    public static string GetColorCode(this UserStatus status) => status switch
    {
        UserStatus.Active => "#28a745",      // Green
        UserStatus.Inactive => "#6c757d",    // Gray
        UserStatus.Suspended => "#dc3545",   // Red
        UserStatus.Pending => "#ffc107",     // Yellow
        _ => "#6c757d"                       // Gray (default)
    };

    /// <summary>
    /// Determines if the current status can transition to the specified new status.
    /// </summary>
    /// <param name="currentStatus">The current user status.</param>
    /// <param name="newStatus">The target status to transition to.</param>
    /// <returns>True if the transition is valid, false otherwise.</returns>
    public static bool CanTransitionTo(this UserStatus currentStatus, UserStatus newStatus)
    {
        return currentStatus switch
        {
            UserStatus.Pending => newStatus is UserStatus.Active or UserStatus.Inactive,
            UserStatus.Active => newStatus is UserStatus.Inactive or UserStatus.Suspended,
            UserStatus.Inactive => newStatus is UserStatus.Active or UserStatus.Suspended,
            UserStatus.Suspended => newStatus is UserStatus.Active or UserStatus.Inactive,
            _ => false
        };
    }

    /// <summary>
    /// Gets the description of what the status means for business context.
    /// </summary>
    /// <param name="status">The user status.</param>
    /// <returns>A descriptive explanation of the status.</returns>
    public static string GetDescription(this UserStatus status) => status switch
    {
        UserStatus.Active => "User can perform all authorized operations",
        UserStatus.Inactive => "User account is deactivated and cannot access the system",
        UserStatus.Suspended => "User account is temporarily suspended due to policy violations",
        UserStatus.Pending => "User account is newly created and pending activation",
        _ => "Unknown status"
    };
}
