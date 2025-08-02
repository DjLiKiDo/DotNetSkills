using FluentAssertions.Collections;
using FluentAssertions.Primitives;

namespace DotNetSkills.Domain.UnitTests.Common;

/// <summary>
/// Custom FluentAssertions extensions for domain-specific validations.
/// </summary>
public static class FluentAssertionsExtensions
{
    /// <summary>
    /// Asserts that a domain event occurred at a specific time.
    /// </summary>
    /// <param name="assertions">The domain event assertions.</param>
    /// <param name="expectedTime">The expected occurrence time.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Arguments for the reason.</param>
    public static AndConstraint<ObjectAssertions> HaveOccurredAt(
        this ObjectAssertions assertions,
        DateTime expectedTime,
        string because = "",
        params object[] becauseArgs)
    {
        if (assertions.Subject is BaseDomainEvent domainEvent)
        {
            domainEvent.OccurredAt.Should().Be(expectedTime, because, becauseArgs);
        }
        else
        {
            throw new InvalidOperationException("Object is not a domain event");
        }

        return new AndConstraint<ObjectAssertions>(assertions);
    }

    /// <summary>
    /// Asserts that a collection contains exactly one item of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to check for.</typeparam>
    /// <param name="assertions">The collection assertions.</param>
    /// <param name="because">The reason for the assertion.</param>
    /// <param name="becauseArgs">Arguments for the reason.</param>
    /// <returns>The single item of the specified type.</returns>
    public static T ContainSingle<T>(
        this GenericCollectionAssertions<object> assertions,
        string because = "",
        params object[] becauseArgs)
    {
        var items = assertions.Subject.OfType<T>().ToList();
        items.Should().HaveCount(1, because, becauseArgs);
        return items.Single();
    }
}