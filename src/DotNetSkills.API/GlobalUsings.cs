// Global using statements for DotNetSkills.API
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// ASP.NET Core
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

// OpenAPI/Swagger (when added)
// global using Microsoft.OpenApi.Models;

// Authentication & Authorization (when added)
// global using Microsoft.AspNetCore.Authentication.JwtBearer;
// global using Microsoft.AspNetCore.Authorization;
// global using Microsoft.IdentityModel.Tokens;

// MediatR (when added)
// global using MediatR;

// FluentValidation (when added)
// global using FluentValidation;

// Application layer references - User Management
global using DotNetSkills.Application.UserManagement.Commands;
global using DotNetSkills.Application.UserManagement.Queries;
global using DotNetSkills.Application.UserManagement.DTOs;

// Application layer references - Team Collaboration
// global using DotNetSkills.Application.TeamCollaboration.Commands;
// global using DotNetSkills.Application.TeamCollaboration.Queries;
// global using DotNetSkills.Application.TeamCollaboration.DTOs;

// Application layer references - Project Management
// global using DotNetSkills.Application.ProjectManagement.Commands;
// global using DotNetSkills.Application.ProjectManagement.Queries;
// global using DotNetSkills.Application.ProjectManagement.DTOs;

// Application layer references - Task Execution
// global using DotNetSkills.Application.TaskExecution.Commands;
// global using DotNetSkills.Application.TaskExecution.Queries;
// global using DotNetSkills.Application.TaskExecution.DTOs;

// Application layer references - Common
// global using DotNetSkills.Application.Common.Models;
// global using DotNetSkills.Application.Common.Interfaces;

// API layer endpoint extensions
global using DotNetSkills.API.Endpoints.UserManagement;

// Domain references (for basic types and exceptions)
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
