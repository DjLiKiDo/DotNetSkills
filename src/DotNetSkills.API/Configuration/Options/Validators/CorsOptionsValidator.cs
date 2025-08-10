using Microsoft.Extensions.Options;

namespace DotNetSkills.API.Configuration.Options.Validators;

public class CorsOptionsValidator : IValidateOptions<CorsOptions>
{
    public ValidateOptionsResult Validate(string? name, CorsOptions options)
    {
        if (options is null)
            return ValidateOptionsResult.Fail("CorsOptions cannot be null");

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.PolicyName))
            failures.Add("CorsOptions.PolicyName: PolicyName is required");

        if (options.AllowCredentials && (options.AllowedOrigins.Length == 0 || options.AllowedOrigins.Contains("*")))
            failures.Add("CorsOptions.AllowedOrigins: When AllowCredentials is true, specify explicit AllowedOrigins (no wildcard)");

        return failures.Count > 0 ? ValidateOptionsResult.Fail(failures) : ValidateOptionsResult.Success;
    }
}
