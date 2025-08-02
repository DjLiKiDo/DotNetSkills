namespace DotNetSkills.Domain.UnitTests.Common;

/// <summary>
/// Base class for all domain unit tests providing common test infrastructure.
/// </summary>
public abstract class TestBase
{
    /// <summary>
    /// Gets a test clock for deterministic time testing.
    /// </summary>
    protected TestClock TestClock { get; } = new();

    /// <summary>
    /// Gets the domain event test helper for verifying domain events.
    /// </summary>
    protected DomainEventTestHelper DomainEventHelper { get; } = new();

    /// <summary>
    /// Asserts that a domain exception is thrown with the expected message.
    /// </summary>
    /// <param name="action">The action that should throw the exception.</param>
    /// <param name="expectedMessage">The expected exception message.</param>
    protected static void AssertDomainException(Action action, string expectedMessage)
    {
        var exception = Assert.Throws<DomainException>(action);
        exception.Message.Should().Be(expectedMessage);
    }

    /// <summary>
    /// Asserts that an argument exception is thrown with the expected parameter name.
    /// </summary>
    /// <param name="action">The action that should throw the exception.</param>
    /// <param name="expectedParamName">The expected parameter name.</param>
    protected static void AssertArgumentException(Action action, string expectedParamName)
    {
        var exception = Assert.Throws<ArgumentException>(action);
        exception.ParamName.Should().Be(expectedParamName);
    }

    /// <summary>
    /// Asserts that an argument null exception is thrown with the expected parameter name.
    /// </summary>
    /// <param name="action">The action that should throw the exception.</param>
    /// <param name="expectedParamName">The expected parameter name.</param>
    protected static void AssertArgumentNullException(Action action, string expectedParamName)
    {
        var exception = Assert.Throws<ArgumentNullException>(action);
        exception.ParamName.Should().Be(expectedParamName);
    }
}