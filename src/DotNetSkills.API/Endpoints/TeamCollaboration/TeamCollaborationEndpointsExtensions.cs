namespace DotNetSkills.API.Endpoints.TeamCollaboration;

/// <summary>
/// Extension methods for registering Team Collaboration bounded context endpoints.
/// This class provides a clean way to register all team-related endpoints following the bounded context pattern.
/// </summary>
public static class TeamCollaborationEndpointsExtensions
{
    /// <summary>
    /// Registers all Team Collaboration bounded context endpoints.
    /// This method groups all team-related endpoint registrations in one place.
    /// </summary>
    /// <param name="app">The endpoint route builder to configure.</param>
    /// <returns>The endpoint route builder for method chaining.</returns>
    public static IEndpointRouteBuilder MapTeamCollaborationEndpoints(this IEndpointRouteBuilder app)
    {
        // Register Team CRUD endpoints
        app.MapTeamEndpoints();

        // Register Team Member management endpoints
        app.MapTeamMemberEndpoints();

        return app;
    }
}
