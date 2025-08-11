using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.Security;

/// <summary>
/// Service for rotating secrets in Azure Key Vault.
/// Implements automatic rotation of JWT signing keys and other sensitive credentials.
/// </summary>
public interface ISecretsRotationService
{
    /// <summary>
    /// Rotates the JWT signing key in Azure Key Vault.
    /// </summary>
    System.Threading.Tasks.Task RotateJwtSigningKeyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a secret needs rotation based on its age.
    /// </summary>
    System.Threading.Tasks.Task<bool> IsSecretRotationRequiredAsync(string secretName, TimeSpan maxAge, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the next scheduled rotation date for a secret.
    /// </summary>
    System.Threading.Tasks.Task<DateTime?> GetNextRotationDateAsync(string secretName, CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of secrets rotation service using Azure Key Vault.
/// </summary>
public sealed class SecretsRotationService : ISecretsRotationService
{
    private readonly ISecretStore? _secretStore;
    private readonly ILogger<SecretsRotationService> _logger;
    private readonly SecretsRotationOptions _options;

    public SecretsRotationService(
        IOptions<SecretsRotationOptions> options,
        ILogger<SecretsRotationService> logger,
        IConfiguration configuration,
        ISecretStore? secretStore = null)
    {
        _options = options.Value;
        _logger = logger;
        if (secretStore is not null)
        {
            _secretStore = secretStore;
        }
        else
        {
            var keyVaultUri = configuration["KeyVault:Uri"];
            if (!string.IsNullOrWhiteSpace(keyVaultUri))
            {
                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
                _secretStore = new KeyVaultSecretStore(client);
            }
        }
    }

    /// <summary>
    /// Rotates the JWT signing key by generating a new secure key and updating Key Vault.
    /// </summary>
    public async System.Threading.Tasks.Task RotateJwtSigningKeyAsync(CancellationToken cancellationToken = default)
    {
    if (_secretStore == null)
        {
            _logger.LogWarning("Key Vault not configured. JWT signing key rotation skipped.");
            return;
        }

        try
        {
            _logger.LogInformation("Starting JWT signing key rotation...");
            // Try to fetch existing primary to archive it
            string primaryName = "DotNetSkills-Jwt--SigningKey";
            string? archivedSecretName = null;
            try
            {
                var existing = await _secretStore.TryGetSecretAsync(primaryName, cancellationToken);
                if (existing is not null)
                {
                    var archivedName = $"{primaryName}-archived-{DateTime.UtcNow:yyyyMMddHHmmss}";
                    archivedSecretName = archivedName;
                    var archiveSecret = new KeyVaultSecret(archivedName, existing.Value);
                    archiveSecret.Properties.Tags.Add("ArchivedFrom", primaryName);
                    archiveSecret.Properties.Tags.Add("ArchivedAt", DateTime.UtcNow.ToString("O"));
                    archiveSecret.Properties.ExpiresOn = DateTime.UtcNow.AddDays(7);
                    await _secretStore.SetSecretAsync(archiveSecret, cancellationToken);
                    _logger.LogInformation("Archived previous JWT signing key as {ArchivedName}", archivedName);
                }
                else
                {
                    _logger.LogInformation("No existing primary JWT key found to archive (first rotation)." );
                }
            }
            catch (Exception exArchive)
            {
                _logger.LogWarning(exArchive, "Failed to archive previous JWT signing key; proceeding with rotation.");
            }

            // Generate new secure signing key (256 bits for HS256)
            var newSigningKey = GenerateSecureSigningKey();

            // Create (replace) primary secret with rotation metadata
            var secret = new KeyVaultSecret(primaryName, newSigningKey);
            secret.Properties.Tags.Add("RotatedAt", DateTime.UtcNow.ToString("O"));
            secret.Properties.Tags.Add("RotatedBy", "SecretsRotationService");
            secret.Properties.Tags.Add("NextRotationDue", DateTime.UtcNow.Add(_options.JwtKeyRotationInterval).ToString("O"));
            if (archivedSecretName is not null)
            {
                secret.Properties.Tags.Add("PreviousKeyReference", archivedSecretName);
            }

            // Set expiration date for the new primary (+ grace period to allow fallback overlap if needed)
            secret.Properties.ExpiresOn = DateTime.UtcNow.Add(_options.JwtKeyRotationInterval).AddDays(7); // 7-day grace period

            await _secretStore.SetSecretAsync(secret, cancellationToken);

            _logger.LogInformation("JWT signing key rotated successfully. Next rotation due: {NextRotation}. Previous archived: {Archived}",
                DateTime.UtcNow.Add(_options.JwtKeyRotationInterval), archivedSecretName ?? "<none>");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rotate JWT signing key");
            throw;
        }
    }

    /// <summary>
    /// Checks if a secret needs rotation based on its age and configuration.
    /// </summary>
    public async System.Threading.Tasks.Task<bool> IsSecretRotationRequiredAsync(string secretName, TimeSpan maxAge, CancellationToken cancellationToken = default)
    {
    if (_secretStore == null)
        {
            return false;
        }

        try
        {
            var secret = await _secretStore.TryGetSecretAsync(secretName, cancellationToken);
            if (secret is null)
            {
                _logger.LogWarning("Secret {SecretName} not found in Key Vault", secretName);
                return true; // Secret doesn't exist, needs creation
            }

            if (secret.Properties.Tags.TryGetValue("RotatedAt", out var rotatedAtString) &&
                DateTime.TryParse(rotatedAtString, out var rotatedAt))
            {
                return DateTime.UtcNow - rotatedAt > maxAge;
            }

            // If no rotation timestamp, check creation date
            var createdOn = secret.Properties.CreatedOn;
            return createdOn.HasValue && DateTime.UtcNow - createdOn.Value > maxAge;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking rotation status for secret {SecretName}", secretName);
            return false;
        }
    }

    /// <summary>
    /// Gets the next scheduled rotation date for a secret.
    /// </summary>
    public async System.Threading.Tasks.Task<DateTime?> GetNextRotationDateAsync(string secretName, CancellationToken cancellationToken = default)
    {
    if (_secretStore == null)
        {
            return null;
        }

        try
        {
            var secret = await _secretStore.TryGetSecretAsync(secretName, cancellationToken);
            if (secret is null)
            {
                return null;
            }
            if (secret.Properties.Tags.TryGetValue("NextRotationDue", out var nextRotationString) &&
                DateTime.TryParse(nextRotationString, out var nextRotation))
            {
                return nextRotation;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next rotation date for secret {SecretName}", secretName);
            return null;
        }
    }

    /// <summary>
    /// Generates a cryptographically secure signing key for JWT tokens.
    /// </summary>
    private static string GenerateSecureSigningKey()
    {
        // Generate 32 bytes (256 bits) for HS256 algorithm
        var keyBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(keyBytes);
        
        return Convert.ToBase64String(keyBytes);
    }
}

/// <summary>
/// Configuration options for secrets rotation.
/// </summary>
public class SecretsRotationOptions
{
    /// <summary>
    /// Gets or sets the interval for JWT signing key rotation.
    /// Default is 90 days.
    /// </summary>
    public TimeSpan JwtKeyRotationInterval { get; set; } = TimeSpan.FromDays(90);

    /// <summary>
    /// Gets or sets whether automatic rotation is enabled.
    /// </summary>
    public bool AutomaticRotationEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the time of day to perform automatic rotations.
    /// Default is 2:00 AM UTC.
    /// </summary>
    public TimeOnly RotationTimeUtc { get; set; } = new(2, 0);
}