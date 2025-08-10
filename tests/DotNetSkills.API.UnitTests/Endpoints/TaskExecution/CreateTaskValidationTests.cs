using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using DotNetSkills.API;
using DotNetSkills.API.Authorization;
using DotNetSkills.API.Configuration.Options;
using DotNetSkills.Domain.TaskExecution.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;

namespace DotNetSkills.API.UnitTests.Endpoints.TaskExecution;

/// <summary>
/// Integration-style tests for task creation endpoint focusing on validation failures.
/// Uses a custom test authentication handler to satisfy authorization requirements.
/// </summary>
public class CreateTaskValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CreateTaskValidationTests(WebApplicationFactory<Program> factory)
    {
        // Customize the factory to inject test auth & policies when JWT is disabled.
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Force Jwt disabled and Swagger off for lean pipeline
                services.Configure<JwtOptions>(o => o.Enabled = false);
                services.Configure<SwaggerOptions>(o => o.Enabled = false);

                // Remove any prior auth registrations then add test auth & policies
                services.RemoveAll(typeof(AuthenticationSchemeOptions));
                // Add policies (not added when JWT disabled) and fake policy evaluator to always authorize
                services.AddApiAuthorization();
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
            });
        });
    }

    [Fact]
    public async Task Post_WithMissingTitle_ShouldReturn400_WithValidationErrors()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var payload = new
        {
            title = "", // invalid - NotEmpty
            description = "Some description",
            projectId = Guid.NewGuid(),
            priority = TaskPriority.High.ToString(),
            parentTaskId = (Guid?)null,
            estimatedHours = 5,
            dueDate = DateTime.UtcNow.AddDays(5),
            assignedUserId = (Guid?)null
        };

        var response = await client.PostAsJsonAsync("/api/v1/tasks", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        root.TryGetProperty("title", out var titleProp)
            .Should().BeTrue($"Response JSON missing 'title' property. Payload: {json}");
        titleProp.GetString().Should().Be("Validation Failed");

        root.TryGetProperty("status", out var statusProp)
            .Should().BeTrue($"Response JSON missing 'status' property. Payload: {json}");
        statusProp.GetInt32().Should().Be(400);

        // errors object should contain Title key with at least one message
        root.TryGetProperty("errors", out var errorsProp)
            .Should().BeTrue($"Response JSON missing 'errors' property. Payload: {json}");
        errorsProp.TryGetProperty("Title", out var titleErrors)
            .Should().BeTrue($"Errors object missing 'Title' key. Payload: {json}");
        titleErrors.EnumerateArray().Any(e => e.GetString()!.Contains("required", StringComparison.OrdinalIgnoreCase))
            .Should().BeTrue($"Expected a 'required' validation message for Title. Payload: {json}");
    }

    private sealed class FakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Developer"),
                new Claim("project", Guid.NewGuid().ToString())
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Fake"));
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal, "Fake")));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
            => Task.FromResult(PolicyAuthorizationResult.Success());
    }
}
