namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Ensures enums are documented as strings with allowed values and an example.
/// Respects JsonStringEnumConverter behavior in the API by reflecting enum names.
/// </summary>
public sealed class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!type.IsEnum)
        {
            return;
        }

        var names = Enum.GetNames(type);

        // Force schema to be string with enum name values
        schema.Type = "string";
        schema.Format = null;
        schema.Enum = names
            .Select(n => (Microsoft.OpenApi.Any.IOpenApiAny)new Microsoft.OpenApi.Any.OpenApiString(n))
            .ToList();

        // Provide a stable example (first value if available)
        if (names.Length > 0)
        {
            schema.Example = new Microsoft.OpenApi.Any.OpenApiString(names[0]);
        }

        // Append allowed values to description for quick reference
        var allowed = string.Join(", ", names);
        var suffix = $"Allowed values: {allowed}.";
        schema.Description = string.IsNullOrWhiteSpace(schema.Description)
            ? suffix
            : $"{schema.Description}\n\n{suffix}";
    }
}
