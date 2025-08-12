using DotNetSkills.Application;
using DotNetSkills.Infrastructure;
using DotNetSkills.API.Configuration.Options;
using DotNetSkills.API.Configuration.Options.Validators;
using DotNetSkills.API.Authorization;
using DotNetSkills.API.Services;
using DotNetSkills.Application.Common.Abstractions; // Added for ICurrentUserService abstraction
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DotNetSkills.Infrastructure.Security;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.DataProtection;

namespace DotNetSkills.API;

/// <summary>
/// Dependency injection configuration for the API layer.
/// Orchestrates registration of all layers and API-specific services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds API layer services to the dependency injection container.
    /// Also orchestrates registration of all other layers.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration instance for settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind API options
        services.AddOptions<SwaggerOptions>()
            .Bind(configuration.GetSection("Swagger"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CorsOptions>()
            .Bind(configuration.GetSection("Cors"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection("Jwt"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

    // Option validators
    services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();
    services.AddSingleton<IValidateOptions<CorsOptions>, CorsOptionsValidator>();
    services.AddSingleton<IValidateOptions<SwaggerOptions>, SwaggerOptionsValidator>();

        // Register all layers in dependency order (Application → Infrastructure → API)
        // Note: Domain services are registered through Application layer
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        // Persist DataProtection keys so cookies/tokens survive restarts
        services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true
                ? "/app/.aspnet/DataProtection-Keys"
                : Path.Combine(AppContext.BaseDirectory, ".aspnet", "DataProtection-Keys")));

        // Configure comprehensive Swagger/OpenAPI documentation
        var swagger = configuration.GetSection("Swagger").Get<SwaggerOptions>() ?? new SwaggerOptions();
        if (swagger.Enabled)
        {
            services.AddSwaggerDocumentation(swagger);
        }

        // Configure JSON serialization options for strongly-typed IDs and domain types
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());

            // Add custom converters for strongly-typed IDs when needed
            // options.SerializerOptions.Converters.Add(new UserIdJsonConverter());
            // options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
            // options.SerializerOptions.Converters.Add(new ProjectIdJsonConverter());
            // options.SerializerOptions.Converters.Add(new TaskIdJsonConverter());
        });

        // Configure model binding for minimal APIs
        services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        // Problem Details configuration for consistent error responses
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = (context) =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
            };
        });

        // FluentValidation integration for endpoint validation
        // Will be activated when Application layer implements validators
        // services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        // services.AddFluentValidationAutoValidation();
        // services.AddFluentValidationClientsideAdapters();

        // FluentValidation integration for endpoint validation
        // Will be activated when Application layer implements validators
        // services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        // services.AddFluentValidationAutoValidation();
        // services.AddFluentValidationClientsideAdapters();

        // Authentication & Authorization (conditional on Jwt.Enabled)
        var jwt = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        if (jwt.Enabled)
        {
            services.AddSingleton<IJwtSigningCredentialsProvider, MultiKeyJwtSigningCredentialsProvider>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    using var scope = services.BuildServiceProvider();
                    var provider = scope.GetRequiredService<IJwtSigningCredentialsProvider>();
                    options.TokenValidationParameters = provider.CreateValidationParameters(jwt.Issuer, jwt.Audience);
                });
            services.AddApiAuthorization();
        }

        // CORS configuration via options
        var cors = configuration.GetSection("Cors").Get<CorsOptions>() ?? new CorsOptions();
        services.AddCors(options =>
        {
            options.AddPolicy(cors.PolicyName, builder =>
            {
                if (cors.AllowedOrigins.Length > 0)
                {
                    builder.WithOrigins(cors.AllowedOrigins);
                }
                else
                {
                    builder.AllowAnyOrigin();
                }

                if (cors.AllowedMethods.Length > 0)
                {
                    builder.WithMethods(cors.AllowedMethods);
                }
                else
                {
                    builder.AllowAnyMethod();
                }

                if (cors.AllowedHeaders.Length > 0)
                {
                    builder.WithHeaders(cors.AllowedHeaders);
                }
                else
                {
                    builder.AllowAnyHeader();
                }

                if (cors.AllowCredentials)
                {
                    builder.AllowCredentials();
                }
            });
        });

        // API Services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IClaimsPopulationService, ClaimsPopulationService>();

        // Health checks
        services.AddHealthChecks();

        // Rate limiting configuration (when needed)
        // services.AddRateLimiter(options =>
        // {
        //     options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        //         httpContext => RateLimitPartition.GetFixedWindowLimiter(
        //             partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
        //             factory: partition => new FixedWindowRateLimiterOptions
        //             {
        //                 AutoReplenishment = true,
        //                 PermitLimit = 100,
        //                 Window = TimeSpan.FromMinutes(1)
        //             }));
        // });

        // Output caching configuration (when needed)
        // services.AddOutputCache(options =>
        // {
        //     options.AddPolicy("DefaultCache", builder =>
        //         builder.Cache()
        //               .Expire(TimeSpan.FromMinutes(5))
        //               .SetVaryByHeader("Accept", "Accept-Language"));
        // });

        // Request decompression (for handling compressed requests)
        services.AddRequestDecompression();

        // Response compression
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            // Add specific MIME types when needed
            // options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
            //     new[] { "application/json", "text/json" });
        });

        // Model binding configuration
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        // API versioning (when needed)
        // services.AddApiVersioning(options =>
        // {
        //     options.DefaultApiVersion = new ApiVersion(1, 0);
        //     options.AssumeDefaultVersionWhenUnspecified = true;
        //     options.ApiVersionReader = ApiVersionReader.Combine(
        //         new UrlSegmentApiVersionReader(),
        //         new HeaderApiVersionReader("X-Version"));
        // });

    return services;
    }
}
