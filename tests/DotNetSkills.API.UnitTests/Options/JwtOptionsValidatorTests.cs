using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DotNetSkills.API.UnitTests.Options;

public class JwtOptionsValidatorTests
{
    private readonly IValidateOptions<JwtOptions> _validator = new JwtOptionsValidator();

    [Fact]
    public void Validate_Disabled_Succeeds()
    {
        var options = new JwtOptions { Enabled = false };
        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EnabledMissingIssuerAudienceSigningKey_Fails()
    {
        var options = new JwtOptions { Enabled = true, Issuer = "", Audience = "", SigningKey = "" };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Issuer is required");
        result.FailureMessage.Should().Contain("Audience is required");
        result.FailureMessage.Should().Contain("SigningKey is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1441)]
    public void Validate_InvalidTokenLifetime_Fails(int minutes)
    {
        var options = new JwtOptions { Enabled = true, Issuer = "i", Audience = "a", SigningKey = "k", TokenLifetimeMinutes = minutes };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("TokenLifetimeMinutes");
    }

    [Fact]
    public void Validate_Valid_Succeeds()
    {
        var options = new JwtOptions { Enabled = true, Issuer = "i", Audience = "a", SigningKey = "k", TokenLifetimeMinutes = 60 };
        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }
}
