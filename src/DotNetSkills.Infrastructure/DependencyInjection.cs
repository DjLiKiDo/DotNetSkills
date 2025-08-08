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
        // Database configuration with SQL Server provider
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            
            // Development-specific configurations
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        // Repository registrations (Application layer interfaces → Infrastructure implementations)
        // Register concrete repositories first, then wrap with cached decorators
        
        // User Repository with caching
        services.AddScoped<UserRepository>();
        services.AddScoped<IUserRepository>(provider =>
        {
            var innerRepository = provider.GetRequiredService<UserRepository>();
            var memoryCache = provider.GetRequiredService<IMemoryCache>();
            return new CachedUserRepository(innerRepository, memoryCache);
        });
        
        // Team Repository with caching
        services.AddScoped<TeamRepository>();
        services.AddScoped<ITeamRepository>(provider =>
        {
            var innerRepository = provider.GetRequiredService<TeamRepository>();
            var memoryCache = provider.GetRequiredService<IMemoryCache>();
            return new CachedTeamRepository(innerRepository, memoryCache);
        });
        
        // Project Repository with caching
        services.AddScoped<ProjectRepository>();
        services.AddScoped<IProjectRepository>(provider =>
        {
            var innerRepository = provider.GetRequiredService<ProjectRepository>();
            var memoryCache = provider.GetRequiredService<IMemoryCache>();
            return new CachedProjectRepository(innerRepository, memoryCache);
        });
        
        // Task Repository with caching
        services.AddScoped<TaskRepository>();
        services.AddScoped<ITaskRepository>(provider =>
        {
            var innerRepository = provider.GetRequiredService<TaskRepository>();
            var memoryCache = provider.GetRequiredService<IMemoryCache>();
            return new CachedTaskRepository(innerRepository, memoryCache);
        });

        // Unit of Work pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Domain Event Dispatcher (placeholder - needs MediatR integration)
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // External services (placeholders for future implementation)
        // services.AddScoped<IEmailService, SmtpEmailService>();
        // services.AddScoped<INotificationService, SignalRNotificationService>();
        // services.AddScoped<IPasswordService, BCryptPasswordService>();

        // Caching (memory cache for development, Redis for production)
        services.AddMemoryCache();
        
        // Health checks for database and external dependencies
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Database");

        // Logging enhancements for development
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
            });
        }

        return services;
    }

    /// <summary>
    /// Configures database-specific options and advanced EF Core features.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure strongly-typed database options
        services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
        
        // Add database configuration validation
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        
        return services;
    }

    /// <summary>
    /// Configures database connection string and provider-specific options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            });

            // Connection pooling configuration
            options.EnableServiceProviderCaching();
        });

        return services;
    }
}
