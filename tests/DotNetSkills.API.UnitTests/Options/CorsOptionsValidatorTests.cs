using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DotNetSkills.API.UnitTests.Options;

public class CorsOptionsValidatorTests
{
    private readonly IValidateOptions<CorsOptions> _validator = new CorsOptionsValidator();

    [Fact]
    public void Validate_MissingPolicyName_Fails()
    {
        var options = new CorsOptions { PolicyName = "" };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("PolicyName is required");
    }

    [Fact]
    public void Validate_AllowCredentialsWithWildcard_Fails()
    {
        var options = new CorsOptions { PolicyName = "Default", AllowCredentials = true, AllowedOrigins = new[] { "*" } };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("AllowedOrigins");
    }

    [Fact]
    public void Validate_Valid_Succeeds()
    {
        var options = new CorsOptions { PolicyName = "Default", AllowCredentials = false, AllowedOrigins = Array.Empty<string>() };
        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }
}
