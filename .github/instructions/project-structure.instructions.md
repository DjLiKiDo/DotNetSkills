---
applyTo: '**/*.cs,**/*.csproj,**/GlobalUsings.cs,**/DependencyInjection.cs'
description: "Domain-Driven Design project structure and semantic organization guidelines"
---

# DDD Project Structure & Semantic Organization Guidelines

> **Core Principle**: Project structure must reflect the business domain, not technical concerns. Each namespace and folder should have clear semantic meaning that aligns with the ubiquitous language of the business.

## 🏗️ Architectural Foundation

### **Clean Architecture Dependency Flow**
```
📦 API Layer (Presentation)
    ↓ depends on
📦 Application Layer (Use Cases)
    ↓ depends on
📦 Domain Layer (Business Logic)
    ↑ implements
📦 Infrastructure Layer (External Concerns)
```

**Critical Rule**: Dependencies flow **inward only**. Domain layer has **zero** external dependencies.

### **Project Structure Hierarchy**
```
src/
├── DotNetSkills.API/              # 🌐 HTTP endpoints, middleware, authentication
├── DotNetSkills.Application/      # 🎯 Use cases, CQRS, validation, DTOs
├── DotNetSkills.Domain/           # 💎 Core business logic, entities, events
└── DotNetSkills.Infrastructure/   # 🔧 Data access, external services, configurations

tests/
├── DotNetSkills.API.UnitTests/
├── DotNetSkills.Application.UnitTests/
├── DotNetSkills.Domain.UnitTests/
└── DotNetSkills.Infrastructure.UnitTests/
```

## 🎯 Domain-Driven Design Structure

### **Bounded Context Organization**
Each bounded context is a **semantic boundary** representing a specific business capability:

```
src/DotNetSkills.Domain/
├── Common/                        # Shared kernel across all contexts
│   ├── Entities/                  # BaseEntity<T>, AggregateRoot<T>
│   ├── Events/                    # IDomainEvent, BaseDomainEvent
│   ├── Exceptions/                # DomainException hierarchy
│   ├── Extensions/                # Domain-specific extensions
│   ├── Rules/                     # Cross-context business rules
│   └── Validation/                # Domain validation abstractions
├── UserManagement/                # 👤 User lifecycle and authentication
│   ├── Entities/                  # User.cs - Rich domain model
│   ├── ValueObjects/              # UserId.cs, EmailAddress.cs
│   ├── Enums/                     # UserRole.cs, UserStatus.cs
│   ├── Events/                    # UserCreatedDomainEvent.cs
│   ├── Services/                  # UserDomainService.cs (if needed)
│   └── BoundedContextUsings.cs    # Context-specific imports
├── TeamCollaboration/             # 👥 Team formation and membership
│   ├── Entities/                  # Team.cs, TeamMember.cs
│   ├── ValueObjects/              # TeamId.cs, TeamMemberId.cs
│   ├── Enums/                     # TeamRole.cs, MembershipStatus.cs
│   ├── Events/                    # TeamCreatedDomainEvent.cs
│   └── BoundedContextUsings.cs
├── ProjectManagement/             # 📋 Project lifecycle and planning
│   ├── Entities/                  # Project.cs
│   ├── ValueObjects/              # ProjectId.cs, ProjectDescription.cs
│   ├── Enums/                     # ProjectStatus.cs, Priority.cs
│   ├── Events/                    # ProjectStartedDomainEvent.cs
│   └── BoundedContextUsings.cs
└── TaskExecution/                 # ✅ Task assignment and tracking
    ├── Entities/                  # Task.cs, SubTask.cs
    ├── ValueObjects/              # TaskId.cs, TaskDescription.cs
    ├── Enums/                     # TaskStatus.cs, TaskPriority.cs
    ├── Events/                    # TaskAssignedDomainEvent.cs
    └── BoundedContextUsings.cs
```

### **Semantic Naming Conventions**

