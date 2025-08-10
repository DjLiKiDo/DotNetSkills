using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

namespace DotNetSkills.Infrastructure.Common.Configuration;

/// <summary>
/// Extension methods and helpers for configuration setup and validation.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds Azure Key Vault configuration provider in Production using a prefixed secret manager.
    /// Secrets must be named using the pattern: "DotNetSkills-Section--Key".
    /// </summary>
    public static IConfigurationBuilder AddAzureKeyVaultIfProduction(
        this IConfigurationBuilder configuration,
        IHostEnvironment environment)
    {
        if (!environment.IsProduction())
        {
            return configuration;
        }

        var temp = configuration.Build();
        var keyVaultUri = temp["KeyVault:Uri"];

        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            configuration.AddAzureKeyVault(client, new PrefixKeyVaultSecretManager("DotNetSkills"));
        }

        return configuration;
    }

    /// <summary>
    /// Forces options validation at startup for critical configuration.
    /// </summary>
    public static void ValidateConfiguration(this IServiceProvider serviceProvider)
    {
        // Trigger validation by accessing .Value
        _ = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    }
}

/// <summary>
/// Custom Key Vault secret manager to filter and transform secret names to configuration keys.
/// </summary>
public sealed class PrefixKeyVaultSecretManager : KeyVaultSecretManager
{
    private readonly string _prefix;

    public PrefixKeyVaultSecretManager(string prefix)
    {
        _prefix = prefix + "-";
    }

    public override bool Load(SecretProperties secret)
        => secret.Name.StartsWith(_prefix, StringComparison.OrdinalIgnoreCase);

    public override string GetKey(KeyVaultSecret secret)
    {
        var trimmed = secret.Name[_prefix.Length..];
        return trimmed.Replace("--", ":");
    }
}

/// <summary>
/// Centralized names for important Key Vault secrets.
/// </summary>
public static class KeyVaultSecretNames
{
    public const string DatabaseConnectionString = "DotNetSkills-Database--ConnectionString";
    public const string JwtSigningKey = "DotNetSkills-Jwt--SigningKey";
}
