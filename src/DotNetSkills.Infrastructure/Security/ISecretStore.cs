using Azure;
using Azure.Security.KeyVault.Secrets;

namespace DotNetSkills.Infrastructure.Security;

/// <summary>
/// Abstraction over secret storage backing (e.g., Azure Key Vault) to enable testability.
/// </summary>
public interface ISecretStore
{
    System.Threading.Tasks.Task<KeyVaultSecret?> TryGetSecretAsync(string name, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task SetSecretAsync(KeyVaultSecret secret, CancellationToken cancellationToken = default);
}

/// <summary>
/// Azure Key Vault implementation of <see cref="ISecretStore"/>.
/// </summary>
public sealed class KeyVaultSecretStore : ISecretStore
{
    private readonly SecretClient _client;

    public KeyVaultSecretStore(SecretClient client)
    {
        _client = client;
    }

    public async System.Threading.Tasks.Task<KeyVaultSecret?> TryGetSecretAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetSecretAsync(name, cancellationToken: cancellationToken);
            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public System.Threading.Tasks.Task SetSecretAsync(KeyVaultSecret secret, CancellationToken cancellationToken = default)
        => _client.SetSecretAsync(secret, cancellationToken);
}
