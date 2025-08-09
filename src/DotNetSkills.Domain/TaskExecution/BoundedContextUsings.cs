// Context-specific global usings for TaskExecution bounded context

// TaskExecution specific imports
global using DotNetSkills.Domain.TaskExecution.Entities;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;
global using DotNetSkills.Domain.TaskExecution.Services;

// Cross-context dependencies (explicit and minimal)
// TaskExecution depends on UserManagement for User assignment and domain events
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Entities;

// TaskExecution depends on ProjectManagement for Project association
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;