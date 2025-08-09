using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace DotNetSkills.API.UnitTests.Options;

public class SwaggerOptionsValidatorTests
{
    private readonly IValidateOptions<SwaggerOptions> _validator = new SwaggerOptionsValidator();

    [Fact]
    public void Validate_Disabled_Succeeds()
    {
        var options = new SwaggerOptions { Enabled = false };
        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EnabledMissingTitle_Fails()
    {
        var options = new SwaggerOptions { Enabled = true, Title = "", Version = "v1" };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Title is required");
    }

    [Fact]
    public void Validate_EnabledMissingVersion_Fails()
    {
        var options = new SwaggerOptions { Enabled = true, Title = "API", Version = "" };
        var result = _validator.Validate(string.Empty, options);
        result.Failed.Should().BeTrue();
        result.FailureMessage.Should().Contain("Version is required");
    }

    [Fact]
    public void Validate_EnabledValid_Succeeds()
    {
        var options = new SwaggerOptions { Enabled = true, Title = "DotNetSkills API", Version = "v1" };
        var result = _validator.Validate(string.Empty, options);
        result.Succeeded.Should().BeTrue();
    }
}
