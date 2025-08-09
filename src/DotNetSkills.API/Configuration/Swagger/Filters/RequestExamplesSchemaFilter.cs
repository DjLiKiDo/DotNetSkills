namespace DotNetSkills.API.Configuration.Swagger.Filters;

/// <summary>
/// Adds example payloads for common request DTOs to showcase enum string values.
/// </summary>
public sealed class RequestExamplesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var t = context.Type;

        if (t.FullName == "DotNetSkills.Application.TaskExecution.Contracts.Responses.CreateTaskRequest")
        {
            schema.Example = Example(new
            {
                title = "Implement OAuth login",
                description = "Add OAuth2 login with Google",
                projectId = "123e4567-e89b-12d3-a456-426614174000",
                priority = "High",
                parentTaskId = (string?)null,
                estimatedHours = 8,
                dueDate = DateTime.UtcNow.AddDays(7).ToString("O"),
                assignedUserId = "123e4567-e89b-12d3-a456-426614174111"
            });
        }
        else if (t.FullName == "DotNetSkills.Application.TaskExecution.Contracts.Responses.UpdateTaskRequest")
        {
            schema.Example = Example(new
            {
                title = "Implement OAuth login",
                description = "Include Google + GitHub",
                priority = "Medium",
                estimatedHours = 12,
                dueDate = DateTime.UtcNow.AddDays(10).ToString("O")
            });
        }
        else if (t.FullName == "DotNetSkills.Application.TaskExecution.Contracts.Responses.UpdateTaskStatusRequest")
        {
            schema.Example = Example(new
            {
                status = "InProgress",
                actualHours = 3
            });
        }
        else if (t.FullName == "DotNetSkills.Application.ProjectManagement.Contracts.Requests.CreateProjectRequest")
        {
            schema.Example = Example(new
            {
                name = "Website Redesign",
                description = "Refresh UI and UX",
                teamId = "123e4567-e89b-12d3-a456-426614174222",
                status = "Active",
                startDate = DateTime.UtcNow.ToString("O"),
                endDate = DateTime.UtcNow.AddMonths(2).ToString("O")
            });
        }
        else if (t.FullName == "DotNetSkills.Application.ProjectManagement.Contracts.Requests.UpdateProjectRequest")
        {
            schema.Example = Example(new
            {
                name = "Website Redesign Phase 2",
                description = "Accessibility improvements",
                status = "OnHold",
                startDate = (string?)null,
                endDate = DateTime.UtcNow.AddMonths(3).ToString("O")
            });
        }
        else if (t.FullName == "DotNetSkills.Application.TeamCollaboration.Contracts.Requests.CreateTeamRequest")
        {
            schema.Example = Example(new
            {
                name = "Platform Team",
                description = "Core platform services",
                status = "Active"
            });
        }
        else if (t.FullName == "DotNetSkills.Application.TeamCollaboration.Contracts.Requests.UpdateTeamRequest")
        {
            schema.Example = Example(new
            {
                name = "Platform Team",
                description = "Core services and SDKs",
                status = "Active"
            });
        }
    }

    private static Microsoft.OpenApi.Any.OpenApiObject Example(object anon)
    {
        var obj = new Microsoft.OpenApi.Any.OpenApiObject();
        foreach (var prop in anon.GetType().GetProperties())
        {
            var name = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);
            var value = prop.GetValue(anon);
            obj[name] = value switch
            {
                null => new Microsoft.OpenApi.Any.OpenApiNull(),
                string s => new Microsoft.OpenApi.Any.OpenApiString(s),
                int i => new Microsoft.OpenApi.Any.OpenApiInteger(i),
                long l => new Microsoft.OpenApi.Any.OpenApiLong(l),
                bool b => new Microsoft.OpenApi.Any.OpenApiBoolean(b),
                _ => new Microsoft.OpenApi.Any.OpenApiString(value.ToString() ?? string.Empty)
            };
        }
        return obj;
    }
}
