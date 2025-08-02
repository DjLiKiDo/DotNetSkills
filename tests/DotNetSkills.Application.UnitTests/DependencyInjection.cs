using Microsoft.Extensions.DependencyInjection;

namespace DotNetSkills.Application.UnitTests;

/// <summary>
/// Dependency injection configuration for Application unit tests.
/// Provides test-specific services and mocks for application layer testing.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application testing services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationTestServices(this IServiceCollection services)
    {
        // Repository mocks
        // services.AddTransient<Mock<IUserRepository>>();
        // services.AddTransient<Mock<ITeamRepository>>();
        // services.AddTransient<Mock<IProjectRepository>>();
        // services.AddTransient<Mock<ITaskRepository>>();
        
        // Unit of Work mock
        // services.AddTransient<Mock<IUnitOfWork>>();
        
        // MediatR test setup (when added)
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        
        // AutoMapper test configuration (when added)
        // services.AddAutoMapper(typeof(MappingProfile));
        
        // Test data builders
        // services.AddTransient<CommandBuilder>();
        // services.AddTransient<QueryBuilder>();
        
        return services;
    }
}
