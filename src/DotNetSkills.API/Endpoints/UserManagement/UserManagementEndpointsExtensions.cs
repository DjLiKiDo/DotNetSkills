namespace DotNetSkills.API.Endpoints.UserManagement;

/// <summary>
/// Extension methods for registering User Management bounded context endpoints.
/// This class provides a clean way to register all user-related endpoints following the bounded context pattern.
/// </summary>
public static class UserManagementEndpointsExtensions
{
    /// <summary>
    /// Registers all User Management bounded context endpoints.
    /// This method groups all user-related endpoint registrations in one place.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder MapUserManagementEndpoints(this IEndpointRouteBuilder app)
    {
        // Register User CRUD endpoints
        app.MapUserEndpoints();

        // Register User Account management endpoints
        app.MapUserAccountEndpoints();

        return app;
    }
}
