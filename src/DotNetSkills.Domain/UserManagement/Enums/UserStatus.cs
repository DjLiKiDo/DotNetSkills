namespace DotNetSkills.Domain.UserManagement.Enums;

/// <summary>
/// Represents the current status of a user in the system.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// The user account is active and can perform operations.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The user account is inactive and cannot perform operations.
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// The user account is suspended temporarily.
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// The user account is pending activation (newly created).
    /// </summary>
    Pending = 4
}
