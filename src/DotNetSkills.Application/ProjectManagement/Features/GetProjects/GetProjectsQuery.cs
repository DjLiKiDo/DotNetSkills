namespace DotNetSkills.Application.ProjectManagement.Features.GetProjects;

/// <summary>
/// Query for retrieving projects with pagination and filtering.
/// Supports filtering by team, status, date ranges, and search.
/// </summary>
public record GetProjectsQuery(
    int Page,
    int PageSize,
    TeamId? TeamId,
    ProjectStatus? Status,
    DateTime? StartDateFrom,
    DateTime? StartDateTo,
    DateTime? EndDateFrom,
    DateTime? EndDateTo,
    string? Search) : IRequest<PagedProjectResponse>;
