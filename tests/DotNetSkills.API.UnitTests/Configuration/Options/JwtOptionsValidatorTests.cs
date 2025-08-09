using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetSkills.API.UnitTests.Configuration.Options;

public class JwtOptionsValidatorTests
{
    private static ValidateOptionsResult Validate(JwtOptions options)
    {
        var validator = new JwtOptionsValidator();
        return validator.Validate(nameof(JwtOptions), options);
    }

    [Fact(DisplayName = "Validate_Disabled_Succeeds")]
    public void Validate_Disabled_Succeeds()
    {
        // Arrange
        var options = new JwtOptions { Enabled = false };

        // Act
        var result = Validate(options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact(DisplayName = "Validate_EnabledMissingFields_Fails")]
    public void Validate_EnabledMissingFields_Fails()
    {
        // Arrange
        var options = new JwtOptions { Enabled = true, Issuer = null!, Audience = string.Empty, SigningKey = "" };

        // Act
        var result = Validate(options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Validate_AllRequiredProvided_Succeeds")]
    public void Validate_AllRequiredProvided_Succeeds()
    {
        // Arrange
        var options = new JwtOptions
        {
            Enabled = true,
            Issuer = "issuer",
            Audience = "audience",
            SigningKey = new string('x', 32) // typical min length
        };

        // Act
        var result = Validate(options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
}
