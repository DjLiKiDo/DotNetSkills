// Context-specific global usings for TeamCollaboration bounded context

// TeamCollaboration specific imports
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Enums;
global using DotNetSkills.Domain.TeamCollaboration.Events;
global using DotNetSkills.Domain.TeamCollaboration.Services;

// Cross-context dependencies (explicit and minimal)
// TeamCollaboration depends on UserManagement for User entity
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;
