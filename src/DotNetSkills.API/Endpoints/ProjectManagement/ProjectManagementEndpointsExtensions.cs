namespace DotNetSkills.API.Endpoints.ProjectManagement;

/// <summary>
/// Extension methods for registering Project Management bounded context endpoints.
/// This class provides a clean way to register all project-related endpoints following the bounded context pattern.
/// </summary>
public static class ProjectManagementEndpointsExtensions
{
    /// <summary>
    /// Registers all Project Management bounded context endpoints.
    /// This method groups all project-related endpoint registrations in one place.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder MapProjectManagementEndpoints(this IEndpointRouteBuilder app)
    {
        // Register Project CRUD endpoints
        app.MapProjectEndpoints();
        
        // Register Project-Task relationship endpoints
        app.MapProjectTaskEndpoints();
        
        return app;
    }
}