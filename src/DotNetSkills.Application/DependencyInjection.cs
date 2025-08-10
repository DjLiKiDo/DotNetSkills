using DotNetSkills.Domain;
using FluentValidation;
using MediatR;
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

        // MediatR registration for CQRS pattern (v12+ API)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            
            // MediatR behaviors registration - order matters!
            // 1. Logging - capture all operations early
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            // 2. Validation - short‑circuit before expensive work
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            // 3. Performance - measure only after validation passes
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            // 4. Domain events - dispatch after successful handler execution
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));
        });

        // AutoMapper registration for entity ↔ DTO mapping
        services.AddAutoMapper(assembly);

        // FluentValidation registration with validator discovery
        services.AddValidatorsFromAssembly(assembly, ServiceLifetime.Transient);

        // Repository interfaces (to be implemented by Infrastructure layer)
        // services.AddScoped<IUserRepository, UserRepository>();
        // services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain event dispatcher (when implemented)
        // services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }
}