#### **Bounded Context Names** - Business Capabilities
- ✅ `UserManagement` - Clear business function
- ✅ `TeamCollaboration` - Describes the business process
- ✅ `ProjectManagement` - Domain expertise area
- ✅ `TaskExecution` - Specific business workflow
- ❌ `Users` - Too generic, not a business capability
- ❌ `Data` - Technical concern, not business domain

#### **Entity Names** - Business Concepts
- ✅ `User` - Core business entity
- ✅ `Team` - Aggregate root representing collaboration unit
- ✅ `Project` - Business planning construct
- ✅ `Task` - Work execution unit
- ❌ `UserModel` - Technical suffix, not domain language
- ❌ `TeamEntity` - Reveals technical implementation

#### **Value Object Names** - Domain Language
- ✅ `UserId` - Strongly-typed identifier
- ✅ `EmailAddress` - Domain concept with validation
- ✅ `ProjectDescription` - Business-meaningful value
- ✅ `TaskPriority` - Domain-specific enumeration
- ❌ `UserGuid` - Reveals technical implementation
- ❌ `StringWrapper` - Technical concept, not domain

## 🎯 Application Layer Organization

### **Feature-Based Structure (Vertical Slices)**
Organize by **business features**, not technical layers:

```
src/DotNetSkills.Application/
├── Common/                        # Cross-cutting application concerns
│   ├── Behaviors/                 # MediatR pipeline behaviors
│   ├── Exceptions/                # Application-specific exceptions
│   ├── Interfaces/                # Repository and service contracts
│   ├── Mappings/                  # AutoMapper profiles
│   └── Models/                    # Shared DTOs and responses
├── UserManagement/                # User-related use cases
│   ├── Features/                  # Vertical slices per use case
│   │   ├── CreateUser/            # Single use case folder
│   │   │   ├── CreateUserCommand.cs
│   │   │   ├── CreateUserCommandHandler.cs
│   │   │   └── CreateUserCommandValidator.cs
│   │   ├── UpdateUser/
│   │   ├── DeactivateUser/
│   │   └── GetUsers/
│   └── Contracts/                 # Public interfaces for this context
│       ├── Requests/              # Input DTOs
│       ├── Responses/             # Output DTOs
│       └── IUserRepository.cs     # Repository interface
├── TeamCollaboration/
│   ├── Features/
│   │   ├── CreateTeam/
│   │   ├── AddTeamMember/
│   │   └── GetTeamMembers/
│   └── Contracts/
├── ProjectManagement/
└── TaskExecution/
```

### **Feature Slice Naming Pattern**
Each feature represents a **single business use case**:

- ✅ `CreateUser/` - Clear business action
- ✅ `AssignTaskToUser/` - Specific business operation
- ✅ `ArchiveCompletedProject/` - Business workflow
- ❌ `UserCRUD/` - Technical operations, not business language
- ❌ `DataManagement/` - Technical concern

## 🔧 Infrastructure Layer Organization

### **Implementation-Based Structure**
Organize by **technical capabilities** that support domain operations:

```
src/DotNetSkills.Infrastructure/
├── Common/                        # Shared infrastructure utilities
│   ├── Extensions/                # Infrastructure-specific extensions
│   └── Configurations/            # Configuration mappings
├── Persistence/                   # Data storage implementations
│   ├── Context/                   # DbContext and database setup
│   │   ├── ApplicationDbContext.cs
│   │   └── DbContextExtensions.cs
│   ├── Configurations/            # EF Core entity configurations
│   │   ├── UserConfiguration.cs
│   │   ├── TeamConfiguration.cs
│   │   └── ProjectConfiguration.cs
│   ├── Migrations/                # Database schema versions
│   └── Extensions/                # Persistence utilities
├── Repositories/                  # Repository implementations
│   ├── Common/                    # Base repository patterns
│   │   ├── BaseRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── UserManagement/            # User-related data access
│   │   └── EfUserRepository.cs
│   ├── TeamCollaboration/
│   ├── ProjectManagement/
│   └── TaskExecution/
└── Services/                      # External service integrations
    ├── Email/                     # Email service implementations
    ├── Authentication/            # JWT and identity services
    └── Notifications/             # Push notification services
```

