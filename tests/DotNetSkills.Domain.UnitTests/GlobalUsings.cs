// Testing frameworks
global using Xunit;
global using FluentAssertions;
global using Moq;

// Domain layer imports
global using DotNetSkills.Domain.Common;
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;
global using DotNetSkills.Domain.Common.Validation;
global using DotNetSkills.Domain.Common.Rules;
global using DotNetSkills.Domain.Common.Extensions;
global using DotNetSkills.Domain;

// UserManagement bounded context
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
global using DotNetSkills.Domain.UserManagement.Events;
global using DotNetSkills.Domain.UserManagement.Enums;
global using DotNetSkills.Domain.UserManagement.Services;

// TeamCollaboration bounded context
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
global using DotNetSkills.Domain.TeamCollaboration.Events;
global using DotNetSkills.Domain.TeamCollaboration.Enums;
global using DotNetSkills.Domain.TeamCollaboration.Services;

// ProjectManagement bounded context
global using DotNetSkills.Domain.ProjectManagement.Entities;
global using DotNetSkills.Domain.ProjectManagement.ValueObjects;
global using DotNetSkills.Domain.ProjectManagement.Events;
global using DotNetSkills.Domain.ProjectManagement.Enums;
global using DotNetSkills.Domain.ProjectManagement.Services;

// TaskExecution bounded context
global using DotNetSkills.Domain.TaskExecution.Entities;
global using DotNetSkills.Domain.TaskExecution.ValueObjects;
global using DotNetSkills.Domain.TaskExecution.Events;
global using DotNetSkills.Domain.TaskExecution.Enums;
global using DotNetSkills.Domain.TaskExecution.Services;

// Test infrastructure
global using DotNetSkills.Domain.UnitTests.Common;
global using DotNetSkills.Domain.UnitTests.Builders;

// System imports
global using System;
global using System.Collections.Generic;
global using System.Linq;