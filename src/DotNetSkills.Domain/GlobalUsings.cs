// Global using statements for DotNetSkills.Domain
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Domain layer specific globals - Common
global using DotNetSkills.Domain.Common;
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;

// Domain layer specific globals - User Management
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;
global using DotNetSkills.Domain.UserManagement.Events;

// Domain layer specific globals - Team Collaboration
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Enums;
global using DotNetSkills.Domain.TeamCollaboration.Events;

// Domain layer specific globals - Project Management
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Events;

// Domain layer specific globals - Task Execution
global using DotNetSkills.Domain.TaskExecution.Entities;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;

// Alias to resolve naming conflicts
global using TaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;
global using Task = DotNetSkills.Domain.TaskExecution.Entities.Task;
