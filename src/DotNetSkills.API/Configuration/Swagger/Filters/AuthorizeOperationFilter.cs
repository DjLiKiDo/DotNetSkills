namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Operation filter to automatically add security requirements to endpoints that require authorization.
/// This filter detects authorization requirements and applies the appropriate JWT security scheme.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint requires authorization
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .OfType<object>()
            .Any(attr => IsAuthorizeAttribute(attr)) ?? false;

        // Also check for RequireAuthorization calls on the endpoint
        var isProtectedEndpoint = IsEndpointProtected(context);

        if (hasAuthorize || isProtectedEndpoint)
        {
            // Add security requirement for JWT Bearer token
            operation.Security ??= new List<OpenApiSecurityRequirement>();

            var jwtSecurityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };

            // Only add if not already present
            if (!operation.Security.Any(req => req.ContainsKey(jwtSecurityRequirement.Keys.First())))
            {
                operation.Security.Add(jwtSecurityRequirement);
            }

            // Extract authorization policies and roles
            var authorizationInfo = ExtractAuthorizationInfo(context);
            if (!string.IsNullOrEmpty(authorizationInfo))
            {
                // Add authorization information to the operation description
                var authDescription = $"\n\n**Authorization**: {authorizationInfo}";
                operation.Description = (operation.Description ?? "") + authDescription;

                // Add to operation extensions for programmatic access
                operation.Extensions.TryAdd("x-authorization-policy",
                    new Microsoft.OpenApi.Any.OpenApiString(authorizationInfo));
            }

            // Add lock icon indicator for Swagger UI
            operation.Extensions.TryAdd("x-security-scopes",
                new Microsoft.OpenApi.Any.OpenApiArray());
        }
    }

    private static bool IsAuthorizeAttribute(object attribute)
    {
        var attributeTypeName = attribute.GetType().Name;
        return attributeTypeName == "AuthorizeAttribute" ||
               attributeTypeName.EndsWith("AuthorizeAttribute");
    }

    private static bool IsEndpointProtected(OperationFilterContext context)
    {
        // This is a heuristic check - in a real implementation, you might need to
        // examine the endpoint metadata more thoroughly
        var apiDescription = context.ApiDescription;

        // Check for authorization metadata in endpoint
        return apiDescription.ActionDescriptor.EndpointMetadata?
            .Any(metadata => metadata.GetType().Name.Contains("Authorize")) ?? false;
    }

    private static string ExtractAuthorizationInfo(OperationFilterContext context)
    {
        // Extract policy and role information from authorization attributes
        var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true))
            .Where(attr => IsAuthorizeAttribute(attr))
            .ToList() ?? new List<object>();

        if (!attributes.Any())
        {
            return "Requires authentication";
        }

        var policies = new List<string>();
        var roles = new List<string>();

        foreach (var attr in attributes)
        {
            // Use reflection to extract policy and roles information
            var attrType = attr.GetType();

            var policyProperty = attrType.GetProperty("Policy");
            if (policyProperty?.GetValue(attr) is string policy && !string.IsNullOrEmpty(policy))
            {
                policies.Add(policy);
            }

            var rolesProperty = attrType.GetProperty("Roles");
            if (rolesProperty?.GetValue(attr) is string rolesList && !string.IsNullOrEmpty(rolesList))
            {
                roles.AddRange(rolesList.Split(',').Select(r => r.Trim()));
            }
        }

        var authInfo = new List<string>();

        if (policies.Any())
        {
            authInfo.Add($"Policies: {string.Join(", ", policies)}");
        }

        if (roles.Any())
        {
            authInfo.Add($"Roles: {string.Join(", ", roles)}");
        }

        return authInfo.Any() ? string.Join(", ", authInfo) : "Requires authentication";
    }
}
