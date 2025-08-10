using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNetSkills.API.UnitTests.Configuration.Options;

public class CorsOptionsValidatorTests
{
    private static ValidateOptionsResult Validate(CorsOptions options)
    {
        var validator = new CorsOptionsValidator();
        return validator.Validate(nameof(CorsOptions), options);
    }

    [Fact(DisplayName = "Validate_DefaultOptions_Succeeds")]
    public void Validate_DefaultOptions_Succeeds()
    {
        // Arrange
        var options = new CorsOptions();

        // Act
        var result = Validate(options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact(DisplayName = "Validate_BlankPolicyName_Fails")]
    public void Validate_BlankPolicyName_Fails()
    {
        // Arrange
        var options = new CorsOptions { PolicyName = "" };

        // Act
        var result = Validate(options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("PolicyName"));
    }

    [Fact(DisplayName = "Validate_CredentialsWithWildcard_Fails")]
    public void Validate_CredentialsWithWildcard_Fails()
    {
        // Arrange
        var options = new CorsOptions
        {
            AllowCredentials = true,
            AllowedOrigins = new[] { "*" }
        };

        // Act
        var result = Validate(options);

        // Assert
        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("wildcard"));
    }

    [Fact(DisplayName = "Validate_CredentialsWithSpecificOrigins_Succeeds")]
    public void Validate_CredentialsWithSpecificOrigins_Succeeds()
    {
        // Arrange
        var options = new CorsOptions
        {
            AllowCredentials = true,
            AllowedOrigins = new[] { "https://app.example.com", "https://admin.example.com" }
        };

        // Act
        var result = Validate(options);

        // Assert
        result.Succeeded.Should().BeTrue();
    }
}
