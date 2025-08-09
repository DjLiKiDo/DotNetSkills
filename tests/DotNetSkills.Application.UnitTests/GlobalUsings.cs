// Global using statements for DotNetSkills.Application.UnitTests
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Testing frameworks
global using Xunit;
global using FluentAssertions;
global using Microsoft.Extensions.DependencyInjection;
global using Moq;

// Application layer references for testing
// global using DotNetSkills.Application.Common.Interfaces;
// global using DotNetSkills.Application.Common.Models;
// global using DotNetSkills.Application.Users.Commands;
// global using DotNetSkills.Application.Users.Queries;

// Domain layer references (for DTOs and mapping tests)
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

// Task Execution Domain references (aliased to avoid conflicts)
global using DomainTask = DotNetSkills.Domain.TaskExecution.Entities.Task;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
// Aliases for conflicting enum names
global using DomainTaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;

// AutoMapper
global using AutoMapper;
