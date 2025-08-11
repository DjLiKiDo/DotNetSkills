using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace DotNetSkills.Infrastructure.Security;

public interface IJwtSigningCredentialsProvider
{
    SigningCredentials GetCurrentSigningCredentials();
    TokenValidationParameters CreateValidationParameters(string issuer, string audience);
}

public sealed class MultiKeyJwtSigningCredentialsProvider : IJwtSigningCredentialsProvider
{
    private const string PrimarySecretName = "DotNetSkills-Jwt--SigningKey";
    private readonly ILogger<MultiKeyJwtSigningCredentialsProvider> _logger;
    private readonly string _configurationFallbackKey;
    private readonly SecretClient? _secretClient;
    private readonly ConcurrentDictionary<string, SecurityKey> _activeKeys = new();
    private SecurityKey? _currentKey;
    private DateTime _lastLoadUtc = DateTime.MinValue;
    private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(10);

    public MultiKeyJwtSigningCredentialsProvider(
        IConfiguration configuration,
        ILogger<MultiKeyJwtSigningCredentialsProvider> logger)
    {
        _logger = logger;
        _configurationFallbackKey = configuration["Jwt:SigningKey"] ?? string.Empty;
        var keyVaultUri = configuration["KeyVault:Uri"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            _secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        }
        LoadKeysAsync().GetAwaiter().GetResult();
    }

    public SigningCredentials GetCurrentSigningCredentials()
    {
        EnsureKeysFresh();
        if (_currentKey == null)
            throw new InvalidOperationException("No JWT signing key available.");
        return new SigningCredentials(_currentKey, SecurityAlgorithms.HmacSha256);
    }

    public TokenValidationParameters CreateValidationParameters(string issuer, string audience)
    {
        EnsureKeysFresh();
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                EnsureKeysFresh();
                return _activeKeys.Values.ToList();
            }
        };
    }

    private void EnsureKeysFresh()
    {
        if (DateTime.UtcNow - _lastLoadUtc > _cacheTtl)
        {
            try { LoadKeysAsync().GetAwaiter().GetResult(); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh JWT keys; continuing with existing set.");
            }
        }
    }

    private async System.Threading.Tasks.Task LoadKeysAsync()
    {
        if (_secretClient == null)
        {
            if (!string.IsNullOrWhiteSpace(_configurationFallbackKey))
            {
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configurationFallbackKey));
                _currentKey = key;
                _activeKeys[key.KeyId ?? "config"] = key;
            }
            _lastLoadUtc = DateTime.UtcNow;
            return;
        }

        try
        {
            var primary = await _secretClient.GetSecretAsync(PrimarySecretName);
            var primaryValue = primary.Value.Value;
            var primaryKey = new SymmetricSecurityKey(Convert.FromBase64String(primaryValue)) { KeyId = PrimarySecretName };
            _currentKey = primaryKey;
            _activeKeys[PrimarySecretName] = primaryKey;

            if (primary.Value.Properties.Tags.TryGetValue("PreviousKeyReference", out var previousKeyName) && !string.IsNullOrWhiteSpace(previousKeyName))
            {
                try
                {
                    var previousSecret = await _secretClient.GetSecretAsync(previousKeyName);
                    var prevKey = new SymmetricSecurityKey(Convert.FromBase64String(previousSecret.Value.Value)) { KeyId = previousKeyName };
                    _activeKeys[previousKeyName] = prevKey;
                }
                catch (Exception exPrev)
                {
                    _logger.LogWarning(exPrev, "Previous JWT key {PrevKey} could not be loaded.", previousKeyName);
                }
            }
            _lastLoadUtc = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load JWT keys from Key Vault. Falling back to configuration key (if available).");
            if (!string.IsNullOrWhiteSpace(_configurationFallbackKey))
            {
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configurationFallbackKey));
                _currentKey = key;
                _activeKeys[key.KeyId ?? "config"] = key;
            }
        }
    }
}
