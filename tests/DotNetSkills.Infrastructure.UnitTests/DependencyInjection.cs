using Microsoft.Extensions.DependencyInjection;

namespace DotNetSkills.Infrastructure.UnitTests;

/// <summary>
/// Dependency injection configuration for Infrastructure unit tests.
/// Provides test-specific services and in-memory databases for infrastructure layer testing.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure testing services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInfrastructureTestServices(this IServiceCollection services)
    {
        // In-memory database for testing (when EF Core is added)
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
        
        // Test repositories (real implementations with in-memory database)
        // services.AddScoped<IUserRepository, EfUserRepository>();
        // services.AddScoped<ITeamRepository, EfTeamRepository>();
        // services.AddScoped<IProjectRepository, EfProjectRepository>();
        // services.AddScoped<ITaskRepository, EfTaskRepository>();
        
        // Unit of Work with in-memory database
        // services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        
        // Mock external services for testing
        // services.AddTransient<Mock<IEmailService>>();
        // services.AddTransient<Mock<INotificationService>>();
        
        // Test data seeders
        // services.AddTransient<ITestDataSeeder, TestDataSeeder>();
        
        return services;
    }
}
