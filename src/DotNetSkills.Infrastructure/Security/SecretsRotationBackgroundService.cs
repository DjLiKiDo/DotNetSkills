using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNetSkills.Infrastructure.Security;

/// <summary>
/// Background service that automatically rotates secrets based on configuration.
/// Runs on a schedule and checks for secrets that need rotation.
/// </summary>
public sealed class SecretsRotationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<SecretsRotationBackgroundService> _logger;
    private readonly SecretsRotationOptions _options;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6); // Check every 6 hours

    public SecretsRotationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<SecretsRotationOptions> options,
        ILogger<SecretsRotationBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.AutomaticRotationEnabled)
        {
            _logger.LogInformation("Automatic secrets rotation is disabled. Service will not run.");
            return;
        }

        _logger.LogInformation("Secrets rotation background service started. Check interval: {Interval}", _checkInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndRotateSecretsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during secrets rotation check");
            }

            // Wait for the next check interval
            try
            {
                await System.Threading.Tasks.Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Service is stopping
                break;
            }
        }

        _logger.LogInformation("Secrets rotation background service stopped.");
    }

    private async System.Threading.Tasks.Task CheckAndRotateSecretsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var rotationService = scope.ServiceProvider.GetRequiredService<ISecretsRotationService>();

        _logger.LogDebug("Checking secrets for required rotation...");

        // Check if it's time to rotate (only during configured rotation hours)
        var now = DateTime.UtcNow;
        var currentTime = TimeOnly.FromDateTime(now);
        var rotationWindow = TimeSpan.FromHours(1); // 1-hour window around rotation time

        var timeDifference = Math.Abs((currentTime - _options.RotationTimeUtc).TotalMinutes);
        if (timeDifference > rotationWindow.TotalMinutes)
        {
            _logger.LogDebug("Outside of rotation time window. Current: {CurrentTime}, Target: {TargetTime}",
                currentTime, _options.RotationTimeUtc);
            return;
        }

        // Check JWT signing key rotation
        var jwtKeyNeedsRotation = await rotationService.IsSecretRotationRequiredAsync(
            "DotNetSkills-Jwt--SigningKey", 
            _options.JwtKeyRotationInterval, 
            cancellationToken);

        if (jwtKeyNeedsRotation)
        {
            _logger.LogInformation("JWT signing key requires rotation. Starting rotation process...");
            await rotationService.RotateJwtSigningKeyAsync(cancellationToken);
            _logger.LogInformation("JWT signing key rotation completed successfully.");
        }
        else
        {
            var nextRotation = await rotationService.GetNextRotationDateAsync(
                "DotNetSkills-Jwt--SigningKey", 
                cancellationToken);
            
            if (nextRotation.HasValue)
            {
                _logger.LogDebug("JWT signing key rotation not required. Next rotation: {NextRotation}", nextRotation.Value);
            }
        }
    }

    public override async System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Secrets rotation background service is stopping...");
        await base.StopAsync(cancellationToken);
    }
}