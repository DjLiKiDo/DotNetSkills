// Global using statements for DotNetSkills.Application
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Application layer specific globals - Common interfaces
global using DotNetSkills.Application.Common;

// Application layer specific globals - AutoMapper (when added)
// global using AutoMapper;

// Application layer specific globals - FluentValidation (when added)
// global using FluentValidation;

// Application layer specific globals - Domain references
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;

// User Management Domain references
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;
global using DotNetSkills.Domain.UserManagement.Events;

// User Management Application references
global using DotNetSkills.Application.UserManagement.Commands;
global using DotNetSkills.Application.UserManagement.Queries;
global using DotNetSkills.Application.UserManagement.DTOs;

// Team Collaboration Domain references
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Enums;
global using DotNetSkills.Domain.TeamCollaboration.Events;

// Team Collaboration Application references
global using DotNetSkills.Application.TeamCollaboration.Commands;
global using DotNetSkills.Application.TeamCollaboration.Queries;
global using DotNetSkills.Application.TeamCollaboration.DTOs;

// Project Management Domain references
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Events;

// Task Execution Domain references (aliased to avoid conflicts)
global using DomainTask = DotNetSkills.Domain.TaskExecution.Entities.Task;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;
