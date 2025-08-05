// Global using statements for DotNetSkills.Infrastructure.UnitTests
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

// Entity Framework Core testing (when added)
// global using Microsoft.EntityFrameworkCore;
// global using Microsoft.EntityFrameworkCore.InMemory;

// Infrastructure layer references for testing
// global using DotNetSkills.Infrastructure.Persistence;
// global using DotNetSkills.Infrastructure.Repositories;

// Application layer interfaces (tested implementations)
// global using DotNetSkills.Application.Common.Interfaces;

// Domain layer references
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;

// User Management Domain references
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Enums;
