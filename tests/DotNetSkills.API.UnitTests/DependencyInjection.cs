using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetSkills.API.UnitTests;

/// <summary>
/// Dependency injection configuration for API unit tests.
/// Provides test-specific services and configurations for endpoint testing.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API testing services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApiTestServices(this IServiceCollection services)
    {
        // Mock application services for endpoint testing
        // services.AddTransient<Mock<IMediator>>();

        // Mock authentication for testing protected endpoints
        // services.AddAuthentication("Test")
        //     .AddScheme<AuthenticationSchemeOptions, TestAuthenticationSchemeHandler>(
        //         "Test", options => { });

        // Test-specific configurations
        // services.Configure<JwtSettings>(options =>
        // {
        //     options.Key = "test-key-for-testing-purposes-only";
        //     options.Issuer = "test-issuer";
        //     options.Audience = "test-audience";
        // });

        // HttpClient factory for integration tests
        // services.AddHttpClient();

        return services;
    }

    /// <summary>
    /// Configures test application for integration testing.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The configured builder for method chaining.</returns>
    public static WebApplicationBuilder ConfigureTestApplication(this WebApplicationBuilder builder)
    {
        // Override configurations for testing
        builder.Configuration.AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string?>("Jwt:Key", "test-key-for-testing-purposes-only-must-be-long-enough"),
            new KeyValuePair<string, string?>("Jwt:Issuer", "test-issuer"),
            new KeyValuePair<string, string?>("Jwt:Audience", "test-audience"),
            new KeyValuePair<string, string?>("ConnectionStrings:DefaultConnection", "DataSource=:memory:")
        });

        return builder;
    }
}
