namespace DotNetSkills.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// Registers EF Core, repositories, external services, and infrastructure concerns.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance for connection strings and settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database configuration
        // services.AddDbContext<ApplicationDbContext>(options =>
        //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repository registrations (Application layer interfaces → Infrastructure implementations)
        // services.AddScoped<IUserRepository, EfUserRepository>();
        // services.AddScoped<ITeamRepository, EfTeamRepository>();
        // services.AddScoped<IProjectRepository, EfProjectRepository>();
        // services.AddScoped<ITaskRepository, EfTaskRepository>();

        // Unit of Work pattern
        // services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // External services
        // services.AddScoped<IEmailService, SmtpEmailService>();
        // services.AddScoped<INotificationService, SignalRNotificationService>();

        // Infrastructure services
        // services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        // services.AddScoped<IPasswordService, BCryptPasswordService>();

        // Caching (if needed)
        // services.AddMemoryCache();
        // services.AddStackExchangeRedisCache(options =>
        // {
        //     options.Configuration = configuration.GetConnectionString("Redis");
        // });

        return services;
    }
}
