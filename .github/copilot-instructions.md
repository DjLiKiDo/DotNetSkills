# DotNetSkills AI Coding Agent Instructions

## Project Overview

This is a .NET 9 project management API demonstrating **Clean Architecture** and **Domain-Driven Design** principles. The project showcases enterprise-grade development practices with modern .NET patterns and is currently in early development phase with domain models established.

## Architecture & Core Patterns

### Clean Architecture Structure (Dependency Rule: API → Application → Domain)
```
src/
├── DotNetSkills.API/           # Minimal APIs, JWT auth, Program.cs startup
├── DotNetSkills.Application/   # Use cases, DTOs, repository interfaces (currently minimal)
├── DotNetSkills.Domain/        # Rich domain models with business logic (primary focus)
└── DotNetSkills.Infrastructure/ # EF Core repositories, external services (not implemented)
```

### Domain-Driven Design Implementation
- **Rich Domain Models**: Business logic in entity methods (e.g., `Task.AssignTo(assignee, assignedBy)`, `Team.AddMember()`)
- **Strongly-Typed IDs**: All entities use value objects like `UserId(Guid)`, `TaskId(Guid)` - see `IStronglyTypedId<T>` pattern
- **Domain Events**: Rich event system - `TaskAssignedDomainEvent`, `UserJoinedTeamDomainEvent` - raised via `AggregateRoot.RaiseDomainEvent()`
- **Aggregate Boundaries**: `User`, `Team`, `Project`, `Task` are separate aggregates; `TeamMember` is part of Team aggregate

### Key Domain Patterns to Follow
```csharp
// Strongly-typed IDs pattern
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId id) => id.Value;
}

// Rich domain methods with business rules
public void AssignTo(User assignee, User assignedBy)
{
    if (Status == TaskStatus.Done) throw new DomainException("Cannot assign completed tasks");
    if (!assignee.CanBeAssignedTasks()) throw new DomainException("User cannot be assigned tasks");
    
    AssignedUserId = assignee.Id;
    RaiseDomainEvent(new TaskAssignedDomainEvent(Id, assignee.Id, assignedBy.Id));
}

// Global usings in each layer (see Domain/GlobalUsings.cs)
global using DotNetSkills.Domain.Common.Entities;
global using DotNetSkills.Domain.UserManagement.ValueObjects;
```

## Essential Development Commands

```bash
# Build entire solution
dotnet build

# Run API (currently basic weather API template at https://localhost:7240)
dotnet run --project src/DotNetSkills.API

# Run tests (when implemented)
dotnet test
dotnet test tests/DotNetSkills.Domain.UnitTests  # Domain logic tests

# EF migrations (when Infrastructure is configured)
dotnet ef migrations add <Name> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Critical Business Rules & Constraints

### Authentication & Authorization
- **JWT-based** with role-based access: Admin > ProjectManager > Developer > Viewer
- **Admin-only user creation** - no self-registration in MVP
- **User.Create()** factory method enforces "only admin users can create new users"

### Core Domain Rules
- **Single task assignment**: `Task.AssignTo()` - one user per task only
- **One-level subtask nesting**: Tasks can have subtasks, but subtasks cannot have children
- **Team membership**: Many-to-many via `TeamMember` entity (part of Team aggregate)
- **Project ownership**: Each project belongs to exactly one team
- **Status transitions**: Business rules prevent invalid state changes (e.g., Done → ToDo)

### Deletion Constraints
- Cannot delete teams with active projects
- Cannot delete projects with active tasks
- Cascading operations handled through domain events

## Implementation Status & Priorities

**Current State**: Domain layer established with rich models, Application/Infrastructure layers are placeholder classes.

**Immediate Priorities** (based on `plan/feature-dotnetskills-mvp-1.md`):
1. **Infrastructure Layer**: EF Core DbContext, repository implementations, entity configurations
2. **Application Layer**: Use cases, DTOs, MediatR commands/queries, FluentValidation
3. **API Layer**: Replace placeholder weather API with real endpoints using extension methods pattern
4. **Authentication**: JWT middleware configuration and user management endpoints

## Key Files & Patterns to Reference

- **Domain Models**: `src/DotNetSkills.Domain/{Module}/Entities/` - Follow existing patterns
- **Domain Events**: `src/DotNetSkills.Domain/{Module}/Events/` - Use `BaseDomainEvent` pattern
- **Value Objects**: `src/DotNetSkills.Domain/{Module}/ValueObjects/` - Implement `IStronglyTypedId<T>`
- **Base Classes**: `AggregateRoot<TId>`, `BaseEntity<TId>` in `Domain/Common/Entities/`
- **Coding Standards**: Follow patterns in `.github/instructions/dotnet-arquitecture.instructions.md`

## Technology Stack

- **.NET 9** with C# 13, nullable reference types enabled
- **Minimal APIs** with endpoint extension methods (see coding principles doc)
- **Entity Framework Core** with SQL Server (when implemented)
- **MediatR** for CQRS pattern in Application layer
- **AutoMapper** for entity ↔ DTO mapping
- **FluentValidation** for input validation
- **Global using statements** per layer (see existing `GlobalUsings.cs` files)

When implementing new features, follow the established patterns in the Domain layer and refer to the comprehensive implementation plan in `plan/feature-dotnetskills-mvp-1.md` for task breakdown and acceptance criteria.
