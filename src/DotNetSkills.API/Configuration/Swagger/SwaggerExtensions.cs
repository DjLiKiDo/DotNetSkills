namespace DotNetSkills.API.Configuration.Swagger;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation.
/// Provides a clean, maintainable approach to API documentation configuration
/// following Clean Architecture principles.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds comprehensive Swagger/OpenAPI documentation to the service collection.
    /// Configures professional-grade API documentation with bounded context organization,
    /// security schemes, and enhanced developer experience features.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configuration">Application configuration for settings.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            ConfigureApiInformation(options, new("DotNetSkills API", "v1"));
            ConfigureDocumentationSettings(options);
            ConfigureSchemaSettings(options);
            ConfigureTagOrganization(options);
            ConfigureSecurity(options);
            ConfigureCustomFilters(options);
        });

        return services;
    }

    /// <summary>
    /// Overload to configure Swagger using bound SwaggerOptions.
    /// </summary>
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services,
        Configuration.Options.SwaggerOptions swaggerOptions)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            ConfigureApiInformation(options, new(swaggerOptions.Title, swaggerOptions.Version));
            ConfigureDocumentationSettings(options);
            ConfigureSchemaSettings(options);
            ConfigureTagOrganization(options);
            ConfigureSecurity(options);
            ConfigureCustomFilters(options);
        });

        return services;
    }

    /// <summary>
    /// Configures the basic API information including title, description, contact, and license.
    /// </summary>
    private static void ConfigureApiInformation(SwaggerGenOptions options, (string Title, string Version) info)
    {
        options.SwaggerDoc(info.Version, new OpenApiInfo
        {
            Title = info.Title,
            Version = info.Version,
            Description = """
                ## DotNetSkills Project Management API

                A comprehensive project management API built with .NET 9, demonstrating:
                - **Clean Architecture** principles with clear layer separation
                - **Domain-Driven Design** with rich domain models and bounded contexts
                - **CQRS** pattern using MediatR for command/query separation
                - **Strongly-typed IDs** for type safety and domain expressiveness

                ### Bounded Contexts

                The API is organized around four key bounded contexts:

                - **👥 User Management**: User CRUD operations, roles, and account management
                - **🤝 Team Collaboration**: Team creation, membership management, and collaboration
                - **📋 Project Management**: Project lifecycle, team assignments, and project-task relationships
                - **✅ Task Execution**: Task management, assignments, status tracking, and subtasks

                ### Authentication & Authorization

                This API uses JWT Bearer token authentication with role-based access control:
                - **Admin**: Full system access, user management
                - **Project Manager**: Project and team management within assigned projects
                - **Developer**: Task management and collaboration within assigned teams
                - **Viewer**: Read-only access to assigned projects and teams

                ### Common Response Patterns

                - **Success**: 200 (OK), 201 (Created), 204 (No Content)
                - **Client Errors**: 400 (Bad Request), 401 (Unauthorized), 403 (Forbidden), 404 (Not Found), 409 (Conflict)
                - **Server Errors**: 500 (Internal Server Error)

                All error responses follow RFC 7807 Problem Details format with detailed error information.
                """,
            Contact = new OpenApiContact
            {
                Name = "DotNetSkills Development Team",
                Email = "api-support@dotnetskills.com",
                Url = new Uri("https://github.com/DjLiKiDo/DotNetSkills")
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
    });
    }

    /// <summary>
    /// Configures XML documentation inclusion and comment settings.
    /// </summary>
    private static void ConfigureDocumentationSettings(SwaggerGenOptions options)
    {
        // Include XML documentation when available
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);

            // Also include XML comments from Application layer for DTOs
            var appXmlFile = "DotNetSkills.Application.xml";
            var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXmlFile);
            if (File.Exists(appXmlPath))
            {
                options.IncludeXmlComments(appXmlPath);
            }
        }
    }

    /// <summary>
    /// Configures schema generation settings and custom type mappings.
    /// </summary>
    private static void ConfigureSchemaSettings(SwaggerGenOptions options)
    {
        // Configure response types and examples
        options.SupportNonNullableReferenceTypes();
        options.UseInlineDefinitionsForEnums();
        options.DescribeAllParametersInCamelCase();

        // Custom schema mappings for strongly-typed IDs
        options.MapType<Guid>(() => new OpenApiSchema
        {
            Type = "string",
            Format = "uuid",
            Example = new Microsoft.OpenApi.Any.OpenApiString("123e4567-e89b-12d3-a456-426614174000")
        });
    }

    /// <summary>
    /// Configures tag organization by bounded context for better API navigation.
    /// </summary>
    private static void ConfigureTagOrganization(SwaggerGenOptions options)
    {
        // Group operations by bounded context tags for better organization
        options.TagActionsBy(api =>
        {
            // Extract from route path to organize by bounded context
            var routeTemplate = api.RelativePath ?? "";
            return routeTemplate switch
            {
                var route when route.StartsWith("api/v1/users") => ["👥 User Management"],
                var route when route.StartsWith("api/v1/teams") => ["🤝 Team Collaboration"],
                var route when route.StartsWith("api/v1/projects") => ["📋 Project Management"],
                var route when route.StartsWith("api/v1/tasks") => ["✅ Task Execution"],
                _ => ["🔧 System"]
            };
        });
    }

    /// <summary>
    /// Configures JWT Bearer authentication scheme for the API.
    /// </summary>
    private static void ConfigureSecurity(SwaggerGenOptions options)
    {
        // Security scheme configuration for JWT Bearer tokens
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = """
                JWT Authorization header using the Bearer scheme.

                Enter your JWT token in the text input below.

                Example: `eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

                The token will be automatically prefixed with "Bearer " when sent in the Authorization header.
                """,
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
    }

    /// <summary>
    /// Registers custom Swagger filters for enhanced documentation.
    /// </summary>
    private static void ConfigureCustomFilters(SwaggerGenOptions options)
    {
        // Schema filters for custom type documentation
        options.SchemaFilter<ProblemDetailsSchemaFilter>();

        // Operation filters for enhanced endpoint documentation
        options.OperationFilter<CommonResponsesOperationFilter>();
        options.OperationFilter<AuthorizeOperationFilter>();

        // Document filters for overall API enhancement
        options.DocumentFilter<BoundedContextDocumentFilter>();
    }
}
