// Global using statements for DotNetSkills.API.UnitTests
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

// ASP.NET Core testing
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Mvc.Testing;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;

// API layer references for testing
// global using DotNetSkills.API.Endpoints;

// Application layer references (for integration tests)
// global using DotNetSkills.Application.Common.Models;
// global using DotNetSkills.Application.Users.Commands;

// Domain layer references
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
