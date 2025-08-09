namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Adds example values for enum parameters (query/path) so that Swagger UI shows string examples.
/// </summary>
public sealed class EnumParameterExampleOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters is null || operation.Parameters.Count == 0)
            return;

        foreach (var parameter in operation.Parameters)
        {
            // Resolve parameter type
            var apiParam = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(p => string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));
            var clrType = apiParam?.Type;
            var underlying = clrType is null ? null : Nullable.GetUnderlyingType(clrType) ?? clrType;

            if (underlying is null || !underlying.IsEnum)
                continue;

            var names = Enum.GetNames(underlying);
            if (names.Length == 0)
                continue;

            parameter.Schema ??= new OpenApiSchema { Type = "string" };
            parameter.Schema.Type = "string";
            parameter.Schema.Enum = names
                .Select(n => (Microsoft.OpenApi.Any.IOpenApiAny)new Microsoft.OpenApi.Any.OpenApiString(n))
                .ToList();

            // Provide a concrete example
            parameter.Example = new Microsoft.OpenApi.Any.OpenApiString(names[0]);
        }
    }
}
