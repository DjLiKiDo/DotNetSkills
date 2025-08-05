namespace DotNetSkills.Domain.Common.Validation;

/// <summary>
/// Centralized validation constants for primitive values used across domain entities.
/// All magic numbers and validation limits are defined here with appropriate RFC references
/// and business justifications for consistency and maintainability.
/// </summary>
/// <remarks>
/// This class follows the Domain-Driven Design principle of making business rules explicit
/// and removes magic numbers from domain entities. All constants are organized by category
/// and include documentation with relevant standards references where applicable.
/// </remarks>
[ExcludeFromCodeCoverage]
public static class ValidationConstants
{
    /// <summary>
    /// String length validation constants for text fields across domain entities.
    /// These values are based on common business requirements and database field limitations.
    /// </summary>
    public static class StringLengths
    {
        /// <summary>
        /// Maximum length for user names.
        /// Based on common database varchar limits and UI display considerations.
        /// </summary>
        public const int UserNameMaxLength = 100;

        /// <summary>
        /// Minimum length for user names to ensure meaningful identification.
        /// </summary>
        public const int UserNameMinLength = 2;

        /// <summary>
        /// Maximum length for task titles.
        /// Allows for descriptive titles while maintaining readability in lists and UI.
        /// </summary>
        public const int TaskTitleMaxLength = 200;

        /// <summary>
        /// Minimum length for task titles to ensure meaningful descriptions.
        /// </summary>
        public const int TaskTitleMinLength = 3;

        /// <summary>
        /// Maximum length for team names.
        /// Based on organizational naming conventions and display requirements.
        /// </summary>
        public const int TeamNameMaxLength = 100;

        /// <summary>
        /// Minimum length for team names to ensure meaningful identification.
        /// </summary>
        public const int TeamNameMinLength = 2;

        /// <summary>
        /// Maximum length for project names.
        /// Slightly longer than team names to accommodate project naming conventions.
        /// </summary>
        public const int ProjectNameMaxLength = 150;

        /// <summary>
        /// Minimum length for project names to ensure meaningful identification.
        /// </summary>
        public const int ProjectNameMinLength = 3;

        /// <summary>
        /// Maximum length for description fields (tasks, projects, etc.).
        /// Balances comprehensive descriptions with storage and performance considerations.
        /// </summary>
        public const int DescriptionMaxLength = 1000;

        /// <summary>
        /// Maximum length for email addresses per RFC 5321 specification.
        /// The total length of an email address is limited to 254 characters.
        /// Reference: RFC 5321 Section 4.1.2
        /// </summary>
        public const int EmailMaxLength = 254;

        /// <summary>
        /// Maximum length for the local part of an email address per RFC 5321.
        /// The local part is limited to 64 octets.
        /// Reference: RFC 5321 Section 4.1.2
        /// </summary>
        public const int EmailLocalPartMaxLength = 64;

        /// <summary>
        /// Maximum length for the domain part of an email address per RFC 5321.
        /// The domain part is limited to 255 octets including the length octet.
        /// Reference: RFC 5321 Section 4.1.2
        /// </summary>
        public const int EmailDomainPartMaxLength = 253;
    }

    /// <summary>
    /// Numeric validation constants for counts, quantities, and measurements.
    /// These values enforce business rules and prevent unrealistic or harmful inputs.
    /// </summary>
    public static class Numeric
    {
        /// <summary>
        /// Maximum number of members allowed in a team.
        /// Based on organizational management best practices and communication efficiency.
        /// Teams larger than this size typically require subdivision.
        /// </summary>
        public const int TeamMaxMembers = 50;

        /// <summary>
        /// Minimum number of members required for a team to be considered active.
        /// A team needs at least one member to perform work.
        /// </summary>
        public const int TeamMinMembers = 1;

        /// <summary>
        /// Maximum estimated hours allowed for a single task.
        /// Prevents unrealistic estimates and encourages task breakdown.
        /// Tasks requiring more effort should be divided into subtasks.
        /// </summary>
        public const int TaskMaxEstimatedHours = 1000;

        /// <summary>
        /// Minimum estimated hours for a task when estimation is provided.
        /// Ensures meaningful time estimates and prevents zero-effort tasks.
        /// </summary>
        public const int TaskMinEstimatedHours = 1;

        /// <summary>
        /// Maximum actual hours that can be logged for a single task.
        /// Prevents data entry errors and unrealistic time logging.
        /// </summary>
        public const int TaskMaxActualHours = 2000;

        /// <summary>
        /// Maximum number of subtasks allowed per parent task.
        /// Prevents excessive nesting and maintains manageable task hierarchies.
        /// </summary>
        public const int TaskMaxSubtasks = 20;

        /// <summary>
        /// Maximum number of tasks allowed per project.
        /// Prevents performance issues and encourages project decomposition.
        /// </summary>
        public const int ProjectMaxTasks = 1000;

        /// <summary>
        /// Maximum number of active projects per team.
        /// Based on team capacity and focus management principles.
        /// </summary>
        public const int TeamMaxActiveProjects = 10;

        /// <summary>
        /// Maximum priority value for tasks (lowest priority).
        /// Used for validation of custom priority systems.
        /// </summary>
        public const int TaskMaxPriorityValue = 10;

