using DotNetSkills.Application.Common.Mappings;
using DotNetSkills.Application.ProjectManagement.Contracts.Responses;

namespace DotNetSkills.Application.ProjectManagement.Mappings;

/// <summary>
/// AutoMapper profile for Project Management bounded context.
/// Provides mappings between domain entities and DTOs with proper value object handling.
/// </summary>
public class ProjectMappingProfile : MappingProfile
{
    /// <summary>
    /// Initializes a new instance of the ProjectMappingProfile class.
    /// Sets up all mappings for Project Management entities and DTOs.
    /// </summary>
    public ProjectMappingProfile()
    {
        CreateProjectMappings();
        CreatePagedResponseMappings();
    }

    /// <summary>
    /// Creates mappings for Project entity and related DTOs.
    /// Handles project data conversion and strongly-typed ID mappings.
    /// </summary>
    private void CreateProjectMappings()
    {
        // Project to ProjectResponse mapping with custom team name parameter
        CreateMap<Project, ProjectResponse>()
            .ConstructUsing((project, context) => 
            {
                var teamName = context.Items.ContainsKey("TeamName") 
                    ? context.Items["TeamName"] as string ?? "Unknown" 
                    : "Unknown";
                    
                return new ProjectResponse(
                    project.Id.Value,
                    project.Name,
                    project.Description,
                    project.Status.ToString(),
                    project.TeamId.Value,
                    teamName,
                    project.StartDate,
                    project.EndDate,
                    project.PlannedEndDate,
                    project.CreatedAt,
                    project.UpdatedAt);
            })
            .ForAllMembers(opt => opt.Ignore()); // Use constructor mapping only
    }

    /// <summary>
    /// Creates paginated response mappings for project collections.
    /// </summary>
    private void CreatePagedResponseMappings()
    {
        CreatePagedMapping<Project, ProjectResponse>();

        // Specific mapping for PagedProjectResponse using correct property names
        CreateMap<PagedResponse<Project>, PagedProjectResponse>()
            .ConvertUsing((src, dest, context) => new PagedProjectResponse(
                Projects: context.Mapper.Map<List<ProjectResponse>>(src.Data).AsReadOnly(),
                TotalCount: src.TotalCount,
                Page: src.PageNumber,
                PageSize: src.PageSize));
    }
}