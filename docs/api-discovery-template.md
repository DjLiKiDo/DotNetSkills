# API Discovery Template

## Endpoint Organization Pattern

### Standard Endpoint Structure
```csharp
public static class {BoundedContext}EndpointsExtensions
{
    public static void Map{BoundedContext}Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/{context}")
            .WithTags("{BoundedContext}")
            .WithOpenApi()
            .RequireAuthorization();
            
        // Map specific endpoint classes
        {Entity}Endpoints.Map(group);
        {Entity}ManagementEndpoints.Map(group);
    }
}
```

### Individual Endpoint Classes
```csharp
public static class {Entity}Endpoints
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", GetById)
            .WithName("Get{Entity}ById")
            .WithSummary("Retrieve a {entity} by ID")
            .WithDescription("Returns detailed information about a specific {entity}")
            .Produces<{Entity}Response>(200, "application/json")
            .Produces<ProblemDetails>(404, "application/problem+json")
            .Produces<ProblemDetails>(400, "application/problem+json");
    }
}
```

### Public API Surface Documentation
```csharp
/// <summary>
/// {BoundedContext} public API surface.
/// Contains all endpoints and contracts for {context} operations.
/// </summary>
/// <remarks>
/// Available operations:
/// - GET /api/v1/{context} - List {entities}
/// - GET /api/v1/{context}/{id} - Get {entity} by ID
/// - POST /api/v1/{context} - Create new {entity}
/// - PUT /api/v1/{context}/{id} - Update {entity}
/// - DELETE /api/v1/{context}/{id} - Delete {entity}
/// 
/// Required permissions: {permissions}
/// Rate limits: {limits}
/// </remarks>
public static class {BoundedContext}PublicApi
{
    public const string BasePath = "/api/v1/{context}";
    public static readonly string[] RequiredScopes = { "{scope1}", "{scope2}" };
}
```
