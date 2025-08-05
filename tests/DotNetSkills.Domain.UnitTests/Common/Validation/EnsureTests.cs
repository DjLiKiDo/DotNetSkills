using FluentAssertions;
using Xunit;
using DotNetSkills.Domain.Common.Validation;
using DotNetSkills.Domain.Common.Exceptions;

namespace DotNetSkills.Domain.UnitTests.Common.Validation;

/// <summary>
/// Unit tests for the Ensure class to verify all validation methods work correctly.
/// These tests ensure consistent validation behavior across the domain layer.
/// </summary>
public class EnsureTests
{
    #region String Validation Tests

    [Fact]
    public void NotNullOrWhiteSpace_WithValidString_ShouldNotThrow()
    {
        // Arrange
        var validString = "Valid content";

        // Act & Assert
        var action = () => Ensure.NotNullOrWhiteSpace(validString, "testParam");
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void NotNullOrWhiteSpace_WithInvalidString_ShouldThrowArgumentException(string? invalidValue)
    {
        // Act & Assert
        var action = () => Ensure.NotNullOrWhiteSpace(invalidValue, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam cannot be null or whitespace*");
    }

    [Fact]
    public void NotNullOrWhiteSpace_WithCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange
        var customMessage = "Custom validation message";

        // Act & Assert
        var action = () => Ensure.NotNullOrWhiteSpace("", "testParam", customMessage);
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage($"{customMessage}*");
    }

    [Fact]
    public void HasMaxLength_WithValidLength_ShouldNotThrow()
    {
        // Arrange
        var validString = "Short";

        // Act & Assert
        var action = () => Ensure.HasMaxLength(validString, 10, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void HasMaxLength_WithExceedingLength_ShouldThrowArgumentException()
    {
        // Arrange
        var longString = "This is a very long string that exceeds the limit";

        // Act & Assert
        var action = () => Ensure.HasMaxLength(longString, 10, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam cannot exceed 10 characters*");
    }

    [Fact]
    public void HasMaxLength_WithNullString_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => Ensure.HasMaxLength(null, 10, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void HasMinLength_WithValidLength_ShouldNotThrow()
    {
        // Arrange
        var validString = "Valid string";

        // Act & Assert
        var action = () => Ensure.HasMinLength(validString, 5, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void HasMinLength_WithShortLength_ShouldThrowArgumentException()
    {
        // Arrange
        var shortString = "Hi";

        // Act & Assert
        var action = () => Ensure.HasMinLength(shortString, 5, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be at least 5 characters*");
    }

    #endregion

    #region Numeric Validation Tests

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void Positive_WithPositiveValue_ShouldNotThrow(int positiveValue)
    {
        // Act & Assert
        var action = () => Ensure.Positive(positiveValue, "testParam");
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void Positive_WithNonPositiveValue_ShouldThrowArgumentException(int nonPositiveValue)
    {
        // Act & Assert
        var action = () => Ensure.Positive(nonPositiveValue, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be positive*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(int.MaxValue)]
    public void PositiveOrZero_WithValidValue_ShouldNotThrow(int validValue)
    {
        // Act & Assert
        var action = () => Ensure.PositiveOrZero(validValue, "testParam");
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(int.MinValue)]
    public void PositiveOrZero_WithNegativeValue_ShouldThrowArgumentException(int negativeValue)
    {
        // Act & Assert
        var action = () => Ensure.PositiveOrZero(negativeValue, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be positive or zero*");
    }

    [Theory]
    [InlineData(5, 1, 10)]
    [InlineData(1, 1, 10)]
    [InlineData(10, 1, 10)]
    public void InRange_WithValueInRange_ShouldNotThrow(int value, int min, int max)
    {
        // Act & Assert
        var action = () => Ensure.InRange(value, min, max, "testParam");
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData(0, 1, 10)]
    [InlineData(11, 1, 10)]
    [InlineData(-5, 1, 10)]
    public void InRange_WithValueOutOfRange_ShouldThrowArgumentException(int value, int min, int max)
    {
        // Act & Assert
        var action = () => Ensure.InRange(value, min, max, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage($"*testParam must be between {min} and {max}*");
    }

    #endregion

    #region DateTime Validation Tests

    [Fact]
    public void FutureDate_WithFutureDate_ShouldNotThrow()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        var action = () => Ensure.FutureDate(futureDate, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void FutureDate_WithPastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var action = () => Ensure.FutureDate(pastDate, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be in the future*");
    }

    [Fact]
    public void FutureDate_WithCurrentTime_ShouldThrowArgumentException()
    {
        // Arrange
        var currentTime = DateTime.UtcNow;

        // Act & Assert
        var action = () => Ensure.FutureDate(currentTime, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be in the future*");
    }

    [Fact]
    public void FutureDateOrNull_WithNullValue_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => Ensure.FutureDateOrNull(null, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void FutureDateOrNull_WithFutureDate_ShouldNotThrow()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        var action = () => Ensure.FutureDateOrNull(futureDate, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void FutureDateOrNull_WithPastDate_ShouldThrowArgumentException()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var action = () => Ensure.FutureDateOrNull(pastDate, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be in the future*");
    }

    [Fact]
    public void PastDate_WithPastDate_ShouldNotThrow()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        var action = () => Ensure.PastDate(pastDate, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void PastDate_WithFutureDate_ShouldThrowArgumentException()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(1);

        // Act & Assert
        var action = () => Ensure.PastDate(futureDate, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam must be in the past*");
    }

    #endregion

    #region Object Validation Tests

    [Fact]
    public void NotNull_WithValidObject_ShouldNotThrow()
    {
        // Arrange
        var validObject = new object();

        // Act & Assert
        var action = () => Ensure.NotNull(validObject, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void NotNull_WithNullObject_ShouldThrowArgumentNullException()
    {
        // Arrange
        object? nullObject = null;

        // Act & Assert
        var action = () => Ensure.NotNull(nullObject, "testParam");
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("testParam");
    }

    [Fact]
    public void NotNull_WithCustomMessage_ShouldUseCustomMessage()
    {
        // Arrange
        object? nullObject = null;
        var customMessage = "Custom null validation message";

        // Act & Assert
        var action = () => Ensure.NotNull(nullObject, "testParam", customMessage);
        action.Should().Throw<ArgumentNullException>()
              .WithParameterName("testParam")
              .WithMessage($"{customMessage}*");
    }

    #endregion

    #region Business Rule Validation Tests

    [Fact]
    public void BusinessRule_WithTrueCondition_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => Ensure.BusinessRule(true, "This should not throw");
        action.Should().NotThrow();
    }

    [Fact]
    public void BusinessRule_WithFalseCondition_ShouldThrowDomainException()
    {
        // Arrange
        var errorMessage = "Business rule violation";

        // Act & Assert
        var action = () => Ensure.BusinessRule(false, errorMessage);
        action.Should().Throw<DomainException>()
              .WithMessage(errorMessage);
    }

    [Fact]
    public void BusinessRule_WithFunctionReturningTrue_ShouldNotThrow()
    {
        // Act & Assert
        var action = () => Ensure.BusinessRule(() => true, "This should not throw");
        action.Should().NotThrow();
    }

    [Fact]
    public void BusinessRule_WithFunctionReturningFalse_ShouldThrowDomainException()
    {
        // Arrange
        var errorMessage = "Business rule violation from function";

        // Act & Assert
        var action = () => Ensure.BusinessRule(() => false, errorMessage);
        action.Should().Throw<DomainException>()
              .WithMessage(errorMessage);
    }

    [Fact]
    public void BusinessRule_WithNullFunction_ShouldThrowArgumentNullException()
    {
        // Arrange
        Func<bool>? nullFunction = null;

        // Act & Assert
        var action = () => Ensure.BusinessRule(nullFunction!, "Message");
        action.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region Collection Validation Tests

    [Fact]
    public void NotEmpty_WithNonEmptyCollection_ShouldNotThrow()
    {
        // Arrange
        var collection = new List<string> { "item1", "item2" };

        // Act & Assert
        var action = () => Ensure.NotEmpty(collection, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void NotEmpty_WithEmptyCollection_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyCollection = new List<string>();

        // Act & Assert
        var action = () => Ensure.NotEmpty(emptyCollection, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam cannot be empty*");
    }

    [Fact]
    public void NotEmpty_WithNullCollection_ShouldThrowArgumentException()
    {
        // Arrange
        List<string>? nullCollection = null;

        // Act & Assert
        var action = () => Ensure.NotEmpty(nullCollection, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam cannot be empty*");
    }

    [Fact]
    public void MaxCount_WithCollectionBelowLimit_ShouldNotThrow()
    {
        // Arrange
        var collection = new List<string> { "item1", "item2" };

        // Act & Assert
        var action = () => Ensure.MaxCount(collection, 5, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void MaxCount_WithCollectionAtLimit_ShouldNotThrow()
    {
        // Arrange
        var collection = new List<string> { "item1", "item2", "item3" };

        // Act & Assert
        var action = () => Ensure.MaxCount(collection, 3, "testParam");
        action.Should().NotThrow();
    }

    [Fact]
    public void MaxCount_WithCollectionExceedingLimit_ShouldThrowArgumentException()
    {
        // Arrange
        var collection = new List<string> { "item1", "item2", "item3", "item4" };

        // Act & Assert
        var action = () => Ensure.MaxCount(collection, 2, "testParam");
        action.Should().Throw<ArgumentException>()
              .WithParameterName("testParam")
              .WithMessage("*testParam cannot have more than 2 items*");
    }

    [Fact]
    public void MaxCount_WithNullCollection_ShouldNotThrow()
    {
        // Arrange
        List<string>? nullCollection = null;

        // Act & Assert
        var action = () => Ensure.MaxCount(nullCollection, 5, "testParam");
        action.Should().NotThrow();
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Fact]
    public void ValidationMethods_WithWhitespaceParameterNames_ShouldHandleGracefully()
    {
        // Act & Assert - Should not throw for parameter name handling
        var action1 = () => Ensure.NotNullOrWhiteSpace("valid", " paramWithSpaces ");
        var action2 = () => Ensure.Positive(5, "param\twith\ttabs");

        action1.Should().NotThrow();
        action2.Should().NotThrow();
    }

    [Fact]
    public void ValidationMethods_WithEmptyParameterNames_ShouldHandleGracefully()
    {
        // Act & Assert - Should not throw for empty parameter names
        var action1 = () => Ensure.NotNullOrWhiteSpace("valid", "");
        var action2 = () => Ensure.Positive(5, "");

        action1.Should().NotThrow();
        action2.Should().NotThrow();
    }

    [Fact]
    public void BusinessRule_WithComplexCondition_ShouldEvaluateCorrectly()
    {
        // Arrange
        var user = new { Role = "Admin", IsActive = true };
        var team = new { MemberCount = 5, MaxMembers = 10 };

        // Act & Assert
        var action = () => Ensure.BusinessRule(
            user.Role == "Admin" && user.IsActive && team.MemberCount < team.MaxMembers,
            "Complex business rule validation failed");

        action.Should().NotThrow();
    }

    #endregion
}
