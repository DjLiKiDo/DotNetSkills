using DotNetSkills.Infrastructure.Security;
using FluentAssertions;

namespace DotNetSkills.Infrastructure.UnitTests.Security;

public class SecretsRotationOptionsValidatorTests
{
    [Fact]
    public void Validate_WithTooSmallInterval_ShouldFail()
    {
        var options = new SecretsRotationOptions
        {
            JwtKeyRotationInterval = TimeSpan.FromMinutes(5),
            AutomaticRotationEnabled = true
        };
        var validator = new SecretsRotationOptionsValidator();
        var result = validator.Validate(options);
        result.IsValid.Should().BeFalse();
        result.Errors.Any(e => e.ErrorMessage.Contains("at least")).Should().BeTrue();
    }

    [Fact]
    public void Validate_WithTooLargeInterval_ShouldFail()
    {
        var options = new SecretsRotationOptions
        {
            JwtKeyRotationInterval = TimeSpan.FromDays(60),
            AutomaticRotationEnabled = true
        };
        var validator = new SecretsRotationOptionsValidator();
        var result = validator.Validate(options);
        result.IsValid.Should().BeFalse();
        result.Errors.Any(e => e.ErrorMessage.Contains("not exceed")).Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidInterval_ShouldPass()
    {
        var options = new SecretsRotationOptions
        {
            JwtKeyRotationInterval = TimeSpan.FromDays(10),
            AutomaticRotationEnabled = true
        };
        var validator = new SecretsRotationOptionsValidator();
        var result = validator.Validate(options);
        result.IsValid.Should().BeTrue();
    }
}
