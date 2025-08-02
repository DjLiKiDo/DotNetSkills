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

// Application layer references (for Commands/Queries/DTOs)
// global using DotNetSkills.Application.Common.Models;
// global using DotNetSkills.Application.Users.Commands;
// global using DotNetSkills.Application.Users.Queries;
// global using DotNetSkills.Application.Teams.Commands;
// global using DotNetSkills.Application.Teams.Queries;
// global using DotNetSkills.Application.Projects.Commands;
// global using DotNetSkills.Application.Projects.Queries;
// global using DotNetSkills.Application.Tasks.Commands;
// global using DotNetSkills.Application.Tasks.Queries;

// MediatR (when added)
// global using MediatR;

// Domain references (for basic types and exceptions)
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
