namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Operation filter to add common response types to all API operations.
/// This ensures consistent documentation of error responses across all endpoints.
/// </summary>
public class CommonResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common error responses that apply to all endpoints

        // 500 Internal Server Error - always possible
        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal server error occurred while processing the request",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/problem+json"] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                    }
                }
            });
        }

        // Add authentication/authorization responses for protected endpoints
        var requiresAuth = context.MethodInfo.GetCustomAttributes(true)
            .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true) ?? [])
            .Any(attr => attr.GetType().Name == "AuthorizeAttribute");

        if (requiresAuth)
        {
            // 401 Unauthorized
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Authentication is required to access this resource",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/problem+json"] = new()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                        }
                    }
                });
            }

            // 403 Forbidden
            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Insufficient permissions to access this resource",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/problem+json"] = new()
                        {
                            Schema = context.SchemaGenerator.GenerateSchema(typeof(ProblemDetails), context.SchemaRepository)
                        }
                    }
                });
            }
        }

        // Add validation error response for operations that accept request bodies
        if (operation.RequestBody != null && !operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Invalid request data or business rule validation failed",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/problem+json"] = new()
                    {
                        Schema = context.SchemaGenerator.GenerateSchema(typeof(HttpValidationProblemDetails), context.SchemaRepository)
                    }
                }
            });
        }

        // Enhance operation descriptions with bounded context information
        var routePath = context.ApiDescription.RelativePath ?? "";
        var boundedContext = GetBoundedContextFromRoute(routePath);

        if (!string.IsNullOrEmpty(boundedContext) && !string.IsNullOrEmpty(operation.Description))
        {
            operation.Description = $"{operation.Description}\n\n**Bounded Context**: {boundedContext}";
        }

        // Add rate limiting information (when implemented)
        operation.Extensions.TryAdd("x-rate-limit", new Microsoft.OpenApi.Any.OpenApiObject
        {
            ["requests"] = new Microsoft.OpenApi.Any.OpenApiInteger(100),
            ["period"] = new Microsoft.OpenApi.Any.OpenApiString("1 minute"),
            ["scope"] = new Microsoft.OpenApi.Any.OpenApiString("per user")
        });
    }

    private static string GetBoundedContextFromRoute(string routePath)
    {
        return routePath switch
        {
            var route when route.Contains("/users") => "User Management",
            var route when route.Contains("/teams") => "Team Collaboration",
            var route when route.Contains("/projects") => "Project Management",
            var route when route.Contains("/tasks") => "Task Execution",
            _ => ""
        };
    }
}
