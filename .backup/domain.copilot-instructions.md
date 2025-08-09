# DotNetSkills AI Coding Agent Instructions

## Project Overview

This is a .NET 9 project management API demonstrating **Clean Architecture** and **Domain-Driven Design** principles. The project showcases enterprise-grade development practices with modern .NET patterns.

**Current State**: Rich domain layer with 4 bounded contexts implemented. Application and Infrastructure layers are placeholder classes awaiting implementation.

## Architecture & Core Patterns

### Clean Architecture Structure (Dependency Rule: API → Application → Domain)
```
src/
├── DotNetSkills.API/           # Minimal API with weather template (replace with real endpoints)
├── DotNetSkills.Application/   # Placeholder Class1.cs (implement CQRS + MediatR)
├── DotNetSkills.Domain/        # ✅ Complete - 4 bounded contexts with rich domain models
└── DotNetSkills.Infrastructure/ # Placeholder Class1.cs (implement EF Core repositories)
```

### Domain-Driven Design Implementation
- **Bounded Contexts**: `UserManagement/`, `TeamCollaboration/`, `ProjectManagement/`, `TaskExecution/` - each with Entities/, Events/, ValueObjects/, Enums/
- **Rich Domain Models**: Business logic in entity methods (e.g., `Team.AddMember()`, `User.JoinTeam()`)
- **Strongly-Typed IDs**: All entities use `record UserId(Guid Value) : IStronglyTypedId<Guid>` pattern with implicit Guid conversion
- **Domain Events**: Raised via `AggregateRoot<TId>.RaiseDomainEvent()` - stored in private `_domainEvents` list, cleared manually
- **Aggregate Boundaries**: `User`, `Team`, `Project`, `Task` are separate roots; `TeamMember` is part of Team aggregate

### Key Domain Patterns (Follow These Exactly)
```csharp
// Strongly-typed ID pattern - ALL entities use this
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid value) => new(value);
}

// Aggregate Root pattern with domain events
public class User : AggregateRoot<UserId>
{
    public void JoinTeam(Team team, TeamRole role)
    {
        // Business rule validation first
        if (_teamMemberships.Any(m => m.TeamId == team.Id))
            throw new DomainException("User is already a member of this team");
        
        // State change
        var membership = new TeamMember(Id, team.Id, role);
        _teamMemberships.Add(membership);
        
        // Domain event (cross-aggregate communication)
        RaiseDomainEvent(new UserJoinedTeamDomainEvent(Id, team.Id, role));
    }
}

// Global usings per layer - see Domain/GlobalUsings.cs for the pattern
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
```

## Essential Development Commands

```bash
# Build entire solution
dotnet build

# Run API (currently placeholder weather API at https://localhost:7240)
dotnet run --project src/DotNetSkills.API

# Run tests (domain logic tests exist but minimal)
dotnet test
dotnet test tests/DotNetSkills.Domain.UnitTests

# EF migrations (when Infrastructure layer is implemented)
dotnet ef migrations add <Name> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Critical Business Rules & Constraints

### Authentication & Authorization (Not Yet Implemented)
- **JWT-based** with role-based access: Admin > ProjectManager > Developer > Viewer
- **Admin-only user creation** - no self-registration in MVP
- **User.Create()** factory method enforces admin-only creation rule

### Core Domain Rules (Implemented in Domain Layer)
- **Team membership limits**: `Team.MaxMembers = 50` constant enforced in `AddMember()`
- **Single task assignment**: Tasks support one assignee only
- **One-level subtask nesting**: Tasks can have subtasks, but subtasks cannot have children
- **Team membership**: Many-to-many via `TeamMember` entity (part of Team aggregate)
- **Project ownership**: Each project belongs to exactly one team
- **Status transitions**: Business rules prevent invalid state changes

### Validation Patterns (Current Focus)
- **Input validation**: Use `ArgumentException` for parameter validation
- **Business rules**: Use `DomainException` for domain constraint violations
- **Consistency**: Follow patterns in `CurrentTask.md` for validation standardization

## Implementation Status & Next Steps

**Current State**: Domain layer complete with 4 bounded contexts. Working on validation standardization (see `CurrentTask.md`).

**Implementation Priority** (based on `plan/feature-dotnetskills-mvp-1.md`):
1. **Infrastructure Layer**: EF Core DbContext, repository implementations, entity configurations
2. **Application Layer**: MediatR commands/queries, DTOs, FluentValidation, AutoMapper
3. **API Layer**: Replace weather template with real endpoints using Minimal API extension methods
4. **Authentication**: JWT middleware and user management endpoints

## Key Files & Patterns to Reference

### Domain Layer Structure (Complete - Follow These Patterns)
- **Entities**: `src/DotNetSkills.Domain/{BoundedContext}/Entities/` - Rich domain models with business logic
- **Value Objects**: `src/DotNetSkills.Domain/{BoundedContext}/ValueObjects/` - Strongly-typed IDs and value types
- **Domain Events**: `src/DotNetSkills.Domain/{BoundedContext}/Events/` - Use `BaseDomainEvent` pattern
- **Base Classes**: `AggregateRoot<TId>`, `BaseEntity<TId>` in `Domain/Common/Entities/`
- **Global Usings**: Each layer has `GlobalUsings.cs` with layer-specific imports

### Bounded Contexts
- **UserManagement**: `User` entity with roles, status, team memberships
- **TeamCollaboration**: `Team` and `TeamMember` entities with membership management
- **ProjectManagement**: Project entities with team associations
- **TaskExecution**: Task entities with assignment and status management

### Coding Standards
- Follow comprehensive patterns in `.github/instructions/dotnet-arquitecture.instructions.md`
- **Nullable Reference Types**: Enabled - use `string?` for nullable, `string.Empty` for non-nullable defaults
- **Primary Constructors**: Used in record types and some entities with C# 13 features
- **Domain Event Pattern**: `RaiseDomainEvent()` in aggregate roots, `ClearDomainEvents()` after processing

## Technology Stack

- **.NET 9** with C# 13, nullable reference types enabled, primary constructors
- **Minimal APIs** (currently placeholder weather endpoint - needs real implementation)
- **Entity Framework Core** (not yet configured - Infrastructure layer is Class1.cs placeholder)
- **MediatR** for CQRS (planned for Application layer)
- **AutoMapper** for entity ↔ DTO mapping (planned)
- **FluentValidation** for input validation (planned)
- **Global using statements** per layer (see `Domain/GlobalUsings.cs` pattern)

## Current Development Context

**Active Task**: Domain validation standardization (see `CurrentTask.md`)
**MVP Plan**: Comprehensive implementation roadmap in `plan/feature-dotnetskills-mvp-1.md`

When implementing new features:
1. Start with domain entities (rich models with business logic)
2. Follow established patterns in existing domain contexts
3. Use strongly-typed IDs for all entity identifiers
4. Implement proper aggregate boundaries and domain events
5. Refer to MVP plan for task breakdown and acceptance criteria
