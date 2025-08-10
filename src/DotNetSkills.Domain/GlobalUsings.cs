// Global using statements for DotNetSkills.Domain
global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Domain layer specific globals - Common only
// These are shared across all bounded contexts
global using DotNetSkills.Domain.Common;
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.Common.Extensions;
global using DotNetSkills.Domain.Common.Rules;
global using DotNetSkills.Domain.Common.Validation;

// REMOVED: Cross-bounded-context globals to maintain loose coupling
// Each bounded context should explicitly import what it needs
// This prevents tight coupling and maintains clear boundaries

// Domain layer specific globals - Project Management
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Events;
global using DotNetSkills.Domain.ProjectManagement.Services;

// Domain layer specific globals - Task Execution
global using DotNetSkills.Domain.TaskExecution.Entities;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;
global using DotNetSkills.Domain.TaskExecution.Services;

// Alias to resolve naming conflicts
global using TaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;
global using Task = DotNetSkills.Domain.TaskExecution.Entities.Task;