        /// <summary>
        /// Minimum priority value for tasks (highest priority).
        /// Used for validation of custom priority systems.
        /// </summary>
        public const int TaskMinPriorityValue = 1;
    }

    /// <summary>
    /// Date and time validation constants for temporal business rules.
    /// All dates use UTC time zone for consistency across global deployments.
    /// </summary>
    public static class DateTimes
    {
        /// <summary>
        /// Minimum allowed date for any date field in the system.
        /// Set to prevent historical data entry errors and Y2K-era date issues.
        /// Represents the approximate start of modern software project management.
        /// </summary>
        public static readonly System.DateTime MinAllowedDate = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Maximum allowed date for any date field in the system.
        /// Set to prevent far-future date entry errors and system overflow issues.
        /// Provides reasonable upper bound for project planning horizons.
        /// </summary>
        public static readonly System.DateTime MaxAllowedDate = new(2100, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        /// <summary>
        /// Maximum number of days in the future a task due date can be set.
        /// Prevents unrealistic planning horizons and encourages iterative planning.
        /// </summary>
        public const int TaskMaxFutureDays = 365 * 2; // 2 years

        /// <summary>
        /// Maximum number of days in the future a project end date can be set.
        /// Allows for longer-term project planning than individual tasks.
        /// </summary>
        public const int ProjectMaxFutureDays = 365 * 5; // 5 years

        /// <summary>
        /// Maximum number of days in the past a task can be marked as completed.
        /// Prevents historical data manipulation while allowing reasonable retroactive updates.
        /// </summary>
        public const int TaskMaxPastCompletionDays = 30;

        /// <summary>
        /// Number of minutes to add as buffer when validating "future" dates.
        /// Accounts for clock skew and processing time between client and server.
        /// </summary>
        public const int FutureDateBufferMinutes = 5;
    }

    /// <summary>
    /// Regular expression patterns for format validation.
    /// These patterns enforce consistent data formats across the domain.
    /// </summary>
    public static class Patterns
    {
        /// <summary>
        /// Regular expression pattern for basic email validation.
        /// This is a simplified pattern for basic format checking.
        /// For comprehensive validation, use the EmailAddress value object.
        /// Reference: RFC 5322 simplified pattern
        /// </summary>
        public const string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        /// <summary>
        /// Regular expression pattern for user names.
        /// Allows letters, numbers, spaces, hyphens, and apostrophes.
        /// Prevents special characters that could cause display or security issues.
        /// </summary>
        public const string UserNamePattern = @"^[a-zA-Z0-9\s\-']+$";

        /// <summary>
        /// Regular expression pattern for team and project names.
        /// Allows alphanumeric characters, spaces, hyphens, underscores, and parentheses.
        /// Suitable for organizational naming conventions.
        /// </summary>
        public const string NamePattern = @"^[a-zA-Z0-9\s\-_()]+$";

        /// <summary>
        /// Regular expression pattern for GUID validation.
        /// Ensures proper GUID format for strongly-typed IDs.
        /// </summary>
        public const string GuidPattern = @"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$";
    }

    /// <summary>
    /// File size and attachment validation constants.
    /// These limits prevent storage abuse and performance issues.
    /// </summary>
    public static class Files
    {
        /// <summary>
        /// Maximum file size in bytes for task attachments (10 MB).
        /// Balances functionality with storage and transfer performance.
        /// </summary>
        public const long MaxAttachmentSizeBytes = 10 * 1024 * 1024;

        /// <summary>
        /// Maximum number of attachments per task.
        /// Prevents excessive attachment accumulation and storage bloat.
        /// </summary>
        public const int MaxAttachmentsPerTask = 10;

        /// <summary>
        /// Allowed file extensions for attachments.
        /// Restricted to common business file types for security.
        /// </summary>
        public static readonly string[] AllowedAttachmentExtensions =
            [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
             ".txt", ".md", ".png", ".jpg", ".jpeg", ".gif", ".zip"];
    }

    /// <summary>
    /// Performance and system limit constants.
    /// These values prevent system overload and ensure reasonable response times.
    /// </summary>
    public static class Performance
    {
        /// <summary>
        /// Maximum number of items to return in a single page for pagination.
        /// Prevents memory issues and maintains acceptable response times.
        /// </summary>
        public const int MaxPageSize = 100;

        /// <summary>
        /// Default page size for paginated results when not specified.
        /// Balances user experience with performance.
        /// </summary>
        public const int DefaultPageSize = 20;

        /// <summary>
        /// Maximum number of concurrent operations per user session.
        /// Prevents abuse and ensures fair resource allocation.
        /// </summary>
        public const int MaxConcurrentOperations = 5;

        /// <summary>
        /// Timeout in seconds for long-running operations.
        /// Prevents hung operations from consuming system resources.
        /// </summary>
        public const int OperationTimeoutSeconds = 300; // 5 minutes

        /// <summary>
        /// Maximum search query length to prevent performance issues.
        /// Encourages specific searches and prevents query abuse.
        /// </summary>
        public const int MaxSearchQueryLength = 200;
    }
}
