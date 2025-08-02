namespace DotNetSkills.Domain.Common.Validation;

/// <summary>
/// Provides centralized validation methods to ensure consistent validation patterns across domain entities.
/// This class follows Domain-Driven Design principles by providing reusable validation logic
/// while maintaining separation between input validation and business rule validation.
/// </summary>
/// <remarks>
/// <para>
/// The Ensure class serves two primary purposes:
/// 1. Parameter validation using ArgumentException for null, empty, or invalid input parameters
/// 2. Business rule validation using DomainException for domain-specific constraint violations
/// </para>
/// <para>
/// Usage Guidelines:
/// - Use ArgumentException for input validation (null checks, format validation, etc.)
/// - Use DomainException for business rule violations (domain constraints, state validation, etc.)
/// - All validation methods are designed to be fast-fail and provide clear error messages
/// - Custom messages can be provided to override default error messages when needed
/// </para>
/// </remarks>
public static class Ensure
{
    #region String Validation

    /// <summary>
    /// Ensures that a string value is not null, empty, or consists only of whitespace characters.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message. If not provided, a standard message will be used.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null, empty, or whitespace.</exception>
    /// <example>
    /// <code>
    /// Ensure.NotNullOrWhiteSpace(userName, nameof(userName));
    /// Ensure.NotNullOrWhiteSpace(title, nameof(title), "Task title is required");
    /// </code>
    /// </example>
    public static void NotNullOrWhiteSpace(string? value, string paramName, string? customMessage = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.CannotBeEmpty, paramName),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that a string value does not exceed the specified maximum length.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value exceeds the maximum length.</exception>
    public static void HasMaxLength(string? value, int maxLength, string paramName, string? customMessage = null)
    {
        if (value != null && value.Length > maxLength)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.ExceedsMaxLength, paramName, maxLength.ToString()),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that a string value meets the specified minimum length requirement.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="minLength">The minimum required length.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is shorter than the minimum length.</exception>
    public static void HasMinLength(string? value, int minLength, string paramName, string? customMessage = null)
    {
        if (value != null && value.Length < minLength)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.BelowMinLength, paramName, minLength.ToString()),
                paramName);
        }
    }

    #endregion

    #region Numeric Validation

    /// <summary>
    /// Ensures that an integer value is positive (greater than zero).
    /// </summary>
    /// <param name="value">The integer value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static void Positive(int value, string paramName, string? customMessage = null)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositive, paramName),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that an integer value is positive or zero (greater than or equal to zero).
    /// </summary>
    /// <param name="value">The integer value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is negative.</exception>
    public static void PositiveOrZero(int value, string paramName, string? customMessage = null)
    {
        if (value < 0)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePositiveOrZero, paramName),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that an integer value falls within the specified inclusive range.
    /// </summary>
    /// <param name="value">The integer value to validate.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is outside the specified range.</exception>
    public static void InRange(int value, int min, int max, string paramName, string? customMessage = null)
    {
        if (value < min || value > max)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeInRange, paramName, min.ToString(), max.ToString()),
                paramName);
        }
    }

    #endregion

    #region DateTime Validation

    /// <summary>
    /// Ensures that a DateTime value represents a future date (after the current UTC time).
    /// </summary>
    /// <param name="value">The DateTime value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not in the future.</exception>
    public static void FutureDate(DateTime value, string paramName, string? customMessage = null)
    {
        var now = DateTime.UtcNow.AddMinutes(ValidationConstants.DateTimes.FutureDateBufferMinutes);
        if (value <= now)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBeFutureDate, paramName),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that a nullable DateTime value, if provided, represents a future date.
    /// </summary>
    /// <param name="value">The nullable DateTime value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is provided but not in the future.</exception>
    public static void FutureDateOrNull(DateTime? value, string paramName, string? customMessage = null)
    {
        if (value.HasValue)
        {
            FutureDate(value.Value, paramName, customMessage);
        }
    }

    /// <summary>
    /// Ensures that a DateTime value represents a past date (before the current UTC time).
    /// </summary>
    /// <param name="value">The DateTime value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not in the past.</exception>
    public static void PastDate(DateTime value, string paramName, string? customMessage = null)
    {
        if (value >= DateTime.UtcNow)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.MustBePastDate, paramName),
                paramName);
        }
    }

    #endregion

    #region Object Validation

    /// <summary>
    /// Ensures that an object reference is not null.
    /// </summary>
    /// <typeparam name="T">The type of the object being validated.</typeparam>
    /// <param name="value">The object reference to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public static void NotNull<T>(T? value, string paramName, string? customMessage = null) where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName, customMessage ?? $"{paramName} cannot be null");
        }
    }

    #endregion

    #region Business Rule Validation

    /// <summary>
    /// Ensures that a business rule condition is satisfied.
    /// This method is used for domain-specific validation that goes beyond simple parameter checking.
    /// </summary>
    /// <param name="condition">The business rule condition that must be true.</param>
    /// <param name="message">The error message to use if the condition is false.</param>
    /// <exception cref="DomainException">Thrown when the business rule condition is false.</exception>
    /// <example>
    /// <code>
    /// Ensure.BusinessRule(
    ///     user.Role == UserRole.Admin,
    ///     ValidationMessages.User.OnlyAdminCanCreate);
    /// 
    /// Ensure.BusinessRule(
    ///     _teamMemberships.Count &lt; ValidationConstants.Numeric.TeamMaxMembers,
    ///     ValidationMessages.Team.MaxMembersExceeded);
    /// </code>
    /// </example>
    public static void BusinessRule(bool condition, string message)
    {
        if (!condition)
        {
            throw new DomainException(message);
        }
    }

    /// <summary>
    /// Ensures that a business rule condition evaluated by a function is satisfied.
    /// This overload is useful for complex conditions or when evaluation is expensive.
    /// </summary>
    /// <param name="condition">A function that evaluates the business rule condition.</param>
    /// <param name="message">The error message to use if the condition is false.</param>
    /// <exception cref="DomainException">Thrown when the business rule condition is false.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the condition function is null.</exception>
    public static void BusinessRule(Func<bool> condition, string message)
    {
        ArgumentNullException.ThrowIfNull(condition);
        
        if (!condition())
        {
            throw new DomainException(message);
        }
    }

    #endregion

    #region Collection Validation

    /// <summary>
    /// Ensures that a collection is not null and contains at least one element.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the collection is null or empty.</exception>
    public static void NotEmpty<T>(IEnumerable<T>? collection, string paramName, string? customMessage = null)
    {
        if (collection is null || !collection.Any())
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.CollectionCannotBeEmpty, paramName),
                paramName);
        }
    }

    /// <summary>
    /// Ensures that a collection does not exceed the specified maximum count.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to validate.</param>
    /// <param name="maxCount">The maximum allowed number of elements.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <param name="customMessage">Optional custom error message.</param>
    /// <exception cref="ArgumentException">Thrown when the collection exceeds the maximum count.</exception>
    public static void MaxCount<T>(IEnumerable<T>? collection, int maxCount, string paramName, string? customMessage = null)
    {
        if (collection != null && collection.Count() > maxCount)
        {
            throw new ArgumentException(
                customMessage ?? ValidationMessages.Formatting.Format(ValidationMessages.Common.ExceedsMaxCount, paramName, maxCount.ToString()),
                paramName);
        }
    }

    #endregion
}
