using Microsoft.Extensions.Options;

namespace DotNetSkills.API.Configuration.Options.Validators;

/// <summary>
/// Validator for Swagger configuration options.
/// Enforces Title and Version when Swagger is enabled.
/// </summary>
public sealed class SwaggerOptionsValidator : IValidateOptions<SwaggerOptions>
{
    public ValidateOptionsResult Validate(string? name, SwaggerOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("SwaggerOptions cannot be null");

        if (!options.Enabled)
            return ValidateOptionsResult.Success; // Skip when disabled

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Title))
            failures.Add("SwaggerOptions.Title: Title is required when Swagger is enabled");

        if (string.IsNullOrWhiteSpace(options.Version))
            failures.Add("SwaggerOptions.Version: Version is required when Swagger is enabled");

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
