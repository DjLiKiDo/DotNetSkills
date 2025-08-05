// Global using statements for DotNetSkills.Infrastructure
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

// Entity Framework Core (when added)
// global using Microsoft.EntityFrameworkCore;

// Application layer interfaces (Infrastructure implements these)
// global using DotNetSkills.Application.Common.Interfaces;
// global using DotNetSkills.Application.Common.Models;

// Domain layer references (for repository implementations)
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;

// User Management Domain references
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;

// Team Collaboration Domain references
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Enums;

// Project Management Domain references
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;

// Task Execution Domain references
global using DotNetSkills.Domain.TaskExecution.Entities;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
