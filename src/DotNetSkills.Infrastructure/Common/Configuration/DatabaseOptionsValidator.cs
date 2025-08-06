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

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ConnectionString))
        {
            failures.Add("ConnectionString is required and cannot be empty");
        }

        if (options.CommandTimeout <= 0)
        {
            failures.Add("CommandTimeout must be greater than 0");
        }

        if (options.MaxRetryCount < 0)
        {
            failures.Add("MaxRetryCount must be 0 or greater");
        }

        if (options.MaxRetryDelaySeconds <= 0)
        {
            failures.Add("MaxRetryDelaySeconds must be greater than 0");
        }

        return failures.Count > 0 
            ? ValidateOptionsResult.Fail(failures) 
            : ValidateOptionsResult.Success;
    }
}
