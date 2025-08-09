// Context-specific global usings for UserManagement bounded context
// These usings are specific to UserManagement and don't leak to other contexts

// UserManagement specific imports
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;
global using DotNetSkills.Domain.UserManagement.Events;
global using DotNetSkills.Domain.UserManagement.Services;

// Cross-context dependencies (explicit and minimal)
// Only import what's absolutely necessary from other contexts
// Example: TeamCollaboration needs User entity
// global using DotNetSkills.Domain.TeamCollaboration.ValueObjects; // Only if needed
