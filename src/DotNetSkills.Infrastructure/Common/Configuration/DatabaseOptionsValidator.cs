using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Validator for database configuration options.
/// </summary>
public class DatabaseOptionsValidator : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("DatabaseOptions cannot be null");
        }

        List<string> failures = [];

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            failures.Add("DatabaseOptions.ConnectionString: ConnectionString is required");
        }

        if (options.CommandTimeout is < 1 or > 300)
        {
            failures.Add("DatabaseOptions.CommandTimeout: CommandTimeout must be between 1 and 300 seconds");
        }

        if (options.MaxRetryCount is < 0 or > 10)
        {
            failures.Add("DatabaseOptions.MaxRetryCount: MaxRetryCount must be between 0 and 10");
        }

        if (options.MaxRetryDelaySeconds is < 1 or > 60)
        {
            failures.Add("DatabaseOptions.MaxRetryDelaySeconds: MaxRetryDelaySeconds must be between 1 and 60 seconds");
        }

        return failures.Count > 0 
            ? ValidateOptionsResult.Fail(failures) 
            : ValidateOptionsResult.Success;
    }
}
