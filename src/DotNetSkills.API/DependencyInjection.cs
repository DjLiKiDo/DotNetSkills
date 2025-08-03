using DotNetSkills.Application;
using DotNetSkills.Domain;
using DotNetSkills.Infrastructure;

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
        // Register all layers in dependency order (Application → Infrastructure → API)
        // Note: Domain services are registered through Application layer
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);
        
        // Configure comprehensive Swagger/OpenAPI documentation
        services.AddSwaggerDocumentation(configuration);
        
        // Configure JSON serialization options for strongly-typed IDs and domain types
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.WriteIndented = true;
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            
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
        
        // Authentication & Authorization (when implemented)
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //     .AddJwtBearer(options =>
        //     {
        //         options.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuer = true,
        //             ValidateAudience = true,
        //             ValidateLifetime = true,
        //             ValidateIssuerSigningKey = true,
        //             ValidIssuer = configuration["Jwt:Issuer"],
        //             ValidAudience = configuration["Jwt:Audience"],
        //             IssuerSigningKey = new SymmetricSecurityKey(
        //                 Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
        //         };
        //     });
        
        // services.AddAuthorization(options =>
        // {
        //     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        //     options.AddPolicy("ProjectManager", policy => 
        //         policy.RequireRole("Admin", "ProjectManager"));
        // });
        
        // CORS configuration
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
        
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
