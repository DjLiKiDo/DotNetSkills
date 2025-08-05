namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Document filter to enhance the overall OpenAPI document with bounded context information.
/// This filter organizes the API documentation by domain contexts and adds additional metadata.
/// </summary>
public class BoundedContextDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Enhance the API description with bounded context overview
        if (swaggerDoc.Info != null)
        {
            swaggerDoc.Info.Extensions.TryAdd("x-bounded-contexts", new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["userManagement"] = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["description"] = new Microsoft.OpenApi.Any.OpenApiString("Manages user accounts, roles, and authentication"),
                    ["endpoints"] = new Microsoft.OpenApi.Any.OpenApiInteger(GetEndpointCount(swaggerDoc, "/users")),
                    ["aggregateRoot"] = new Microsoft.OpenApi.Any.OpenApiString("User")
                },
                ["teamCollaboration"] = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["description"] = new Microsoft.OpenApi.Any.OpenApiString("Handles team formation, membership, and collaboration"),
                    ["endpoints"] = new Microsoft.OpenApi.Any.OpenApiInteger(GetEndpointCount(swaggerDoc, "/teams")),
                    ["aggregateRoot"] = new Microsoft.OpenApi.Any.OpenApiString("Team")
                },
                ["projectManagement"] = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["description"] = new Microsoft.OpenApi.Any.OpenApiString("Manages project lifecycle, planning, and coordination"),
                    ["endpoints"] = new Microsoft.OpenApi.Any.OpenApiInteger(GetEndpointCount(swaggerDoc, "/projects")),
                    ["aggregateRoot"] = new Microsoft.OpenApi.Any.OpenApiString("Project")
                },
                ["taskExecution"] = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["description"] = new Microsoft.OpenApi.Any.OpenApiString("Handles task management, assignment, and execution tracking"),
                    ["endpoints"] = new Microsoft.OpenApi.Any.OpenApiInteger(GetEndpointCount(swaggerDoc, "/tasks")),
                    ["aggregateRoot"] = new Microsoft.OpenApi.Any.OpenApiString("Task")
                }
            });

            // Add architectural patterns information
            swaggerDoc.Info.Extensions.TryAdd("x-architecture-patterns", new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["cleanArchitecture"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
                ["domainDrivenDesign"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
                ["cqrs"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true),
                ["eventSourcing"] = new Microsoft.OpenApi.Any.OpenApiBoolean(false),
                ["stronglyTypedIds"] = new Microsoft.OpenApi.Any.OpenApiBoolean(true)
            });

            // Add technology stack information
            swaggerDoc.Info.Extensions.TryAdd("x-technology-stack", new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["framework"] = new Microsoft.OpenApi.Any.OpenApiString(".NET 9"),
                ["language"] = new Microsoft.OpenApi.Any.OpenApiString("C# 13"),
                ["apiPattern"] = new Microsoft.OpenApi.Any.OpenApiString("Minimal APIs"),
                ["mediatr"] = new Microsoft.OpenApi.Any.OpenApiString("Command/Query handling"),
                ["entityFramework"] = new Microsoft.OpenApi.Any.OpenApiString("Data access layer"),
                ["fluentValidation"] = new Microsoft.OpenApi.Any.OpenApiString("Input validation"),
                ["authentication"] = new Microsoft.OpenApi.Any.OpenApiString("JWT Bearer tokens")
            });
        }

        // Sort tags for better organization in Swagger UI
        if (swaggerDoc.Tags != null && swaggerDoc.Tags.Any())
        {
            var sortedTags = swaggerDoc.Tags
                .OrderBy(tag => GetTagOrder(tag.Name))
                .ThenBy(tag => tag.Name)
                .ToList();

            swaggerDoc.Tags = sortedTags;
        }

        // Add custom headers for API versioning and request tracking
        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                // Add common headers
                operation.Parameters ??= new List<OpenApiParameter>();

                // Request ID header for tracking
                if (!operation.Parameters.Any(p => p.Name == "X-Request-ID"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "X-Request-ID",
                        In = ParameterLocation.Header,
                        Required = false,
                        Description = "Optional unique identifier for request tracking and correlation",
                        Schema = new OpenApiSchema { Type = "string", Format = "uuid" }
                    });
                }

                // API Version header
                if (!operation.Parameters.Any(p => p.Name == "X-API-Version"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = "X-API-Version",
                        In = ParameterLocation.Header,
                        Required = false,
                        Description = "API version override (defaults to v1)",
                        Schema = new OpenApiSchema { Type = "string", Default = new Microsoft.OpenApi.Any.OpenApiString("v1") }
                    });
                }
            }
        }
    }

    private static int GetEndpointCount(OpenApiDocument document, string pathSegment)
    {
        return document.Paths.Keys.Count(path => path.Contains(pathSegment, StringComparison.OrdinalIgnoreCase));
    }

    private static int GetTagOrder(string tagName)
    {
        return tagName switch
        {
            var name when name.Contains("User") => 1,
            var name when name.Contains("Team") => 2,
            var name when name.Contains("Project") => 3,
            var name when name.Contains("Task") => 4,
            _ => 99
        };
    }
}
