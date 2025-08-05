// Global using statements for DotNetSkills.Application
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Microsoft Extensions for logging
global using Microsoft.Extensions.Logging;

// Application layer specific globals - Common interfaces
// Removed global using DotNetSkills.Application.Common to avoid conflicts with MediatR
// Use fully qualified names when needed: DotNetSkills.Application.Common.IRequest


// Application layer specific globals - MediatR
global using MediatR;

// Application layer specific globals - AutoMapper
global using AutoMapper;

// Application layer specific globals - FluentValidation
global using FluentValidation;
global using FluentValidation.Results;
// Note: Using FluentValidation main namespace for validators and ValidationContext

// Application layer specific globals - Application Common
global using DotNetSkills.Application.Common.Behaviors;
global using DotNetSkills.Application.Common.Interfaces;
global using DotNetSkills.Application.Common.Models;

// Application layer specific globals - Domain references
global using DotNetSkills.Domain.Common;
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.Common.Rules;
global using DotNetSkills.Domain.Common.Validation;

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

// Project Management Application references
global using DotNetSkills.Application.ProjectManagement.Commands;
global using DotNetSkills.Application.ProjectManagement.Queries;
global using DotNetSkills.Application.ProjectManagement.DTOs;

// Task Execution Domain references (aliased to avoid conflicts)
global using DomainTask = DotNetSkills.Domain.TaskExecution.Entities.Task;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;

// Task Execution Application references
global using DotNetSkills.Application.TaskExecution.Commands;
global using DotNetSkills.Application.TaskExecution.Queries;
global using DotNetSkills.Application.TaskExecution.DTOs;
