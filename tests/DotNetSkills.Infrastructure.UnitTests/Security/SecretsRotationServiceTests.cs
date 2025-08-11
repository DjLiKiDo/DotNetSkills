using Azure.Security.KeyVault.Secrets;
using DotNetSkills.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace DotNetSkills.Infrastructure.UnitTests.Security;

public class SecretsRotationServiceTests
{
    private static SecretsRotationService CreateService(FakeSecretStore store, SecretsRotationOptions? options = null)
    {
        options ??= new SecretsRotationOptions
        {
            JwtKeyRotationInterval = TimeSpan.FromDays(30),
            AutomaticRotationEnabled = false,
            RotationTimeUtc = new TimeOnly(2,0)
        };
    return new SecretsRotationService(Microsoft.Extensions.Options.Options.Create(options), new NullLogger<SecretsRotationService>(), new ConfigurationBuilder().Build(), store);
    }

    [Fact]
    public async Task RotateJwtSigningKeyAsync_FirstRotation_ShouldCreatePrimaryWithoutPrevious()
    {
        var store = new FakeSecretStore();
        var service = CreateService(store);

        await service.RotateJwtSigningKeyAsync();

        store.Secrets.ContainsKey("DotNetSkills-Jwt--SigningKey").Should().BeTrue();
        var primary = store.Secrets["DotNetSkills-Jwt--SigningKey"];        
        primary.Properties.Tags.ContainsKey("PreviousKeyReference").Should().BeFalse();
        primary.Properties.Tags.ContainsKey("RotatedAt").Should().BeTrue();
    }

    [Fact]
    public async Task RotateJwtSigningKeyAsync_SubsequentRotation_ShouldArchiveAndTagPrevious()
    {
        var store = new FakeSecretStore();
        var service = CreateService(store);

        await service.RotateJwtSigningKeyAsync(); // first
        var firstPrimaryValue = store.Secrets["DotNetSkills-Jwt--SigningKey"].Value;

        await service.RotateJwtSigningKeyAsync(); // second

        var primary = store.Secrets["DotNetSkills-Jwt--SigningKey"];        
        primary.Properties.Tags.TryGetValue("PreviousKeyReference", out var prevRef).Should().BeTrue();
        prevRef.Should().NotBeNullOrWhiteSpace();
        prevRef!.StartsWith("DotNetSkills-Jwt--SigningKey-archived-").Should().BeTrue();

        // archived secret exists
        store.Secrets.ContainsKey(prevRef!).Should().BeTrue();
        store.Secrets[prevRef!].Value.Should().Be(firstPrimaryValue);
    }

    [Fact]
    public async Task IsSecretRotationRequiredAsync_SecretMissing_ShouldReturnTrue()
    {
        var store = new FakeSecretStore();
        var service = CreateService(store);

        var result = await service.IsSecretRotationRequiredAsync("DotNetSkills-Jwt--SigningKey", TimeSpan.FromDays(30));
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsSecretRotationRequiredAsync_WithinInterval_ShouldReturnFalse()
    {
        var store = new FakeSecretStore();
        var service = CreateService(store);
        await service.RotateJwtSigningKeyAsync();

        var result = await service.IsSecretRotationRequiredAsync("DotNetSkills-Jwt--SigningKey", TimeSpan.FromDays(90));
        result.Should().BeFalse();
    }
}
