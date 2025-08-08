// Global using statements for DotNetSkills.Infrastructure
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Threading.Tasks;

// Microsoft Extensions
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Caching.Memory;

// Entity Framework Core
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

// MediatR
global using MediatR;

// Application layer interfaces
global using DotNetSkills.Application.Common.Abstractions;
global using DotNetSkills.Application.Common.Models;
global using DotNetSkills.Application.UserManagement.Contracts;
global using DotNetSkills.Application.TeamCollaboration.Contracts;
global using DotNetSkills.Application.ProjectManagement.Contracts;
global using DotNetSkills.Application.TaskExecution.Contracts;

// Domain layer references
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

// Infrastructure layer
global using DotNetSkills.Infrastructure.Persistence.Context;
global using DotNetSkills.Infrastructure.Persistence.Extensions;
global using DotNetSkills.Infrastructure.Repositories.Common;
global using DotNetSkills.Infrastructure.Repositories.UserManagement;
global using DotNetSkills.Infrastructure.Repositories.TeamCollaboration;
global using DotNetSkills.Infrastructure.Repositories.ProjectManagement;
global using DotNetSkills.Infrastructure.Repositories.TaskExecution;
global using DotNetSkills.Infrastructure.Services.Events;
global using DotNetSkills.Infrastructure.Common.Configuration;

// Health checks and options
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Options;
