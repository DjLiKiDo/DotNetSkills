using System.Text.RegularExpressions;

namespace DotNetSkills.Domain.UserManagement.ValueObjects;

/// <summary>
/// Represents a valid email address value object with built-in validation.
/// </summary>
public record EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the email address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the EmailAddress class.
    /// </summary>
    /// <param name="value">The email address string.</param>
    /// <exception cref="ArgumentException">Thrown when the email address is invalid.</exception>
    public EmailAddress(string value)
    {
        Ensure.NotNullOrWhiteSpace(value, nameof(value));
        Ensure.HasMaxLength(value, ValidationConstants.StringLengths.EmailMaxLength, nameof(value));
        Ensure.BusinessRule(IsValidEmail(value), ValidationMessages.ValueObjects.InvalidEmailFormat);

        Value = value.ToLowerInvariant().Trim();
    }

    /// <summary>
    /// Validates if the provided string is a valid email address.
    /// </summary>
    /// <param name="email">The email string to validate.</param>
    /// <returns>True if the email is valid, false otherwise.</returns>
    private static bool IsValidEmail(string email)
    {

        return EmailRegex.IsMatch(email);
    }

    /// <summary>
    /// Implicitly converts an EmailAddress to its string value.
    /// </summary>
    /// <param name="email">The EmailAddress to convert.</param>
    public static implicit operator string(EmailAddress email) => email.Value;

    /// <summary>
    /// Returns the string representation of the email address.
    /// </summary>
    /// <returns>The email address value.</returns>
    public override string ToString() => Value;
}
