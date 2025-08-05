# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Essential Commands

### Build and Test
```bash
# Build entire solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/DotNetSkills.Domain.UnitTests

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run API (currently placeholder weather endpoint at https://localhost:7240)
dotnet run --project src/DotNetSkills.API
```

### Database Operations (When Infrastructure is implemented)
```bash
# Add migration
dotnet ef migrations add <Name> --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API

# Update database
dotnet ef database update --project src/DotNetSkills.Infrastructure --startup-project src/DotNetSkills.API
```

## Architecture Overview

This is a .NET 9 project management API built with **Clean Architecture** and **Domain-Driven Design** principles.

### Clean Architecture Layers
- **API Layer**: `DotNetSkills.API` - Minimal APIs (currently placeholder weather endpoint)
- **Application Layer**: `DotNetSkills.Application` - Currently placeholder `Class1.cs`, will contain CQRS with MediatR
- **Domain Layer**: `DotNetSkills.Domain` - **Complete** - Rich domain models with 4 bounded contexts
- **Infrastructure Layer**: `DotNetSkills.Infrastructure` - Currently placeholder `Class1.cs`, will contain EF Core repositories

### Domain Layer Structure (Complete - Follow These Patterns)

The domain is organized into 4 bounded contexts:
- `UserManagement/` - User entity with roles, status, team memberships
- `TeamCollaboration/` - Team and TeamMember entities with membership management
- `ProjectManagement/` - Project entities with team associations
- `TaskExecution/` - Task entities with assignment and status management

Each bounded context contains:
- `Entities/` - Rich domain models with business logic
- `ValueObjects/` - Strongly-typed IDs and value types
- `Events/` - Domain events using `BaseDomainEvent` pattern
- `Enums/` - Status and role enumerations with extension methods

### Key Domain Patterns (Follow Exactly)

**Strongly-Typed IDs**: All entities use this pattern:
```csharp
public record UserId(Guid Value) : IStronglyTypedId<Guid>
{
    public static UserId New() => new(Guid.NewGuid());
    public static implicit operator Guid(UserId id) => id.Value;
    public static explicit operator UserId(Guid value) => new(value);
}
```

**Aggregate Root Pattern**: Business logic in entity methods with domain events:
```csharp
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

        // Domain event for cross-aggregate communication
        RaiseDomainEvent(new UserJoinedTeamDomainEvent(Id, team.Id, role));
    }
}
```

**Global Usings**: Each layer has `GlobalUsings.cs` with layer-specific imports. The domain layer includes comprehensive global usings for all bounded contexts.

### Critical Business Rules (Implemented in Domain)

- **Team membership limits**: `Team.MaxMembers = 50` enforced in `AddMember()`
- **Single task assignment**: Tasks support one assignee only
- **One-level subtask nesting**: Tasks can have subtasks, but subtasks cannot have children
- **Project ownership**: Each project belongs to exactly one team
- **Admin-only user creation**: User.Create() factory method enforces this rule

### Technology Stack

- **.NET 9** with C# 13, nullable reference types enabled, primary constructors
- **Minimal APIs** (currently placeholder - needs real implementation)
- **Entity Framework Core** (planned for Infrastructure layer)
- **MediatR** for CQRS (planned for Application layer)
- **AutoMapper** for entity ↔ DTO mapping (planned)
- **FluentValidation** for input validation (planned)
- **xUnit + FluentAssertions + Moq** for testing

### Current Implementation Status

**Domain Layer**: ✅ Complete with rich domain models, value objects, events, and business rules
**Application Layer**: 📋 Placeholder - needs CQRS commands/queries with MediatR
**Infrastructure Layer**: 📋 Placeholder - needs EF Core DbContext and repository implementations
**API Layer**: 📋 Placeholder weather endpoint - needs real project management endpoints

### Validation Patterns

- **Input validation**: Use `ArgumentException` for parameter validation
- **Business rules**: Use `DomainException` for domain constraint violations
- **Validation messages**: Centralized in `ValidationMessages.cs` with formatting support

### Authentication & Authorization (Planned)

- **JWT-based** authentication with role hierarchy: Admin > ProjectManager > Developer > Viewer
- **Admin-only user creation** - no self-registration in MVP
- **Role-based access control** for all operations

## Development Guidelines

When implementing new features:
1. Start with domain entities (rich models with business logic)
2. Follow established patterns in existing domain contexts
3. Use strongly-typed IDs for all entity identifiers
4. Implement proper aggregate boundaries and domain events
5. Follow the dependency rule: API → Application → Domain ← Infrastructure

The project follows strict SOLID principles, Clean Architecture patterns, and comprehensive Domain-Driven Design implementation as documented in the GitHub Copilot instructions.
