using FluentValidation;
using DotNetSkills.Infrastructure.Security;
using DotNetSkills.Infrastructure.HealthChecks;
using DotNetSkills.Infrastructure.Common.Performance;

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
        // Ensure options are bound and validated
        services.AddDatabaseConfiguration(configuration);

        // Database configuration with SQL Server provider and enhanced connection settings
        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var connectionString = !string.IsNullOrWhiteSpace(dbOptions.ConnectionString)
                ? dbOptions.ConnectionString
                : configuration.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: dbOptions.MaxRetryCount,
                    maxRetryDelay: TimeSpan.FromSeconds(dbOptions.MaxRetryDelaySeconds),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(dbOptions.CommandTimeout);
            });

            // Diagnostics & logging (only enable; rely on EF default 'off' otherwise)
            if (dbOptions.EnableSensitiveDataLogging && env.IsDevelopment())
            {
                options.EnableSensitiveDataLogging(); // do NOT force disable in other environments to avoid overriding future provider defaults
            }

            if (dbOptions.EnableDetailedErrors)
            {
                options.EnableDetailedErrors();
            }

            if (dbOptions.EnableQueryLogging && env.IsDevelopment())
            {
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }

            // Performance optimizations
            options.EnableServiceProviderCaching();
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

        // Domain Event Collection Service for request-scoped tracking
        services.AddScoped<IDomainEventCollectionService, DomainEventCollectionService>();

        // External services (placeholders for future implementation)
        // services.AddScoped<IEmailService, SmtpEmailService>();
        // services.AddScoped<INotificationService, SignalRNotificationService>();
        // services.AddScoped<IPasswordService, BCryptPasswordService>();

        // Caching (memory cache for development, Redis for production)
        services.AddMemoryCache();

        // Performance monitoring
        services.Configure<PerformanceMonitoringOptions>(options =>
        {
            options.SlowOperationThreshold = TimeSpan.FromMilliseconds(1000);
            options.EnableMetricRecording = true;
            options.LogOperationStart = false;
        });
        services.AddSingleton<IPerformanceMonitoringService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<PerformanceMonitoringService>>();
            var options = provider.GetRequiredService<IOptions<PerformanceMonitoringOptions>>().Value;
            return new PerformanceMonitoringService(logger, options);
        });
        
        // Health checks for database and external dependencies
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("Database")
            .AddCheck<DatabaseHealthCheck>("Database-Custom")
            .AddCheck<CacheHealthCheck>("MemoryCache");

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
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection("Database"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Security Services
        services.AddOptions<SecretsRotationOptions>()
            .Bind(configuration.GetSection("SecretsRotation"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Fluent validation for SecretsRotationOptions (interval bounds etc.)
        services.AddSingleton<IValidator<SecretsRotationOptions>, SecretsRotationOptionsValidator>();

        // Register secret store only if Key Vault configured
        var keyVaultUri = configuration["KeyVault:Uri"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            services.AddSingleton<ISecretStore>(sp =>
            {
                var client = new Azure.Security.KeyVault.Secrets.SecretClient(new Uri(keyVaultUri), new Azure.Identity.DefaultAzureCredential());
                return new KeyVaultSecretStore(client);
            });
        }

    services.AddScoped<ISecretsRotationService, SecretsRotationService>();
        services.AddHostedService<SecretsRotationBackgroundService>();
        
        return services;
    }

    /// <summary>
    /// Configures database connection string and provider-specific options with enhanced settings.
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
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(30);
            });

            // NOTE: This overload is legacy; prefer AddInfrastructureServices path with DatabaseOptions.
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var isDev = string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase);
            if (isDev)
            {
                options.EnableSensitiveDataLogging(); // only enable in Development
                options.EnableDetailedErrors();
            }

            // Performance optimizations
            options.EnableServiceProviderCaching();
        });

        return services;
    }
}
