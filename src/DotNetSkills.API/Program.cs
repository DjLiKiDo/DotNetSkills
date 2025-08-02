using DotNetSkills.API;

var builder = WebApplication.CreateBuilder(args);

// Add all services using centralized DI configuration
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Authentication & Authorization (when implemented)
// app.UseAuthentication();
// app.UseAuthorization();

// Health checks
app.MapHealthChecks("/health");

// Domain endpoints organized by bounded context
app.MapUserManagementEndpoints();

// TODO: Add remaining bounded context endpoints when implemented
// app.MapTeamCollaborationEndpoints();
// app.MapProjectManagementEndpoints();
// app.MapTaskExecutionEndpoints();

await app.RunAsync();