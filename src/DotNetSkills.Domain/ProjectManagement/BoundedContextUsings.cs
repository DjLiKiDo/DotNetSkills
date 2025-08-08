// Context-specific global usings for ProjectManagement bounded context

// ProjectManagement specific imports
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Events;
global using DotNetSkills.Domain.ProjectManagement.Services;

// Cross-context dependencies (explicit and minimal)
// ProjectManagement depends on UserManagement for User ID in domain events
global using DotNetSkills.Domain.UserManagement.ValueObjects;

// ProjectManagement depends on TeamCollaboration for Team association
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;