namespace DotNetSkills.API.Configuration.Options;

public class SwaggerOptions
{
    public bool Enabled { get; set; } = true;
    public string Title { get; set; } = "DotNetSkills API";
    public string Version { get; set; } = "v1";
}
