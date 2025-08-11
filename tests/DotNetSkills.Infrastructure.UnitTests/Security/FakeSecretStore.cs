using Azure.Security.KeyVault.Secrets;
using DotNetSkills.Infrastructure.Security;

namespace DotNetSkills.Infrastructure.UnitTests.Security;

internal sealed class FakeSecretStore : ISecretStore
{
    private readonly Dictionary<string, KeyVaultSecret> _secrets = new();

    public Task<KeyVaultSecret?> TryGetSecretAsync(string name, CancellationToken cancellationToken = default)
    {
        _secrets.TryGetValue(name, out var secret);
        return Task.FromResult<KeyVaultSecret?>(secret);
    }

    public Task SetSecretAsync(KeyVaultSecret secret, CancellationToken cancellationToken = default)
    {
        _secrets[secret.Name] = secret;
        return Task.CompletedTask;
    }

    public IReadOnlyDictionary<string, KeyVaultSecret> Secrets => _secrets;
}
