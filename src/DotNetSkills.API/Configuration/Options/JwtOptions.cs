namespace DotNetSkills.API.Configuration.Options;

public class JwtOptions
{
    public bool Enabled { get; set; } = false;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty; // Overridden by Key Vault in prod
    public int TokenLifetimeMinutes { get; set; } = 60;
}
