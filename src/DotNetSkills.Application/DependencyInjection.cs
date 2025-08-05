using DotNetSkills.Domain;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DotNetSkills.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// Registers MediatR, AutoMapper, FluentValidation, and application services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register domain services using the factory pattern
        foreach (var (serviceType, implementationType) in DomainServiceFactory.GetDomainServices())
        {
            services.AddTransient(serviceType, implementationType);
        }

        // MediatR registration for CQRS pattern
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // AutoMapper registration for entity ↔ DTO mapping
        // services.AddAutoMapper(assembly);

        // FluentValidation registration
        // services.AddValidatorsFromAssembly(assembly);

        // Application services registration
        // Example: services.AddTransient<IApplicationService, ApplicationService>();

        // Command and Query handlers (automatically registered by MediatR)
        // Example: services.AddTransient<IRequestHandler<CreateUserCommand, UserResponse>, CreateUserCommandHandler>();

        // Domain event dispatchers (when implemented)
        // Example: services.AddTransient<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
