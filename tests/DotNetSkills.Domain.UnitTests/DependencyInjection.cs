using Microsoft.Extensions.DependencyInjection;

namespace DotNetSkills.Domain.UnitTests;

/// <summary>
/// Dependency injection configuration for Domain unit tests.
/// Provides test-specific services and configurations.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Domain testing services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddDomainTestServices(this IServiceCollection services)
    {
        // Test builders and factories
        // Example: services.AddTransient<UserBuilder>();
        // Example: services.AddTransient<TeamBuilder>();
        // Example: services.AddTransient<ProjectBuilder>();
        // Example: services.AddTransient<TaskBuilder>();
        
        // Test data providers
        // Example: services.AddTransient<ITestDataProvider, TestDataProvider>();
        
        // Domain service mocks (if needed for complex scenarios)
        // Example: services.AddTransient<IDomainService, MockDomainService>();
        
        return services;
    }
}
