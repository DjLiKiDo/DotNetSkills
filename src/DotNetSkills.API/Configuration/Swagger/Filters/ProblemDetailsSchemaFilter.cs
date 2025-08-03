namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Schema filter to ensure ProblemDetails is properly documented in OpenAPI.
/// This filter adds comprehensive documentation for RFC 7807 Problem Details responses.
/// </summary>
public class ProblemDetailsSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(ProblemDetails))
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["type"] = new Microsoft.OpenApi.Any.OpenApiString("https://httpstatuses.com/400"),
                ["title"] = new Microsoft.OpenApi.Any.OpenApiString("Bad Request"),
                ["status"] = new Microsoft.OpenApi.Any.OpenApiInteger(400),
                ["detail"] = new Microsoft.OpenApi.Any.OpenApiString("The request contains invalid parameters or violates business rules."),
                ["instance"] = new Microsoft.OpenApi.Any.OpenApiString("/api/v1/users"),
                ["requestId"] = new Microsoft.OpenApi.Any.OpenApiString("0HN7NOPQRSTUVWXYZ012345"),
                ["timestamp"] = new Microsoft.OpenApi.Any.OpenApiString("2025-08-03T10:30:00.000Z"),
                ["errors"] = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["Name"] = new Microsoft.OpenApi.Any.OpenApiArray
                    {
                        new Microsoft.OpenApi.Any.OpenApiString("The Name field is required.")
                    },
                    ["Email"] = new Microsoft.OpenApi.Any.OpenApiArray
                    {
                        new Microsoft.OpenApi.Any.OpenApiString("The Email field must be a valid email address.")
                    }
                }
            };
            
            schema.Description = """
                RFC 7807 Problem Details response format.
                
                Provides structured error information including:
                - type: URI identifying the problem type
                - title: Short, human-readable summary
                - status: HTTP status code
                - detail: Human-readable explanation specific to this occurrence
                - instance: URI reference identifying the specific occurrence
                - requestId: Unique identifier for request tracing
                - timestamp: When the error occurred
                - errors: Validation errors (when applicable)
                """;
        }
    }
}
