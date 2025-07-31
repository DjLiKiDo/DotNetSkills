namespace DotNetSkills.Domain.Common;

/// <summary>
/// Contains all domain-level constants and validation limits.
/// Centralizes business rules and RFC compliance requirements.
/// </summary>
public static class DomainConstants
{
    public static class User
    {
        /// <summary>
        /// Maximum length for user first name.
        /// Based on common database field lengths and UI constraints.
        /// </summary>
        public const int MaxFirstNameLength = 50;
        
        /// <summary>
        /// Maximum length for user last name.
        /// Based on common database field lengths and UI constraints.
        /// </summary>
        public const int MaxLastNameLength = 50;
    }
    
    public static class Email
    {
        /// <summary>
        /// Maximum email address length according to RFC 5321.
        /// The maximum total length of an email address is 254 characters.
        /// </summary>
        public const int MaxEmailLength = 254; // RFC 5321
    }
    
    public static class Team
    {
        /// <summary>
        /// Minimum length for team name.
        /// Ensures meaningful team identification.
        /// </summary>
        public const int MinNameLength = 2;
        
        /// <summary>
        /// Maximum length for team name.
        /// Based on display constraints and database field lengths.
        /// </summary>
        public const int MaxNameLength = 100;
    }
    
    public static class Project
    {
        /// <summary>
        /// Minimum length for project name.
        /// Ensures meaningful project identification.
        /// </summary>
        public const int MinNameLength = 2;
        
        /// <summary>
        /// Maximum length for project name.
        /// Based on display constraints and database field lengths.
        /// </summary>
        public const int MaxNameLength = 200;
    }
    
    public static class Task
    {
        /// <summary>
        /// Minimum length for task title.
        /// Ensures meaningful task identification.
        /// </summary>
        public const int MinTitleLength = 3;
        
        /// <summary>
        /// Maximum length for task title.
        /// Based on display constraints and readability.
        /// </summary>
        public const int MaxTitleLength = 200;
        
        /// <summary>
        /// Minimum value for estimated or actual hours.
        /// Business rule: time tracking cannot be negative.
        /// </summary>
        public const decimal MinHours = 0;
    }
}