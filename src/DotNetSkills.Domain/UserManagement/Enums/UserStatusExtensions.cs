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

    // ...existing code...

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
