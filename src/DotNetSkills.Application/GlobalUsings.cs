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
global using DotNetSkills.Application.Common.Abstractions;
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

// User Management Application references - Feature Slices
global using DotNetSkills.Application.UserManagement.Features.CreateUser;
global using DotNetSkills.Application.UserManagement.Features.UpdateUser;
global using DotNetSkills.Application.UserManagement.Features.UpdateUserRole;
global using DotNetSkills.Application.UserManagement.Features.DeactivateUser;
global using DotNetSkills.Application.UserManagement.Features.GetUser;
global using DotNetSkills.Application.UserManagement.Features.GetUsers;
global using DotNetSkills.Application.UserManagement.Contracts;
global using DotNetSkills.Application.UserManagement.Contracts.Requests;
global using DotNetSkills.Application.UserManagement.Contracts.Responses;

// Team Collaboration Domain references
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Enums;
global using DotNetSkills.Domain.TeamCollaboration.Events;

// Team Collaboration Application references - Feature Slices
global using DotNetSkills.Application.TeamCollaboration.Features.CreateTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.UpdateTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.DeleteTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeams;
global using DotNetSkills.Application.TeamCollaboration.Features.AddTeamMember;
global using DotNetSkills.Application.TeamCollaboration.Features.RemoveTeamMember;
global using DotNetSkills.Application.TeamCollaboration.Features.UpdateMemberRole;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeamMembers;
global using DotNetSkills.Application.TeamCollaboration.Contracts;
global using DotNetSkills.Application.TeamCollaboration.Contracts.Requests;
global using DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

// Project Management Domain references
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Events;

// Project Management Application references - Feature Slices
global using DotNetSkills.Application.ProjectManagement.Features.CreateProject;
global using DotNetSkills.Application.ProjectManagement.Features.UpdateProject;
global using DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;
global using DotNetSkills.Application.ProjectManagement.Features.GetProject;
global using DotNetSkills.Application.ProjectManagement.Features.GetProjects;
global using DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;
global using DotNetSkills.Application.ProjectManagement.Features.CreateTaskInProject;
global using DotNetSkills.Application.ProjectManagement.Features.UpdateTaskInProject;
global using DotNetSkills.Application.ProjectManagement.Contracts;
global using DotNetSkills.Application.ProjectManagement.Contracts.Requests;
global using DotNetSkills.Application.ProjectManagement.Contracts.Responses;

// Task Execution Domain references (aliased to avoid conflicts)
global using DomainTask = DotNetSkills.Domain.TaskExecution.Entities.Task;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Events;
// Aliases for conflicting enum names
global using DomainTaskStatus = DotNetSkills.Domain.TaskExecution.Enums.TaskStatus;

// Task Execution Application references - Feature Slices
global using DotNetSkills.Application.TaskExecution.Features.CreateTask;
global using DotNetSkills.Application.TaskExecution.Features.UpdateTask;
global using DotNetSkills.Application.TaskExecution.Features.DeleteTask;
global using DotNetSkills.Application.TaskExecution.Features.GetTask;
global using DotNetSkills.Application.TaskExecution.Features.GetTasks;
global using DotNetSkills.Application.TaskExecution.Features.AssignTask;
global using DotNetSkills.Application.TaskExecution.Features.UpdateTaskStatus;
global using DotNetSkills.Application.TaskExecution.Features.GetTaskSubtasks;
global using DotNetSkills.Application.TaskExecution.Contracts;
global using DotNetSkills.Application.TaskExecution.Contracts.Responses;
