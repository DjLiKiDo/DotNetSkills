using Microsoft.Extensions.Options;

namespace DotNetSkills.API.Configuration.Options.Validators;

public class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("JwtOptions cannot be null");

        if (!options.Enabled)
            return ValidateOptionsResult.Success; // Nothing to validate if disabled

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Issuer))
            failures.Add("JwtOptions.Issuer: Issuer is required when JWT is enabled");

        if (string.IsNullOrWhiteSpace(options.Audience))
            failures.Add("JwtOptions.Audience: Audience is required when JWT is enabled");

        if (string.IsNullOrWhiteSpace(options.SigningKey))
            failures.Add("JwtOptions.SigningKey: SigningKey is required when JWT is enabled (use Key Vault in production)");

        if (options.TokenLifetimeMinutes <= 0 || options.TokenLifetimeMinutes > 24 * 60)
            failures.Add("JwtOptions.TokenLifetimeMinutes: Must be between 1 and 1440 minutes");

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
