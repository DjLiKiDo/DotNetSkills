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
