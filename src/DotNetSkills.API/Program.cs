using DotNetSkills.API;
using DotNetSkills.Infrastructure.Common.Configuration;
using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration providers in order of precedence
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: "DOTNETSKILLS_")
    .AddAzureKeyVaultIfProduction(builder.Environment);

// Development-only user secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add all services using centralized DI configuration
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Validate configuration on startup
app.Services.ValidateConfiguration();

// Run database migrations if configured to do so
// Uses RUN_MIGRATIONS environment variable for containerized deployments
await app.RunDatabaseMigrationsAsync();

// Configure the HTTP request pipeline.
app.UseCorrelationId();
app.UsePerformanceLogging();
app.UseExceptionHandling();

var swaggerOptions = app.Services.GetRequiredService<IOptions<SwaggerOptions>>().Value;
if (app.Environment.IsDevelopment() && swaggerOptions.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disable HTTPS redirection when running inside a Docker container for now
var runningInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
if (!runningInContainer)
{
    app.UseHttpsRedirection();
}

var corsOptions = app.Services.GetRequiredService<IOptions<CorsOptions>>().Value;
app.UseCors(corsOptions.PolicyName);

var jwtOptions = app.Services.GetRequiredService<IOptions<JwtOptions>>().Value;
if (jwtOptions.Enabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

// Health checks
app.MapHealthChecks("/health");

// Domain endpoints organized by bounded context
app.MapUserManagementEndpoints();
app.MapTeamCollaborationEndpoints();
app.MapProjectManagementEndpoints();
app.MapTaskExecutionEndpoints();

await app.RunAsync();

// Expose Program class for WebApplicationFactory in tests
public partial class Program { }
