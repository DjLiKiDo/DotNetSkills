namespace DotNetSkills.Domain.UnitTests.UserManagement.ValueObjects;

[Trait("Category", "Unit")]
[Trait("BoundedContext", "UserManagement")]
public class EmailAddressTests : TestBase
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user@domain.org")]
    [InlineData("admin@company.co.uk")]
    [InlineData("firstname.lastname@company.com")]
    [InlineData("user+tag@example.com")]
    [InlineData("123@example.com")]
    [Trait("TestType", "Creation")]
    public void Constructor_WithValidEmail_ShouldCreateEmailAddress(string validEmail)
    {
        // Act
        var emailAddress = new EmailAddress(validEmail);

        // Assert
        emailAddress.Value.Should().Be(validEmail.ToLowerInvariant().Trim());
    }

    [Theory]
    [InlineData("TEST@EXAMPLE.COM", "test@example.com")]
    [InlineData("User@Domain.ORG", "user@domain.org")]
    [Trait("TestType", "Creation")]
    public void Constructor_ShouldNormalizeEmail(string input, string expected)
    {
        // Act
        var emailAddress = new EmailAddress(input);

        // Assert
        emailAddress.Value.Should().Be(expected);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithLeadingTrailingWhitespace_ShouldThrowDomainException()
    {
        // Arrange
        var emailWithWhitespace = "  admin@company.com  ";

        // Act & Assert
        AssertDomainException(() => new EmailAddress(emailWithWhitespace), ValidationMessages.ValueObjects.InvalidEmailFormat);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullOrWhitespace_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        AssertArgumentException(() => new EmailAddress(invalidEmail), "value");
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithNullEmail_ShouldThrowArgumentException()
    {
        // Act & Assert
        AssertArgumentException(() => new EmailAddress(null!), "value");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    [InlineData("user@.com")]
    [InlineData("user@domain.")]
    [InlineData("user name@example.com")]
    [InlineData("user@exam ple.com")]
    [Trait("TestType", "Validation")]
    public void Constructor_WithInvalidEmailFormat_ShouldThrowDomainException(string invalidEmail)
    {
        // Act & Assert
        AssertDomainException(() => new EmailAddress(invalidEmail), ValidationMessages.ValueObjects.InvalidEmailFormat);
    }

    [Fact]
    [Trait("TestType", "Validation")]
    public void Constructor_WithTooLongEmail_ShouldThrowArgumentException()
    {
        // Arrange
        var longLocalPart = new string('a', 250); // Exceeds typical email length limits
        var longEmail = $"{longLocalPart}@example.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => new EmailAddress(longEmail));
        exception.Message.Should().Contain("cannot exceed");
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ImplicitOperator_ShouldConvertToString()
    {
        // Arrange
        var emailValue = "test@example.com";
        var emailAddress = new EmailAddress(emailValue);

        // Act
        string result = emailAddress;

        // Assert
        result.Should().Be(emailValue);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var emailValue = "test@example.com";
        var emailAddress = new EmailAddress(emailValue);

        // Act
        var result = emailAddress.ToString();

        // Assert
        result.Should().Be(emailValue);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var emailValue = "test@example.com";
        var emailAddress1 = new EmailAddress(emailValue);
        var emailAddress2 = new EmailAddress(emailValue);

        // Act & Assert
        emailAddress1.Should().Be(emailAddress2);
        emailAddress1.GetHashCode().Should().Be(emailAddress2.GetHashCode());
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var emailAddress1 = new EmailAddress("test1@example.com");
        var emailAddress2 = new EmailAddress("test2@example.com");

        // Act & Assert
        emailAddress1.Should().NotBe(emailAddress2);
    }

    [Fact]
    [Trait("TestType", "BusinessLogic")]
    public void Equality_WithDifferentCasing_ShouldBeEqual()
    {
        // Arrange
        var emailAddress1 = new EmailAddress("TEST@EXAMPLE.COM");
        var emailAddress2 = new EmailAddress("test@example.com");

        // Act & Assert
        emailAddress1.Should().Be(emailAddress2);
    }

    [Fact]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithMinimalValidEmail_ShouldWork()
    {
        // Arrange
        var minimalEmail = "a@b.c";

        // Act
        var emailAddress = new EmailAddress(minimalEmail);

        // Assert
        emailAddress.Value.Should().Be(minimalEmail);
    }

    [Theory]
    [InlineData("user+tag@example.com")]
    [InlineData("user.name@example.com")]
    [InlineData("user-name@example.com")]
    [InlineData("user_name@example.com")]
    [Trait("TestType", "EdgeCase")]
    public void Constructor_WithSpecialCharacters_ShouldWork(string emailWithSpecialChars)
    {
        // Act
        var emailAddress = new EmailAddress(emailWithSpecialChars);

        // Assert
        emailAddress.Value.Should().Be(emailWithSpecialChars.ToLowerInvariant());
    }
}