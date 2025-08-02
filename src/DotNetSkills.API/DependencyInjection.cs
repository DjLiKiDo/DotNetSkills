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
        
        // API-specific services
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() 
            { 
                Title = "DotNetSkills API", 
                Version = "v1",
                Description = "Project Management API demonstrating Clean Architecture and DDD"
            });
            
            // JWT Bearer configuration for Swagger
            // options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            // {
            //     Description = "JWT Authorization header using the Bearer scheme",
            //     Name = "Authorization",
            //     In = ParameterLocation.Header,
            //     Type = SecuritySchemeType.ApiKey,
            //     Scheme = "Bearer"
            // });
        });
        
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
