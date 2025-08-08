using DotNetSkills.API;
using DotNetSkills.Infrastructure.Common.Configuration;
using DotNetSkills.API.Configuration.Options;

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

// Configure the HTTP request pipeline.
app.UseExceptionHandling();

var swaggerOptions = app.Services.GetRequiredService<IOptions<SwaggerOptions>>().Value;
if (app.Environment.IsDevelopment() && swaggerOptions.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
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
