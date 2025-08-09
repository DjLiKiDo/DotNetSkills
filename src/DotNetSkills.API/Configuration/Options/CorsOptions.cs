namespace DotNetSkills.API.Configuration.Options;

public class CorsOptions
{
    public string PolicyName { get; set; } = "AllowAll";
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public string[] AllowedMethods { get; set; } = new[] { "GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS" };
    public string[] AllowedHeaders { get; set; } = new[] { "*" };
    public bool AllowCredentials { get; set; } = false;
}
