using FluentValidation;

namespace DotNetSkills.Infrastructure.Security;

/// <summary>
/// Validation rules for <see cref="SecretsRotationOptions"/> to enforce sane operational bounds.
/// </summary>
public sealed class SecretsRotationOptionsValidator : AbstractValidator<SecretsRotationOptions>
{
    private static readonly TimeSpan MinInterval = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan MaxInterval = TimeSpan.FromDays(30);

    public SecretsRotationOptionsValidator()
    {
        RuleFor(o => o.JwtKeyRotationInterval)
            .Must(i => i >= MinInterval)
            .WithMessage(o => $"JwtKeyRotationInterval must be at least {MinInterval} (current: {o.JwtKeyRotationInterval}).")
            .Must(i => i <= MaxInterval)
            .WithMessage(o => $"JwtKeyRotationInterval must not exceed {MaxInterval} (current: {o.JwtKeyRotationInterval}).");

        RuleFor(o => o.RotationTimeUtc)
            .Must(t => t.Hour >= 0 && t.Hour < 24 && t.Minute >= 0 && t.Minute < 60)
            .WithMessage(o => $"RotationTimeUtc must be a valid time of day (current: {o.RotationTimeUtc}).");

        When(o => o.AutomaticRotationEnabled, () =>
        {
            // When automation enabled ensure interval not absurdly large/small (already covered) and time present
            RuleFor(o => o.RotationTimeUtc)
                .NotNull();
        });
    }
}
