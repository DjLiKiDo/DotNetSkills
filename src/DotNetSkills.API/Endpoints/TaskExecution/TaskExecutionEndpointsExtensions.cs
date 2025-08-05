namespace DotNetSkills.API.Endpoints.TaskExecution;

/// <summary>
/// Extension methods for registering Task Execution bounded context endpoints.
/// This class provides a clean way to register all task-related endpoints following the bounded context pattern.
/// </summary>
public static class TaskExecutionEndpointsExtensions
{
    /// <summary>
    /// Registers all Task Execution bounded context endpoints.
    /// This method groups all task-related endpoint registrations in one place.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder MapTaskExecutionEndpoints(this IEndpointRouteBuilder app)
    {
        // Register Task CRUD endpoints
        app.MapTaskEndpoints();

        // Register Task Assignment and Subtask endpoints
        app.MapTaskAssignmentEndpoints();

        return app;
    }
}
