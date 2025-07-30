using System.Text.RegularExpressions;
using DotNetSkills.Domain.Common;

namespace DotNetSkills.Domain.ValueObjects;

public readonly record struct EmailAddress
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(100));

    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email address cannot be null or empty.");

        var normalizedValue = value.Trim().ToLowerInvariant();
        
        if (!EmailRegex.IsMatch(normalizedValue))
            throw new DomainException("Invalid email address format.");

        if (normalizedValue.Length > 254) // RFC 5321 limit
            throw new DomainException("Email address cannot exceed 254 characters.");

        Value = normalizedValue;
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;
    public static explicit operator EmailAddress(string email) => new(email);
}