## 🌐 API Layer Organization

### **Endpoint-Based Structure**
Organize by **business contexts** with focused endpoints:

```
src/DotNetSkills.API/
├── Configuration/                 # API-specific configurations
│   ├── Swagger/                   # API documentation setup
│   └── Authentication/           # JWT and auth setup
├── Endpoints/                     # Business-focused endpoint groups
│   ├── UserManagement/            # User-related HTTP endpoints
│   │   └── UserEndpoints.cs       # All user operations
│   ├── TeamCollaboration/
│   │   └── TeamEndpoints.cs
│   ├── ProjectManagement/
│   │   └── ProjectEndpoints.cs
│   └── TaskExecution/
│       └── TaskEndpoints.cs
├── Middleware/                    # Cross-cutting HTTP concerns
│   ├── ExceptionHandlingMiddleware.cs
│   ├── PerformanceLoggingMiddleware.cs
│   └── SecurityHeadersMiddleware.cs
└── Extensions/                    # API-specific extensions
    ├── ServiceCollectionExtensions.cs
    └── WebApplicationExtensions.cs
```

## 📋 Namespace Conventions

### **Semantic Namespace Structure**
Namespaces must reflect **business meaning**, not folder structure:

```csharp
// ✅ Domain Layer - Business Concepts
namespace DotNetSkills.Domain.UserManagement.Entities;
namespace DotNetSkills.Domain.TeamCollaboration.ValueObjects;
namespace DotNetSkills.Domain.ProjectManagement.Events;

// ✅ Application Layer - Use Cases
namespace DotNetSkills.Application.UserManagement.Features.CreateUser;
namespace DotNetSkills.Application.TeamCollaboration.Contracts.Responses;

// ✅ Infrastructure Layer - Technical Implementations
namespace DotNetSkills.Infrastructure.Persistence.Configurations;
namespace DotNetSkills.Infrastructure.Repositories.UserManagement;

// ✅ API Layer - HTTP Concerns
namespace DotNetSkills.API.Endpoints.UserManagement;
namespace DotNetSkills.API.Middleware;
```

### **Cross-Context Dependencies**
Handle dependencies between bounded contexts **explicitly**:

```csharp
// UserManagement/BoundedContextUsings.cs
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
// No cross-context imports - maintain boundaries

// TeamCollaboration/BoundedContextUsings.cs
global using DotNetSkills.Domain.TeamCollaboration.Entities;
global using DotNetSkills.Domain.TeamCollaboration.ValueObjects;
// Explicit dependency on UserManagement
global using DotNetSkills.Domain.UserManagement.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
```

## 🛡️ Dependency Management

### **Layer-Specific Global Usings**
Each layer has its own semantic imports:

```csharp
// Domain/GlobalUsings.cs - Pure business logic
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.Common.Events;
global using DotNetSkills.Domain.Common.Exceptions;
// NO external dependencies

// Application/GlobalUsings.cs - Use case orchestration
global using MediatR;
global using FluentValidation;
global using AutoMapper;
global using DotNetSkills.Domain.Common.Entities;

// Infrastructure/GlobalUsings.cs - Technical implementations
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Configuration;
global using DotNetSkills.Application.Common.Interfaces;

// API/GlobalUsings.cs - HTTP and presentation
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using DotNetSkills.Application.Common.Models;
```

### **Dependency Injection Architecture**
Each layer registers its dependencies through semantic extension methods:

```csharp
// Program.cs - Clean composition root
var builder = WebApplication.CreateBuilder(args);

// Layer registration follows dependency order
builder.Services.AddApiServices(builder.Configuration);
// AddApiServices calls AddApplicationServices
// AddApplicationServices calls AddInfrastructureServices
// Domain layer uses factory pattern (no DI dependency)

// Each layer's DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application-specific services
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // Chain to infrastructure
        return services.AddInfrastructureServices();
    }
}
```

