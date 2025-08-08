// Global using statements for DotNetSkills.API
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Reflection;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Text.Json;

// ASP.NET Core
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;

// OpenAPI/Swagger
global using Microsoft.OpenApi.Models;
global using Swashbuckle.AspNetCore.SwaggerGen;

// API Configuration
global using DotNetSkills.API.Configuration.Swagger;
global using DotNetSkills.API.Configuration.Swagger.Filters;

// Authentication & Authorization (when added)
// global using Microsoft.AspNetCore.Authentication.JwtBearer;
// global using Microsoft.AspNetCore.Authorization;
// global using Microsoft.IdentityModel.Tokens;

// MediatR
global using MediatR;

// FluentValidation (when added)
// global using FluentValidation;

// Application layer references - User Management (Feature Slices)
global using DotNetSkills.Application.UserManagement.Features.CreateUser;
global using DotNetSkills.Application.UserManagement.Features.UpdateUser;
global using DotNetSkills.Application.UserManagement.Features.UpdateUserRole;
global using DotNetSkills.Application.UserManagement.Features.DeactivateUser;
global using DotNetSkills.Application.UserManagement.Features.ActivateUser;
global using DotNetSkills.Application.UserManagement.Features.DeleteUser;
global using DotNetSkills.Application.UserManagement.Features.GetUser;
global using DotNetSkills.Application.UserManagement.Features.GetUsers;
global using DotNetSkills.Application.UserManagement.Features.GetUserTeamMemberships;
global using DotNetSkills.Application.UserManagement.Contracts.Requests;
global using DotNetSkills.Application.UserManagement.Contracts.Responses;

// Application layer references - Team Collaboration (Feature Slices)
global using DotNetSkills.Application.TeamCollaboration.Features.CreateTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.UpdateTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.DeleteTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeam;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeams;
global using DotNetSkills.Application.TeamCollaboration.Features.AddTeamMember;
global using DotNetSkills.Application.TeamCollaboration.Features.RemoveTeamMember;
global using DotNetSkills.Application.TeamCollaboration.Features.UpdateMemberRole;
global using DotNetSkills.Application.TeamCollaboration.Features.GetTeamMembers;
global using DotNetSkills.Application.TeamCollaboration.Contracts.Requests;
global using DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

// Application layer references - Project Management (Feature Slices)
global using DotNetSkills.Application.ProjectManagement.Features.CreateProject;
global using DotNetSkills.Application.ProjectManagement.Features.UpdateProject;
global using DotNetSkills.Application.ProjectManagement.Features.ArchiveProject;
global using DotNetSkills.Application.ProjectManagement.Features.GetProject;
global using DotNetSkills.Application.ProjectManagement.Features.GetProjects;
global using DotNetSkills.Application.ProjectManagement.Features.GetProjectTasks;
global using DotNetSkills.Application.ProjectManagement.Features.CreateTaskInProject;
global using DotNetSkills.Application.ProjectManagement.Features.UpdateTaskInProject;
global using DotNetSkills.Application.ProjectManagement.Contracts.Requests;
global using DotNetSkills.Application.ProjectManagement.Contracts.Responses;

// Application layer references - Task Execution (Feature Slices)
global using DotNetSkills.Application.TaskExecution.Features.CreateTask;
global using DotNetSkills.Application.TaskExecution.Features.UpdateTask;
global using DotNetSkills.Application.TaskExecution.Features.DeleteTask;
global using DotNetSkills.Application.TaskExecution.Features.GetTask;
global using DotNetSkills.Application.TaskExecution.Features.GetTasks;
global using DotNetSkills.Application.TaskExecution.Features.AssignTask;
global using DotNetSkills.Application.TaskExecution.Features.UnassignTask;
global using DotNetSkills.Application.TaskExecution.Features.CreateSubtask;
global using DotNetSkills.Application.TaskExecution.Features.UpdateSubtask;
global using DotNetSkills.Application.TaskExecution.Features.UpdateTaskStatus;
global using DotNetSkills.Application.TaskExecution.Features.GetTaskSubtasks;
global using DotNetSkills.Application.TaskExecution.Contracts.Responses;

// Application layer references - Common
global using DotNetSkills.Application.Common.Models;

// API layer endpoint extensions
global using DotNetSkills.API.Endpoints.UserManagement;
global using DotNetSkills.API.Endpoints.TeamCollaboration;
global using DotNetSkills.API.Endpoints.ProjectManagement;
global using DotNetSkills.API.Endpoints.TaskExecution;

// API layer middleware
global using DotNetSkills.API.Middleware;

// Domain references (for basic types and exceptions)
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Enums;