## 🧪 Test Organization

### **Mirror Production Structure**
Test projects follow the same semantic organization:

```
tests/
├── DotNetSkills.Domain.UnitTests/
│   ├── Common/                    # Common test utilities
│   ├── UserManagement/            # Domain business logic tests
│   │   ├── Entities/              # Entity behavior tests
│   │   ├── ValueObjects/          # Value object validation tests
│   │   └── Services/              # Domain service tests
│   ├── TeamCollaboration/
│   ├── ProjectManagement/
│   └── TaskExecution/
├── DotNetSkills.Application.UnitTests/
│   ├── UserManagement/            # Use case tests
│   │   └── Features/
│   │       ├── CreateUser/        # Command handler tests
│   │       └── GetUsers/          # Query handler tests
│   └── Common/                    # Application service tests
└── DotNetSkills.Infrastructure.UnitTests/
    ├── Persistence/               # Repository integration tests
    └── Services/                  # External service tests
```

## 📐 File Naming Conventions

### **Domain Layer Files**
- **Entities**: `{EntityName}.cs` (e.g., `User.cs`, `Team.cs`)
- **Value Objects**: `{ConceptName}.cs` (e.g., `UserId.cs`, `EmailAddress.cs`)
- **Events**: `{BusinessEvent}DomainEvent.cs` (e.g., `UserCreatedDomainEvent.cs`)
- **Enums**: `{DomainConcept}.cs` (e.g., `UserRole.cs`, `TaskStatus.cs`)

### **Application Layer Files**
- **Commands**: `{Action}Command.cs` (e.g., `CreateUserCommand.cs`)
- **Queries**: `{Data}Query.cs` (e.g., `GetUsersQuery.cs`)
- **Handlers**: `{Command|Query}Handler.cs` (e.g., `CreateUserCommandHandler.cs`)
- **Validators**: `{Command|Query}Validator.cs` (e.g., `CreateUserCommandValidator.cs`)

### **Test Files**
- **Domain Tests**: `{EntityName}Tests.cs` (e.g., `UserTests.cs`)
- **Application Tests**: `{Handler}Tests.cs` (e.g., `CreateUserCommandHandlerTests.cs`)
- **Test Builders**: `{Entity}Builder.cs` (e.g., `UserBuilder.cs`)

## ✅ Developer Experience Guidelines

### **Discoverability Principles**
1. **Folder names** should be **business terms** from ubiquitous language
2. **File names** should clearly indicate their **business purpose**
3. **Namespace hierarchy** should guide developers to related concepts
4. **Dependencies** should be explicit and follow architectural boundaries

### **IDE-Friendly Organization**
- Use **meaningful folder names** that appear logically in Solution Explorer
- Group related files in **feature folders** for easy navigation
- Implement **consistent naming patterns** across all layers
- Provide **clear separation** between business and technical concerns

### **Onboarding Support**
- **New developers** should easily understand business domains from folder structure
- **Related code** should be physically located together (vertical slices)
- **Dependencies** should be obvious from project and namespace organization
- **Business concepts** should be discoverable through semantic naming

---

## 🎯 Implementation Checklist

When creating new features, ensure:

- [ ] **Business capability** has its own bounded context folder
- [ ] **Use cases** are organized as vertical slices with all related files together
- [ ] **Domain concepts** use ubiquitous language in naming
- [ ] **Cross-context dependencies** are explicit and minimal
- [ ] **Technical concerns** are separated from business logic
- [ ] **Test structure** mirrors production organization
- [ ] **Namespace hierarchy** reflects business meaning, not technical folders

**Remember**: Structure follows domain, not technology. Every folder and file should have clear business meaning that aligns with how the business thinks about these concepts.